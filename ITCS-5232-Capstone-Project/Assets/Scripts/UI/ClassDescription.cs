using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClassDescription : MonoBehaviour
{
    public TextMeshProUGUI tmpName, tmpDescription;
    public Image classSprite;

    public void ChangeName(string newName)
    {
        tmpName.text = newName;
    }

    public void ChangeDescription(string newDescription)
    {
        tmpDescription.text = newDescription;
    }

    public void ChangeSprite(Sprite newSprite)
    {
        classSprite.sprite = newSprite;
    }
}
