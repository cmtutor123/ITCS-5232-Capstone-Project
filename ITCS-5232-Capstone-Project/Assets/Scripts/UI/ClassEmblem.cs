using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClassEmblem : MonoBehaviour
{
    public TextMeshProUGUI tmpLevel;
    public Image emblemFront, emblemBack;

    public void ChangeLevel(int newLevel)
    {
        tmpLevel.text = newLevel.ToString();
    }

    public void ChangeColor(Color newFrontColor, Color newBackColor)
    {
        emblemFront.color = newFrontColor;
        emblemBack.color = newBackColor;
    }
}
