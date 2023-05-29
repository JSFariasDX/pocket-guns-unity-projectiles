using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFixForDisabledUI : MonoBehaviour
{
    bool isActivated = false;
    private void Awake()
    {
        if (!isActivated)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.keepAnimatorControllerStateOnDisable = true;
                isActivated = true;
            }
        }
    }
}
