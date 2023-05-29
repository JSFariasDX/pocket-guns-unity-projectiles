using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RastejanteController : MonoBehaviour
{
	[Header("Components")]
	Enemy enemy;
	Health health;

	[Header("Target")]
	public Transform target;
	public LayerMask targetMask;
	public float detectionRange = 15;
	public float minimumRange = 5;
	public float wallRange = 1;

	[Header("Animation")]
	Animator anim;
	string currentState;

	[Header("Move")]
	bool isMoving = true;
	Vector2 targetDirection;
	float targetDistance = 15;

	public float moveTime = 5;
	float timer = 0;
	Vector2 neutralDirection;

	[Header("Aim")]
	public Transform aimTransform;

	[Header("Fire")]
	public GameObject projectilePrefab;
	public float bulletSpeed = 5;

	bool canMove = false;
	bool moveNeutral = false;

	const string IDLE = "rastejante_idle";
	const string IDLE_UNDER = "rastejante_idle_under";
	const string DIG = "rastejante_dig";
	const string EMERGE = "rastejante_emerge";
	const string ATTACK = "rastejante_emerge_attack";

	// Start is called before the first frame update
	void Start()
	{
		enemy = GetComponent<Enemy>();
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (enemy.target && !target)
			target = enemy.target;

		DetectCollision();
	}

	private void FixedUpdate()
	{
		if (target == null) return;

		targetDistance = Vector2.Distance(transform.position, target.position);
		targetDirection = (transform.position - target.position).normalized;

		if (targetDistance < detectionRange && targetDistance > minimumRange)
		{
			ChangeAnimationState(DIG);

			if (isMoving && canMove)
				Move();

			//if (targetDistance < minimumRange)
			//    ChangeAnimationState(ATTACK);

			moveNeutral = false;
		}
		else if (targetDistance < minimumRange)
		{
			ChangeAnimationState(ATTACK);
		}
		else
		{
			moveNeutral = true;

			if (currentState != IDLE)
			{
				ChangeAnimationState(EMERGE);

				if (anim.GetCurrentAnimatorStateInfo(0).IsName(EMERGE) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
				{
					ChangeAnimationState(IDLE);
				}
			}

			if (timer >= moveTime)
				SelectDirection();
			else
				timer += Time.deltaTime;

			if (isMoving && moveNeutral)
				NeutralMove();
		}
	}

	void Move()
	{
		Vector2 direction = (transform.position - target.position).normalized;

		transform.Translate(-direction * enemy.speed * Time.deltaTime);
	}

	void SelectDirection()
	{
		int x = Random.Range(-2, 2);
		int y = Random.Range(-2, 2);

		neutralDirection = new Vector2(x, y).normalized;

		timer = 0;
	}

	void NeutralMove()
	{
		transform.Translate(neutralDirection * enemy.speed * Time.deltaTime);
	}

	public void Shoot()
	{
		//if (fireTimer < fireRate) return;

		Vector2 direction = (aimTransform.position - target.position).normalized;

		GameObject projectile = Instantiate(projectilePrefab, aimTransform.position, Quaternion.identity);
		projectile.GetComponent<Rigidbody2D>().AddForce(-direction * bulletSpeed, ForceMode2D.Impulse);

		//fireTimer = 0;
	}

	public void CanMoveOn()
	{
		canMove = true;
	}

	public void CanMoveOff()
	{
		canMove = false;
	}

	void DetectCollision()
	{
		RaycastHit2D hitUp = Physics2D.Raycast(transform.position, moveNeutral ? neutralDirection : -targetDirection, wallRange, targetMask);

		Debug.DrawRay(transform.position, moveNeutral ? neutralDirection : -targetDirection * wallRange, Color.blue);

		if (hitUp.transform)
			isMoving = false;
		else
			isMoving = true;

		//Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, wallRange, targetMask);

		//if (col.Length > 0)
		//    isMoving = false;
		//else
		//    isMoving = true;

		//foreach (Collider2D item in col)
		//{
		//    if (item.transform != null)
		//    {
		//        if (item.transform.CompareTag("Player"))
		//        {
		//            item.transform.GetComponent<Health>().Decrease(10);
		//        }

		//        //ResetDirection();
		//    }
		//}
	}

	public void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		anim.Play(newState);

		currentState = newState;
	}
}
