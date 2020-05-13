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
    protected float TimeBetweenLinesTimer;           //Little timer so that the player doesn't start a dialogue again by mistake
    protected float TimebetweenLinesCooldown = 0.3f; //Cooldown start a conversation again
    protected bool IsLineBeingDisplayed = false;    //True is a line is being written but it's not completed
    protected bool IsInOptions = false;             //True when choosing for an option
    protected Yarn.OptionSet optionsAvailable;      //We save the optoins here when you have to choose
    protected int optionSelected;
    protected Transform[] targets;
    public GameObject LeftArrow, RightArrow;        //Arrows for the options UI
    protected System.Action systemAction;



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
            dialogueRunner.StartDialogue(NPCToTalk.talkToNode, targets);
            SetNormalLineDisplaySpeed();
            IsInConversation = true;
            TimeBetweenLinesTimer = TimebetweenLinesCooldown;

            //disable movement
            m_Input.ReleaseControl();
        }
        else if (IsInConversation && m_Input.InteractInput && IsLineBeingDisplayed)
        {
            if (TimeBetweenLinesTimer <= 0)
            {
                //Make the line display faster
                dialogueUI.textSpeed = LineDisplaySpeedFast;
            }
        }
        else if (IsInOptions)
        {
            HandleOptions();
        }
        else if (IsInConversation && m_Input.InteractInput)
        {
            dialogueUI.MarkLineComplete();
            TimeBetweenLinesTimer = TimebetweenLinesCooldown;
        }

        //Handle any timers needed
        HandleTimers();
    }

    public void SetupOptions(Yarn.OptionSet optionsCollection, IDictionary<string, string> strings, System.Action<int> selectOption)
    {
        //Do the initial setup for the options
        IsInOptions = true;
        optionSelected = 0;
        optionsAvailable = optionsCollection;
        RightArrow.SetActive(true);
        dialogueUI.SetIsInOptions(true);
        dialogueUI.SetWaitingForOptionSelection(true);
        dialogueUI.SetcurrentOptionSelectionHandler(selectOption);
        dialogueUI.CallOnOptionsStart();

        //This is to handle localisation, but I'm not doing that yet
        /*
        if (strings.TryGetValue(optionsCollection.Options[0].Line.ID, out var optionText) == false)
        {
            Debug.LogWarning($"Option {optionsCollection.Options[0].Line.ID} doesn't have any localised text");
            optionText = optionsCollection.Options[0].Line.ID;
        }
        */
        //Show the first option
        dialogueRunner.HandleLine(optionsCollection.Options[0].Line);
    }

    public void HandleOptions()
    {
        if(m_Input.InteractInput)
        {
            //select an option and set everything back to normal
            dialogueUI.SetIsInOptions(false);
            IsInOptions = false;
            RightArrow.SetActive(false);
            LeftArrow.SetActive(false);
            dialogueUI.SelectOption(optionSelected);
            dialogueUI.CallOnOptionsEnd();
        }
        else if(Input.GetKeyDown("right") && optionSelected < optionsAvailable.Options.Length -1 && TimeBetweenLinesTimer <=0)
        {
            //allows to use the option arrows for the dialogue
            optionSelected++;
            dialogueUI.MarkLineComplete();
            dialogueRunner.HandleLine(optionsAvailable.Options[optionSelected].Line);
            LeftArrow.SetActive(true);
            if (optionSelected == optionsAvailable.Options.Length - 1) 
            {
                RightArrow.SetActive(false);
            }
            TimeBetweenLinesTimer = TimebetweenLinesCooldown;
        }
        else if(Input.GetKeyDown("left") && optionSelected > 0 && TimeBetweenLinesTimer <= 0)
        {
            //allows to use the option arrows for the dialogue
            optionSelected--;
            dialogueUI.MarkLineComplete();
            dialogueRunner.HandleLine(optionsAvailable.Options[optionSelected].Line);
            RightArrow.SetActive(true);
            if (optionSelected == 0)
            {
                LeftArrow.SetActive(false);
            }
            TimeBetweenLinesTimer = TimebetweenLinesCooldown;
        }
    }



    //Set the NPC you can talk to and make it available
    public void SetDialogueAvailable(NPCDialogue npc, Transform[] newTargets)
    {
        NPCToTalk = npc;
        IsDialogueAvailable = true;
        targets = newTargets;
        //m_Input.ReleaseControl();
    }

    //Reset the dialogue options
    public void SetDialogueUnavailable()
    {
        NPCToTalk = null;
        IsDialogueAvailable = false;
        targets = null;
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
        if (TimeBetweenLinesTimer >= 0)
        {
            TimeBetweenLinesTimer -= Time.deltaTime;
        }

        if (TalkToNPCAgainTimer >= 0)
        {
            TalkToNPCAgainTimer -= Time.deltaTime;
        }
    }
}
