using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void ButtonStart()
    {
        GameManager.instance.SwitchMenu(MenuState.CharacterSelect);
    }

    public void ButtonOptions()
    {
        GameManager.instance.SwitchMenu(MenuState.Options);
    }

    public void ButtonExit()
    {
        Application.Quit();
    }

    public void ButtonSelectCharacter(int character)
    {
        GameManager.instance.SetCurrentCharacter(character);
        GameManager.instance.SwitchMenu(MenuState.StageSelect);
    }

    public void ButtonSelectStage(int stage)
    {
        GameManager.instance.SetCurrentStage(stage);
    }

    public void ButtonSelectDifficulty(int difficulty)
    {
        GameManager.instance.SetCurrentDifficulty(difficulty);
        GameManager.instance.SwitchMenu(MenuState.PerkSelect);
    }

    public void ButtonRemoveMenu()
    {
        GameManager.instance.RemoveMenu();
    }
}
