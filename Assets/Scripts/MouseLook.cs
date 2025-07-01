using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MouseLook : NetworkBehaviour
{
    public Transform cameraHolder;
    public float mouseSensitivity = 2f;
    float xRotation = 0f;

    void Start()
    {
        if (!isLocalPlayer)
        {
            // Tắt camera người khác
            cameraHolder.gameObject.SetActive(false);
            enabled = false;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;


        transform.Rotate(Vector3.up * mouseX);

      
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); 
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
