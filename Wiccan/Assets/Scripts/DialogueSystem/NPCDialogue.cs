﻿using UnityEngine;
using Yarn.Unity;

/// attached to the non-player characters, and stores the name of the Yarn
/// node that should be run when you talk to them.
/// Modified versoin of the NPC script from the Yarn Examples

public class NPCDialogue : MonoBehaviour
{

    public string characterName = "";

    public string talkToNode = "";

    public Transform[] targets;

    [Header("Optional")]
    public YarnProgram scriptToLoad;

    protected DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Start()
    {
        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            dialogueManager.SetDialogueAvailable(this, targets);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player)
        {
            dialogueManager.SetDialogueUnavailable();
        }
    }
}
