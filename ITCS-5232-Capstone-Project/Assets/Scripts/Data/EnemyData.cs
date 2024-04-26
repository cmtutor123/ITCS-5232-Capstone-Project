using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int enemyLevel;
    public float flatHealth, scaledHealth;
    public float flatDamage, scaledDamage;
    public float moveSpeed;
    public Sprite sprite;
}
