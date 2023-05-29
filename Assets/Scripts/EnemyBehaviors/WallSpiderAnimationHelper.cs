using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpiderAnimationHelper : MonoBehaviour
{
    private WallSpiderController _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<WallSpiderController>();
    }

    public void TryBreakObstacles(float radius)
    {
        _controller.TryBreakObstacles(radius);
    }
}
