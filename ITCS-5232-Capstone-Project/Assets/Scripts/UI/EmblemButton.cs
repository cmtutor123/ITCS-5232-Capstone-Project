using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmblemButton : MonoBehaviour
{
    public TextButton textButton;
    public ClassEmblem classEmblem;

    public void ChangeText(string newText) => textButton.ChangeText(newText);

    public void ChangeLevel(int newLevel) => classEmblem.ChangeLevel(newLevel);

    public void ChangeColor(Color newColor) => classEmblem.ChangeColor(newColor);
}
