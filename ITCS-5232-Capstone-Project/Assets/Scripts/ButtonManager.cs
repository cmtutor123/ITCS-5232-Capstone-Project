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
        if (GameManager.instance.playerData.stages[GameManager.instance.currentCharacter, stage] < 0) return;
        GameManager.instance.SetCurrentStage(stage);
    }

    public void ButtonSelectDifficulty(int difficulty)
    {
        if (!TryScreenChange()) return;
        if (GameManager.instance.playerData.stages[GameManager.instance.currentCharacter, GameManager.instance.currentStage] < difficulty) return;
        GameManager.instance.SetCurrentDifficulty(difficulty);
        GameManager.instance.SwitchMenu(MenuState.PerkSelect);
        GameManager.instance.UpdatePerkLoadout();
    }

    public void ButtonRemoveMenu()
    {
        if (!TryScreenChange()) return;
        GameManager.instance.RemoveMenu();
    }

    public void ButtonDifficultyBack()
    {
        GameManager.instance.UpdateStageSelect();
    }

    public void ButtonPerkLoadout(int index)
    {
        GameManager.instance.UpdatePerkLoadout();
        GameManager.instance.UpdatePerkSelection(index);
    }   
    
    public void ButtonPerkSelection(int index)
    {
        if (GameManager.instance.PerkLocked(GameManager.instance.currentSlot, index))
        {
            if (GameManager.instance.GetPoints() > 0)
            {
                if (GameManager.instance.currentSlot >= 0 || GameManager.instance.currentSlot <= 5)
                {
                    int main = index / 4;
                    if (main == index)
                    {
                        GameManager.instance.UnlockPerk(GameManager.instance.currentSlot, index);
                    }
                    else if (!GameManager.instance.PerkLocked(GameManager.instance.currentSlot, main))
                    {
                        GameManager.instance.UnlockPerk(GameManager.instance.currentSlot, index);
                    }
                }
                else
                {
                    GameManager.instance.UnlockPerk(GameManager.instance.currentSlot, index);
                }
            }
            return;
        }
        if (GameManager.instance.currentSlot >= 6 && GameManager.instance.currentSlot <= 9 && GameManager.instance.loadoutData[GameManager.instance.currentCharacter, GameManager.instance.currentSlot] == index)
        {
            GameManager.instance.UpdateLoadoutPerk(GameManager.instance.currentSlot, -1);
            GameManager.instance.UpdatePerkLoadout();
        }
        GameManager.instance.UpdateLoadoutPerk(GameManager.instance.currentSlot, index);
        GameManager.instance.UpdatePerkLoadout();
    }

    public void ButtonMatchStart()
    {
        GameManager.instance.SetMenu(MenuState.Load);
        GameManager.instance.StartGame();
    }

    public void ButtonTryPause()
    {
        GameManager.instance.TryPause();
    }
}
