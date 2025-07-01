using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public Image healthFillImage; 

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth != null)
        {
            UpdateHealthBar(playerHealth.health);
        }
    }

    void UpdateHealthBar(int health)
    {
        if (healthFillImage == null) return;
        healthFillImage.fillAmount = (float)health / playerHealth.maxHealth;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        var teamComponent = GetComponent<PlayerTeam>();
        if (teamComponent != null)
        {
            Team team = teamComponent.team;
            healthFillImage.color = (team == Team.Red) ?  Color.blue : Color.red;
        }
        if (playerHealth != null)
        {
            UpdateHealthBar(playerHealth.health);
        }
    }
}

