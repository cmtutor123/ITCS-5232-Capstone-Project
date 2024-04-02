using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ButtonIndex))]
public class PerkTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    PerkData perk = GameManager.instance.perkNone;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.ShowPerkTooltip(perk);
        GetComponent<PerkButton>().ChangeColor(GameManager.instance.classData[GameManager.instance.currentCharacter].classColorLight);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.HidePerkTooltip();
        GetComponent<PerkButton>().ChangeColor(GameManager.instance.classData[GameManager.instance.currentCharacter].classColorDark);
    }

    public void UpdatePerk(PerkData newPerk)
    {
        perk = newPerk;
    }
}
