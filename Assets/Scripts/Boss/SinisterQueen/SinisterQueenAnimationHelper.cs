using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinisterQueenAnimationHelper : MonoBehaviour
{
    private SinisterQueen _sinisterQueen;

    private void Awake()
    {
        _sinisterQueen = GetComponentInParent<SinisterQueen>();
    }

    public void Die()
    {
        _sinisterQueen.Die();
    }
}
