using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TrackType
{
    Player, Pet, Enemy
}

public class HealthBar : MonoBehaviour
{
    public TrackType type;
    [SerializeField] private Health health;
    private Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        //health = toTrack.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health)
        {
            float currentHealth = health.GetCurrentHealth();
            float maxHealth = health.GetMaxHealth();
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    public void SetupBar(Health health)
	{
        this.health = health;
	}
}
