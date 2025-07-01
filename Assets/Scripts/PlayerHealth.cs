using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int health = 100;
    [SyncVar]
    public int maxHealth = 100;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        health -= amount;
        if (health <= 0)
        {
            RpcRespawn();
            Die();
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }

    [Server]
    void Die()
    {
        var teamComp = GetComponent<PlayerTeam>();
        if (teamComp != null)
        {
   
            Team enemy = teamComp.team == Team.Red ? Team.Blue : Team.Red;
            ScoreManager.Instance.AddScore(enemy);
        }

        NetworkServer.Destroy(gameObject);
    }
}
