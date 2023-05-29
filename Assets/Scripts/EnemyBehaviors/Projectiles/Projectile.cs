using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType
    {
        Orange, Pink, Blue, Cross, Star
    }

    public float damage = 15f;
    [HideInInspector] public float actualSpeed = 0;

    [Header("Projectile Settings")]
    public ProjectileType type;
    public Gradient projectileColor;
    protected Transform shooterTransform;
    public bool onHitObstacleIndestructible;
    public GameObject muzzleFlashPrefab;
    public GameObject blastParticle;

    [Header("Effect Settings")]
    public bool dashStun;
    public GameObject hitPlayerParticle;


    [Header("Animation Settings")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    Animator anim;
    string currentState;

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer != null)
            anim = spriteRenderer.GetComponent<Animator>();
    }

    private void Start()
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, transform.position, Quaternion.identity);
    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
	{
        this.damage = damage;

        this.shooterTransform = shooterTransform;
        Toolkit2D.RotateAt(transform, targetPosition);

        actualSpeed = speed;

        rb.velocity = transform.right * speed;

        ConfigureColorsAndAnimation();
    }

    public virtual void Setup(Transform target, float damage, float speed, Transform shooterTransform)
	{
        this.damage = damage;

        this.shooterTransform = shooterTransform;
        Toolkit2D.RotateAt(transform, target.position);

        actualSpeed = speed;

        rb.velocity = transform.right * speed;

        ConfigureColorsAndAnimation();
    }

    protected void ConfigureColorsAndAnimation()
    {
        //var color = GetComponentInChildren<ParticleSystem>().colorOverLifetime;
        //color.color = projectileColor;

        //switch (type)
        //{
        //    case ProjectileType.Orange:
        //        ChangeAnimationState("orange_projectile");
        //        break;
        //    case ProjectileType.Pink:
        //        ChangeAnimationState("pink_projectile");
        //        break;
        //    case ProjectileType.Blue:
        //        ChangeAnimationState("blue_projectile");
        //        break;
        //    case ProjectileType.Cross:
        //        ChangeAnimationState("cross_projectile");
        //        break;
        //    case ProjectileType.Star:
        //        ChangeAnimationState("star_projectile");
        //        break;
        //    default:
        //        ChangeAnimationState("pink_projectile");
        //        break;
        //}
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (onHitObstacleIndestructible && other.CompareTag("DestructibleObstacle"))
            return;
        
        if (other.CompareTag("DestructibleObstacle") || other.CompareTag("UnbreakableWall") || other.CompareTag("Gate"))
        {
            OnTrigger();
        }
    }

    public virtual void OnTrigger()
    {
        GameObject blast = Instantiate(blastParticle, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}