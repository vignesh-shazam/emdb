using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    public IInteractable CurrentInteractable { get; private set; }

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        DetectInteractable();

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void DetectInteractable()
    {
        CurrentInteractable = null;

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionLayer))
        {
            return;
        }

        CurrentInteractable =
            hit.collider.GetComponentInParent<IInteractable>();
    }

    private void Interact()
    {
        if (CurrentInteractable == null)
        {
            return;
        }

        CurrentInteractable.Interact();
    }
}