using UnityEngine;

public class BedInteraction : MonoBehaviour, IInteractable
{
    private bool isSleeping;

    public string InteractionPrompt =>
        isSleeping ? "Wake Up" : "Sleep";

    public void Interact()
    {
        isSleeping = !isSleeping;

        if (isSleeping)
        {
            Debug.Log("Player is sleeping.");
        }
        else
        {
            Debug.Log("Player woke up.");
        }
    }
}