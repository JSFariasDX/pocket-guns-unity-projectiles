using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class SinisterQueen : Boss
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float horizontalRayDistance;

    [Header("Attack Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Projectile twoStepsProjectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float singleShootInterval;
    [SerializeField] private float randomShootCount;
    [SerializeField] private float randomShootInterval;
    [SerializeField] private Vector2 targetOffset;

    [Header("Spawn Settings")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private SpawnParticle spawnParticlePrefab;
    [SerializeField] private int spawnedEnemiesCount;

    [Header("Floating Settings")]
    public bool canFloat = true;
    public AnimationCurve floatCurve;
    [Range(0, 5)]
    public float curveSpeed = 1;
    float curveTime = 0;

    private List<Enemy> _spawnedEnemies;

    private Vector3 _horizontalDirection;

    private Coroutine _movementCoroutine;
    private Coroutine _attackCoroutine;

    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private NavMeshAgent _agent;

    bool furious = false;

    [Header("Phase 2")]
    public GameObject shadowFigurePrefab;

    [Header("Random Shots")]
    public float attackDuration = 5;
    float attackTimer;
    public float attackInterval = .05f;
    bool isRandomAttacking = false;
    public Transform targetTest;

    [Header("Spiral")]
    public int projectilesSpiralCount = 15;
    float angle = 0;
    float angleStep;
    public int shotsToFire;
    int shotsFired;
    public float spiralShotsInterval = .15f;

    bool isFollowing = false;

    private void Awake()
    {
        _spawnedEnemies = new();

        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        //StartCoroutine(FirstAttackTest());
        //Activate();

        SetCinemachineTarget();

        base.Start();
        SetHealthBar(health);
        StartBossTheme();

        gameObject.SetActive(false);

    }

    IEnumerator FirstAttackTest()
    {
        print("|————————————————————| QUEEN: SPIRAL SHOTS");
        for (int i = 0; i < shotsToFire; i++)
        {
            SpiralShots();
            angle += angleStep;
            shotsFired++;
            yield return new WaitForSeconds(spiralShotsInterval);
        }
    }

    public void Activate()
    {
        _movementCoroutine = StartCoroutine(HorizontalMovement());
        _attackCoroutine = StartCoroutine(Level2Attack());
        _horizontalDirection = Vector3.right;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);

        if (canFloat)
        {
            if (curveTime <= 1) curveTime += Time.deltaTime * curveSpeed;
            else curveTime = 0;

            transform.GetChild(0).localPosition = new Vector3(0, floatCurve.Evaluate(curveTime), 0);
            GetComponent<Collider2D>().offset = new Vector2(0, floatCurve.Evaluate(curveTime) + -.25f);
        }

        if (furious)
        {
            if ((GetNearPlayer() != null))
            {
                _agent.destination = GetNearPlayer().transform.position;
            }
        }

        if (isRandomAttacking)
        {
            if (attackTimer < attackDuration)
                attackTimer += Time.deltaTime;
        }
    }

    private IEnumerator HorizontalMovement()
    {
        while (!furious)
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, horizontalRayDistance, LayerMask.GetMask("Walls")))
                _horizontalDirection = Vector3.left;
            else if (Physics2D.Raycast(transform.position, Vector2.left, horizontalRayDistance, LayerMask.GetMask("Walls")))
                _horizontalDirection = Vector3.right;

            _agent.destination = transform.position + _horizontalDirection;
            yield return null;
        }
    }

    private IEnumerator Level1Attack()
    {
        while (true)
        {
            _agent.isStopped = false;
            int singleCounter = 0;

            while (singleCounter < 3)
            {
                yield return new WaitForSeconds(singleShootInterval);
                SingleShoot(Vector3.zero, true);
                singleCounter++;
            }

            yield return new WaitForSeconds(Random.Range(1.5f, 3f));

            _agent.isStopped = true;
            var chosenAttack = Random.value;

            if (chosenAttack <= 0.5f)
            {
                // Attack 2
                yield return new WaitForSeconds(randomShootInterval);
            }
            else
            {
                for (int i = 0; i < randomShootCount; i++)
                {
                    RandomShoot(GetEnemy().GetTarget().position);
                    yield return new WaitForSeconds(randomShootInterval);
                }
            }
            
            _agent.isStopped = false;
        }
    }

    private IEnumerator Level2Attack()
    {
        while (true)
        {
            _agent.isStopped = false;
            int singleCounter = 0;

            while (singleCounter < 3)
            {
                yield return new WaitForSeconds(singleShootInterval);
                SingleShoot(Vector3.zero, true);
                singleCounter++;
            }

            yield return new WaitForSeconds(Random.Range(1.5f, 3f));

            _agent.isStopped = true;
            var chosenAttack = Random.value;

            if (chosenAttack < 0.2f && !HaveEnemiesRemaining())
            {
                for (int i = 0; i < spawnedEnemiesCount; i++)
                {
                    SpawnShadows();
                    yield return new WaitForSeconds(randomShootInterval);
                }

                yield return new WaitForSeconds(randomShootInterval);
            }
            else if (chosenAttack >= 0.2f && chosenAttack < 0.4f)
            {
                // Attack 4

                for (int i = 0; i < randomShootCount; i++)
                {
                    RandomShoot(GetEnemy().GetTarget().position);
                    yield return new WaitForSeconds(attackInterval);
                }
            }
            else if (chosenAttack >= 0.4f && chosenAttack < 0.6f)
            {
                // Attack 5

                angle = 0;
                shotsFired = 0;

                for (int i = 0; i < shotsToFire; i++)
                {
                    SpiralShots();
                    angle += angleStep;
                    shotsFired++;
                    yield return new WaitForSeconds(spiralShotsInterval);
                }
            }
            else if(chosenAttack >= 0.6f && chosenAttack < 0.8f)
            {
                // Attack 6

                for (int i = 0; i < randomShootCount; i++)
                {
                    RandomShoot(GetEnemy().GetTarget().position, true);
                    yield return new WaitForSeconds(randomShootInterval / 2);
                }
            }
            else
            {
                // Attack 7

                for (int i = 0; i < randomShootCount; i++)
                {
                    RandomShoot(Vector3.zero);
                    yield return new WaitForSeconds(randomShootInterval);
                }
            }

            _agent.isStopped = false;
        }
    }

    private void SingleShoot(Vector3 shootDirection, bool twoStepsProjectile = false)
    {
        if (shootDirection == Vector3.zero)
            shootDirection = GetEnemy().GetTarget().position;

        animator.SetTrigger("Shoot");
        Projectile projectile = Instantiate(twoStepsProjectile ? twoStepsProjectilePrefab : projectilePrefab, shootPoint.position, Quaternion.identity);
        projectile.Setup(shootDirection, projectileDamage, projectileSpeed, transform);
    }

    private void RandomShoot(Vector3 target, bool twoStepsProjectile = false)
    {
        Vector3 shootDirection;

        if (target == Vector3.zero)
            shootDirection = transform.position + (Vector3)Random.insideUnitCircle.normalized;
        else
        {
            var randomX = Random.Range(-targetOffset.x, targetOffset.x);
            var randomY = Random.Range(-targetOffset.y, targetOffset.y);
            var offset = new Vector3(randomX, randomY, 0f);
            shootDirection = target + offset;
        }

        SingleShoot(shootDirection, twoStepsProjectile);
    }

    IEnumerator TrailOfShots()
    {
        Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        projectile.Setup(transform.position, projectileDamage, 0, transform);

        Destroy(projectile.gameObject, 3);

        yield return new WaitForSeconds(.5f);

        StartCoroutine(TrailOfShots());
    }

    IEnumerator RandomAimShots()
    {
        isRandomAttacking = true;

        while (attackTimer <= attackDuration)
        {
            Projectile projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            float randomX = Random.Range(-targetOffset.x, targetOffset.x);
            float randomY = Random.Range(-targetOffset.y, targetOffset.y);
            var offset = new Vector3(randomX, randomY, 0f);

            projectile.Setup(targetTest.position + offset, projectileDamage, projectileSpeed, transform.parent);

            yield return new WaitForSeconds(attackInterval);
        }

        isRandomAttacking = false;
    }

    void SpiralShots()
    {
        Vector2 startPoint = transform.position;
        angleStep = 360f / projectilesSpiralCount;

        float projectileDirXposition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * 5;
        float projectileDirYposition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * 5;

        Vector2 projectileVector = new Vector2(projectileDirXposition, projectileDirYposition);

        SingleShoot(projectileVector);
    }

    private void SpawnShadows()
    {
        var spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, null);
        var spawnParticle = Instantiate(spawnParticlePrefab, spawnedEnemy.transform.position, Quaternion.identity);
        spawnParticle.SetupEnemySpawn(spawnedEnemy);

        spawnedEnemy.SetCurrentRoom(enemy.currentRoom);
        _spawnedEnemies.Add(spawnedEnemy);
    }

    private bool HaveEnemiesRemaining()
    {
        if (_spawnedEnemies.Count == 0)
            return false;

        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (_spawnedEnemies[i] != null)
                continue;

            _spawnedEnemies.Remove(_spawnedEnemies[i]);
        }

        return _spawnedEnemies.Count > 0;
    }

    public void OnHitted()
    {
        if (health.IsDamageEnabled())
        {
            StartCoroutine(ActiveHitFilter());
            if (health.GetCurrentPercentage() <= .5f)
            {
                if(!furious)
                    StartCoroutine(TrailOfShots());

                furious = true;
            }
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
        GameObject shadow = Instantiate(shadowFigurePrefab, transform.position, Quaternion.identity, transform.parent);

        //shadow.GetComponent<Collider2D>().enabled = false;

        var agent = shadow.GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        foreach (var item in _spawnedEnemies)
        {
            item.GetHealth().Decrease(9999);
        }

        Enemy boss = shadow.GetComponent<Enemy>();
        boss.currentRoom = enemy.currentRoom;
        enemy.currentRoom.enemies.Add(boss);

        FindObjectOfType<SinisterBossRoom>().sinisterShadow = shadow;
        FindObjectOfType<SinisterBossRoom>().SetupScene();

        _collider2D.enabled = false;
		base.onHealthEnd();
		animator.SetTrigger("Die");
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.left * horizontalRayDistance);
        Gizmos.DrawRay(transform.position, Vector3.right * horizontalRayDistance);
    }
}
