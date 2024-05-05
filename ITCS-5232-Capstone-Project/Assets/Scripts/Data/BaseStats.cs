using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Base Stats", menuName = "BaseStats")]
public class BaseStats : ScriptableObject
{
    public Sprite playerSprite;
    public float maxHealth = 100;
    public float damage = 50;
    public float critChance = 0.05f;
    public float critDamage = 0.5f;
    public float dashDuration = 1.5f;
    public float dashDistance = 2f;
    public float windUpTime = 0.3f;
    public float castTime = 0.4f;
    public float windDownTime = 0.3f;
    public float moveSpeed = 1.5f;
}
