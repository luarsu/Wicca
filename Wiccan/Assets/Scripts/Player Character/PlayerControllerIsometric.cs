﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerIsometric : MonoBehaviour
{
    protected static PlayerControllerIsometric s_Instance;
    public static PlayerControllerIsometric instance { get { return s_Instance; } }

    public float maxForwardSpeed = 8f;        // How fast the character can run.
    public float gravity = 20f;               // How fast the character accelerates downwards when airborne.
    public float jumpSpeed = 10f;             // How fast the character takes off when jumping.
    public float minTurnSpeed = 400f;         // How fast the character turns when moving at maximum speed.
    public float maxTurnSpeed = 1200f;        // How fast the character turns when stationary.
    public float idleTimeout = 5f;            // How long before the character starts considering random idles.
    public bool canAttack;                    // Whether or not the character can attack.

    protected bool m_IsGrounded = true;            // Whether or not the character is currently standing on the ground.
    protected bool m_PreviouslyGrounded = true;    // Whether or not the character was standing on the ground last frame.
    protected bool m_ReadyToJump;                  // Whether or not the input state and the character are correct to allow jumping.
    protected float m_DesiredForwardSpeed;         // How fast the character aims be going along the ground based on input.
    protected float m_ForwardSpeed;                // How fast the character is currently going along the ground.
    protected float m_VerticalSpeed;               // How fast the character is currently moving up or down.
    protected PlayerInput m_Input;                  // Reference used to determine how the character should move.
    protected CharacterController m_CharCtrl;      // Reference used to actually move the character.
    protected Material m_CurrentWalkingSurface;    // Reference used to make decisions about audio.
    protected Quaternion m_TargetRotation;         // What rotation the character is aiming to have based on input.
    protected float m_AngleDiff;                    // Angle in degrees between Ellen's current rotation and her target rotation.
    protected bool m_InAttack;                     // Whether the character is currently in the middle of a melee attack.
    protected bool m_InCombo;                      // Whether the character is currently in the middle of her melee combo.

    //As the camera is isometric, we need to get the foward and right vector for our game (That is not the same as the character forward or the world normal)
    protected Vector3 ForwardIsometric;
    protected Vector3 RightIsometric;

    // These constants are used to ensure the character moves and behaves properly.
    // It is advised you don't change them without fully understanding what they do in code.
    const float k_AirborneTurnSpeedProportion = 5.4f;
    const float k_GroundedRayDistance = 1f;
    const float k_JumpAbortSpeed = 10f;
    const float k_MinEnemyDotCoeff = 0.2f;
    const float k_InverseOneEighty = 1f / 180f;
    const float k_StickingGravityProportion = 0.3f;
    const float k_GroundAcceleration = 20f;
    const float k_GroundDeceleration = 25f;

    void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        m_CharCtrl = GetComponent<CharacterController>();
        s_Instance = this;

        //
        ForwardIsometric = Camera.main.transform.forward;
        ForwardIsometric.y = 0;
        ForwardIsometric = Vector3.Normalize(ForwardIsometric);

        //This basically is saying that Right = Forward rotated 90 degrees in the Y axis
        RightIsometric = Quaternion.Euler(new Vector3(0, 90, 0)) * ForwardIsometric;
    }

    void FixedUpdate()
    {
        CalculateForwardMovement();
        if(Input.anyKey)
        {
            MoveCharacter();
        }
    }

    void MoveCharacter()
    {
        Vector3 direction = new Vector3(m_Input.MoveInput.x, 0, m_Input.MoveInput.y);

        //Gets the speed in each of the axis in isometric
        Vector3 rightMovement = RightIsometric * m_ForwardSpeed * Time.deltaTime * m_Input.MoveInput.x;
        Vector3 upMovement = ForwardIsometric * m_ForwardSpeed * Time.deltaTime * m_Input.MoveInput.y;

        //This is the direction that the character should head
        Vector3 characterDirection = Vector3.Normalize(rightMovement + upMovement);


        //This is temporary, make it rotate prosperly next time
        //transform.forward = characterDirection;
        UpdateOrientation(characterDirection);


        //transform.position += rightMovement + upMovement;
        m_CharCtrl.Move(characterDirection * m_ForwardSpeed * Time.deltaTime);
    }

    // Called each physics step.
    void CalculateForwardMovement()
    {
        // Cache the move input and cap it's magnitude at 1.
        Vector2 moveInput = m_Input.MoveInput;

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        // Calculate the speed intended by input.
        m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

        // Determine change to speed based on whether there is currently any move input.
        float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

        // Adjust the forward speed towards the desired speed.
        m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, acceleration * Time.deltaTime);
    }

    void UpdateOrientation(Vector3 characterDirection)
        {
        m_TargetRotation = Quaternion.LookRotation(characterDirection);

        float groundedTurnSpeed = Mathf.Lerp(maxTurnSpeed, minTurnSpeed, m_ForwardSpeed / m_DesiredForwardSpeed);
        m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, groundedTurnSpeed * Time.deltaTime);

        transform.rotation = m_TargetRotation;
    }

    protected bool IsMoveInput
    {
        get { return !Mathf.Approximately(m_Input.MoveInput.sqrMagnitude, 0f); }
    }
}
