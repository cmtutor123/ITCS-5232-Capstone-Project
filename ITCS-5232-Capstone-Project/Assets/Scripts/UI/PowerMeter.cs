using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerMeter : MonoBehaviour
{
    public TextMeshProUGUI tmpMeter;

    public void SetMeter(int current, int max)
    {
        tmpMeter.text = current.ToString() + "/" + max.ToString();
    }
}
