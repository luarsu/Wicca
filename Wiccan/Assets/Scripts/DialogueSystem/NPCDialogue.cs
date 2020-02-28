using UnityEngine;
using Yarn.Unity;

/// attached to the non-player characters, and stores the name of the Yarn
/// node that should be run when you talk to them.
/// Modified versoin of the NPC script from the Yarn Examples

public class NPCDialogue : MonoBehaviour
{

    public string characterName = "";

    public string talkToNode = "";

    [Header("Optional")]
    public YarnProgram scriptToLoad;

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
        PlayerControllerIsometric player = other.GetComponent<PlayerControllerIsometric>();
        if (player)
        {
            player.SetDialogueAvailable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerControllerIsometric player = other.GetComponent<PlayerControllerIsometric>();
        if (player)
        {
            player.SetDialogueUnavailable();
        }
    }
}
