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
        if (GameManager.instance.playerData.stages[stage] < 0) return;
        GameManager.instance.SetCurrentStage(stage);
    }

    public void ButtonSelectDifficulty(int difficulty)
    {
        if (!TryScreenChange()) return;
        if (GameManager.instance.playerData.stages[GameManager.instance.currentStage] < difficulty) return;
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
        int slot = GameManager.instance.currentSlot;
        int pIndex = GameManager.instance.GetPerkIndex(slot, index);
        if (GameManager.instance.PerkLocked(pIndex))
        {
            if (GameManager.instance.HasUnlockPoints())
            {
                if (slot < 6)
                {
                    int pIndexBase = pIndex - (pIndex % 4);
                    if (pIndexBase == pIndex)
                    {
                        GameManager.instance.UnlockPerk(pIndex);
                    }
                    else
                    {
                        if (!GameManager.instance.PerkLocked(pIndexBase))
                        {
                            GameManager.instance.UnlockPerk(pIndex);
                        }
                    }
                }
                else
                {
                    GameManager.instance.UnlockPerk(pIndex);
                }
            }
        }
        else
        {
            int slotMod = slot % 2;
            if (slot < 6)
            {
                int pIndexBase = pIndex - (pIndex % 4);
                int baseSlot = slot - slotMod;
                int modSlot = baseSlot + 1;
                if (pIndexBase == pIndex)
                {
                    GameManager.instance.UpdateLoadoutPerk(baseSlot, pIndex);
                    GameManager.instance.UpdateLoadoutPerk(modSlot, -1);
                }
                else
                {
                    GameManager.instance.UpdateLoadoutPerk(baseSlot, pIndexBase);
                    GameManager.instance.UpdateLoadoutPerk(modSlot, pIndex);
                }
            }
            else
            {
                int slotOther = slot + ((slotMod == 0) ? 1 : -1);
                if (pIndex == GameManager.instance.GetLoadoutIndex(slot))
                {
                    GameManager.instance.UpdateLoadoutPerk(slot, -1);
                }
                else if (pIndex == GameManager.instance.GetLoadoutIndex(slotOther))
                {
                    GameManager.instance.UpdateLoadoutPerk(slotOther, -1);
                    GameManager.instance.UpdateLoadoutPerk(slot, pIndex);
                }
                else
                {
                    GameManager.instance.UpdateLoadoutPerk(slot, pIndex);
                }
            }
        }
        GameManager.instance.UpdatePowerMeter();
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

    public void ButtonResetData()
    {
        GameManager.instance.playerData = new PlayerData();
        FileManager.SavePlayerData(GameManager.instance.playerData);
    }
}
