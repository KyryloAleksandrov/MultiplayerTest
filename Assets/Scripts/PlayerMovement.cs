using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera playerCamera;

    void Awake()
    {
        //controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir;
        if (playerCamera != null)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            right.Normalize();

            moveDir = forward * v + right * h;
        }

        else
        {
            moveDir = new Vector3(h, 0, v);
        }

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }
}
