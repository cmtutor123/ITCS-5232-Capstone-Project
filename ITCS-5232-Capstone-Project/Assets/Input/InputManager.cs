using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            GameManager.instance.TryMove(direction);
        }
        if (context.canceled)
        {
            GameManager.instance.TryMove(new Vector2(0, 0));
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.TryDash();
        }
    }

    public void OnSkill1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.TryAbility(AbilityType.Normal);
        }
        if (context.started)
        {
            GameManager.instance.TryStartCharge(AbilityType.Normal);
        }
        if (context.canceled)
        {
            GameManager.instance.TryEndCharge(AbilityType.Normal);
        }
    }

    public void OnSkill2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.TryAbility(AbilityType.Special);
        }
        if (context.started)
        {
            GameManager.instance.TryStartCharge(AbilityType.Special);
        }
        if (context.canceled)
        {
            GameManager.instance.TryEndCharge(AbilityType.Special);
        }
    }

    public void OnSkill3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.TryAbility(AbilityType.Charged);
        }
        if (context.started)
        {
            GameManager.instance.TryStartCharge(AbilityType.Charged);
        }
        if (context.canceled)
        {
            GameManager.instance.TryEndCharge(AbilityType.Charged);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.TryPause();
        }
    }
}
