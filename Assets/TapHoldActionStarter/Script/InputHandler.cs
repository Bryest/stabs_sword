using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("INPUT CONTROLS")]
    [SerializeField] private Key mainInputKey = Key.Space;
    [SerializeField] private bool enableTouchInput = true;

    // Events
    public event System.Action OnInputPressed;
    public event System.Action OnInputReleased;

    // Properties
    public bool IsInputHeld { get; private set; }

    private Keyboard keyboard;
    private Touchscreen touchscreen;

    void Start()
    {
        keyboard = Keyboard.current;
        touchscreen = Touchscreen.current;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        bool inputPressed = false;
        bool inputReleased = false;
        bool inputIsHeld = false;

        // Keyboard input detection
        if (keyboard != null)
        {
            if (keyboard[mainInputKey].wasPressedThisFrame)
                inputPressed = true;
            if (keyboard[mainInputKey].wasReleasedThisFrame)
                inputReleased = true;
            if (keyboard[mainInputKey].isPressed)
                inputIsHeld = true;
        }

        // Touch input detection
        if (enableTouchInput && touchscreen != null)
        {
            try
            {
                for (int i = 0; i < touchscreen.touches.Count; i++)
                {
                    var touch = touchscreen.touches[i];
                    if (touch?.press != null)
                    {
                        if (touch.press.wasPressedThisFrame)
                            inputPressed = true;
                        if (touch.press.wasReleasedThisFrame)
                            inputReleased = true;
                        if (touch.press.isPressed)
                            inputIsHeld = true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Touch input error: {e.Message}");
            }
        }

        // Update state and fire events
        IsInputHeld = inputIsHeld;

        if (inputPressed)
            OnInputPressed?.Invoke();

        if (inputReleased)
            OnInputReleased?.Invoke();
    }
}
