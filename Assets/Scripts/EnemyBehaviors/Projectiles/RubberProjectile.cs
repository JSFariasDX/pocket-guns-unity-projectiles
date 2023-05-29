using UnityEngine;

public class RubberProjectile : Projectile
{
    [Header("Dependencies")]
    [SerializeField] private Collider2D bulletCollider;
    [SerializeField] private GameObject light2D;
    [SerializeField] private GameObject trail;

    [Header("Rubber Settings")]
    [SerializeField] private Material unsaturatedMaterial;
    [SerializeField] private float knockbackSpeed;
    [SerializeField] private float fallingTime;
    [SerializeField] private float timeToDestroy;

    [Header("Shadow Settings")]
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private float moveToShadowSpeed;
    [SerializeField] private Vector2 offset;

    private bool _isInactive;
    private float _fallTimer;
    private bool _willDestroy;

    protected override void FixedUpdate()
    {
        if (!_isInactive)
            return;

        if (_fallTimer < fallingTime)
        {
            spriteRenderer.transform.localPosition = Vector3.MoveTowards(spriteRenderer.transform.localPosition, shadowTransform.localPosition + (Vector3)offset, moveToShadowSpeed * Time.deltaTime);
            _fallTimer += Time.deltaTime;
            return;
        }

        if (!_willDestroy)
        {            
            Destroy(gameObject, timeToDestroy);
            _willDestroy = true;
        }
    }

    private void SetInactive()
    {
        rb.velocity = Vector2.zero;
        _isInactive = true;
        bulletCollider.enabled = false;
        spriteRenderer.material = unsaturatedMaterial;
        light2D.SetActive(false);
        trail.SetActive(false);
    }

    public override void OnTrigger()
    {
        SetInactive();
    }

    public Vector2 GetKnockbackForce(Transform other, Vector2 cameraModifier)
    {
        var direction = (other.transform.position - transform.position) * cameraModifier;
        return direction.normalized * knockbackSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Pocket>(out Pocket pocket))
        {
            if (!pocket.GetHealth().isInvulnerable)
                SetInactive();
        }

        base.OnTriggerEnter2D(other);
    }
}
