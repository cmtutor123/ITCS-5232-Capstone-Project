using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEnemy : MonoBehaviour
{
    public int difficultyLevel => GameManager.instance.GetDifficultyLevel();
    public Vector2 nextTile => GameManager.instance.pathfindingHelper.PathToPlayer(transform.position);

    public SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public Rigidbody2D rb;

    public float currentHealth, maxHealth;
    public float damage;
    public float moveSpeed;
    public float invincibilityFrames = 0;

    Vector2 targetMove;

    void FixedUpdate()
    {
        targetMove = nextTile;
        rb.velocity = (targetMove - (Vector2)transform.position).normalized * moveSpeed;
    }

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

    public void Damage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            TriggerDeath();
        }
    }

    public void TriggerDeath()
    {
        GameManager.instance.matchEnemies.Remove(this);
        Destroy(gameObject);
    }
}
