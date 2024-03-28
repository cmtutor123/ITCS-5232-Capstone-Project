using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockableButton : MonoBehaviour
{
    public TextButton textButton;
    public Image lockIcon;

    public void SetLocked()
    {
        textButton.ChangeText("");
        lockIcon.gameObject.SetActive(false);
    }

    public void SetUnlocked(string newText)
    {
        textButton.ChangeText(newText);
        lockIcon.gameObject.SetActive(true);
    }
}
