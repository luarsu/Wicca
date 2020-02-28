using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    protected static DialogueManager s_Instance;
    public static DialogueManager instance { get { return s_Instance; } }

    protected PlayerInput m_Input;                 // Reference to the input manager
    protected PlayerController m_playerController;           // Reference to the player controller

    //Dialogue related variables
    private float LineDisplaySpeedNormal = 0.04f;    //Velocidad normal para el texto (cuanto menor es mas rapido va)
    private float LineDisplaySpeedFast = 0.01f;       //Velocidad rapida para el texto (cuanto menor es mas rapido va)
    protected NPCDialogue NPCToTalk;                //Npc that is close enough to you to talk
    protected DialogueRunner dialogueRunner;        //Reference to the dialogueRunner
    protected DialogueUI dialogueUI;                //Reference to the dialogueUI
    protected bool IsDialogueAvailable = false;     //True is there's someone you can talk to near
    protected bool IsInConversation = false;        //True if you're in the middle of a conversation/dialogue
    protected float TalkToNPCAgainTimer;            //Little timer so that the player doesn't start a dialogue again by mistake
    protected float TalkToNPCAgainCooldown = 0.3f;  //Cooldown start a conversation again
    protected float TimebetweenLinesTimer;            //Little timer so that the player doesn't start a dialogue again by mistake
    protected float TimebetweenLinesCooldown = 0.3f;  //Cooldown start a conversation again
    protected bool IsLineBeingDisplayed = false;    //True is a line is being written but it's not completed


    void Awake()
    {
        m_Input = FindObjectOfType<PlayerInput>();
        m_playerController = FindObjectOfType<PlayerController>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueUI = FindObjectOfType<DialogueUI>();
        s_Instance = this;
    }

    private void Update()
    {
        if (IsDialogueAvailable && m_Input.InteractInput && !IsInConversation && TalkToNPCAgainTimer <= 0)
        {
            //Start the dialogue and set everything up
            dialogueRunner.StartDialogue(NPCToTalk.talkToNode);
            SetNormalLineDisplaySpeed();
            IsInConversation = true;
            TimebetweenLinesTimer = TimebetweenLinesCooldown;

            //disable movement
            m_Input.ReleaseControl();
        }
        else if (IsInConversation && m_Input.InteractInput && IsLineBeingDisplayed)
        {
            if (TimebetweenLinesTimer <= 0)
            {
                //Make the line display faster
                dialogueUI.textSpeed = LineDisplaySpeedFast;
            }
        }
        else if (IsInConversation && m_Input.InteractInput)
        {
            dialogueUI.MarkLineComplete();
            TimebetweenLinesTimer = TimebetweenLinesCooldown;
        }

        //Handle any timers needed
        HandleTimers();
    }

    //Set the NPC you can talk to and make it available
    public void SetDialogueAvailable(NPCDialogue npc)
    {
        NPCToTalk = npc;
        IsDialogueAvailable = true;
        //m_Input.ReleaseControl();
    }

    //Reset the dialogue options
    public void SetDialogueUnavailable()
    {
        NPCToTalk = null;
        IsDialogueAvailable = false;
    }

    public void EndConversation()
    {
        IsInConversation = false;
        m_Input.GainControl();

        //Set timer so you don't start a conversation immediately again
        TalkToNPCAgainTimer = TalkToNPCAgainCooldown;
    }

    public void SetLineBeingDisplayed(bool displayed)
    {
        IsLineBeingDisplayed = displayed;
    }

    public void SetNormalLineDisplaySpeed()
    {
        dialogueUI.textSpeed = LineDisplaySpeedNormal;
    }

    void HandleTimers()
    {
        if (TimebetweenLinesTimer >= 0)
        {
            TimebetweenLinesTimer -= Time.deltaTime;
        }

        if (TalkToNPCAgainTimer >= 0)
        {
            TalkToNPCAgainTimer -= Time.deltaTime;
        }
    }
}
