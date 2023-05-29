using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class EggRoom : MonoBehaviour
{
    public Transform entryPoint;

    private void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        p.GetComponent<Rigidbody2D>().position = entryPoint.position;
        GameObject mc = GameObject.FindGameObjectWithTag("MainCamera");
        mc.GetComponentInChildren<CinemachineVirtualCamera>().Follow = p.transform;
        ThemeMusicManager.Instance.Stop();
    }
}
