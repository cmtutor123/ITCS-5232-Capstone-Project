using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public ClassEmblem classEmblem;
    public Image expBar;

    public void SetEmblem(Color colorLight, Color colorDark, int level)
    {
        classEmblem.ChangeColor(colorLight, colorDark);
        classEmblem.ChangeLevel(level);
    }

    public void SetExp(int current, int max)
    {
        expBar.fillAmount = Mathf.Clamp(current / max, 0, 1);
    }
}
