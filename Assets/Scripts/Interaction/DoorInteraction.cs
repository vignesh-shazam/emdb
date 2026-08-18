using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    public string InteractionPrompt => isOpen ? "Close Door" : "Open Door";

    private void Awake()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen
            ? openRotation
            : closedRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    public void Interact()
    {
        isOpen = !isOpen;
    }
}