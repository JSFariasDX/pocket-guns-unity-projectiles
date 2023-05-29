using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JikiCharge : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private JikiArea jikiArea;
    [SerializeField] private CircleCollider2D areaCollider;
    [SerializeField] private SpriteRenderer areaSprite;

    [Header("General Settings")]
    [SerializeField] private Vector2 activationTimerRange;
    [SerializeField] private Vector2 changeTimerRange;

    [Header("Transition Settings")]
    [SerializeField] private float maxAreaRadius = 3.5f;
    [SerializeField] private Vector3 maxAreaSpriteScale = new Vector3(2.15f, 2.65f, 2.15f);
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField] private AudioClip deactivateClip;
    [SerializeField] private AudioClip attractClip;
    [SerializeField] private AudioClip repelClip;

    private Coroutine _changeCoroutine;
    private bool _isActivated;

    private Animator _animator;
    private AudioSource _audioSource;

    public bool controlable = false;

    Transform targetGroup;
    float playerDistance;

    bool started = false;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Destroy(gameObject);

        targetGroup = GameObject.Find("TargetGroup").transform;

        areaCollider.radius = 0f;
        areaSprite.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        playerDistance = Vector2.Distance(transform.position, targetGroup.position);

        if (!controlable && playerDistance < 15 && !started)
        {
            started = true;
            Activate();
        }
        else if (!controlable && playerDistance >= 15 && _isActivated)
            Deactivate();

        //if (playerDistance > 15 && _isActivated)
        //{
        //    Deactivate();
        //    return;
        //}

        if (_isActivated)
        {
            areaCollider.radius = Mathf.MoveTowards(areaCollider.radius, maxAreaRadius, transitionSpeed * Time.deltaTime);
            areaSprite.transform.localScale = Vector3.MoveTowards(areaSprite.transform.localScale, maxAreaSpriteScale, transitionSpeed * Time.deltaTime);
        }
        else
        {
            areaCollider.radius = Mathf.MoveTowards(areaCollider.radius, 0f, transitionSpeed * Time.deltaTime);
            areaSprite.transform.localScale = Vector3.MoveTowards(areaSprite.transform.localScale, Vector3.zero, transitionSpeed * Time.deltaTime);
        }

        if (controlable)
        {
            if (areaCollider.radius < .1f) areaCollider.enabled = false;
            else areaCollider.enabled = true;
        }
    }

    public void Activate(int isAttracting = 0)
    {
        _isActivated = true;
        
        var randomDirection = isAttracting == 0 ? Random.value < 0.5f : isAttracting == 1;

        if (!controlable)
            jikiArea.SetupJiki(randomDirection);
        else
            jikiArea.SetupJiki(true);

        _animator.SetBool("IsAttracting", jikiArea.IsAttracting);
        _animator.SetBool("IsActive", true);

        PlayAudio(activateClip, true);
        
        var clip = jikiArea.IsAttracting ? attractClip : repelClip;
        PlayAudio(clip, false);

        if(!controlable)
            _changeCoroutine = StartCoroutine(ChangeCoroutine());
    }

    public void Deactivate(int isAttracting = 0, float delay = -1f)
    {
        if (!_isActivated)
            return;

        if (_changeCoroutine != null)
            StopCoroutine(_changeCoroutine);

        _isActivated = false;

        _animator.SetBool("IsActive", false);

        PlayAudio(deactivateClip, true);

        if (!controlable && playerDistance < 15)
            StartCoroutine(Reactivate(isAttracting, delay));
        else
            started = false;
    }

    private IEnumerator ChangeCoroutine()
    {
        var delay = Random.Range(changeTimerRange.x, changeTimerRange.y);
        yield return new WaitForSeconds(delay);

        var reverse = jikiArea.IsAttracting ? -1 : 1;

        Deactivate(reverse, 0.75f);
    }

    private IEnumerator Reactivate(int isAttracting, float delay)
    {
        var wait = delay == -1f ? Random.Range(activationTimerRange.x, activationTimerRange.y) : delay;
        yield return new WaitForSeconds(wait);

        Activate(isAttracting);
    }

    private void PlayAudio(AudioClip clip, bool oneShot)
    {
        if (oneShot)
            _audioSource.PlayOneShot(clip);
        else
        {
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.PlayDelayed(0.75f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Deactivate();
        }
    }
}
