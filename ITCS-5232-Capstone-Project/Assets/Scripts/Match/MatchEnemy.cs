using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEnemy : MonoBehaviour
{
    public float invincibilityFrames = 0;

    public bool GetInvincibilityFrames()
    {
        return invincibilityFrames > 0;
    }
}
