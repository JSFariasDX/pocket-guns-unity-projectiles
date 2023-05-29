using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
	public bool canCatch;
	private SpriteRenderer lootSprite;
	protected SpriteRenderer shadowSprite;
	private AudioSource getLootSound;

	protected Vector2 spriteMoveDirection;
	protected float toggleSpriteDirectionTimer = .5f;
	protected AnimationState animationState;

	protected ItemPopup popup;
	protected bool lootCollected;
	protected Player player;

	protected virtual void Start()
	{
		lootSprite = transform.Find("LootSprite").GetComponent<SpriteRenderer>();
		shadowSprite = transform.Find("Shadow").GetComponent<SpriteRenderer>();

		startLootSpriteY = lootSprite.transform.position.y;
		spriteMoveDirection = Vector2.up;
		getLootSound = GetComponent<AudioSource>();
		PlayDropAnimation();
		popup = FindObjectOfType<ItemPopup>();
	}

	protected void FixedUpdate()
	{
		if (animationState == AnimationState.Idle) IdleAnimation();
		else if (animationState == AnimationState.Dropping) DropAnimation();
	}

	protected virtual void IdleAnimation()
	{
		if (!lootSprite) return;

		lootSprite.transform.Translate(spriteMoveDirection * .25f * Time.fixedDeltaTime);
		toggleSpriteDirectionTimer -= Time.fixedDeltaTime;

		if (toggleSpriteDirectionTimer < 0)
		{
			spriteMoveDirection = spriteMoveDirection * -1;
			toggleSpriteDirectionTimer = .5f;
		}
	}

	protected float startLootSpriteY;
	float horizontalDropSpeed;
	float verticalDropSpeed;
	protected virtual void DropAnimation()
	{
		verticalDropSpeed -= Time.fixedDeltaTime * 10;

		lootSprite.transform.Translate(0, verticalDropSpeed * Time.deltaTime, 0);
		transform.Translate(horizontalDropSpeed * Time.deltaTime, 0, 0);

		if (Mathf.Abs(lootSprite.transform.position.y - startLootSpriteY) < .1f && verticalDropSpeed < 0)
		{
			transform.position = new Vector3(transform.position.x, startLootSpriteY, transform.position.z);
			animationState = AnimationState.Idle;
			canCatch = true;
		}
	}

	public virtual void ShowPopUp(Player player)
	{
	}

	private void PlayDropAnimation()
	{
		animationState = AnimationState.Dropping;
		verticalDropSpeed = 4;

		List<float> speeds = new List<float>();
		if (Physics2D.Raycast(transform.position, Vector2.left, 2, LayerMask.GetMask("Walls", "Obstacles")))
		{
			speeds.Add(1.5f);
		}
		if (Physics2D.Raycast(transform.position, Vector2.right, 2, LayerMask.GetMask("Walls", "Obstacles")))
		{
			speeds.Add(-1.5f);
		}

		if (speeds.Count == 0)
		{
			speeds.Add(0);
		}

		horizontalDropSpeed = 0;
		//horizontalDropSpeed = speeds[Random.Range(0, speeds.Count)];
	}

	public virtual void GetLoot(Player player)
	{
		lootCollected = true;

		if (popup)
		{
			popup.transform.parent = null;
			popup.Hide();
		}

		if (getLootSound != null)
			getLootSound.Play();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (lootCollected)
			return;
			
		if (collision.CompareTag("PlayerCollider") && canCatch)
		{
			Player player = collision.GetComponentInParent<Player>();
			player.SetCurrentLoot(this);

			if (player)
				ShowPopUp(player);
			else
				print("NO PLAYER HERE");
			//GetLoot(player);
		}
	}

	protected ItemPopup GetPopup()
	{
		if (!popup) popup = FindObjectOfType<ItemPopup>();
		return popup;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("PlayerCollider") && canCatch)
		{
			Player player = collision.GetComponentInParent<Player>();
			if (player.currentLoot == this)
			{
				popup.Hide();
				player.SetCurrentLoot(null);
				player = null;
			}
			//else if (player.currentLoot != this)
			//{
			//	popup.gameObject.SetActive(false);
			//}
		}
	}

}


public enum LootType
{
	Item, Gun
}

public enum AnimationState
{
	Idle, Dropping
}