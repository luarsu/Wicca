using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
//Not used for now
//[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    protected static PlayerController s_Instance;
    public static PlayerController instance { get { return s_Instance; } }

    public float maxForwardSpeed = 8f;        // How fast the character can run.
    public float gravity = 20f;               // How fast the character accelerates downwards when airborne.
    public float jumpSpeed = 10f;             // How fast the character takes off when jumping.
    public float minTurnSpeed = 400f;         // How fast the character turns when moving at maximum speed.
    public float maxTurnSpeed = 1200f;        // How fast the character turns when stationary.
    public float idleTimeout = 5f;            // How long before the character starts considering random idles.
    public bool canAttack;                    // Whether or not the character can attack.

    /*
    public CameraSettings cameraSettings;            // Reference used to determine the camera's direction.
    public MeleeWeapon meleeWeapon;                  // Reference used to (de)activate the staff when attacking. 
    public RandomAudioPlayer footstepPlayer;         // Random Audio Players used for various situations.
    public RandomAudioPlayer hurtAudioPlayer;
    public RandomAudioPlayer landingPlayer;
    public RandomAudioPlayer emoteLandingPlayer;
    public RandomAudioPlayer emoteDeathPlayer;
    public RandomAudioPlayer emoteAttackPlayer;
    public RandomAudioPlayer emoteJumpPlayer;
    */
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

    // Called automatically by Unity when the script first exists in the scene.
    void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        //m_Animator = GetComponent<Animator>();
        m_CharCtrl = GetComponent<CharacterController>();

        //meleeWeapon.SetOwner(gameObject);

        s_Instance = this;
    }

    // Called automatically by Unity once every Physics step.
    void FixedUpdate()
    {

        //UpdateInputBlocking();
        /*

        if (m_Input.Attack && canAttack)
        {
            //m_Animator.SetTrigger(m_HashMeleeAttack);
        }


        CalculateForwardMovement();
        CalculateVerticalMovement();

        SetTargetRotation();

        if (IsMoveInput)
            UpdateOrientation();

        m_PreviouslyGrounded = m_IsGrounded;

        MoveCharacter();
        */
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

    // Called each physics step.
    void CalculateVerticalMovement()
    {
        // If jump is not currently held and the character is on the ground then she is ready to jump.
        if (!m_Input.DashInput && m_IsGrounded)
            m_ReadyToJump = true;

        if (m_IsGrounded)
        {
            // When grounded we apply a slight negative vertical speed to make the character "stick" to the ground.
            m_VerticalSpeed = -gravity * k_StickingGravityProportion;

            // If jump is held, the character is ready to jump and not currently in the middle of a melee combo...
            if (m_Input.DashInput && m_ReadyToJump)
            {
                // ... then override the previously set vertical speed and make sure she cannot jump again.
                m_VerticalSpeed = jumpSpeed;
                m_IsGrounded = false;
                m_ReadyToJump = false;
            }
        }
        else
        {
            // If the character is in the air, the jump button is not held and the character is currently moving upwards...
            if (!m_Input.DashInput && m_VerticalSpeed > 0.0f)
            {
                // ... decrease the character's vertical speed.
                // This is what causes holding jump to jump higher that tapping jump.
                m_VerticalSpeed -= k_JumpAbortSpeed * Time.deltaTime;
            }

            // If a jump is approximately peaking, make it absolute.
            if (Mathf.Approximately(m_VerticalSpeed, 0f))
            {
                m_VerticalSpeed = 0f;
            }

            // If the character is in the air, apply gravity.
            m_VerticalSpeed -= gravity * Time.deltaTime;
        }
    }
    // Called each physics step to set the rotation Ellen is aiming to have.
    void SetTargetRotation()
    {
        // Create three variables, move input local to the player, flattened forward direction of the camera and a local target rotation.
        Vector2 moveInput = m_Input.MoveInput;
        Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        //Change this when I change the camera for the isometric camera. Check how transistor did it
        Vector3 forward = Vector3.forward;//transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, forward * 5, Color.red);
        forward.y = 0f;
        forward.Normalize();

        Quaternion targetRotation;

        // If the local movement direction is the opposite of forward then the target rotation should be towards the camera.
        if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
        {
            targetRotation = Quaternion.LookRotation(-forward);
        }
        else
        {
            // Otherwise the rotation should be the offset of the input from the camera's forward.
            Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
            targetRotation = Quaternion.LookRotation(cameraToInputOffset * forward);
        }

        // The desired forward direction of Ellen.
        Vector3 resultingForward = targetRotation * Vector3.forward;

        // Find the difference between the current rotation of the player and the desired rotation of the player in radians.
        float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

        m_AngleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);
        m_TargetRotation = targetRotation;
    }

    // Called each physics step (In the example they call it from the animator, look when animations are added)
    void MoveCharacter()
    {
        Vector3 movement = transform.position;

        // If the character is on the ground...
        if (m_IsGrounded)
        {
            // ... raycast into the ground...
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            if (Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                // ... and get the movement of the root motion rotated to lie along the plane of the ground.
                movement = Vector3.ProjectOnPlane(transform.position, hit.normal);
            }
        }
        else
        {
            // If not grounded the movement is just in the forward direction.
            movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
        }

        // Rotate the transform of the character controller by the animation's root rotation.
        m_CharCtrl.transform.rotation *= transform.rotation;

        // Add to the movement with the calculated vertical speed.
        movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

        // Move the character controller.
        m_CharCtrl.Move(movement);

        // After the movement store whether or not the character controller is grounded.
        m_IsGrounded = m_CharCtrl.isGrounded;
    }

    // Called each physics step after SetTargetRotation if there is move input and the character is in the correct animator state according to IsOrientationUpdated.
    void UpdateOrientation()
    {

        Vector3 localInput = new Vector3(m_Input.MoveInput.x, 0f, m_Input.MoveInput.y);
        float groundedTurnSpeed = Mathf.Lerp(maxTurnSpeed, minTurnSpeed, m_ForwardSpeed / m_DesiredForwardSpeed);
        float actualTurnSpeed = m_IsGrounded ? groundedTurnSpeed : Vector3.Angle(transform.forward, localInput) * k_InverseOneEighty * k_AirborneTurnSpeedProportion * groundedTurnSpeed;
        m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, actualTurnSpeed * Time.deltaTime);

        transform.rotation = m_TargetRotation;
    }

    protected bool IsMoveInput
    {
        get { return !Mathf.Approximately(m_Input.MoveInput.sqrMagnitude, 0f); }
    }
}
