using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHead : MonoBehaviour
{
    [SerializeField] protected Totem totem;
    [SerializeField] float riseAnimationTime;
    [SerializeField] private ParticleSystem totemParticle;
    float currentPosition;
    protected Animator animator;
    SpriteRenderer spriteRenderer;
    bool isAlive;
    bool isRising;

    protected virtual void Awake()
    {
        currentPosition = transform.localPosition.y;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255, 255, 255, 0);
        isRising = true;
    }

	private void Update()
	{
        GoingDown();
    }

	public bool IsAlive() { return isAlive; }

    public IEnumerator Spin(float spinTime, bool disableLater = true)
    {
        animator.SetBool("Spin", true);
        yield return new WaitForSeconds(spinTime);
        if (disableLater)
            animator.SetBool("Spin", false);
    }

    public virtual void SetDark()
	{
        animator.SetTrigger("Dark");
	}

    public virtual void Rise(bool triggerAnimation = true)
    {
        isAlive = true;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        
        if (triggerAnimation) 
            animator.SetTrigger("Rise");

        StartCoroutine(WaitFinishRiseAnimation());
    }

    public virtual void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        StopAllCoroutines();
    }

    public void ApplyHitFilter()
    {
        StartCoroutine(ActiveHitFilter());
    }

    IEnumerator ActiveHitFilter()
    {
        spriteRenderer.material.SetInt("_Hit", 1);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetInt("_Hit", 0);
    }

    public void SetGoingDown()
	{
        currentPosition -= .84f;
	}

    public void GoingDown()
	{
        if (IsGoingDown())
        {
            transform.Translate(0, -10 * Time.deltaTime, 0);
        }
    }

    public bool IsGoingDown()
	{
        return transform.localPosition.y > currentPosition;
    }

    public IEnumerator WaitFinishRiseAnimation()
	{
        yield return new WaitForSeconds(riseAnimationTime);
        EnableAttacks();
        totem.SetTotemCinemachineTarget();
        isRising = false;
	}

    public virtual void EnableAttacks()
	{

	}

    protected bool IsRising()
	{
        return isRising;
	}

    //Animation Event Methods
    public void PlayParticleSystem()
    {
        if (!totemParticle.isPlaying)
            totemParticle.Play();
    }

    public void SetSpriteAlphaToZero()
    {
        spriteRenderer.color = new Color(255, 255, 255, 0);
    }
}
