using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonMovement : MonoBehaviour
{
    public Transform Followtarget;
    public float minVelocity;
    public float maxVelocity;
    public float minFollowDistance;
    public float maxFollowDistance;
    public float desiredRotationSpeed = 0.1f;

    protected CharacterController characterController;      // Reference used to actually move the character.
    protected float distance;
    protected Vector3 desiredMoveDirection;

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(Followtarget.position, gameObject.transform.position);

        //if the demon is further away than the offset distance then start moving
        if (Mathf.Abs(distance) > minFollowDistance)
        {
            FollowTarget();
        }
    }

    void FollowTarget()
    {
        desiredMoveDirection = Followtarget.position - gameObject.transform.position;


        //adjust velocity based on how close or far it is from the target
        float lerpValue = distance / maxFollowDistance;
        float adjustedVelocity = Mathf.Lerp(minVelocity, maxVelocity, lerpValue);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, Followtarget.position, Time.deltaTime * adjustedVelocity);
    }
}
