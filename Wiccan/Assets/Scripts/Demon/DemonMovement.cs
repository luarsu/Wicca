using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonMovement : MonoBehaviour
{
    public Transform Followtarget;
    public float Velocity;
    public float FollowOffset;
    public float desiredRotationSpeed = 0.1f;

    protected CharacterController characterController;      // Reference used to actually move the character.
    protected Vector3 desiredMoveDirection;

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(Followtarget.position, gameObject.transform.position);

        //if the demon is further away than the offset distance then start moving
        if (Mathf.Abs(dist) > FollowOffset)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        desiredMoveDirection = Followtarget.position - gameObject.transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Followtarget.position, Time.deltaTime * Velocity);
    }
}
