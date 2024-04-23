using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public ProjectileShape shape;
    public int bounces;
    public float duration, sizeX, sizeY, sizeXGrow, sizeYGrow, growDuration, moveSpeed, homingStrength, rotateSpeed, pierce, periodLength;
    public bool grow, followPlayer, move, homing, rotate, returning, bouncing, piercing, periodic, chargeDuration;
    public Vector2 initialMoveDirection, moveDirection, startPositionOffset, rotatePositionOffset;
    public AbilityType abilityType;
    public bool chargeActive;
    public bool active;
    public float activeTime;

    public Rigidbody2D rb;

    void FixedUpdate()
    {
        if (active)
        {

        }
    }

    public void PrepareProjectile()
    {

    }

    public void StartProjectile()
    {

    }

    public void TriggerCollision(MatchEnemy enemy)
    {

    }
}

public enum ProjectileShape
{
    Rectangle,
    Circle,
    Slash
}