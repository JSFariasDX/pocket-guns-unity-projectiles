using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Boss : MonoBehaviour
{
    protected Health health;
    protected Enemy enemy;
    protected Animator animator;
    protected AudioSource audioSource;
    protected List<Player> players;

    private CinemachineTargetGroup.Target _camTarget;
    private bool _camTargetIsSet;

    [Header("Health bar")]
    [SerializeField] protected GameObject healthBarPrefab;
    protected GameObject healthBarInstance;

    [Header("DNA")]
    public GameObject DNAPrefab;
    public int DNADropAmount = 1;
    bool dropped = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = GetComponent<Health>();
        enemy = GetComponent<Enemy>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        players = GameplayManager.Instance.GetPlayers(true);

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        
    }

    protected void SetCinemachineTarget(float radius = 2)
    {
        if (_camTargetIsSet)
            return;

        _camTarget = FindObjectOfType<CameraManager>().AddTarget(transform, radius, 0.75f);
        _camTargetIsSet = true;
    }

    protected void DisableCinemachineTarget()
    {
        FindObjectOfType<CameraManager>().DisableTarget(_camTarget);
        _camTargetIsSet = false;
    }

    protected void SetHealthBar(Health health)
    {
        GameObject c = GameObject.FindGameObjectWithTag("HUDCanvas");
        healthBarInstance = Instantiate(healthBarPrefab, c.transform);
        HealthBar h = healthBarInstance.GetComponentInChildren<HealthBar>();
        h.type = TrackType.Enemy;
        h.SetupBar(health);
    }

    protected void StartBossTheme()
    {
        //ThemeMusicManager.Instance.SetTheme(enemy.currentRoom.GetOST());
        //ThemeMusicManager.Instance.StartTheme(true);
        MusicManager.Instance.StartBossTheme();
    }

    public virtual void DestroyBoss()
    {
        onHealthEnd();
        Destroy(gameObject);
    }

    public virtual void Die()
    {
        DropDNA(3);
        var bossRoom = FindObjectOfType<BossRoom>();
        
        if (bossRoom != null)
            bossRoom.FinishRoom();

        DisableCinemachineTarget();
        Destroy(gameObject);
    }

    public virtual void onHealthEnd()
    {
        DropDNA(3);
        Destroy(healthBarInstance.gameObject);
    }

    public virtual void DropDNA(float dropForce)
    {
        if (!dropped)
        {
            for (int i = 0; i < DNADropAmount; i++)
            {
                GameObject coin = Instantiate(DNAPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

                float randomAngle = Random.Range(0f, 6.28319f); //radians
                Vector2 randomVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

                rb.AddForce(randomVector.normalized * dropForce, ForceMode2D.Impulse);
            }
            dropped = true;
        }
    }

    public Player GetNearPlayer()
	{
        return enemy.GetNearPlayer();
	}

    public Player GetRandomPlayer()
	{
        return enemy.GetRandomPlayer();
    }

    public Enemy GetEnemy() { return enemy; }
}
