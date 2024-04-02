using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PerkButton : MonoBehaviour
{
    public PerkIcon icon;
    public TextMeshProUGUI tmpCost;

    public void ChangeColor(Color newColor) => icon.ChangeColor(newColor);

    public void ChangeIcon(Sprite newSprite) => icon.ChangeIcon(newSprite);

    public void ChangeCost(int newCost)
    {
        if (newCost > 0)
        {
            tmpCost.text = newCost.ToString();
        }
        else
        {
            tmpCost.text = "";
        }
    }

    public void SetPerk(PerkData data)
    {
        ChangeIcon(data.perkIcon);
        ChangeCost(data.cost);
    }
}
