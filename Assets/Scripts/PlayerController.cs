using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    public float speed = 1f;
    public float speedRotate = 10f;
    public float speedLook = 10f;
    public float speedLag = 10f;

    public Vector3 cameraOffset = Vector3.zero;
    public GameObject tower;
    public GameObject bullet;
    public Transform bulletPos;
    public Camera cam;

    private Rigidbody rb;
    private Vector2 moveInput = Vector2.zero;
    private Vector2 mouseLook = Vector2.zero;
    private Vector2 mouseInput = Vector2.zero;
    private Quaternion targetRotation;
    private Quaternion rotate = Quaternion.identity;

    public Vector3 centerPosition = Vector3.zero;
    public Vector3 spawnRange = new Vector3(20f, 0f, 20f);

    private NetworkVariable<Quaternion> towerRotation = new NetworkVariable<Quaternion>(
        Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
            cam.gameObject.SetActive(true);
        }
        else
        {
            GetComponent<PlayerInput>().enabled = false;
            cam.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRange.x / 2f, spawnRange.x / 2f),
            Random.Range(-spawnRange.y / 2f, spawnRange.y / 2f),
            Random.Range(-spawnRange.z / 2f, spawnRange.z / 2f)
        );
        Vector3 spawnPosition = centerPosition + randomOffset;
        transform.position = spawnPosition;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        moveInput = context.ReadValue<Vector2>();
        SendInputToServerRpc(moveInput, mouseLook);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        mouseLook = context.ReadValue<Vector2>();
        SendInputToServerRpc(moveInput, mouseLook);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!IsOwner || !context.performed) return;

        FireServerRpc();
    }

    [ServerRpc]
    private void SendInputToServerRpc(Vector2 move, Vector2 look)
    {
        HandleMovement(move);
        HandleRotation(look);
        SendInputToClientRpc(move, look);
    }

    [ClientRpc]
    private void SendInputToClientRpc(Vector2 move, Vector2 look)
    {
        if (IsOwner) return; 
        HandleMovement(move);
        HandleRotation(look);
    }

    private void HandleMovement(Vector2 move)
    {
        if (cam == null || rb == null) return;

        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cam.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = (forward * move.y + right * move.x).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speedRotate);
            rotate = transform.rotation;
        }
        else
        {
            transform.rotation = rotate;
        }

        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void HandleRotation(Vector2 look)
    {
        mouseInput.x += look.y * speedLook * Time.deltaTime;
        mouseInput.y += look.x * speedLook * Time.deltaTime;

        float xRotationClamp = Mathf.Clamp(mouseInput.x, -50f, 50f);
        targetRotation = Quaternion.Euler(-xRotationClamp, mouseInput.y, 0f);

        Quaternion targetTowerRotate = Quaternion.Euler(0f, mouseInput.y, 0f);

        if (IsOwner)
        {
            tower.transform.rotation = Quaternion.Lerp(tower.transform.rotation, targetTowerRotate, Time.deltaTime * speedLag);
            towerRotation.Value = tower.transform.rotation;
        }
        else
        {
            tower.transform.rotation = towerRotation.Value;
        }

        if (cam != null)
        {
            cam.transform.position = transform.position - targetRotation * cameraOffset + Vector3.up * Time.deltaTime;
            cam.transform.rotation = targetRotation;
        }
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        if (bullet != null && bulletPos != null)
        {
            GameObject b = Instantiate(bullet, bulletPos.position, Quaternion.identity);
            b.GetComponent<Bullet>().Initialize(tower.transform.forward);
            b.GetComponent<NetworkObject>().Spawn(true);
            b.GetComponent<Bullet>().InitializeClientRpc(tower.transform.forward);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            tower.transform.rotation = towerRotation.Value;
            return;
        }
        SendInputToServerRpc(moveInput, mouseLook);
        HandleRotation(mouseLook);
    }

}
