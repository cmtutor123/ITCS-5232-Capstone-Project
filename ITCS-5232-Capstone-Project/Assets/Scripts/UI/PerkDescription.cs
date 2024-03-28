using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PerkDescription : MonoBehaviour
{
    public PerkIcon icon;
    public TextMeshProUGUI tmpName, tmpDescription;

    public void ChangeColor(Color newColor) => icon.ChangeColor(newColor);

    public void ChangeIcon(Sprite newSprite) => icon.ChangeIcon(newSprite);

    public void ChangeName(string newName)
    {
        tmpName.text = newName;
    }

    public void ChangeDescription(string newDescription)
    {
        tmpDescription.text = newDescription;
    }
}
