using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDisplay : MonoBehaviour
{
    public Image health, barrier;

    public void SetColor(Color light, Color dark)
    {
        health.color = light;
        barrier.color = dark;
    }

    public void SetValues(float currentHealth, float maxHealth, float currentBarrier)
    {
        health.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0, 1);
        barrier.fillAmount = Mathf.Clamp(currentBarrier / maxHealth, 0, health.fillAmount);
    }
}
