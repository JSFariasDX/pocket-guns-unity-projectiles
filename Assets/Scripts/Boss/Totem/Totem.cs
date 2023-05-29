using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : Boss
{
    public BottomTotemHead BottomHead => botHead.GetComponent<BottomTotemHead>();

    int currentState;
    TotemHead currentUpperHead;
    [SerializeField] TotemHead botHead;
    [SerializeField] TotemHead midHead;
    [SerializeField] TotemHead topHead;
    [SerializeField] GameObject crystals;

    private bool _isDark;
    
    protected override void Start()
    {
        base.Start();

        SetupBoss();
    }

    void SetupBoss()
	{
        currentState = 1;

        currentUpperHead = botHead;
        botHead.Rise(false);
        SetHealthBar(health);
        StartBossTheme();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void OnHitted()
    {
        // This method is called by Message in Enemy.cs
        if (health.IsDamageEnabled())
        {
            if (botHead.IsAlive()) botHead.ApplyHitFilter();
            if (midHead.IsAlive()) midHead.ApplyHitFilter();
            if (topHead.IsAlive()) topHead.ApplyHitFilter();
        
            if (health.GetCurrentPercentage() <= 0)
            {
                // Boss Defeat
                if (currentState == 5)
                {
                    StartCoroutine(DieHead(topHead));
                }
            }
            if (health.GetCurrentPercentage() < 0.2f)
            {
                if (currentState == 4)
                {
                    currentState = 5;
                    StartCoroutine(DieHead(midHead));
                }
            }
            if (health.GetCurrentPercentage() < 0.4f)
            {
                if (currentState == 3)
                {
                    currentState = 4;
                    StartCoroutine(DieHead(botHead));
                }
            }
            if (health.GetCurrentPercentage() < 0.5f)
			{
                if (!_isDark)
                {
                    botHead.SetDark();
                    midHead.SetDark();
                    topHead.SetDark();
                    crystals.gameObject.SetActive(true);
                    crystals.transform.parent = null;
                    _isDark = true;
                }
			}
            if (health.GetCurrentPercentage() < 0.6f)
            {
                if (currentState == 2)
                {
                    currentState = 3;
                    StartCoroutine(RiseHead(topHead));
                }
            }
            if (health.GetCurrentPercentage() < 0.8f)
            {
                if (currentState == 1)
                {
                    currentState = 2;
                    StartCoroutine(RiseHead(midHead));
                }
            }
        }
    }

    IEnumerator RiseHead(TotemHead head)
    {
        health.SetDamageEnabled(false);
        if (currentUpperHead != botHead) StartCoroutine(currentUpperHead.Spin(.1f));
        yield return new WaitForSeconds(.75f); // spin animation time
        head.Rise();
        currentUpperHead = head;
        health.SetDamageEnabled(true);
    }

    IEnumerator DieHead(TotemHead head)
    {
        head.Die();
        health.SetDamageEnabled(false);
        topHead.GetComponent<TopTotemHead>().SetCanRicochet(false);

        float dieAnimationTime = 1f;
        yield return new WaitForSeconds(dieAnimationTime);

        if (head == botHead)
        {
            currentUpperHead = midHead;
            midHead.SetGoingDown();
            topHead.SetGoingDown();
        }
        else if (head == midHead)
        {
            currentUpperHead = topHead;
            topHead.SetGoingDown();
            topHead.GetComponent<TopTotemHead>().SetCanRicochet(true);
        }

        health.SetDamageEnabled(true);
    }

    public int GetCurrentState()
	{
        return currentState;
	}

    public void SetTotemCinemachineTarget()
    {
        SetCinemachineTarget();
    }
}
