using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalProjectile : Projectile
{
    [Header("Orbital Settings")]
    [SerializeField] private Vector2 centerOffset;
    [SerializeField] private float rotationTimer = 2f;
    [SerializeField] float rotateSpeed;
    [SerializeField] float maxDistance = 3;
    float _angle;

    Vector2 startPosition;
    private float _count;

    private Collider2D _collider2D;
    private AutoDestroyAfterTime _autoDestroy;

    protected override void Awake()
    {
        base.Awake();
        _collider2D = GetComponent<Collider2D>();
        _autoDestroy = GetComponent<AutoDestroyAfterTime>();
    }

    public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
    {
        base.Setup(targetPosition, damage, speed, shooterTransform);
        startPosition = transform.position;
        _angle = Toolkit2D.GetAngleBetweenTwoPoints(transform.position, targetPosition) * Mathf.Deg2Rad;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DestructibleObstacle") || other.CompareTag("UnbreakableWall") || other.CompareTag("Gate"))
        {
            _collider2D.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DestructibleObstacle") || other.CompareTag("UnbreakableWall") || other.CompareTag("Gate"))
        {
            _collider2D.enabled = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_count <= rotationTimer)
        {
            var delta = Time.deltaTime / GetMaxDistance();
            _count += delta;
            return;
        }

        var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle)) * maxDistance;
        if (shooterTransform != null)
            transform.position = shooterTransform.position + (Vector3)centerOffset + offset;

        _angle += rotateSpeed * Time.deltaTime;
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public void DecreaseMaxDistance(float value)
    {
        maxDistance -= value;
    }

    public void SetAutoDestroyMultiplier(float multiplier)
    {
        _autoDestroy.SetAutoDestroy(multiplier);
    }
}
