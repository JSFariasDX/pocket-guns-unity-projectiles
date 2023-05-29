using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UiAnimation", menuName = "Ui Animation")]
public class UiSpriteAnimationData : ScriptableObject
{
    public Sprite[] sprites;
    public float frameSpeed = .1f;
}
