using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularOrganizer : MonoBehaviour
{
	public float fDistance;

	[Range(0, 360)]
	public float angle = 90;

	//[Range(0, 360)]
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

		CreateCircle();
	}

	void CreateCircle()
	{
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
}
