using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum AimStates
{
    Idle = 1,
    Activated = 2,
}

public class Aim : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite shootingCrosshair;
    public Sprite pistolCrosshair;
    public Sprite machinegunCrosshair;
    public Sprite shotgunCrosshair;
    public Sprite bazookaCrosshair;

    float aimReach = 2.5f;

    AimStates aimState = AimStates.Idle;

    public Vector2 aimInput;
    public Vector2 mouseInput;

    Player player;

    float aimAngle = 0;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = player.transform.position;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        Cursor.visible = false;
    }

    void Update()
    {
        if (PauseManager.Instance.IsGamePaused())
        {
            return;
        }

        if (aimAngle <= 20 && aimAngle >= -45)
        {
            //Debug.Log("Looking right");
            player.SetFacingDirection(PlayerFacingDirections.Right);
        }
        else if (aimAngle < -45 && aimAngle >= -140)
        {
            //Debug.Log("Looking down");
            player.SetFacingDirection(PlayerFacingDirections.Down);
        }
        else if (aimAngle <= -140 || aimAngle > 120)
        {
            //Debug.Log("Looking left");
            player.SetFacingDirection(PlayerFacingDirections.Left);
        }
        else if (aimAngle <= 120 && aimAngle > 20)
        {
            //Debug.Log("Looking up");
            player.SetFacingDirection(PlayerFacingDirections.Up);
        }

        SetAimState(player.GetCurrentGun().IsShootHold() ? AimStates.Activated : AimStates.Idle);

        UpdateSprite();
    }

    public void SetAimState(AimStates state)
	{
        aimState = state;
	}

    public void SetupAim(PlayerIndexSettings playerSettings)
	{
        shootingCrosshair = playerSettings.shootingCrosshair;
        pistolCrosshair = playerSettings.pistolCrosshair;
        machinegunCrosshair = playerSettings.machinegunCrosshair;
        shotgunCrosshair = playerSettings.shotgunCrosshair;
        bazookaCrosshair = playerSettings.bazookaCrosshair;

        UpdateSprite();
    }

    public SpriteRenderer GetSpriteRenderer()
	{
        return spriteRenderer;
	}

    public void UpdateSprite()
	{
        if (aimState == AimStates.Idle)
        {
            spriteRenderer.sprite = GetAimSprite();
        }
        else if (aimState == AimStates.Activated)
        {
            if (ScreenManager.currentScreen == Screens.Lobby && !player.isOnTutorial) return;

            spriteRenderer.sprite = shootingCrosshair;
        }
    }

    Sprite GetAimSprite()
	{
		switch (player.GetCurrentGun().gunType)
		{
            case GunType.Pistol: return pistolCrosshair;
            case GunType.MachineGun: return machinegunCrosshair;
            case GunType.Shotgun: return shotgunCrosshair;
            default: return bazookaCrosshair;
        }
	}

    public void OnAimInput(InputAction.CallbackContext ctx)
    {
        if (spriteRenderer != null)
        {
            if (!spriteRenderer.enabled)
                spriteRenderer.enabled = true;
        }
        
        SetAimAlpha(1);
        // Input value
        aimInput = ctx.ReadValue<Vector2>();
        //Debug.Log(aimInput);

        string device = ctx.control.device.ToString();
        bool isMouse = device.Contains("Mouse");

        if (isMouse)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(mouseInput);
            if(gameObject != null)
                transform.position = new Vector3(newPos.x, newPos.y, 0);
        } 
        else
        {
            Vector3 position = new Vector3(aimInput.x * aimReach, aimInput.y * aimReach, 0);
            Vector3 pos = player.transform.position + position;
            if (gameObject != null)
                transform.position = Vector3.MoveTowards(transform.position, pos, 5f);
        }

        SetGunsLookToAim();
    }

    public void OnMouseMoved(InputAction.CallbackContext ctx)
	{
        if (ctx.performed) mouseInput = ctx.ReadValue<Vector2>();
		else if (ctx.canceled) mouseInput = Vector2.zero;
    }

    public void SetGunsLookToAim()
    {
        foreach (Gun gun in player.currentGuns)
        {
            SetGunLookToAim(gun);
        }
    }

    public void SetGunLookToAim(Gun gun)
	{
        Vector2 direction = transform.position - player.GetCurrentGun().transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aimAngle = angle;

        if(!PauseManager.Instance.IsGamePaused())
            gun.SetRotation(aimAngle);
    }

    public void ResetAim(InputAction.CallbackContext ctx)
    {
        string device = ctx.control.device.ToString();
        bool isMouse = device.Contains("Mouse");
        if (!isMouse)
        {
            SetAimAlpha(0);
            aimInput = Vector2.zero;
        }
    }

    void SetAimAlpha(float value)
    {
        if (spriteRenderer != null)
        {
            Color current = spriteRenderer.color;
            current.a = value;
            spriteRenderer.color = current;
        }
    }
}
