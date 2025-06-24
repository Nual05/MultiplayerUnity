using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 10f;
    public float distance = 50f;
    private Vector3 originalPos;
    private Vector3 direction;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            originalPos = transform.position;
        }
    }

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    [ClientRpc]
    public void InitializeClientRpc(Vector3 dir)
    {
        if (IsServer) return;
        Initialize(dir);
    }

    void Update()
    {
        HandleMove();
    }

    private void HandleMove()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (IsServer && Vector3.Distance(originalPos, transform.position) > distance)
        {
            NetworkObject.Despawn(true);
        }
    }
}
