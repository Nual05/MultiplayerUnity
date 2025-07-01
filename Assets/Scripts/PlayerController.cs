using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float speedShoot = 10f;
    public Animator anim;

    void Start()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.Self);

    
        float speed = direction.magnitude;
        anim.SetFloat("Velocity", direction.magnitude);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Shoot");
            CmdFire();
        }
    }

    [Command] // Client ruft aber in server wird gearbeitet
    void CmdFire()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.GetComponent<Projectile>().shooter = this.gameObject;
        NetworkServer.Spawn(proj); // Copy prefab to Client 
        proj.GetComponent<Rigidbody>().velocity = firePoint.forward * speedShoot;

        Destroy(proj, 2f);
    }
}
