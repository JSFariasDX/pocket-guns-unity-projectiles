using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeFire : MonoBehaviour
{
    public float fDistance;

    [Range(0, 360)]
    public float angle = 90;
    float offAngle; // Use this if the angle is 360

    [Range(0, 360)]
    public float startAngle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (angle >= 360)
            angle = 360 - (360 / transform.childCount);

        CreateCone();
    }

    void CreateCone()
    {
        //muzzles[0].transform.localRotation = Quaternion.AngleAxis(0, Vector3.forward);
        //muzzles[1].transform.localRotation = Quaternion.AngleAxis(-angle / (muzzles.Count - 1), Vector3.forward);
        //muzzles[2].transform.localRotation = Quaternion.AngleAxis(angle / (muzzles.Count - 1), Vector3.forward);

        float fOffsetAngle = (angle) / (transform.childCount - 1);

        float fAngle = startAngle - (angle / (transform.childCount / 2));
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform item = transform.GetChild(i);

            Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
            item.localPosition = vPos * fDistance;

            fAngle += fOffsetAngle;
        }
    }

    public void Shoot()
    {

    }
}
