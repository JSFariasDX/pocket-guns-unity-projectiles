using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineTargetGroup group;

	private void Start()
	{
        transform.position = new Vector3(0, -1, -10);
        group = FindObjectOfType<CinemachineTargetGroup>();
	}
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    public void SetSingleTargetCameraFocus(Transform target)
	{
        Transform t = new GameObject("PlayerCamTarget").transform;
        t.parent = target;
        t.localPosition = new Vector3(0, 0, -50);

        FindObjectOfType<CinemachineVirtualCamera>().m_Follow = t;
    }

	public CinemachineTargetGroup.Target AddTarget(Transform target, float radius = 1f, float weight = 1f)
	{
        List<CinemachineTargetGroup.Target> list = new List<CinemachineTargetGroup.Target>(group.m_Targets);

        CinemachineTargetGroup.Target targetObject = new CinemachineTargetGroup.Target(); 
        targetObject.target = new GameObject("PlayerCamTarget").transform;
        targetObject.target.parent = target;
        targetObject.target.localPosition = new Vector3(0, 0, -50);
        targetObject.radius = radius;
        targetObject.weight = weight;

        list.Add(targetObject);

        group.m_Targets = list.ToArray();

        return targetObject;
    }

    public void DisableTarget(CinemachineTargetGroup.Target target)
	{
        List<CinemachineTargetGroup.Target> list = new List<CinemachineTargetGroup.Target>(group.m_Targets);
        list.Remove(target);

        group.m_Targets = list.ToArray();
    }

    public CinemachineTargetGroup GetTargetGroup() { return group; }
}
