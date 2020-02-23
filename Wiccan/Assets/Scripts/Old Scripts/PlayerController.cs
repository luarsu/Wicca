using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private CharacterController charController;
    public float walkSpeed;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirectionSide = transform.right * horizontalInput * walkSpeed;
        Vector3 moveDirectionForward = transform.forward * verticalInput * walkSpeed;

        charController.SimpleMove(moveDirectionSide);
        charController.SimpleMove(moveDirectionForward);

    }
}
