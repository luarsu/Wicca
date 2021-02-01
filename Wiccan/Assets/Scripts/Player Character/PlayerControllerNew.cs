using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerNew : MonoBehaviour
{
    protected static PlayerControllerNew s_Instance;
    public static PlayerControllerNew Instance { get { return s_Instance; } }

    public float dashSpeed = 50f;             // How fast does the character go whe you dash.
    public float dashDuration = 0.2f;         // How long does the dash last
    public float dashCooldownDuration = 0.2f; // How long does the dash cooldown take


    /*Variables to change movement to Jammo´s*/
    public float Velocity;
    [Space]
    public float desiredRotationSpeed = 0.1f;
    public float allowPlayerRotation = 0.1f;
    public bool blockRotationPlayer;
    private Camera cam;
    public float Speed;
    public float AnimSpeed;
    private float InputX;
    private float InputZ;
    private Vector3 desiredMoveDirection;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    protected bool m_IsGrounded = true;            // Whether or not the character is currently standing on the ground.
    protected bool m_PreviouslyGrounded = true;    // Whether or not the character was standing on the ground last frame.
   
    protected PlayerInput m_Input;                 // Reference used to determine how the character should move.
    protected CharacterController m_CharCtrl;      // Reference used to actually move the character.
    protected Animator m_Animator;


    //Dash related variables
    protected bool m_IsDashAvialable;                // Wheter the dash is available or not
    protected bool m_IsDashing;                    //True when the character is in the middle of a Dash
    protected float m_DashTimer;                    //Timer to controll how long does the dash lasts
    protected float m_DashCooldownTimer;           //Timer to decide when to reenable the dash


    void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        m_CharCtrl = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        s_Instance = this;

        m_IsDashing = false;
        m_IsDashAvialable = true;

        cam = Camera.main;
    }

    //Update function called every physics frame. Used for movement related things
    void FixedUpdate()
    {
        //detect input from the player and do whatever is needed
        if (m_Input.HaveControl())
        {
            if (m_Input.DashInput && m_IsDashAvialable)
            {
                StartDash();
            }
            else if (!m_IsDashing)
            {
                MoveCharacter();
            }
        }

        //Do it outside because otherwise it just detects the dashing while the key is pressed
        if (m_IsDashing)
        {
            Dashing();
        }

        //Takes account of the movement related timers if needed
        HandleTimers();
    }

    void MoveCharacter()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //If speed is too low, blend the animation to make it stop.
        if (Speed > allowPlayerRotation)
        {
            PlayerMoveAndRotation();
            m_Animator.SetFloat("moving", Speed, StartAnimTime, Time.deltaTime);
        }
        else if (Speed < allowPlayerRotation)
        {
            m_Animator.SetFloat("moving", Speed, StopAnimTime, Time.deltaTime);
        }
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * m_Input.MoveInput.y + right * m_Input.MoveInput.x;

        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            m_CharCtrl.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

    void StartDash()
    {
        m_IsDashing = true;
        m_IsDashAvialable = false;
        m_DashTimer = 0f;
    }

    void Dashing()
    {
        Vector3 dashDirection = transform.forward;

        m_CharCtrl.Move(dashDirection * dashSpeed * Time.deltaTime);

        m_DashTimer += Time.deltaTime;
        if (m_DashTimer >= dashDuration)
        {
            m_IsDashing = false;
            //set a timer to reenable the dash after a brief period.
            m_DashCooldownTimer = 0f;
        }
    }

    void HandleTimers()
    {
        if (m_DashCooldownTimer < dashCooldownDuration)
        {
            m_DashCooldownTimer += Time.fixedDeltaTime;

            if (m_DashCooldownTimer >= dashCooldownDuration)
            {
                m_IsDashAvialable = true;
            }
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }
}
