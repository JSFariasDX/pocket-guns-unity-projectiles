using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Queen : Boss
{
    FireToPlayerDirection ftpd;
    int phase = 0;

    Image healthBarImage;

    bool isFollowing = false;

    float followCooldown = 3f;
    bool isFollowCooldown = false;

    int projectileAmount = 10;
    float fireRadius = 5f;
    float projectileSpeed = 5f;
    public GameObject projectilePrefab;

    public int maxLarvaes;
    public GameObject larvaePrefab;
    float layCooldown = 3f;
    bool isLaying = false;

    bool justTouched = false;
    float touchCooldown = 3f;

    bool isMovingRight = true;

    [Header("Sound FX")]
    public AudioClip entranceSound;
    public AudioClip spawningSound;
    public AudioClip ragingSound;
    public AudioClip rageIdleSound;
    public AudioClip rageAttackSound;
    public AudioClip rageHittedSound;

    NavMeshAgent agent;

    Vector3 direction = Vector3.right;

    [Header("Animation")]
    string currentState;
    bool isFurious = false;
    public AnimationCurve spawnCurve;
    public float evaluateTime;

    bool started = false;
    float height;

    bool isDead = false;

    List<Enemy> larvaes = new List<Enemy>();

    private Collider2D _collider2D;

    Transform bossSpawn;
    Vector3 spawnPos;
    Vector3 desiredSpawnPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        _collider2D = GetComponent<Collider2D>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.speed = enemy.speed;
    }

    protected override void Start()
    {
        base.Start();

        _collider2D.enabled = false;

        ftpd = GetComponent<FireToPlayerDirection>();
        ftpd.SetActivation(false);

        health.SetInvulnerability(true);

        ChangeAnimationState("Queen_idle");
        PlayEntranceSound();

        bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn").transform;
        spawnPos = bossSpawn.position;
        desiredSpawnPos = new Vector3(spawnPos.x, spawnPos.y + 20);
        height = 0;
    }
    
    void EnterTheRoom()
    {
        if(evaluateTime < 1)
        {
            evaluateTime += Time.deltaTime / 2;
        }
        else
        {
            if (!started)
            {
                InstantiateHP();
                SetCinemachineTarget();
                _collider2D.enabled = true;
                StartBossTheme();
            }
        }

        bossSpawn.position = Vector3.Lerp(desiredSpawnPos, spawnPos, spawnCurve.Evaluate(evaluateTime));
    }


    private void Update()
    {
        EnterTheRoom();

        float currentHealth = health.GetCurrentHealth();
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / health.GetMaxHealth();
        }
        if (phase == 0)
        {
            AxisMove();
        }
        if (phase == 1)
        {
            FollowPlayer();
        }
        float healthPercentage = (100 * currentHealth) / health.GetMaxHealth();
        if (healthPercentage <= 75 && phase < 1)
        {
            StartPhase01();

            if(!isDead)
                ChangeAnimationState("Queen_evolution");
        }
        if (phase > 0 && !isFollowing && !isFollowCooldown)
        {
            StartCoroutine("FollowPlayer");
        }
        if (healthPercentage <= 50 && phase < 2)
        {
            StartPhase02();
        }
        if (phase > 1 && !isLaying)
        {
            StartCoroutine("Lay");
        }
        if ((GetNearPlayer() != null) && isFollowing)
        {
            agent.destination = GetNearPlayer().transform.position;
        }

        if (currentHealth <= 0)
        {
            if(!isDead)
                ChangeAnimationState("Queen_death");
            onHealthEnd();
        }
    }

    void AxisMove()
    {
        if (isMovingRight && Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Walls")))
        {
            Debug.DrawRay(transform.position, Vector2.right, Color.green);
            direction = Vector2.left;
            isMovingRight = false;
        } else if (!isMovingRight && Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Walls")))
        {
            Debug.DrawRay(transform.position, Vector2.left, Color.green);
            direction = Vector2.right;
            isMovingRight = true;
        }
        agent.destination = transform.position + direction;
    }

    void StartPhase01()
    {
        IncrementPhase();

        ftpd.SetActivation(false);
    }

    public void SetFurious()
    {
        isFurious = true;
    }

    public int GetLiveLarvaeCount()
	{
        int count = 0;
        foreach (Enemy larvae in larvaes)
		{
            if (larvae)
			{
                count++;
			}
		}

        return count;
	}

    void StartPhase02()
    {
        audioSource.PlayOneShot(ragingSound);
        enemy.idleSound = rageIdleSound;
		enemy.attackSound = rageAttackSound;
		enemy.hittedSound = rageHittedSound;
        //ThemeMusicManager.Instance.PlayAction();
        IncrementPhase();
        projectileAmount = 20;
    }

    void IncrementPhase()
    {
        phase = phase + 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCollider") && !justTouched)
        {
            StopCoroutine("FollowPlayer");
            StopFollowingPlayer();
            StartCoroutine("TouchCooldown");
        }
    }

    public void PlayEntranceSound()
    {
        audioSource.PlayOneShot(entranceSound);
    }

    public void InstantiateHP()
    {
        //ThemeMusicManager.Instance.StartDrums();
        GameObject c = GameObject.FindGameObjectWithTag("HUDCanvas");
        healthBarInstance = Instantiate(healthBarPrefab, c.transform);
        HealthBar h = healthBarInstance.GetComponentInChildren<HealthBar>();
        h.type = TrackType.Enemy;
        h.SetupBar(health);
        health.SetInvulnerability(false);
        ftpd.SetActivation(true);
        animator.applyRootMotion = true;
        agent.isStopped = false;

        started = true;
    }

    IEnumerator FollowPlayer()
    {
        StartFollowingPlayer();
        yield return new WaitForSeconds(5f);
        StopFollowingPlayer();
    }

    void StartFollowingPlayer()
    {
        if (GetNearPlayer() != null)
        {
            isFollowing = true;
        }
    }

    void StopFollowingPlayer()
    {
        agent.destination = transform.position;
        
        if (health.GetCurrentHealth() > 0)
        {
            ToggleFollowCooldown();
            Invoke("ToggleFollowCooldown", followCooldown);
            Invoke("PrepareToFireCircle", 1f);
        }

        isFollowing = false;
    }

    void ToggleFollowCooldown()
    {
        isFollowCooldown = !isFollowCooldown;
    }

    void PrepareToFireCircle()
    {
        if (!isDead)
        {
            if (isFurious)
            {
                ChangeAnimationState("Queen_furious_attack");
            }
            else
            {
                ChangeAnimationState("Queen_attack");
            }
        }
    }

    public void GoToIdle()
    {
        if (!isDead)
        {
            if (isFurious)
            {
                ChangeAnimationState("Queen_furious_idle");
            }
            else
            {
                ChangeAnimationState("Queen_idle");
            }
        }
    }

    public void FireCircle()
    {
        RadiusFire(projectileAmount);
    }

    public override void onHealthEnd()
    {
        base.onHealthEnd();
        isDead = true;

        ChangeAnimationState("Queen_death");
        StopFollowingPlayer();
        agent.speed = 0;

        //DropDNA(1);
    }

    public void DestroyOnDeath()
    {
        Destroy(gameObject);
    }

    void LayLarvae()
    {
        if (GetLiveLarvaeCount() < maxLarvaes)
        {
            audioSource.PlayOneShot(spawningSound);
            GameObject l = Instantiate(larvaePrefab, transform.position, Quaternion.identity, transform.parent);
            Health lh = l.GetComponent<Health>();
            lh.toTrack = l;
            Enemy larvae = l.GetComponent<Enemy>();
            larvae.SetCurrentRoom(enemy.currentRoom);
            larvaes.Add(larvae);
        }
    }

    void RadiusFire(int projectilesCount)
    {
        Vector2 startPoint = transform.position;
        float angleStep = 360f / projectilesCount;
        float angle = 0f;

        for (int i = 0; i <= projectilesCount - 1; i++)
        {

            float projectileDirXposition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * fireRadius;
            float projectileDirYposition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * fireRadius;

            Vector2 projectileVector = new Vector2(projectileDirXposition, projectileDirYposition);
            Vector2 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;

            var proj = Instantiate(projectilePrefab, startPoint, Quaternion.identity);
            if (i != 0)
            {
                proj.GetComponent<AudioSource>().mute = true;
            }
            proj.GetComponent<Rigidbody2D>().velocity =
                new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

            angle += angleStep;
        }

        if (enemy) enemy.PlayAttackSound();
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;

    }

    public void SetAggressive()
    {
    }

    IEnumerator Lay()
    {
        isLaying = true;
        LayLarvae();
        yield return new WaitForSeconds(layCooldown);
        isLaying = false;
    }

    IEnumerator TouchCooldown()
    {
        justTouched = true;
        yield return new WaitForSeconds(touchCooldown);
        justTouched = false;
    }

    public void OnHitted()
    {

    }
}
