using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    public Transform contactPoint;
    public float livingTime = 0.8f;
    public float damage = 10;
    public GameObject hitEffectPrefab;
    Collider2D col;

    [Header("Homing")]
    public float chaseSpeed = 5;
    float actualSpeed;
    List<Transform> enemies = new List<Transform>();
    Transform lockedTarget;
    bool shouldFollow = false;

    float bulletDistance;
    Vector3 startPoint;

    // Splash damage
    bool haveSplashDamage;
    float splashRadius;
    float splashDamage;
    Gun gun;

    bool flight = false;
    bool pierce = false;

    AudioClip onHitSolidSound;
    public AudioMixerGroup sfxGroup;

    private void Start()
    {
        startPoint = transform.position;
        col = GetComponent<Collider2D>();

        if (SceneManager.GetActiveScene().name.Contains("PocketMor"))
		{
            GetComponentInChildren<Light2D>().enabled = false;
		}
    }

    private void Update()
    {
        if (gun && gun.GetPlayer().homingProjectiles)
        {
            DetectEnemies();

            if (!lockedTarget)
            {
                lockedTarget = GetClosestTarget(enemies);
            }
            else
            {
                ChaseTarget();
            }
        }
    }

    private void FixedUpdate()
	{
		if (Vector2.Distance(transform.position, startPoint) > bulletDistance)
		{
            Destroy(gameObject);
		}
    }

    public void Setup(Gun gun, Vector2 targetPosition, AudioClip onHitSolidSound, bool instability = true)
    {
        this.gun = gun;
        damage = gun.damage;

        float damageBonus = damage * gun.GetPlayer().DamageBonus;
        float finalDamage = damage + damageBonus;
        damage = finalDamage;

        flight = gun.GetPlayer().ProjectileFlight;
        pierce = gun.GetPlayer().ProjectilePierce;

        this.onHitSolidSound = onHitSolidSound;

        shouldFollow = gun.GetPlayer().homingProjectiles;

        actualSpeed = chaseSpeed;

        Toolkit2D.RotateAt(transform, targetPosition);
        if (instability) {
            float stabilityBonus = gun.GetPlayer().RecoilStabilization * gun.actualInstability;
            float finalStability = gun.actualInstability + stabilityBonus;
            if (finalStability > 0)
            {
                float random = Random.Range(-finalStability, finalStability);
                transform.Rotate(0, 0, random);
            }
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (!lockedTarget)
        {
            if (gun.gunConfig.scatteredShots)
            {
                rb.velocity = GetBulletForce(gun.bulletForce * Random.Range(.5f, 1), gun.GetPlayer().BulletForceBonus);
            }
            else
            {
                rb.velocity = GetBulletForce(gun.bulletForce, gun.GetPlayer().BulletForceBonus);
            }
        }

        transform.localScale = transform.localScale * GetBulletSize(gun.bulletSize, gun.GetPlayer().BulletSizeBonus);

        SpriteRenderer bs = GetComponentInChildren<SpriteRenderer>();
        bs.material.SetColor("_Tint", gun.currentTint);
        if (gun.gunConfig.scatteredShots)
        {
            this.bulletDistance = GetBulletDistance(gun.bulletDistance, gun.GetPlayer().BulletDistanceBonus) * Random.Range(.1f, 1f);
        }
        else
        {
            this.bulletDistance = GetBulletDistance(gun.bulletDistance, gun.GetPlayer().BulletDistanceBonus);
        }

        if (gun.splashRadius > 0)
        {
            if (gun.gunConfig.scatteredShots)
            {
                SetupSplash(gun.gunConfig.scatteredShots ? gun.splashRadius * Random.Range(.75f, 2f) : gun.splashRadius, gun.damage / 2);
            } 
            else
            {
                SetupSplash(gun.splashRadius, gun.damage / 2);
            }
            
        }

        
    }

    private float GetBulletDistance(float baseBulletDistance, float modifierValue)
    {
        float modifier = Mathf.Abs(modifierValue);
        float difference = baseBulletDistance * modifier;
        float result;
        if (modifierValue > 0)
        {
            result = baseBulletDistance + difference;
        }
        else
        {
            result = baseBulletDistance - difference;
        }
        return result;
    }

    private Vector2 GetBulletForce(float baseBulletForce, float modifierValue)
    {
        float modifier = Mathf.Abs(modifierValue);
        float difference = baseBulletForce * modifier;
        float result;
        if (modifierValue > 0)
        {
            result = baseBulletForce + difference;
        } else
        {
            result = baseBulletForce - difference;
        }
        return transform.right * result;
    }

    private float GetBulletSize(float baseBulletSize, float modifierValue)
    {
        float modifier = Mathf.Abs(modifierValue);
        float difference = baseBulletSize * modifier;
        float result;
        if (modifierValue > 0)
        {
            result = baseBulletSize + difference;
        }
        else
        {
            result = baseBulletSize - difference;
        }
        return result;
    }

    public Gun GetGun()
	{
        return gun;
	}

    public void AreaDamage()
	{
        // Area damage
        float radius = splashRadius;
        if (splashRadius == 0)
		{
            radius = .35f;
		}

        foreach(Collider2D col in Physics2D.OverlapCircleAll(contactPoint.position, radius, LayerMask.GetMask("Obstacles")))
        {
            if (col.GetComponent<WallLight>())
			{
                col.GetComponent<WallLight>().OnDamage();
			}
		}
	}

    public void SpawnVfx(bool setColors)
    {
        if (gun.bulletParticlePrefab)
        {
            ParticleSystem particle = Instantiate(gun.bulletParticlePrefab, contactPoint.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

            if(gun.gunConfig.hasSplash)
            {
                ParticleSystem.MainModule main = particle.transform.GetChild(0).GetComponent<ParticleSystem>().main;

                main.startSize = splashRadius * 2;
            }

            if (particle.TryGetComponent<BulletParticlesChanger>(out var changeParticles))
                changeParticles.Setup(damage * 0.5f, splashRadius, gun.gunConfig.particleGradient);
                
            if (setColors)
            {
                particle.startColor = gun.bulletTint;
            }

            Destroy(particle.gameObject, particle.main.duration * 2);
        }

        if (hitEffectPrefab)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, contactPoint.transform.position, Quaternion.identity);
            hitEffect.transform.parent = null;
        }
	}

    public void SpawnEnemyHitVFX()
    {
        if (gun.enemyHitParticlePrefab)
        {
            ParticleSystem particle = Instantiate(gun.enemyHitParticlePrefab, contactPoint.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

            Destroy(particle.gameObject, particle.main.duration * 2);
        }
    }

    public void SetupSplash(float splashRadius, float splashDamage)
    {
        haveSplashDamage = true;
        this.splashRadius = splashRadius;
        this.splashDamage = splashDamage;
    }

    public void Splash(GameObject first)
    {
        if (haveSplashDamage) 
        { 
            haveSplashDamage = false;

            if (gun.gunType == GunType.Bazooka)
            {
                TremorsController tremor = FindObjectOfType<TremorsController>();
                if (tremor != null)
                {
                    tremor.CauseManmadeTremor();
                }
            }

            foreach (Collider2D col in Physics2D.OverlapCircleAll(contactPoint.position, splashRadius, LayerMask.GetMask("Enemies", "FlyEnemies", "Obstacles")))
            {
                if (first == col.gameObject) continue;

                if (col.gameObject.tag.Equals("Enemy"))
                {
                    var health = col.GetComponent<Health>();
                    
                    if (health != null)
                        health.Decrease(splashDamage, null, this);
                }
                else if (col.gameObject.tag.Equals("DestructibleObstacle"))
                {
                    print(col.GetComponent<BreakableObject>());
                    print(gun);
                    print(gun.GetPlayer());
                    col.GetComponent<BreakableObject>().OnDamage(damage, gun.GetPlayer());
                }            
            }
        }
    }

    void DetectEnemies()
    {
        Collider2D[] detected = Physics2D.OverlapCircleAll(transform.position, 3);

        for (int i = 0; i < detected.Length; i++)
        {
            if (detected[i].transform.CompareTag("Enemy"))
            {
                if (!enemies.Contains(detected[i].transform))
                    enemies.Add(detected[i].transform);
            }
        }
    }

    void ChaseTarget()
    {
        Toolkit2D.RotateAt(transform, lockedTarget, actualSpeed * Time.fixedDeltaTime);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = GetBulletForce(gun.bulletForce, gun.GetPlayer().BulletForceBonus);

        actualSpeed += Time.deltaTime * 10;
    }

    Transform GetClosestTarget(List<Transform> list)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    bool damaged;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !damaged)
        {
            Health health = other.GetComponent<Health>();
            if (health != null && !health.isInvulnerable)
            {
                if(!pierce)
                    damaged = true;

                Projectile p = other.GetComponent<Projectile>();
                if (p)
                {
                    return;
                }
                if (health)
                {
                    health.Decrease(damage, gun.GetPlayer(), this);
                    Splash(other.gameObject);
                    SpawnEnemyHitVFX();
                }

                if (gun.gunConfig.hasSplash)
                {
                    SpawnVfx(gun.setBulletParticleColor);
                }

                if (!pierce)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (other.CompareTag("DestructibleObstacle"))
        {
            BreakableObject brkObject = other.GetComponent<BreakableObject>();

            AreaDamage();
            PlaySfx(onHitSolidSound);
            SpawnVfx(gun.setBulletParticleColor);
            Splash(other.gameObject);

            if (!flight)
            {
                Destroy(gameObject);
            }

            if (!brkObject || brkObject && !brkObject.isBreakable) return;

            brkObject.OnDamage(damage, gun.GetPlayer());
        }
        else if (other.CompareTag("UnbreakableWall") || other.CompareTag("Gate") || other.CompareTag("CocoonShield"))
        {
            PlaySfx(onHitSolidSound);
            SpawnVfx(gun.setBulletParticleColor);
            Splash(other.gameObject);
            AreaDamage();

            if (!flight)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (gun.gunType == GunType.Bazooka)
        {
            SpawnVfx(gun.setBulletParticleColor);
            Splash(null);
        }
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    public void PlaySfx(AudioClip clip)
	{
        AudioSource audioSource = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<AudioSource>();
        audioSource.transform.localScale = Vector3.zero;
        audioSource.transform.position = transform.position;
        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.spatialBlend = .8f;
        audioSource.maxDistance = 1000;
        audioSource.PlayOneShot(clip);
        audioSource.gameObject.name = gameObject.name + " - SFX";
        Destroy(audioSource.gameObject, clip.length + .5f);
    }
}
