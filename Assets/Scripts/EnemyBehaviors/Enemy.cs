using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Settings")]
    public bool isBoss = false;
    [SerializeField] private bool detachAndPinHealth = false;
    [SerializeField]
    public EnemyType enemyType;
    [SerializeField]
    public float challengeRating = 1f;

    [Space(10)]
    public int damage = 10;
    public float speed = 5f;
    public int pointsPerHit = 0;
    public List<GunDamageModifier> gunDamageModifiers = new List<GunDamageModifier>();
    public bool canIncrementEggs = true;
    public bool needToFinishRoom = true;
    public bool wallEnemy;
    [SerializeField] private Material colorSwapMaterial;
    [SerializeField] private bool canBeBuffedByOoze = false;
    SpriteRenderer spriteRenderer;
    private Animator _animator;

    [Header("AI Settings")]
    public Transform target;
    [HideInInspector] public float rangeView;
    //[HideInInspector] 
    public NavMeshAgent agent;
    [SerializeField] bool shouldSufferHitLag = true;
    public float agentSpeed = 0f;
    float agentAcceleration;

    [Header("Feedbacks")]
    public bool shouldDisplayHealth = true;
    public AudioClip idleSound;
    public AudioClip hittedSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public Color primaryColor;
    public Color secondaryColor;
    public SpriteRenderer minimapIconPrefab;
    public AudioMixerGroup sfxGroup;

    [Header("Death Settings")]
    public bool deathAnimationTrigger;
    //public AudioClip deathSound;
	public bool alwaysDrop;
    public GameObject coinPrefab;
    public GameObject deathEffect;
    public GameObject customDeathPrefab;

    public Room currentRoom;

    [HideInInspector] public AudioSource audioSource;
    Health health;
    CanvasGroup healthBar;
    private Coroutine _displayHealthCoroutine;

    private Material _hitMaterial;

    bool isDebuff = false;

    public Player player;
    [HideInInspector] public Player lastShooterPlayer;
    List<Player> players = new List<Player>();

    private EnemyBehaviourController _controller;
   
    private void Start()
    {
        _controller = GetComponent<EnemyBehaviourController>();

        SetSpriteRenderer();
        SetAnimator();
        _hitMaterial = spriteRenderer.material;
        if (colorSwapMaterial != null)
            spriteRenderer.material = colorSwapMaterial;

        players = GameplayManager.Instance.GetPlayers(false);
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<CanvasGroup>();
        if(!isBoss && detachAndPinHealth)
            healthBar.GetComponent<PinToParent>().Setup();

        if (transform.name.Contains("D2_GiantCocoon"))
            health = GetComponentInChildren<Health>();
        else
            health = GetComponent<Health>();

        agent = GetComponent<NavMeshAgent>();

		StartCoroutine(PlayIdleSound());
        StartCoroutine(FindTargetLoop(.5f));

        DeactivateOozeBuff(1f);
    }

    private void Update()
    {
        if (!player)
        {
            player = FindObjectOfType<Player>();
            if (player != null && !player.GetIsDead())
            {
                target = player.transform;
            }
        }

        TryFindTarget();

        if (PauseManager.Instance.IsGamePaused())
        {
            if (audioSource.isPlaying) audioSource.Pause();
        }
        else
        {
            if (!audioSource.isPlaying) audioSource.UnPause();
        }

        if (agent && agentAcceleration == 0)
            agentAcceleration = agent.acceleration;
    }

    IEnumerator FindTargetLoop(float delay)
	{
        TryFindTarget();

        yield return new WaitForSeconds(delay);

        StartCoroutine(FindTargetLoop(delay));
	}

    public SpriteRenderer GetSpriteRenderer()
	{
        return spriteRenderer;
    }

    void SetSpriteRenderer()
	{
        if (gameObject.GetComponent<SpriteRenderer>() != null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        else
        {
            spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void SetSpriteRendererAlpha(float alpha)
	{
        Color color = spriteRenderer.color;
        color.a = alpha;

        spriteRenderer.color = color;
	}

    private void SetAnimator()
    {
        _animator = GetComponent<Animator>();
        
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    void TryFindTarget()
    {
        if (target && !target.GetComponent<Player>().GetIsDead())
        {
            return;
        }
        List<Player> players = GameplayManager.Instance.GetPlayers(false);
        Player nearPlayer = null;
        foreach(Player player in players)
		{
            if (player.GetIsDead()) continue;

            if (!nearPlayer)
			{
                nearPlayer = player;
			}
			else
			{
                if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, nearPlayer.transform.position))
				{
                    nearPlayer = player;
				}
			}
		}

        if (nearPlayer)
        {
            player = nearPlayer;
            target = nearPlayer.transform;
        } 
        else
        {
            target = null;
        }
    }

    public Transform GetTarget()
	{
        return target;
	}

    public void onDamage(float valueDecreased)
    {
        if (gameObject == null)
        {
            return;
        }

        GlobalData.Instance.AddScore(pointsPerHit);
        if (!isDebuff)
        {
            StartCoroutine(SpeedDebuff());
        }

        if (health.IsDamageEnabled())
        {
            StartCoroutine(DamageFeedback());
        }

        SendMessage("OnHitted");

        if (health && shouldDisplayHealth)
        {
            Image h = healthBar.GetComponentInChildren<Image>();
            float finalValue = health.GetCurrentHealth() / health.GetMaxHealth();
            if (!(finalValue > 0))
            {
                finalValue = 0;
            }
            h.fillAmount = finalValue;

            if (_displayHealthCoroutine != null)
                StopCoroutine(_displayHealthCoroutine);
                
            _displayHealthCoroutine = StartCoroutine(DisplayHealth());
        }
    }

    public void onHealthEnd()
    {
        if (lastShooterPlayer && canIncrementEggs)
		{
            lastShooterPlayer.currentPocket.OnExperienceGain();
            lastShooterPlayer.IncreaseDefeatedEnemies();
		}

        if (deathAnimationTrigger)
		{
            GetComponent<NavMeshAgent>().speed = 0;

            _animator.SetTrigger("Die");
        }
		else
		{
            if (_controller)
            {
                if (_controller.divideOnDie)
				{
                    GetComponent<Collider2D>().enabled = false;
                    health.gameObject.SetActive(false);
				}
            }
            onDie();
		}
    }

    public void onDie()
    {
        if (TryGetComponent<ShootOnDeath>(out ShootOnDeath shootOnDeath))
            shootOnDeath.Shoot();
        if (TryGetComponent<SpawnEnemyOnDeath>(out SpawnEnemyOnDeath spawnEnemy))
            spawnEnemy.Spawn(currentRoom);
        
        GlobalData.Instance.killedEnemies += 1;

        PlaySfx(deathSound);

        // Death effects
        if (deathEffect) Instantiate(deathEffect, transform.position, Quaternion.identity, transform.parent);

        // Drop
        DropCoins dropCoin = GetComponent<DropCoins>();
        if (dropCoin) dropCoin.DropCoin(transform.position);

        // Divide if required
        if (_controller)
        {
            if (_controller.divideOnDie)
            {
                _controller.Divide();
                return;
            }
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (player.healOnKill)
        {
            player.GetHealth().Increase(3);
        }

        // Destroy
        if (!isBoss)
        {
            Destroy(gameObject);
		}
    }

    private void OnDestroy()
    {
        int count = 0;
        foreach (Player player in GameplayManager.Instance.GetPlayers(false))
        {
            if (player)
            {
                if (player.gameObject.activeSelf) count++;
            }
        }

        bool hasPlayer = count > 0;

        if (hasPlayer && currentRoom != null && needToFinishRoom)
		{
            currentRoom.RemoveEnemyFromList(this);  
		}
    }

    IEnumerator DamageFeedback()
    {
        if (transform.GetChild(0).name == "Swarm")
        {
            foreach (Transform item in transform.GetChild(0).transform)
            {
                item.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 1);
            }
        }
        else if (GetComponent<JumperController>() && GetComponent<JumperController>().fade)
        {
            GetComponent<JumperController>().graphics.GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 1);
            GetComponent<JumperController>().graphics.GetChild(0).GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 1);
        }
        else
        {
            if (colorSwapMaterial != null)
                spriteRenderer.material = _hitMaterial;

            spriteRenderer.material.SetInt("_Hit", 1);
        }

        //if (!audioSource.isPlaying) audioSource.PlayOneShot(hittedSound);

        if (shouldSufferHitLag)
        {
            if (_animator != null)
                _animator.speed = 0;
                
            if (agent)
            {
                agent.acceleration = 1000;
                agent.speed = 0f;
            }
        }

        yield return new WaitForSeconds(0.1f);
        //spriteRenderer.material.SetInt("_Hit", 0);

        if (transform.GetChild(0).name == "Swarm")
        {
            foreach (Transform item in transform.GetChild(0).transform)
            {
                item.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 0);
            }
        }
        else if (GetComponent<JumperController>() && GetComponent<JumperController>().fade)
        {
            GetComponent<JumperController>().graphics.GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 0);
            GetComponent<JumperController>().graphics.GetChild(0).GetComponent<SpriteRenderer>().material.SetFloat("_HitEffectBlend", 0);
        }
        else
        {
            spriteRenderer.material.SetInt("_Hit", 0);

            if (colorSwapMaterial != null)
                spriteRenderer.material = colorSwapMaterial;
        }

        if (shouldSufferHitLag)
        {
            if (_animator != null)
                _animator.speed = 1;

            if (agent)
            {
                agent.acceleration = agentAcceleration;
                agent.speed = agentSpeed;
            }
        }

        DeactivateOozeBuff(1f);
    }

    IEnumerator SpeedDebuff()
    {
        isDebuff = true;
        float speedCache = speed;
        float debuffedSpeed = speed * 0.6f;
        SetSpeed(debuffedSpeed);
        yield return new WaitForSeconds(0.5f);
        SetSpeed(speedCache);
        isDebuff = false;
    }

    void SetSpeed(float value)
    {
        speed = value;
    }

    IEnumerator DisplayHealth()
    {
        if (healthBar != null)
        {
            healthBar.alpha = 1;
            yield return new WaitForSeconds(.5f);
            healthBar.alpha = 0;
        }
    }

    public Player GetPlayer()
	{
        return player;
	}
	
    public Health GetHealth()
	{
        return health;
	}

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeView);
	}

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;
        RegisterEnemyOnRoom();
    }

    public void RegisterEnemyOnRoom()
    {
        currentRoom.enemies.Add(this);
	}

	private IEnumerator PlayIdleSound()
	{
		yield return new WaitForSeconds(Random.Range(3, 5));
	
        audioSource.PlayOneShot(idleSound);

		if (OnCamera())
		{
			StartCoroutine(PlayIdleSound());
		}
	}

    private bool OnCamera()
	{
        Vector2 min = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        if (transform.position.x > min.x && transform.position.x < max.x 
            && transform.position.y > min.y && transform.position.y < max.y)
		{
            return true;
		}

        return false;
	}

    public void PlayAttackSound()
	{
        audioSource.PlayOneShot(attackSound);
    }

    public void PlaySfx(AudioClip clip)
    {
        if (audioSource && clip)
        {
            AudioSource audioSource = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<AudioSource>();
            audioSource.transform.localScale = Vector3.zero;
            audioSource.transform.position = transform.position;

            audioSource.spatialBlend = .8f;

            audioSource.outputAudioMixerGroup = sfxGroup;

            audioSource.PlayOneShot(clip);
            Destroy(audioSource, clip.length + .5f);
        }
    }

    public void SetInvulnerable(bool isInvulnerable)
	{
        health.isInvulnerable = isInvulnerable;
    }

    public float GetDamageModifier(Bullet bullet)
	{
        foreach (GunDamageModifier modifier in gunDamageModifiers)
		{
            if (bullet)
            {
                if (bullet.GetGun().gunType == modifier.gunType)
                {
                    return modifier.damageModifier;
                }
            }
		}

        return 1;
	}

    public void EnableAudioSource()
	{
        if (GetComponent<AudioSource>())
		{
            GetComponent<AudioSource>().enabled = true;
		}
	}

    public void SetPlacementDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                SetRotation(0, .5f, 0);
                return;
            case Direction.Left:
                SetRotation(90, 1, .5f);
                return;
            case Direction.Right:
                SetRotation(-90, 0, .5f);
                return;
            default:
                SetRotation(180, 0.5f, 1);
                return;
        }
    }

	public void SetRotation(float angle, float offsetX = 0, float offsetY = 0)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
    }


    public Player GetNearPlayer()
    {
        Player player = null;
        foreach (Player p in GameplayManager.Instance.GetPlayers(true))
        {
            if (!player)
            {
                if (p.GetHealth().IsAlive())
                {
                    player = p;
                }
			}
			else
			{
                if (player.GetHealth().IsAlive())
                {
                    if (Vector2.Distance(transform.position, p.transform.position) < Vector2.Distance(transform.position, player.transform.position))
                    {
                        player = p;
                    }
                }
			}
        }

        return player;
    }

    public Player GetRandomPlayer()
	{
        List<Player> alivePlayers = new List<Player>();
        foreach (Player p in GameplayManager.Instance.GetPlayers(true))
        {
            if (!p.GetIsDead())
            {
                alivePlayers.Add(p);
            }
        }

        if (alivePlayers.Count > 0)
            return alivePlayers[Random.Range(0, alivePlayers.Count)];
        else
            return null;
    }

    public void ActivateOozeBuff(float newMultiplier)
    {
        if (!canBeBuffedByOoze)
            return;
        
        spriteRenderer.material.SetFloat("_OutlineAlpha", 1f);
        spriteRenderer.material.SetFloat("_OutlineWidth", 0.004f);

        spriteRenderer.material.SetFloat("_HandDrawnAmount", 10f);
        spriteRenderer.material.SetFloat("_ShakeUvSpeed", 8f);

        if (_controller != null)
            _controller.aggressiveStateController.ActivateOozeBuff(newMultiplier);
    }

    public void DeactivateOozeBuff(float standardMultiplier)
    {
        if (!canBeBuffedByOoze)
            return;

        spriteRenderer.material.SetFloat("_OutlineAlpha", 0f);
        spriteRenderer.material.SetFloat("_OutlineWidth", 0f);

        spriteRenderer.material.SetFloat("_HandDrawnAmount", 0f);
        spriteRenderer.material.SetFloat("_ShakeUvSpeed", 0f);

        if (_controller != null)
            _controller.aggressiveStateController.DeactivateOozeBuff(standardMultiplier);
    }
}

public enum EnemyType
{
    Aim,
    Awareness,
    Dash,
    DungeonMeta,
    EnemyMeta,
    Joker,
    Movement,
    Strategy,
}

[System.Serializable]
public struct GunDamageModifier
{
    public GunType gunType;
    public float damageModifier;
}