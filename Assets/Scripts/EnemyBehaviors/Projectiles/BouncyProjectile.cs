using UnityEngine;

public class BouncyProjectile : Projectile
{
    [Header("Bouncy Settings")]
    [SerializeField] private int bounceCount;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask bounceMask;

    private int _bounceCounter;

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        base.Setup(targetPosition, damage, speed, shooterTransform);

        _bounceCounter = 0;
    }

    private void Bounce()
    {
        if (_bounceCounter < bounceCount || bounceCount == 0)
        {
            _bounceCounter++;
            //ChangeDirection();
            return;
        }

        OnTrigger();
    }

    private void ChangeDirection()
    {
        var upRay = Physics2D.Raycast(transform.position, Vector2.up, rayDistance, bounceMask);
        var rightRay = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, bounceMask);
        var downRay = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, bounceMask);
        var leftRay = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, bounceMask);

        Vector2 normal;

        if (upRay)
            normal = Vector2.down;
        else if (rightRay)
            normal = Vector2.left;
        else if (downRay)
            normal = Vector2.up;
        else if (leftRay)
            normal = Vector2.right;
        else
            normal = Vector2.zero;

        if (normal != Vector2.zero)
        {
            var newVelocity = Vector2.Reflect(rb.velocity, normal);
            rb.velocity = newVelocity;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DestructibleObstacle") || other.CompareTag("UnbreakableWall") || other.CompareTag("Gate"))
        {
            Bounce();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("DestructibleObstacle") ||other.transform.CompareTag("UnbreakableWall") || other.transform.CompareTag("Gate"))
        {
            Bounce();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, Vector2.up * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.right * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.down * rayDistance);
        Gizmos.DrawRay(transform.position, Vector2.left * rayDistance);
    }
}
