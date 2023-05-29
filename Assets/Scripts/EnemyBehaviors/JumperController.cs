using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumperController : MonoBehaviour
{
    [Header("Components")]
    Enemy enemy;
    NavMeshAgent agent;
    CapsuleCollider2D col;
    Transform target;
    Rigidbody2D rb;

    [Header("Jump")]
    public AnimationCurve jumpCurve;
    public Transform graphics;
    public Transform enemyHealth;
    float zCurve = 1;

    public bool isGrounded = true;
    public bool jumping = false;

    Vector2 healthStartPosition;
    Vector2 graphicsStartPosition;
    Vector2 colliderOffset;

    [Header("Move")]
    public float minimumRange = 2;
    public float detectionRange = 5;
    float targetDistance;

    [Header("Fall")]
    public float fallDuration = 1;
    public bool fellInHole = false;

    [Header("Fade")]
    public bool fade;
    float fadeAmount = 1;
    bool seen = false;
    
    public enum TypeOfAttack
    {
        Up, Forward
    }
    [Header("Attack")]
    public TypeOfAttack typeOfAtack;
    public bool willShoot = false;
    bool willAttack = false;
    bool isAttacking = false;
    // typeOfAttack == Forward
    [Tooltip("This will only be used if the type of attack is set to \'Forward\' ")]
    public float dashPower = 2;
    [Tooltip("This will only be used if the type of attack is set to \'Forward\' ")]
    public float dashDuration = 1;
    float actualDashDuration;
    Vector2 dashDirection;

    bool attackJump = false;
    float jumperMultiplier = 1;
    float durationMultiplier = 1;

    [Header("Shoot")]
    [Tooltip("This will only be used if \'Will Shoot\' is set to TRUE")]
    public GameObject projectilePrefab;
    [Tooltip("This will only be used if \'Will Shoot\' is set to TRUE")]
    public Transform muzzle;

    [Header("Animation")]
    [Tooltip("Name of the parameter used to transition to the Jump animation from Idle. (PARAMETER MUST BE A BOOL)")] 
    public string jumpParameter = "Jump";
    [Tooltip("Name of the parameter used to transition to the Idle animation from Jump. (PARAMETER MUST BE A TRIGGER)")]
    public string touchedGroundParameter = "Touched Ground";
    [Tooltip("Name of the parameter used to transition to the Attack animation. (PARAMETER MUST BE A TRIGGER)")]
    public string attackParameter = "Attack";
    [Tooltip("Name of the parameter used to transition to the Idle animation from Attack. (PARAMETER MUST BE A TRIGGER)")]
    public string exitParameter = "Exit Attack";
    Animator anim;

    [SerializeField] Transform shadow;

    private bool _updateZCurve;
    private float _zCurveMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        _updateZCurve = true;
        _zCurveMultiplier = 1f;
        zCurve = 1;

        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        graphicsStartPosition = graphics.localPosition;
        healthStartPosition = enemyHealth.localPosition;
        colliderOffset = col.offset;

        SetupAgent();

        enemy.agentSpeed = enemy.speed;

        //anim.SetTrigger("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (fellInHole)
        {
            if (fade)
            {
                seen = true;
                Fade();
            }

            Destroy(GetComponent<DropCoins>());

            graphics.localScale = Vector3.Lerp(graphics.localScale, Vector3.zero, fallDuration * Time.deltaTime);

            graphics.Rotate(0, 0, 270 * Time.deltaTime);

            if (graphics.localScale.x <= 0.1f)
                enemy.onDie();

            return;
        }

        if (enemy.target && !target)
            target = enemy.target;
        else
        {
            targetDistance = Vector2.Distance(target.position, transform.position);

            if (targetDistance < detectionRange && targetDistance > minimumRange || seen)
                anim.SetBool(jumpParameter, true);
            else
            {
                anim.SetBool(jumpParameter, false);

                if (targetDistance < minimumRange)
                {
                    if (isGrounded)
                    {
                        Move(false);

                        if (!willAttack)
                        {

                            anim.SetTrigger(attackParameter);

                            willAttack = true;
                        }
                    }
                }

                //if(targetDistance < .5f && !isGrounded)
                //{
                //    actualDashDuration = dashDuration / 2;
                //}
                //else
                //{
                //    actualDashDuration = dashDuration;
                //}
            }

            if (fade)
                Fade();
        }

        if (_updateZCurve)
        {
            zCurve += isAttacking ? (1 / dashDuration) * _zCurveMultiplier * Time.deltaTime : Time.deltaTime;

            graphics.localPosition = new Vector3(graphicsStartPosition.x, graphicsStartPosition.y + jumpCurve.Evaluate(zCurve) * jumperMultiplier, 0);
            col.offset = new Vector2(colliderOffset.x, colliderOffset.y + jumpCurve.Evaluate(zCurve) * jumperMultiplier);
            enemyHealth.localPosition = new Vector3(healthStartPosition.x, healthStartPosition.y + jumpCurve.Evaluate(zCurve), 0);

            isGrounded = graphics.localPosition.y > graphicsStartPosition.y ? false : true;
        }


        if (jumping)
        {
            if (zCurve > .1f && isGrounded)
            {
                anim.SetTrigger(touchedGroundParameter);
                jumping = false;
            }
        }

        if (typeOfAtack == TypeOfAttack.Forward)
        {
            if (!willAttack)
                Move(!isGrounded);

            shadow.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, graphics.localPosition.y);
        }
        else
        {
            Move(!isGrounded);

            if (isAttacking)
            {
                if (!isGrounded)
                    col.enabled = false;
                else
                    col.enabled = true;
            }

            shadow.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, graphics.localPosition.y / (jumperMultiplier / 2));
        }

        if (graphics.localPosition.y > 6)
            graphics.GetComponent<SpriteRenderer>().enabled = false;
        else
            graphics.GetComponent<SpriteRenderer>().enabled = true;

        //if(typeOfAtack == TypeOfAttack.Up && attackJump)
        //{
        //    if(isAttacking && graphics.localPosition.y < 20)
        //        graphics.localPosition += new Vector3(0, graphics.localPosition.y + Time.deltaTime * 5, 0);
        //    else if(!isAttacking && graphics.localPosition.y > 0)
        //        graphics.localPosition += new Vector3(0, graphics.localPosition.y - Time.deltaTime * 5, 0);
        //}

        FallInHole();
    }

    void Fade()
    {
        SpriteRenderer transitionSprite = graphics.GetChild(0).GetComponent<SpriteRenderer>();

        if(targetDistance < minimumRange || seen)
        {
            if (fadeAmount > 0)
                fadeAmount -= fellInHole ? Time.deltaTime * 4 : Time.deltaTime;
            else
                fadeAmount = 0;
        } 
        else if(targetDistance > detectionRange || !seen)
        {
            if (fadeAmount < 1)
                fadeAmount += Time.deltaTime;
            else
                fadeAmount = 1;
        }

        transitionSprite.material.SetFloat("_FadeAmount", fadeAmount);
    }

    void Move(bool move)
    {
        if (move)
        {
            agent.destination = enemy.GetPlayer().transform.position;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
        else
        {
            agent.destination = transform.position;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    public void Jump()
    {
        zCurve = 0;
        jumping = true;
    }

    public void Attack()
    {
        switch (typeOfAtack)
        {
            case TypeOfAttack.Up:
                StartCoroutine(UpAttack(dashDuration));
                break;

            case TypeOfAttack.Forward:
                Jump();
                StartCoroutine(ForwardAttack(dashDuration));
                break;

            default:
                break;
        }
    }

    public void OnHitted()
    {
        if (targetDistance > detectionRange)
            StartCoroutine(SetSeen());
    }

    IEnumerator SetSeen()
    {
        seen = true;

        yield return new WaitForSeconds(3);

        seen = false;
    }

    IEnumerator ForwardAttack(float duration)
    {
        isAttacking = true;

        dashDirection = target.position - transform.position;
        rb.velocity = dashDirection.normalized * dashPower;

        yield return new WaitForSeconds(duration);

        if (willShoot)
        {
            GameObject projectile = Instantiate(projectilePrefab, muzzle ? muzzle.position : transform.position, transform.rotation);
            projectile.GetComponent<Projectile>().Setup(projectile.transform.position + (Vector3)dashDirection, enemy.damage, 5, transform);
        }

        isAttacking = false;
        anim.SetTrigger(exitParameter);

        rb.velocity = Vector2.zero;
        willAttack = false;
    }

    IEnumerator UpAttack(float duration)
    {
        isAttacking = true;

        jumperMultiplier = 30;
        Jump();

        yield return new WaitForSeconds(duration * 0.25f);

        _updateZCurve = false;
        var elapsedTime = 0f;
        var randomDelay = Random.Range(1.5f, 3.5f);

        while (elapsedTime < randomDelay)
        {
            elapsedTime += Time.deltaTime;

            if (targetDistance < 1f)
            {
                _zCurveMultiplier = 2.5f;
                elapsedTime = randomDelay;
            }

            yield return null;
        }

        _updateZCurve = true;

        yield return new WaitForSeconds((duration * 0.75f) / _zCurveMultiplier);

        _zCurveMultiplier = 1f;
        isAttacking = false;
        anim.SetTrigger(exitParameter);

        if (willShoot)
        {
            GameObject projectile = Instantiate(projectilePrefab, muzzle ? muzzle.position : transform.position, transform.rotation);
            projectile.GetComponent<Projectile>().Setup(projectile.transform.position + (Vector3)dashDirection, enemy.damage, 5, transform);
        }

        jumperMultiplier = 1;
        willAttack = false;
    }

    void FallInHole()
    {
        if (fellInHole) return;

        if (isGrounded)
        {
            Collider2D hole = GetOverlappingHole();
            if (hole)
            {
                graphics.transform.localPosition = new Vector3(0, 0, 0);
                transform.position = hole.transform.position + new Vector3(.5f, .5f, 0);
                fellInHole = true;

                transform.Find("shadow").gameObject.SetActive(false);

                agent.enabled = false;
                col.enabled = false;
            }
        }
    }

    Collider2D GetOverlappingHole()
    {
        foreach (Collider2D hole in Physics2D.OverlapCircleAll(transform.position, .05f, LayerMask.GetMask("Holes")))
        {
            return hole;
        }
        return null;
    }

    private void SetupAgent()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.speed = enemy.speed;

        actualDashDuration = dashDuration;
    }
}
