using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CrabState
{
    Rise,
    Roar,
    Idle,
    Move,
    Attack1,
    Attack2,
    Attack3,
    Attack4
}

public class Crab : Boss
{
    public Animator Animator => animator;
    public AudioSource AudioSource => audioSource;

    public CrabState CurrentState { get; set; }
    public Coroutine CurrentCoroutine { get; set; }

    [Header("Attack Dependencies")]
    [SerializeField] private CrabLaserAttack attack1;
    [SerializeField] private CrabShootAttack attack2;
    [SerializeField] private CrabShootAttack attack3;
    [SerializeField] private CrabRandomAttack attack4;
    [SerializeField] private CrabRadialAttack attack5;

    [Header("Position Dependencies")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform shootPoint;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private SpawnParticle spawnParticlePrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private float spawnDelay;

    [Header("Start Settings")]
    [SerializeField] private Vector3 startPositionOffset;
    [SerializeField, Range(0f, 1f)] private float percentageToRise;
    [SerializeField] private float startDuration;

    [Header("Movement Settings")]
    [SerializeField] private float horizontalDuration;

    [Header("Timing Settings")]
    [SerializeField] private float idleInterval;

    [Header("VFX Settings")]
    [SerializeField] private ParticleSystem waterParticles;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip roarClip;

    private List<Enemy> _spawnedEnemies;
    private List<Projectile> _bouncyProjectiles;

    private bool _isActive;
    private bool _isAboveWater;
    private float _nextHealthQuarter;
    private bool _canPerformAttack3;

    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    protected override void Start()
    {
        _nextHealthQuarter = 0.75f;

        _spawnedEnemies = new();
        _bouncyProjectiles = new();

        CurrentState = CrabState.Rise;
        transform.position += startPositionOffset;

        DetachPoints();
        transform.position = startPoint.position;

        base.Start();
        SetHealthBar(health);
        
        StartBossTheme();

        SetAboveWater(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        CheckCurrentState();
    }

    private void CheckCurrentState()
    {
        if (CurrentCoroutine != null)
            return;

        if (health.GetCurrentPercentage() <= _nextHealthQuarter)
        {
            _canPerformAttack3 = true;
            CurrentState = CrabState.Attack3;
        }

        switch (CurrentState)
        {
            case CrabState.Rise: CurrentCoroutine = StartCoroutine(MoveCoroutine(centerPoint.position, startDuration, CrabState.Roar, true)); break;
            case CrabState.Roar: CurrentCoroutine = StartCoroutine(SpawnEnemies()); break;
            case CrabState.Idle: CurrentCoroutine = StartCoroutine(IdleCoroutine(DecideNextState())); break;
            case CrabState.Move: CurrentCoroutine = StartCoroutine(MoveCoroutine(DecideNextPosition(), horizontalDuration, CrabState.Idle)); break;
            
            case CrabState.Attack1:
                if (Vector3.Distance(transform.position, centerPoint.position) <= 1f)
                    CurrentCoroutine = StartCoroutine(attack1.LaserAttack());
                else
                    CurrentCoroutine = StartCoroutine(MoveCoroutine(centerPoint.position, horizontalDuration, CrabState.Attack1));
                break;

            case CrabState.Attack2:
                CurrentCoroutine = StartCoroutine(attack2.NormalShoot());
                break;

            case CrabState.Attack3:
                if (_canPerformAttack3)
                {
                    StartCoroutine(attack3.NormalShoot(_bouncyProjectiles));
                    _nextHealthQuarter -= 0.25f;
                    _canPerformAttack3 = false;
                }
                break;

            case CrabState.Attack4:
                CurrentCoroutine = StartCoroutine(attack4.RandomShoot());
                break;
        }
    }

    private CrabState DecideNextState()
    {
        var r = Random.value;

        if (r < 0.36f)
            return CrabState.Move;
        
        if (r < 0.58f)
            return CrabState.Attack1;

        if (r < 0.79f)
            return CrabState.Attack2;

        return CrabState.Attack4;
    }

    private Vector3 DecideNextPosition()
    {
        if (Vector3.Distance(transform.position, leftPoint.position) <= 1f)
            return rightPoint.position;
        else
            return leftPoint.position;
    }

    private IEnumerator IdleCoroutine(CrabState nextState)
    {
        animator.SetBool("Dive", false);
        animator.SetBool("Move", false);
        yield return new WaitForSeconds(idleInterval);
        attack5.CanPerformAttack = false;
        CurrentState = nextState;
        CurrentCoroutine = null;
    }

    private IEnumerator MoveCoroutine(Vector3 targetPosition, float moveDuration, CrabState nextState, bool isStarting = false)
    {
        var waitForNextFrame = new WaitForSeconds(Time.fixedDeltaTime);

        var timeElapsed = 0f;
        var startPosition = transform.position;

        var useHalfDuration = Vector3.Distance(transform.position, centerPoint.position) <= 1f || targetPosition == centerPoint.position;
        var duration = useHalfDuration ? moveDuration * 0.5f : moveDuration;

        while (timeElapsed < duration)
        {
            if (!isStarting)
                EmergeAndSinkDuringMovement(targetPosition, timeElapsed, duration);
            // animator.SetBool("Move", true);

            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);

            if (targetPosition == centerPoint.position && Vector3.Distance(transform.position, leftPoint.position) <= 1f)
                animator.SetFloat("Direction", 1);
            else if (targetPosition == centerPoint.position && Vector3.Distance(transform.position, rightPoint.position) <= 1f)
                animator.SetFloat("Direction", 0);

            if (!_isActive && timeElapsed >= duration * percentageToRise)
            {
                _isActive = true;
                animator.SetBool("Active", true);
            }

            timeElapsed += Time.fixedDeltaTime;
            yield return waitForNextFrame;
        }

        transform.position = targetPosition;
        CurrentState = nextState;
        CurrentCoroutine = null;
    }

    private void EmergeAndSinkDuringMovement(Vector3 target, float timeElapsed, float duration)
    {
        if (duration < horizontalDuration)
        {
            SetMovementDirection(target);                
            return;
        }

        attack5.CanPerformAttack = true;
        var direction = target == leftPoint.position ? 0 : 1;
        animator.SetFloat("Direction", direction);

        if (timeElapsed / duration < 0.33f)
        {
            animator.SetBool("Dive", true);
            animator.SetBool("Move", false);
            return;
        }

        if (timeElapsed / duration < 0.66f)
        {
            animator.SetBool("Dive", false);
            animator.SetBool("Move", true);
            return;
        }

        animator.SetBool("Dive", true);
        // animator.SetBool("Move", false);
    }

    private void SetMovementDirection(Vector3 target)
    {
        animator.SetBool("Dive", false);
        animator.SetBool("Move", true);

        if (target != centerPoint.position)
        {
            var direction = target == leftPoint.position ? 0 : 1;
            animator.SetFloat("Direction", direction);
            return;
        }

        if (Vector3.Distance(transform.position, leftPoint.position) <= 1f)
            animator.SetFloat("Direction", 1);
        else if (Vector3.Distance(transform.position, rightPoint.position) <= 1f)
            animator.SetFloat("Direction", 0);
    }

    private IEnumerator SpawnEnemies()
    {
        SetCinemachineTarget();
        yield return new WaitForSeconds(spawnDelay);
        var enemiesRemaining = 0;

        animator.SetTrigger("Roar");
        audioSource.PlayOneShot(roarClip);
        FindObjectOfType<CinemachineImpulseSource>().GenerateImpulse();

        foreach (Transform point in spawnPoints)
        {
            var spawnedEnemy = Instantiate(enemyPrefab, point.position, Quaternion.identity, null);
			var spawnParticle = Instantiate(spawnParticlePrefab, spawnedEnemy.transform.position, Quaternion.identity);
			spawnParticle.SetupEnemySpawn(spawnedEnemy);

			_spawnedEnemies.Add(spawnedEnemy);
			spawnedEnemy.SetCurrentRoom(enemy.currentRoom);
            enemiesRemaining++;
        }

        animator.SetBool("Dive", true);
        DisableCinemachineTarget();
        

        while (enemiesRemaining > 0)
        {
            yield return new WaitForSeconds(1f);

            for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (_spawnedEnemies[i] != null)
                    continue;

                _spawnedEnemies.Remove(_spawnedEnemies[i]);
                enemiesRemaining--;
            }
        }

        animator.SetBool("Dive", false);
        SetCinemachineTarget();
        CurrentState = CrabState.Idle;
        CurrentCoroutine = null;
    }

    private void DetachPoints()
    {
        startPoint.SetParent(transform.parent);
        leftPoint.SetParent(transform.parent);
        centerPoint.SetParent(transform.parent);
        rightPoint.SetParent(transform.parent);

        foreach (Transform point in spawnPoints)
        {
            point.SetParent(transform.parent);
        }
    }

    public void OnHitted()
    {
        if (health.IsDamageEnabled())
        {
            StartCoroutine(ActiveHitFilter());
        }
    }

    IEnumerator ActiveHitFilter()
    {
        _spriteRenderer.material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.material.SetInt("_Hit", 0);
    }

    public override void onHealthEnd()
	{
        for (int i = _bouncyProjectiles.Count - 1; i >= 0; i--)
        {
            if (_bouncyProjectiles[i] == null)
                continue;

            Destroy(_bouncyProjectiles[i].gameObject);
            _bouncyProjectiles.RemoveAt(i);
        }
        _bouncyProjectiles.Clear();

        attack1.IsFiringLaser = false;
        ActivateLaser(false);
        _collider2D.enabled = false;
		base.onHealthEnd();
		animator.SetTrigger("Die");
	}

    public void SetAboveWater(bool value)
    {
        var health = enemy.GetHealth();
        if (health != null)
            health.SetDamageEnabled(value);

        _isAboveWater = value;
        _collider2D.enabled = value;

        if (value)
            waterParticles.Stop();
        else
            waterParticles.Play();
    }

    public void SetIsFiringLaser(bool value)
    {
        attack1.IsFiringLaser = value;
    }

    public void ActivateLaser(bool value)
    {
        attack1.ActiveLaser(value);
    }

    public void RisingAttack()
    {
        if (!attack5.CanPerformAttack)
            return;
        
        attack5.RadialShoot();
    }
}
