using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    float delay = 0.6f;
    float timer = 0;

    private void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool TryScreenChange()
    {
        if (timer < 0)
        {
            timer = delay;
            return true;
        }
        return false;
    }

    public void ButtonStart()
    {
        if (!TryScreenChange()) return;
        GameManager.instance.UpdateCharacterSelect();
        GameManager.instance.SwitchMenu(MenuState.CharacterSelect);
    }

    public void ButtonOptions()
    {
        if (!TryScreenChange()) return;
        GameManager.instance.SwitchMenu(MenuState.Options);
    }

    public void ButtonExit()
    {
        if (!TryScreenChange()) return;
        Application.Quit();
    }

    public void ButtonSelectCharacter(int character)
    {
        if (!TryScreenChange()) return;
        GameManager.instance.SetCurrentCharacter(character);
        GameManager.instance.SwitchMenu(MenuState.StageSelect);
    }

    public void ButtonSelectStage(int stage)
    {
        GameManager.instance.SetCurrentStage(stage);
    }

    public void ButtonSelectDifficulty(int difficulty)
    {
        if (!TryScreenChange()) return;
        GameManager.instance.SetCurrentDifficulty(difficulty);
        GameManager.instance.SwitchMenu(MenuState.PerkSelect);
    }

    public void ButtonRemoveMenu()
    {
        if (!TryScreenChange()) return;
        GameManager.instance.RemoveMenu();
    }
}
