using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    bool isRotationSensitive = false;

    [Header("Culling")]
    [SerializeField] bool needsCulling = false;
    [SerializeField] float cullingDistance = 15;
    [SerializeField] SpriteRenderer secondaryRenderer;
    Transform targetGroup;

    private void Start()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        targetGroup = GameObject.Find("TargetGroup").transform;
    }

    private void Update()
    {
        float distance = Vector2.Distance(targetGroup.position, transform.position);

        if (needsCulling)
        {
            if (distance > cullingDistance)
            {
                spriteRenderer.enabled = false;
                if (secondaryRenderer)
                    secondaryRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = true;
                if (secondaryRenderer)
                    secondaryRenderer.enabled = true;
            }
        }
    }

    public void SetRotation(RelativePosition relativePosition)
    {
        if (!isRotationSensitive)
        {
            return;
        }
        switch (relativePosition)
        {
            case RelativePosition.Up:
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                return;
            case RelativePosition.Right:
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            case RelativePosition.Left:
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -180);
                return;
            default:
                gameObject.transform.rotation = Quaternion.Euler(0, 0, -90);
                return;
        }
    }

    public void FlipX(bool value)
    {
        if (!spriteRenderer)
        {
            return;
        }
        spriteRenderer.flipX = value;
    }

    public void FlipY(bool value)
    {
        if (!spriteRenderer)
        {
            return;
        }
        spriteRenderer.flipY = value;
    }
}
