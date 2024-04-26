using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEnemy : MonoBehaviour
{
    public int difficultyLevel => GameManager.instance.GetDifficultyLevel();

    public SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public float currentHealth, maxHealth;
    public float damage;
    public float moveSpeed;
    public float invincibilityFrames = 0;

    public void SetEnemy(EnemyData enemyData)
    {
        maxHealth = enemyData.flatHealth + enemyData.scaledHealth * difficultyLevel;
        currentHealth = maxHealth;
        moveSpeed = enemyData.moveSpeed;
        damage = enemyData.flatDamage + enemyData.scaledDamage * difficultyLevel;
        spriteRenderer.sprite = enemyData.sprite;
    }

    public bool HasInvincibilityFrames()
    {
        return invincibilityFrames > 0;
    }
}
