using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Interaction")]
    [SerializeField] private InteractionDetector interactionDetector;

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        if (promptText == null)
        {
            Debug.LogError("InteractionPromptUI: Prompt Text is missing.");
            return;
        }

        if (interactionDetector == null)
        {
            Debug.LogError(
                "InteractionPromptUI: Interaction Detector is missing."
            );
            HidePrompt();
            return;
        }

        IInteractable interactable =
            interactionDetector.CurrentInteractable;

        if (interactable == null)
        {
            HidePrompt();
            return;
        }

        ShowPrompt(interactable);
    }

    private void ShowPrompt(IInteractable interactable)
    {
        promptText.text =
            $"[E] {interactable.InteractionPrompt}";

        if (!promptText.gameObject.activeSelf)
        {
            promptText.gameObject.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (promptText.gameObject.activeSelf)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}