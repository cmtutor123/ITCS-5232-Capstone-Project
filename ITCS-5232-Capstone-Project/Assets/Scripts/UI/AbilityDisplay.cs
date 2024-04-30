using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
    public Color grayedColor, whiteColor;
    public PerkIcon perkIcon;
    public TextMeshProUGUI charges;
    public Image cooldown;
    public Image manaBar;
    public GameObject manaBackground;

    public void SetPerk(PerkData perk)
    {
        perkIcon.ChangeIcon(perk.perkIcon);
    }

    public void SetColor(Color color)
    {
        perkIcon.ChangeColor(color);
    }    

    public void ShowManaBar(Color color)
    {
        manaBackground.SetActive(true);
    }

    public void SetGrayed(bool grayed)
    {
        perkIcon.GetComponent<SpriteRenderer>().color = grayed ? grayedColor : whiteColor;
    }

    public void UpdateNormal(bool available)
    {
        SetGrayed(!available);
    }

    public void UpdateSpecial(bool available, int currentCharge, int maxCharge, float currentTime, float maxTime)
    {
        if (maxCharge > 1 && currentCharge > 0) charges.text = currentCharge.ToString();
        if (currentCharge == 0) cooldown.fillAmount = Mathf.Clamp(1 - (currentTime / maxTime), 0, 1);
        else cooldown.fillAmount = 0;
        SetGrayed(available && currentCharge > 0);
    }

    public void UpdateCharged(bool available, bool chargedActive, bool chargedTogglable, float currentMana, float maxMana)
    {
        manaBar.fillAmount = Mathf.Clamp(currentMana / maxMana, 0, 1);
        SetGrayed((available && chargedTogglable) || chargedActive);
    }
}
