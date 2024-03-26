using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextButton : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void ChangeText(string newText)
    {
        tmp.text = newText;
    }
}
