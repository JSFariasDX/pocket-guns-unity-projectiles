using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelProjectile : Projectile
{
    [Header("Shrapnel")]
    public bool commonShrapnel = false;
    public GameObject shrapnelPrefab;
    public GameObject shrapnelChasePrefab;
    [SerializeField] int shrapnelCount = 15;

    [Header("SFX")]
    [SerializeField] AudioClip explosionSound;
    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (shooterTransform)
            shooterTransform.GetComponent<Enemy>().audioSource.PlayOneShot(explosionSound);
        else if (source)
            source.PlayOneShot(explosionSound);

        OnTrigger();
        SendShrapnel(shrapnelCount);
    }

    void SendShrapnel(int projectilesCount)
    {
        Vector2 startPoint = transform.position;
        float angleStep = 360f / projectilesCount;
        float angle = 0f;

        for (int i = 0; i <= projectilesCount - 1; i++)
        {

            float projectileDirXposition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * 5;
            float projectileDirYposition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * 5;

            Vector2 projectileVector = new Vector2(projectileDirXposition, projectileDirYposition);
            Vector2 projectileMoveDirection = (projectileVector - startPoint).normalized * 7;

            var proj = Instantiate(commonShrapnel ? shrapnelPrefab : shrapnelChasePrefab, startPoint, Quaternion.identity);
            if (i != 0)
            {
                proj.GetComponent<AudioSource>().mute = true;
            }

            //proj.GetComponent<Projectile>().Setup(GameObject.Find("Target").transform, 10, 7, shooterTransform);
            if (commonShrapnel)
            {
                proj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);
            }
            else
            {
                Vector3 toTarget = (Vector3)projectileVector - transform.position;
                float p_angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg - 90;
                var q = Quaternion.AngleAxis(p_angle, proj.transform.forward);
                proj.transform.rotation = q;

                proj.GetComponent<Projectile>().Setup(shooterTransform.GetComponent<Boss>().GetNearPlayer().transform, 10, 5, shooterTransform);
            }

            angle += angleStep;
        }
    }
}
