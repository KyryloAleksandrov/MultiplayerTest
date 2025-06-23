using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 75f;

    [SerializeField] public Camera playerCamera;

    private float pitch = 0f;
    private bool cursorLocked = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            playerCamera.gameObject.SetActive(false);
            return;
        }

        LockCursor();
    }



    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
            if (cursorLocked)
                LockCursor();
            else
                UnlockCursor();
            return;
        }

        if (!cursorLocked) return;

        float mx = Input.GetAxis("Mouse X") * lookSensitivity;
        float my = Input.GetAxis("Mouse Y") * lookSensitivity;

        transform.Rotate(Vector3.up * mx);

        pitch = Mathf.Clamp(pitch - my, minPitch, maxPitch);
        playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            UnlockCursor();
        }
    }
}
