using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SinisterShadow : Boss
{
    public bool isIllusion = false;

    [Header("Animation")]
    public Animator anim;
    public SpriteRenderer graphics;

    [Header("Eye")]
    [SerializeField] private GameObject eye;
    [SerializeField] private Transform eyePosition;
    [SerializeField] Transform target;
    [SerializeField] float eyeRadius = .1f;
    float actualRadius;
    [SerializeField] float lerpTrackSpeed = 5;
    Vector3 targetDirection;
    public bool trackTarget = true;
    bool eyeTrack = true;
    bool dove = false;

    bool wokenUp = false;

    NavMeshAgent agent;

    [SerializeField] CircleCollider2D attackCollider;

    bool isFollowing = false;
    bool isFollowCooldown = false;
    float followCooldown = 5f;

    [Header("Attacks")]
    public bool furious = false;
    public JikiCharge jiki;
    bool performingAction = false;
    public Projectile projectilePrefab;
    public int maxEyeShootTimes = 50;
    public float shotInterval = .1f;
    int shootTimes;
    [SerializeField] BoxCollider2D spikeCollider;
    [SerializeField] float actionTime = 1f;
    public bool beVisible = true;
    public bool awakening = false;
    float sleepLerp = 5;
    float sleepSpeed = 5;

    [Header("Attack | Random Shrapnel")]
    public int shrapnelAmountToSpawn = 2;
    public List<Transform> shrapnelSpawnPositions = new List<Transform>();

    [Header("Attack | Illusion")]
    public float stunedTime = 6;
    List<SinisterShadow> myIllusions = new List<SinisterShadow>();
    bool isStunned = false;
    Transform cloneTarget;
    Transform targetParent;

    [Header("Attack | Laser")]
    public Transform laserParent;
    public List<LineRenderer> laser = new List<LineRenderer>();
    public Transform laserPointTop;
    public Transform laserPointBottom;
    public BoxCollider2D laserCollider;
    bool laserBothSides = false;
    public float laserRotationSpeed = 5;
    bool laserOn = false;
    bool rotateLasers = false;
    float laserIgnition = 0;
    float laserAngle = 0;
    bool after180 = false;

    [Header("Furious Attack | Spikes")]
    public GameObject trapAttackPrefab;
    public int maxTraps = 5;
    int traps;

    [Header("Furious Attack | Blink")]
    public Projectile shrapnelBulletPrefab;
    public bool blinking = false;
    public float blinkDuration = 5;
    float blinkSpeed = 1;
    float alpha = 1;

    [Header("Spawn Settings")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private SpawnParticle spawnParticlePrefab;

    [Header("SFX")]
    [SerializeField] AudioClip shadowRise;
    [SerializeField] AudioClip fireballShots;
    [SerializeField] AudioClip giantFireballShots;
    [SerializeField] AudioClip submerge;
    [SerializeField] AudioClip emerge;
    [SerializeField] AudioClip rightCloneAudio;
    [SerializeField] AudioClip wrongCloneAudio;
    [SerializeField] AudioClip transformationAudio;
    [SerializeField] AudioClip phase2Idle;

    [Header("Debug")]
    public Transform targetTest;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 3;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        actualRadius = eyeRadius;

        attackCollider = GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;

        spikeCollider.enabled = false;
        awakening = true;
        StartCoroutine(Awaken());

        anim.GetComponent<SpriteRenderer>().sortingOrder = 1;

        //StartCoroutine(Dive());
        //StartCoroutine(CrazyLookingEye());
        //StartCoroutine(Blink());
        //StartCoroutine(Stunned());

        base.Start();

        //List<Transform> positions = new List<Transform>(shrapnelSpawnPositions);
        //Queue<Transform> posQueue = new Queue<Transform>(positions);

        //for (int i = 0; i < shrapnelAmountToSpawn; i++)
        //{
        //    Projectile shrapnelShot = Instantiate(shrapnelBulletPrefab, posQueue.Dequeue().position, Quaternion.identity);
        //    shrapnelShot.GetComponent<ShrapnelProjectile>().commonShrapnel = true;
        //    shrapnelShot.Setup(targetTest, 20, 0, transform);
        //}

        //laserBothSides = true;
        //StartCoroutine(LaserAttack());
    }

    IEnumerator Awaken()
    {
        yield return new WaitForSeconds(1);

        awakening = false;
    }

    public void Setup()
    {
        if (!isIllusion)
        {
            SetCinemachineTarget(6);

            SetHealthBar(health);
        }

        targetParent = GameObject.Find("ClonesPoints").transform;

        anim.GetComponent<SpriteRenderer>().sortingOrder = 10;

        anim.SetBool("Wake Up", true);

        //attackCollider.enabled = true;

        awakening = true;
    }

    void EyeTracker(Transform track)
    {
        if (track)
        {
            targetDirection = track.position - eyePosition.position;

            targetDirection = Vector2.ClampMagnitude(targetDirection, actualRadius);

            eye.transform.position = Vector2.Lerp(eye.transform.position, eyePosition.position + targetDirection, lerpTrackSpeed * Time.deltaTime);
        }
    }

    protected override void FixedUpdate()
    {
        SetupLaser(laserOn, laserBothSides);
        if (rotateLasers)
        {
            laserParent.Rotate(laserParent.forward, laserRotationSpeed);
            if (laserParent.eulerAngles.z > 180) after180 = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //health.isInvulnerable = wokenUp;

        Transform toTrack = target == null ? enemy.GetTarget() : target;

        //EyeTracker(trackTarget ? toTrack : transform);

        if (health.GetCurrentPercentage() <= .5f)
        {
            if (!furious)
                GetFurious();

            furious = true;
        }

        
        //return;

        if (wokenUp)
        {
            EyeTracker(trackTarget ? toTrack : transform);

            if (!isFollowing && !performingAction && !isStunned)
            {
                StartCoroutine(FollowPlayer());
            }

            if ((GetNearPlayer() != null) && isFollowing && dove && !isStunned && !performingAction)
            {
                agent.destination = GetNearPlayer().transform.position;
            }

            if (!isIllusion)
            {
                if (blinking)
                {
                    if (blinkSpeed < 5)
                        blinkSpeed += Time.deltaTime / (blinkDuration / 5);
                }
                else
                {
                    blinkSpeed = 1;

                }

                anim.SetBool("Blink", blinking);
                anim.SetFloat("Blink Speed", blinkSpeed);
            }

            if (dove && !beVisible)
            {
                if (graphics.color.a > 0)
                    graphics.color = new Color(1, 1, 1, graphics.color.a - Time.deltaTime * 2);
            }
            else
            {
                if (graphics.color.a < 1)
                    graphics.color = new Color(1, 1, 1, graphics.color.a + Time.deltaTime * 2);
            }
        }
        else
        {
            if (!awakening)
            {
                agent.speed = 5;
                agent.destination = GetNearPlayer().transform.position;

                if (Vector2.Distance(transform.position, agent.destination) < .1f)
                    FindObjectOfType<SinisterBossRoom>().TouchedPlayer();
            }
            else
            {
                agent.speed = 3;
                agent.destination = transform.position;
            }
        }
    }

    public void DeactivateAwakening()
    {
        awakening = false;
    }

    void SortLandAction()
    {
        performingAction = true;

        var random = Random.value;

        if (random <= .2f)
        {
            StartCoroutine(GravitationalAttack());
            if (myIllusions.Count > 0)
            {
                foreach (var item in myIllusions)
                {
                    item.performingAction = true;
                    item.StartCoroutine(item.GravitationalAttack());
                }
            }
        }
        else if (random > .2f && random <= .4f)
        {
            StartCoroutine(Dive());
            if (myIllusions.Count > 0)
            {
                foreach (var item in myIllusions)
                {
                    item.performingAction = true;
                    item.StartCoroutine(item.Dive());
                }
            }
        }
        else if(random > .4f && random <= .6f)
        {
            StartCoroutine(SpawnShrapnelShots());
        }
        else if(random > .6f && random <= .8f)
        {
            laserBothSides = false;
            StartCoroutine(LaserAttack());
        }
        else
        {
            shootTimes = 0;
            StartCoroutine(CrazyLookingEye());
            if (myIllusions.Count > 0)
            {
                foreach (var item in myIllusions)
                {
                    item.performingAction = true;
                    item.StartCoroutine(item.CrazyLookingEye());
                }
            }
        }
    }

    void SortFuriousAction()
    {
        performingAction = true;

        var random = Random.value;

        if (furious)
        {
            if (random <= .25f)
            {
                StartCoroutine(FuriousGroundAttack());
            }
            else if(random > .25f && random < .5f)
            {
                StartCoroutine(SpawnShrapnelShots());
            } 
            else if(random > .5f && random < .75f)
            {
                laserBothSides = true;
                StartCoroutine(LaserAttack());
            }
            else 
            {
                StartCoroutine(Blink());
            }
        }
    }

    void SortBurriedAction()
    {
        performingAction = true;

        var random = Random.value;

        if (random <= 0f)
        {
            SpikeAttack();
        }
        else if(random > .25f && random <= .5f)
        {
            SpawnSorridentes();
        }
        else if(random > .5f && random <= .75f)
        {
            Emerge();
        }
        else
        {
            if (HowManyIllusions() < 1)
                StartCoroutine(CreateIllusion());
            else
                performingAction = false;
        }
    }

    void RadiusFire(int projectilesCount)
    {
        Vector2 startPoint = transform.position;
        float angleStep = 360f / projectilesCount;
        float angle = 0f;

        for (int i = 0; i <= projectilesCount - 1; i++)
        {

            float projectileDirXposition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * 5;
            float projectileDirYposition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * 5;

            Vector2 projectileVector = new Vector2(projectileDirXposition, projectileDirYposition);
            Vector2 projectileMoveDirection = (projectileVector - startPoint).normalized * 15;

            var proj = Instantiate(projectilePrefab, startPoint, Quaternion.identity);
            if (i != 0)
            {
                proj.GetComponent<AudioSource>().mute = true;
            }
            proj.GetComponent<Rigidbody2D>().velocity =
                new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

            angle += angleStep;
        }

        if (enemy) enemy.PlayAttackSound();
    }

    void GetFurious()
    {
        performingAction = true;
        anim.SetTrigger("Furious");
        GetEnemy().audioSource.PlayOneShot(transformationAudio);
        GetEnemy().idleSound = phase2Idle;

        SinisterShadow[] clones = FindObjectsOfType<SinisterShadow>();

        foreach (var item in clones)
        {
            if (item.isIllusion) Destroy(item.gameObject);
        }
    }

    IEnumerator FollowPlayer()
    {
        StartFollowingPlayer();
        yield return new WaitForSeconds(actionTime);
        StopFollowingPlayer();
    }

    void StartFollowingPlayer()
    {
        if (GetNearPlayer() != null)
        {
            isFollowing = true;
        }
    }

    void StopFollowingPlayer()
    {
        agent.destination = transform.position;

        if (!isIllusion && wokenUp)
        {
            if (health.GetCurrentHealth() > 0)
            {
                if (dove)
                    SortBurriedAction();
                else
                {
                    if (furious)
                        SortFuriousAction();
                    else
                        SortLandAction();
                }
            }
        }

        isFollowing = false;
    }

    IEnumerator Stunned()
    {
        performingAction = true;
        wokenUp = false;

        SetEyeTracking(false);
        GetEnemy().audioSource.PlayOneShot(rightCloneAudio);

        jiki.Deactivate();
        anim.SetBool("Gravitational", false);
        anim.SetBool("Dove", false);
        anim.Play("IDLE");

        agent.destination = transform.position;
        isFollowing = false;

        StartCoroutine(StunnedEyes());

        isStunned = true;

        yield return new WaitForSeconds(stunedTime);

        performingAction = false;
        SetEyeTracking(true);
        wokenUp = true;

        StopCoroutine(StunnedEyes());
        target = null;

        isStunned = false;
    }

    IEnumerator StunnedEyes()
    {
        Vector3 lookDirection = new Vector3(Random.Range(-.5f, .5f), 1);

        GameObject point = new GameObject();
        point.transform.parent = transform;
        point.transform.localPosition = lookDirection;

        target = point.transform;

        Destroy(point, .2f);

        yield return new WaitForSeconds(.05f);

        if(isStunned)
            StartCoroutine(StunnedEyes());
    }

    #region Attacks
    IEnumerator ContinuousShots()
    {
        Shoot(GetNearPlayer().transform);
        if (myIllusions.Count > 0)
        {
            foreach (var item in myIllusions)
            {
                item.Shoot(GetNearPlayer().transform);
            }
        }

        yield return new WaitForSeconds(.25f);

        if (HowManyIllusions() > 0)
            StartCoroutine(ContinuousShots());
    }

    IEnumerator GravitationalAttack()
    {
        anim.SetBool("Gravitational", true);
        jiki.Activate();

        yield return new WaitForSeconds(3);

        anim.SetBool("Gravitational", false);
        jiki.Deactivate();
    }

    IEnumerator FuriousGroundAttack()
    {
        SetEyeTracking(false);

        yield return new WaitForSeconds(.5f);

        anim.SetBool("Gravitational", true);

        yield return new WaitForSeconds(2f);

        traps = 0;
        StartCoroutine(Traps());
    }

    IEnumerator Dive()
    {
        SetEyeTracking(false);

        yield return new WaitForSeconds(.5f);

        actionTime = 3;
        agent.speed = 8;
        anim.SetBool("Dove", true);
        GetEnemy().audioSource.PlayOneShot(submerge);
    }

    void SetupLaser(bool on, bool bothSides = false)
    {
        if (on)
        {
            if (laserIgnition < 1)
                laserIgnition += Time.deltaTime;
        }
        else
        {
            if (laserIgnition > 0)
                laserIgnition -= Time.deltaTime * 2;
        }

        foreach (var item in laser)
        {
            laserCollider.enabled = on;

            if (bothSides)
            {
                //item.SetPosition(0, laserPointTop.position);
                //item.SetPosition(1, laserPointBottom.position);

                item.SetPosition(0, Vector2.Lerp(eye.transform.position, laserPointTop.position, laserIgnition));
                item.SetPosition(1, Vector2.Lerp(eye.transform.position, laserPointBottom.position, laserIgnition));

                laserCollider.size = new Vector2(0.25f, Vector2.Distance(laserPointTop.position, laserPointBottom.position));
                laserCollider.offset = Vector2.zero;
            }
            else
            {
                item.SetPosition(0, Vector2.Lerp(eye.transform.position, laserPointTop.position, laserIgnition));
                item.SetPosition(1, eye.transform.position);

                laserCollider.size = new Vector2(0.25f, Vector2.Distance(laserPointTop.position, transform.position));
                laserCollider.offset = new Vector2(0, (laserCollider.size.y / 2));
            }
        }
    }

    IEnumerator LaserAttack()
    {
        laserOn = true;
        after180 = false;

        GetEnemy().audioSource.PlayOneShot(fireballShots);

        yield return new WaitForSeconds(1);

        rotateLasers = true;

        yield return new WaitForSeconds(.25f);

        yield return new WaitUntil(() => after180 && laserParent.eulerAngles.z > 0 && laserParent.eulerAngles.z < 180);

        rotateLasers = false;
        laserParent.rotation = Quaternion.Euler(Vector3.zero);

        yield return new WaitForSeconds(1);

        laserOn = false;
        performingAction = false;
    }

    void SpikeAttack()
    {
        beVisible = true;
        anim.SetTrigger("Ground Attack");
        if (myIllusions.Count > 0)
        {
            foreach (var item in myIllusions)
            {
                item.performingAction = true;
                item.SpikeAttack();
            }
        }
    }

    void Emerge()
    {
        SetCinemachineTarget(6);

        actionTime = 1f;
        agent.speed = 3;
        anim.SetBool("Dove", false);
        dove = false;

        GetEnemy().audioSource.PlayOneShot(emerge);

        if (myIllusions.Count > 0)
        {
            foreach (var item in myIllusions)
            {
                item.Emerge();
            }
        }
    }

    IEnumerator CreateIllusion()
    {
        performingAction = true;
        CreateClone();

        yield return new WaitForSeconds(5);

        Emerge();

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(ContinuousShots());
    }

    void CreateClone()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        this.agent.isStopped = true;

        Vector3 randomPosition = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);

        SinisterShadow myClone = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent).GetComponent<SinisterShadow>();
        myClone.graphics.color = graphics.color;

        myClone.eye.GetComponent<SpriteRenderer>().color = Color.green;

        var agent = myClone.GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;

        Enemy boss = myClone.GetComponent<Enemy>();
        boss.currentRoom = enemy.currentRoom;
        enemy.currentRoom.enemies.Add(boss);

        //myClone.GetComponent<NavMeshAgent>().enabled = false;

        myClone.GetAnimator().SetBool("Gravitational", anim.GetBool("Gravitational"));
        myClone.GetAnimator().SetBool("Dove", anim.GetBool("Dove"));
        myClone.GetAnimator().SetBool("Blink", anim.GetBool("Blink"));
        myClone.GetAnimator().Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash);

        Destroy(myClone.transform.Find("PlayerCamTarget").gameObject);

        if (!myIllusions.Contains(myClone))
            myIllusions.Add(myClone);

        SetClonePoint(GetRandomPoint());
        myClone.SetClonePoint(GetRandomPoint());

        //agent.SetDestination(cloneTarget.position);
        //myClone.agent.SetDestination(myClone.cloneTarget.position);
        transform.position = cloneTarget.position;
        myClone.transform.position = myClone.cloneTarget.position;

        myClone.isIllusion = true;

        myClone.wokenUp = true;
        myClone.StopFollowingPlayer();
        myClone.performingAction = true;
        //performingAction = false;
    }

    Transform GetRandomPoint()
    {
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < targetParent.childCount; i++)
        {
            if(targetParent.GetChild(i) != cloneTarget)
                points.Add(targetParent.GetChild(i));
        }

        Transform output = points[Random.Range(0, points.Count)];
        return output;
    }

    void SetClonePoint(Transform target)
    {
        cloneTarget = target;
    }

    void Shoot(Transform target)
    {
        GetEnemy().audioSource.PlayOneShot(fireballShots);

        Projectile projectile = Instantiate(projectilePrefab, eye.transform.position, Quaternion.identity);
        projectile.Setup(target, 15, 15, transform);
    }

    private void SpawnSorridentes()
    {
        var spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform.parent);
        var spawnParticle = Instantiate(spawnParticlePrefab, spawnedEnemy.transform.position, Quaternion.identity);
        spawnParticle.SetupEnemySpawn(spawnedEnemy);

        spawnedEnemy.SetCurrentRoom(enemy.currentRoom);
        performingAction = false;

        if (myIllusions.Count > 0)
        {
            foreach (var item in myIllusions)
            {
                item.performingAction = true;
                item.SpawnSorridentes();
            }
        }
    }

    void SpawnTraps()
    {
        if (traps >= maxTraps) return;

        Vector3 spawnPos = GetNearPlayer().transform.position + (Vector3)GetNearPlayer().movement;

        GameObject trap = Instantiate(trapAttackPrefab, spawnPos, Quaternion.identity, transform.parent);
        traps++;
    }

    IEnumerator Traps()
    {
        SpawnTraps();

        yield return new WaitForSeconds(1f);

        if (traps >= maxTraps)
        {
            anim.SetBool("Gravitational", false);

            yield return new WaitForSeconds(.5f);

            SetEyeTracking(true);
        }
        else
        {
            StartCoroutine(Traps());
        }
    }

    IEnumerator SpawnShrapnelShots()
    {
        List<Transform> positions = new List<Transform>(shrapnelSpawnPositions);
        Queue<Transform> posQueue = new Queue<Transform>(RoomByRoomGenerator.Instance.Shuffle(positions));

        GetEnemy().audioSource.PlayOneShot(giantFireballShots);

        for (int i = 0; i < shrapnelAmountToSpawn; i++)
        {
            Projectile shrapnelShot = Instantiate(shrapnelBulletPrefab, posQueue.Dequeue().position, Quaternion.identity);
            shrapnelShot.GetComponent<ShrapnelProjectile>().commonShrapnel = true;
            shrapnelShot.Setup(GetNearPlayer().transform, 20, 0, transform);
        }

        yield return new WaitForSeconds(1);

        performingAction = false;
    }

    IEnumerator CrazyLookingEye()
    {
        eyeTrack = false;

        Vector3 lookDirection = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));

        GameObject point = new GameObject();
        point.transform.parent = transform;
        point.transform.localPosition = lookDirection;

        target = point.transform;

        Destroy(point, .2f);

        if (shootTimes >= 10)
        {
            Shoot(point.transform);
            actualRadius = eyeRadius;
        }
        else
        {
            actualRadius = eyeRadius / 2;
        }

        shootTimes++;

        yield return new WaitForSeconds(shotInterval);

        if (shootTimes < maxEyeShootTimes)
            StartCoroutine(CrazyLookingEye());
        else
        {
            target = null;
            performingAction = false;
        }
    }

    IEnumerator Blink()
    {
        blinking = true;
        anim.SetBool("Blink", true);

        yield return new WaitForSeconds(blinkDuration);

        // Play sound

        yield return new WaitForSeconds(.2f);

        anim.SetBool("Blink", false);

        GetEnemy().audioSource.PlayOneShot(giantFireballShots);
        Projectile shrapnelShot = Instantiate(shrapnelBulletPrefab, eye.transform.position, Quaternion.identity);
        shrapnelShot.Setup(GetNearPlayer().transform, 20, 5, transform);

        blinking = false;
        performingAction = false;
    }
    #endregion

    #region Animation Helpers
    public void WakeUp()
    {
        wokenUp = true;
        attackCollider.enabled = true;
        GetEnemy().audioSource.PlayOneShot(shadowRise);
    }

    public void SetEyeTracking(bool track)
    {
        trackTarget = track;
    }

    public void SetDive(bool action)
    {
        dove = action;
        anim.GetComponent<SpriteRenderer>().sortingOrder = dove ? 1 : 10;
        DeactivateCollissions(!dove);
        if (dove)
        {
            beVisible = false;
            DisableCinemachineTarget();
        }
    }

    public void SpikeCollision(bool active)
    {
        spikeCollider.enabled = active;
    }

    void DeactivateCollissions(bool active)
    {
        attackCollider.enabled = active;
    }

    public void SetIsAttacking(bool value)
    {
        performingAction = value;
    }
    #endregion

    void OnHitted()
    {
        if (dove) return;

        if (isIllusion)
        {
            RadiusFire(40);
            Destroy(gameObject);
            GetEnemy().audioSource.PlayOneShot(wrongCloneAudio);
        }
        else
        {
            SinisterShadow[] clones = FindObjectsOfType<SinisterShadow>();

            if (HowManyIllusions() > 0) 
            {
                StopAllCoroutines();
                SetEyeTracking(false);
                StartCoroutine(Stunned()); 
            }

            foreach (var item in clones)
            {
                if (item.isIllusion) Destroy(item.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        SinisterShadow[] shadows = FindObjectsOfType<SinisterShadow>();
        foreach (var item in shadows)
        {
            if (!item.isIllusion)
            {
                item.agent.isStopped = false;
                item.performingAction = false;

                if (item.myIllusions.Contains(this))
                    item.myIllusions.Remove(this);
            }
        }
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public int HowManyIllusions()
    {
        List<SinisterShadow> illusions = new List<SinisterShadow>();
        SinisterShadow[] clones = FindObjectsOfType<SinisterShadow>();

        for (int i = 0; i < clones.Length; i++)
        {
            if (clones[i].isIllusion)
            {
                if (!illusions.Contains(clones[i]))
                    illusions.Add(clones[i]);
            }
        }

        return illusions.Count;
    }

    void ToggleFollowCooldown()
    {
        isFollowCooldown = !isFollowCooldown;
    }

    public override void onHealthEnd()
    {
        base.onHealthEnd();
        Die();
    }
}
