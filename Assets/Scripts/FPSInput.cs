using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSInput : MonoBehaviour
{
    public Vector2 movement;
    public Vector2 look;

    public bool jumpPressed;

    public bool firePressed;
    public bool fireHeld;

    public bool altFirePressed;

    public bool sprintHeld;

    public float scrollValue;

    // MOVE
    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    // LOOK
    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    // JUMP
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    // FIRE
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            firePressed = true;
            fireHeld = true;
        }

        if (context.canceled)
        {
            fireHeld = false;
        }
    }

    // ALT FIRE
    public void OnAltFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            altFirePressed = true;
    }

    // SPRINT
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprintHeld = context.ReadValueAsButton();
    }

    // SCROLL WHEEL
    public void OnScroll(InputAction.CallbackContext context)
    {
        scrollValue = context.ReadValue<Vector2>().y;
    }

    // reset one-frame presses
    private void LateUpdate()
    {
        jumpPressed = false;
        firePressed = false;
        altFirePressed = false;

        scrollValue = 0;
    }
}
