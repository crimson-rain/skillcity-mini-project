using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }

    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> BasicAttackEvent;
    public event Action<int > AbilitySwitchEvent;
    public event Action<int> ItemSwitchEvent;


    public Vector2 MouseVector { get; private set; }
    public Vector2 ZoomDelta { get; private set; }
    public bool isLook { get; private set; }

    public event Action ToggleCameraEvent;

    private Controls controls;

    private void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();

    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    //public void OnJump(InputAction.CallbackContext context)
    //{
    //    if (!context.performed) return;
    //    JumpEvent?.Invoke();
    //}

    //public void OnDodge(InputAction.CallbackContext context)
    //{
    //    if (!context.performed) return;
    //    DodgeEvent?.Invoke();
    //}

    public void OnPosition(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        MoveEvent?.Invoke(MovementValue); // Pass movement value
    }

    public void OnAttackBasic(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        BasicAttackEvent?.Invoke(MovementValue);
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AbilitySwitchEvent?.Invoke(1);
    }

    public void OnAbility2(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AbilitySwitchEvent?.Invoke(2);
    }

    public void OnAbility3(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AbilitySwitchEvent?.Invoke(3);
    }

    public void OnAbility4(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        AbilitySwitchEvent?.Invoke(4);
    }

    public void OnItem1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ItemSwitchEvent?.Invoke(0);
    }

    public void OnItem2(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ItemSwitchEvent?.Invoke(1);
    }

    public void OnItem3(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ItemSwitchEvent?.Invoke(2);
    }

    public void OnLook(InputAction.CallbackContext context)
    {

        if (context.performed) // the key has been pressed
        {
            isLook = true;
        }
        if (context.canceled) //the key has been released
        {
            isLook = false;
        }
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        MouseVector = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {

        ZoomDelta = context.ReadValue<Vector2>();
        Debug.Log("Zoom Delta: " + ZoomDelta);
    }

    public void OnToggleCamera(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ToggleCameraEvent?.Invoke();
    }
}
