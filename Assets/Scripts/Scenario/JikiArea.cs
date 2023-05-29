using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JikiArea : MonoBehaviour
{
    public bool IsAttracting { get; private set; }
    public float Force { get; private set; }

    [field: SerializeField] private Vector2 forceRange;
    [SerializeField] private Material attractMaterial;
    [SerializeField] private Material repelMaterial;
    [SerializeField] JikiCharge charge;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        charge = GetComponentInParent<JikiCharge>();
    }

    public void SetupJiki(bool isAttracting)
    {
        Force = Random.Range(forceRange.x, forceRange.y);
        
        IsAttracting = isAttracting;
        _spriteRenderer.material = IsAttracting ? attractMaterial : repelMaterial;
    }

    public float GetForceRatio()
    {
        var ratio = Mathf.InverseLerp(forceRange.x, forceRange.y, Force);
        return 1f + ratio;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            collision.GetComponentInParent<JikiObject>().AddJiki(this);
        }
        else
        {
            if (collision.TryGetComponent(out JikiObject jikiObject))
            {
                jikiObject.AddJiki(this);
            }
        }
    }

    public JikiCharge GetCharge()
    {
        return charge;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider"))
        {
            collision.GetComponentInParent<JikiObject>().RemoveJiki(this);
            return;
        }
        else
        {
            if (collision.TryGetComponent<JikiObject>(out JikiObject jikiObject))
            {
                jikiObject.RemoveJiki(this);
            }
        }
    }
}
