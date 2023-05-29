using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SinisterQueenIntro : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform cameraPosition;

    private CinemachineTargetGroup.Target _camTarget;

    private bool _isIntroActivated;

    public void ActivateSinisterQueen()
    {
        var sinisterQueen = FindObjectOfType<SinisterQueen>(true);
        sinisterQueen.transform.position = spawnPosition.position;
        sinisterQueen.gameObject.SetActive(true);
        sinisterQueen.Activate();
    }

    public void DisableCinemachineTarget()
    {
        FindObjectOfType<CameraManager>().DisableTarget(_camTarget);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isIntroActivated)
            return;

        if (other.CompareTag("PlayerCollider"))
        {
            GetComponent<Animator>().SetTrigger("Activate");
            _camTarget = FindObjectOfType<CameraManager>().AddTarget(cameraPosition, 2f, 0.75f);
            _isIntroActivated = true;
        }
    }
}
