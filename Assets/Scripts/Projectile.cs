using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Mirror.NetworkRuntimeProfiler;

public class Projectile : NetworkBehaviour
{
    public GameObject shooter;
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            var targetTeam = collision.gameObject.GetComponent<PlayerTeam>();
            var shooterTeam = shooter.GetComponent<PlayerTeam>(); 

            if (targetTeam != null && shooterTeam != null)
            {
                if (targetTeam.team == shooterTeam.team)
                {
                    NetworkServer.Destroy(gameObject);
                    return;
                }
            }

            var health = collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(20);

            NetworkServer.Destroy(gameObject);
        }
    }
}
