using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerMeter : MonoBehaviour
{
    public TextMeshProUGUI tmpMeter;
    public int points = 0;

    public void SetMeter(int current, int max)
    {
        string power = current.ToString() + "/" + max.ToString() + " Perk Power";
        string point = points.ToString() + " Unlock Points\n";
        if (points == 0) point = "";
        tmpMeter.text = point + power;
    }

    public void SetPoints(int points)
    {
        this.points = points;
    }
}
