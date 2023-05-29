using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public bool isAlive = true;
    public float maxValue = 100f;
    public float currentMaxValue;
    float value = 100f;
    public bool isInvulnerable = false;
    public float invulnerabilityTime = 0f;
    public Image healthBar;
    public GameObject toTrack;
    public bool keepInvulnerable;
    float currentHPModifier = 0;
    [HideInInspector] public bool damageEnabled = true;
    bool lastHit = false;

    private void Start()
    {
        damageEnabled = true;
        value = maxValue;
        currentMaxValue = maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (value <= 0 && isAlive)
        {
            isAlive = false;
            if (toTrack != null)
            {
                if (transform.tag == "Enemy")
                {
                    if (GetComponent<Enemy>().canIncrementEggs)
                    {
                        foreach(PlayerAttributes attributes in FindObjectsOfType<PlayerAttributes>())
						{
                            attributes.AddEnemyCount(1);
                        }
                    }
                }

                toTrack.SendMessage("onHealthEnd");
            }
        }

        lastHit = value == 1;
    }

    private void FixedUpdate()
    {
        bool isPlayer = GetComponentInParent<Player>();
        if (isPlayer)
        {
            float playerHPModifier = GetComponentInParent<Player>().MaxHPBonus;
            if (playerHPModifier != currentHPModifier)
            {
                HandleMaxHPChange(playerHPModifier);
            }
        }
    }

    private void HandleMaxHPChange(float newModifier)
    {
        float modifier = Mathf.Abs(newModifier);
        float difference = maxValue * modifier;
        float currentPercentage = GetCurrentPercentage();
        if (newModifier > 0)
        {
            currentMaxValue = maxValue + difference;
            SetHealth(currentMaxValue * currentPercentage);
        } else
        {
            currentMaxValue = maxValue - difference;
            SetHealth(currentMaxValue * currentPercentage);
        }
        currentHPModifier = newModifier;
    }

    public void Decrease(float damageAmount, Player shooterPlayer = null, Bullet bullet = null, float invulnerability = -Mathf.Infinity) 
    {
        if (isInvulnerable && invulnerability == -Mathf.Infinity)
        {
            return;
        }

        float damageModifier = 1;
        if (GetComponent<Enemy>())
        {
            Enemy enemy = GetComponent<Enemy>();
            enemy.lastShooterPlayer = shooterPlayer;

            damageModifier = enemy.GetDamageModifier(bullet);
        }

        damageAmount = damageAmount * damageModifier;
        
        if (IsDamageEnabled()) 
		{
            if (damageAmount >= value && GetComponent<Player>() && !lastHit)
                value = 1;
            else
                value -= damageAmount;

            if (value < 0f)
                value = 0f;
        }

        if (toTrack != null)
        {
            toTrack.SendMessage("onDamage", damageAmount);
            if (invulnerabilityTime > 0 || invulnerability > 0)
            {
                SetInvulnerability(true);
                Invoke("DisableInvulnerability", Mathf.Min(invulnerabilityTime, invulnerability));
            }
        }
    }

    public void SetDamageEnabled(bool enabled)
	{
        damageEnabled = enabled;
	}
    
    public bool IsDamageEnabled()
	{
        return damageEnabled;
	}

    public void SetInvulnerability(bool value)
    {
        isInvulnerable = value;
    }

    public bool IsAlive()
	{
        return GetCurrentHealth() > 0;
	}

    void DisableInvulnerability()
    {
        if (keepInvulnerable) return;
        SetInvulnerability(false);
    }

    public float GetCurrentHealth()
    {
        return value;
    }

    public float GetMaxHealth()
    {
        return currentMaxValue;
    }

    public void Increase(float amount)
    {
        if (!isAlive)
            return;
            
        value += amount;

        if (value > currentMaxValue)
            SetHealth(currentMaxValue);
    }

    public void SetHealth(float amount)
    {
        value = amount;

        if (value > currentMaxValue)
            value = currentMaxValue;

        if (amount > 0)
        {
            isAlive = true;
        }
    }

    public void SetMaxHealth(float maxHealth)
	{
        this.maxValue = maxHealth;
        currentMaxValue = maxHealth;
        value = maxHealth;
	}

    public float GetCurrentPercentage()
    {
        return value / currentMaxValue;
    }

    public void Heal(float rate)
    {
        float healAmount = maxValue * rate;
        float finalHealth = GetCurrentHealth() + healAmount;
        SetHealth(Mathf.Min(finalHealth, maxValue));
    }
}