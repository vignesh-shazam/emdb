using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedForce = -2f;

    private CharacterController characterController;
    private PlayerInputHandler inputHandler;

    private Vector3 verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        Vector2 input = inputHandler.MoveInput;

        if (input.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement =
            cameraForward * input.y +
            cameraRight * input.x;

        movement.Normalize();

        float currentSpeed = inputHandler.IsRunning
            ? runSpeed
            : walkSpeed;

        characterController.Move(
            movement * currentSpeed * Time.deltaTime
        );

        RotatePlayer(movement);
    }

    private void RotatePlayer(Vector3 movement)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(movement);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedForce;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        characterController.Move(
            verticalVelocity * Time.deltaTime
        );
    }
}