using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextButton : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    public void ChangeText(string newText)
    {
        tmpText.text = newText;
    }
}
