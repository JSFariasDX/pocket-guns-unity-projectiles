using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class DungeonContext : MonoBehaviour
{
    [SerializeField]
    Sprite labSprite;
    [SerializeField]
    Sprite forestSprite;

    public void SetContext(DungeonType dgType)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (dgType)
        {
            case DungeonType.Forest:
                spriteRenderer.sprite = forestSprite;
                return;
            default:
                spriteRenderer.sprite = labSprite;
                return;
        }
    }
}
