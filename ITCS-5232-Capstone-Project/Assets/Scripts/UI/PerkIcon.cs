using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkIcon : MonoBehaviour
{
    public Image outline, icon;

    public void ChangeColor(Color newColor)
    {
        outline.color = newColor;
    }

    public void ChangeIcon(Sprite newSprite)
    {
        icon.sprite = newSprite;
    }
}
