using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketMorBattlePlatform : MonoBehaviour
{
	[SerializeField] GameObject col;
    [SerializeField] Transform target;
	[SerializeField] float moveSpeed;
	[SerializeField] GameObject background;
	[SerializeField] float rotateSpeed;
    bool isMoving;
	bool isRotatingBackground;

	bool isStarted;
	[SerializeField] CinemachineVirtualCamera cam;


	private void Start()
	{
		col.SetActive(false);
	}

	private void Update()
	{
		if (isMoving)
		{
			if (Vector2.Distance(transform.position, target.position) > .25f)
			{
				Vector2 moveVector = (target.position - transform.position).normalized * moveSpeed * Time.deltaTime;
				transform.Translate(moveVector);
				CinemachineShake.Instance.ShakeCamera(.5f, 1f);
			}
			else
			{
				isMoving = false;
				isRotatingBackground = true;
				FindObjectOfType<PocketMor>().StartFight();
				ActivateExternalForcedMovementOnPlayers(false, Vector2.zero, 0);

				CinemachineFramingTransposer body = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
				body.m_MaximumOrthoSize = 9;

				CameraManager cameraManager = FindObjectOfType<CameraManager>();
				for(int i = 0; i < cameraManager.GetTargetGroup().m_Targets.Length; i++)
				{
					cameraManager.GetTargetGroup().m_Targets[i].radius = 10;
				}
				cameraManager.AddTarget(transform, 10, 1);

				//cam.m_Lens.OrthographicSize = 10;
				//cam.Follow = transform;
			}
		}
		else if (isRotatingBackground)
		{
			background.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
		}
	}

	public void StartMovement()
	{
		isStarted = true;
		isMoving = true;
		col.SetActive(true);

		ActivateExternalForcedMovementOnPlayers(true, Vector2.up, moveSpeed);
	}

	void ActivateExternalForcedMovementOnPlayers(bool activated, Vector2 direction, float speed)
	{
		foreach(Player player in GameplayManager.Instance.players)
		{
			player.SetExternalForcedMovement(activated, direction, speed);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isStarted) return;

		if (collision.CompareTag("CollectCol"))
		{
			StartMovement();

			Player player = collision.GetComponentInParent<Player>();

			foreach (Player p in GameplayManager.Instance.players)
			{
				if (p != player)
				{
					p.transform.position = player.transform.position + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), 0);
				}
			}
		}
	}
}
