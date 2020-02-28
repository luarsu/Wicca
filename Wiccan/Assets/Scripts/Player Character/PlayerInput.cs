using UnityEngine;
using System;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    //Singleton class for the player input
    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_Dash;
    protected bool m_Attack;
    protected bool m_Pause;
    protected bool m_Interact;
    protected bool m_ExternalInputBlocked;

    //Wait and coroutine used for the attacks. Not used for now
    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;

    //Update just updates the values of the different input every frame. Must be used somewhere else to work
    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Dash = Input.GetButton("Jump");
        m_Interact = Input.GetButton("Interact");

        m_Pause = Input.GetButtonDown("Pause");
    }

    //The following methods just return a value calculated during update.
    //Either a bool or a vector2

    //If the player movement is blocked then return vector2.zero. 
    //Else return the movement calculated during update
    public Vector2 MoveInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool DashInput
    {
        get { return m_Dash && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Attack
    {
        get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause
    {
        get { return m_Pause; }
    }

    public bool InteractInput
    {
        get { return m_Interact;}
    }

    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }

    IEnumerator AttackWait()
    {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }
}
