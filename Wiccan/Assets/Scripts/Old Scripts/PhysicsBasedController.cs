using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBasedController : MonoBehaviour {
    /*
    private float maxSpeed = 9.0f;
    private Vector3 speed = new Vector3(0,0,0);
    private Vector3 lastSpeed;
    private float decelerationSpeed = 0.0f;
    private float horSpeed = 6.0f;
    private float acceleration = 2.0f;
    private float jumpSpeed = 300.0f;
    private float distToGround;
    
    private Rigidbody rigidbody;
    private CapsuleCollider collider;

    void Start()
    {
        // no physics rotations, please!
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        rigidbody.freezeRotation = true;
        distToGround = collider.bounds.extents.y;
    }

    void  Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            horSpeed = 6;
            if (decelerationSpeed > 0)
            {
                decelerationSpeed = decelerationSpeed - 10.0f * Time.deltaTime;
            }
            else
            {
                decelerationSpeed = 0;
            }

            Vector3 decSpeed = decelerationSpeed * lastSpeed;
            decSpeed.y = rigidbody.velocity.y;

            //Debug.Log(decSpeed);
            // set new rigidbody velocity:
            rigidbody.velocity = decSpeed;
        }
        else
        {
            // get the direction it must walk in:
            Vector3 speed = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal")).normalized;
            // convert from local to world space and multiply by horizontal speed:
            if (horSpeed < maxSpeed)
            {
                horSpeed = horSpeed + acceleration * Time.deltaTime;
            }

            //Record speed for decceleration
            decelerationSpeed = horSpeed;

            lastSpeed = transform.TransformDirection(speed);

            //apply the speed to the direction
            speed = horSpeed * transform.TransformDirection(speed);
            // keep rigidbody vertical velocity to preserve gravity action:
            speed.y = rigidbody.velocity.y;


            // set new rigidbody velocity:
            rigidbody.velocity = speed;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rigidbody.AddForce(0, jumpSpeed, 0);
        }
        
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }
    */
}
