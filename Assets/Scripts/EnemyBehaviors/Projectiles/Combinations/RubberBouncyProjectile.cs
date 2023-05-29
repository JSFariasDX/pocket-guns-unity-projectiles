using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBouncyProjectile : Projectile
{
    [Header("Rubber Settings")]
    [SerializeField] private float knockbackSpeed;

    [Header("Bouncy Settings")]
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private int bounceCount;
    [SerializeField] private float bounceDelay;

    private int _threshold;
    private bool _canBounce;
    private int _bounceCounter;
    private float _bounceDelay;

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        base.Setup(targetPosition, damage, speed, shooterTransform);

        _bounceCounter = 0;
        _threshold = 0;
        _bounceDelay = 0f;
    }

    private void Update()
    {
        if (_bounceDelay < bounceDelay)
        {
            _bounceDelay += Time.deltaTime;
        }
    }

    public Vector2 GetKnockbackForce(Transform other, Vector2 cameraModifier)
    {
        var direction = Random.insideUnitCircle * cameraModifier;
        return direction.normalized * knockbackSpeed;
    }

    public override void OnTrigger()
    {
    }

    private void Bounce(Vector2 normalVector)
    {
        if (!_canBounce || _bounceDelay < bounceDelay)
            return;

        if (_bounceCounter < bounceCount || bounceCount == 0)
        {
            _bounceCounter++;
            ChangeDirection(normalVector);
            return;
        }

        OnTrigger();
    }

    private void ChangeDirection(Vector2 normalVector)
    {
        var newVelocity = Vector2.Reflect(rb.velocity, normalVector);
        
        if (Mathf.Abs(newVelocity.x) < 0.01f || Mathf.Abs(newVelocity.y) < 0.01f)
            newVelocity = Vector2.Reflect(rb.velocity, Random.insideUnitCircle.normalized);

        rb.velocity = newVelocity;

        _bounceDelay = 0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<UnityEngine.AI.NavMeshObstacle>(out UnityEngine.AI.NavMeshObstacle obstacle))
        {
            Bounce(other.contacts[0].normal);
            return;
        }

        if (other.gameObject.CompareTag("DestructibleObstacle") || other.gameObject.CompareTag("UnbreakableWall") || other.gameObject.CompareTag("Gate"))
        {
            Bounce(other.contacts[0].normal);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<UnityEngine.AI.NavMeshObstacle>(out UnityEngine.AI.NavMeshObstacle obstacle))
            return;

        _collider2D.enabled = true;
        _canBounce = true;
    }
}
