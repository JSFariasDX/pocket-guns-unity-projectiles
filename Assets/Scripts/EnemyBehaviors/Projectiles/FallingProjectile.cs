using UnityEngine;

public class FallingProjectile : Projectile
{
    [Header("Falling Settings")]
    [SerializeField] private Vector2 speedModifierRange;
    [SerializeField] private float upDirectionWeight;
    [SerializeField] private float gravityForce;
    [SerializeField] private float stopForce;
    [SerializeField] private float fallingTime;
    [SerializeField] private float timeToDestroy;

    [Header("Shadow Settings")]
    [SerializeField] private Collider2D bulletCollider;
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private float moveToShadowSpeed;
    [SerializeField] private Vector2 offset;

    private float _fallTimer;
    private bool _canFall;
    private bool _willDestroy;

    private WindObject _windObject;

    protected override void Awake()
    {
        base.Awake();

        _windObject = GetComponent<WindObject>();
        if (_windObject) _windObject.enabled = false;
    }

    protected override void FixedUpdate()
    {
        if (!_canFall)
            return;

        if (_fallTimer < fallingTime)
        {
            rb.AddForce(Vector2.down * gravityForce);
            spriteRenderer.transform.localPosition = Vector3.MoveTowards(spriteRenderer.transform.localPosition, shadowTransform.localPosition + (Vector3)offset, moveToShadowSpeed * Time.deltaTime);
            bulletCollider.offset = new Vector2(0f, spriteRenderer.transform.localPosition.y);
            _fallTimer += Time.deltaTime;
            return;
        }

        var velocity = rb.velocity;
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, stopForce * Time.deltaTime);
        rb.velocity = velocity;

        if (!_willDestroy)
        {
            if (_windObject)
                _windObject.enabled = true;
            
            Destroy(gameObject, timeToDestroy);
            _willDestroy = true;
        }
    }

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        this.damage = damage;

        this.shooterTransform = shooterTransform;
        Toolkit2D.RotateAt(spriteRenderer.transform, targetPosition);

        var direction = spriteRenderer.transform.right * speed * Random.Range(speedModifierRange.x, speedModifierRange.y);
        direction += Vector3.up * upDirectionWeight;
        rb.velocity = direction;

        ConfigureColorsAndAnimation();

        _canFall = true;
    }
}