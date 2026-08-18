using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public bool IsRunning { get; private set; }

    private void Update()
    {
        ReadKeyboardInput();
    }

    private void ReadKeyboardInput()
    {
        if (Keyboard.current == null)
        {
            MoveInput = Vector2.zero;
            IsRunning = false;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            horizontal += 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            vertical -= 1f;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            vertical += 1f;
        }

        MoveInput = Vector2.ClampMagnitude(
            new Vector2(horizontal, vertical),
            1f
        );

        IsRunning = Keyboard.current.leftShiftKey.isPressed;
    }
}