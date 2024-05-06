using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchEnemy : MonoBehaviour
{
    public int difficultyLevel => GameManager.instance.GetDifficultyLevel();
    public Vector2 targetPosition => GameManager.instance.pathfindingHelper.PathToPlayer(transform.position);
    public Vector2 playerPosition => GameManager.instance.matchPlayer.transform.position;

    public SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public Rigidbody2D rb;

    public float currentHealth, maxHealth;
    public float damage;
    public float moveSpeed;
    public float invincibilityFrames = 0;

    Vector2 targetMove;

    public bool stunned => stunStacks > 0;
    public int stunStacks;
    public float stunTimer;
    public const float stunDuration = 0.5f;

    public bool bleeding => bleedStacks > 0;
    public int bleedStacks;

    public bool poisoned => poisonStacks > 0;
    public int poisonStacks;
    public float poisonTimer;
    public const float poisonDuration = 2f;
    public float poisonDamage => GameManager.instance.matchPlayer.poisonDamage;

    public bool chilled => chillStacks > 0;
    public int chillStacks;
    public float chillTimer;
    public const float chillDuration = 1f;
    public float chillSlowMin => GameManager.instance.matchPlayer.chillSlowMin;
    public float chillSlowIncrease => GameManager.instance.matchPlayer.chillSlowIncrease;
    public const int chillMaxStacks = 10;

    public bool burned => burnStacks > 0;
    public int burnStacks;
    public float burnTimer;
    public const float burnDuration = 1f;
    public float burnDamage => GameManager.instance.matchPlayer.burnDamage;
    public bool burnCrit => GameManager.instance.matchPlayer.burnCrit;
    public float burnCritChance => GameManager.instance.matchPlayer.burnCritChance;
    public float burnCritDamage => GameManager.instance.matchPlayer.burnCritDamage;

    public bool smited => smiteStacks > 0;
    public int smiteStacks;
    public float smiteTimer;
    public const float smiteDuration = 1f;

    public bool cursed => curseStacks > 0;
    public int curseStacks;
    public float curseTimer;
    public const float curseDuration = 1f;
    public float currentCurseDamage;
    public float curseBaseDamage => GameManager.instance.matchPlayer.curseBaseDamage;
    public float curseDamageMult => GameManager.instance.matchPlayer.curseDamageMult;

    public bool pushed => pushForce > 0;
    public int pushForce;
    public float pushTimer;
    public const float pushDuration = 0.5f;

    public bool stunVulnerable => stunVulnerableStacks > 0;
    public int stunVulnerableStacks;

    void FixedUpdate()
    {
        float currentMoveSpeed = moveSpeed;
        if (poisoned)
        {
            poisonTimer += Time.fixedDeltaTime;
            if (poisonTimer > poisonDuration)
            {
                poisonTimer -= poisonDuration;
                Damage(poisonDamage * poisonStacks);
                poisonStacks = poisonStacks / 2;
            }
        }
        if (chilled)
        {
            chillTimer += Time.fixedDeltaTime;
            if (chillTimer > chillDuration)
            {
                chillTimer -= chillDuration;
                chillStacks--;
            }
            currentMoveSpeed *= 1 - (chillSlowMin + (chillSlowIncrease * chillStacks));
        }
        if (burned)
        {
            burnTimer += Time.fixedDeltaTime;
            if (burnTimer > burnDuration)
            {
                burnTimer -= burnDuration;
                float currentBurnDamage = burnDamage;
                if (burnCrit && Random.value < burnCritChance)
                {
                    currentBurnDamage *= 1 + burnCritDamage;
                }
                Damage(currentBurnDamage);
                burnStacks--;
            }
        }
        if (cursed)
        {
            curseTimer += Time.fixedDeltaTime;
            if (curseTimer > curseDuration)
            {
                curseTimer -= curseDuration;
                currentCurseDamage *= curseDamageMult;
                curseStacks--;
                if (curseStacks <= 0)
                {
                    Damage(currentCurseDamage);
                }
            }
        }
        if (smited)
        {
            smiteTimer += Time.fixedDeltaTime;
            if (smiteTimer > smiteDuration)
            {
                smiteTimer -= smiteDuration;
                smiteStacks++;
                if (smiteStacks >= currentHealth)
                {
                    TriggerDeath();
                    return;
                }
            }
        }
        if (pushed)
        {
            pushTimer += Time.fixedDeltaTime;
            if (pushTimer > pushDuration)
            {
                pushTimer -= pushDuration;
                pushForce--;
            }
            rb.velocity = ((Vector2)transform.position - targetPosition).normalized * pushForce;
        }
        else if (stunned)
        {
            stunTimer += Time.fixedDeltaTime;
            if (stunTimer > stunDuration)
            {
                stunTimer -= stunDuration;
                stunStacks--;
            }
        }
        else
        {
            targetMove = targetPosition;
            rb.velocity = (targetMove - (Vector2)transform.position).normalized * currentMoveSpeed;
        }
    }

    public void SetEnemy(EnemyData enemyData)
    {
        maxHealth = enemyData.flatHealth + enemyData.scaledHealth * difficultyLevel;
        currentHealth = maxHealth;
        moveSpeed = enemyData.moveSpeed;
        damage = enemyData.flatDamage + enemyData.scaledDamage * difficultyLevel;
        spriteRenderer.sprite = enemyData.sprite;
        if (enemyData.isBoss) GameManager.instance.bossActive = true;
    }

    public bool HasInvincibilityFrames()
    {
        return invincibilityFrames > 0;
    }

    public void Damage(float damage)
    {
        if (stunVulnerable && stunned)
        {
            damage *= 1 + (stunVulnerableStacks / 10f);
        }
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

    public void DestroyEnemy()
    {
        GameManager.instance.matchEnemies.Remove(this);
        Destroy(gameObject);
    }

    public void InflictStatus(Status status, int stacks = 0)
    {
        if (status == Status.Stun) InflictStun(stacks);
        else if (status == Status.Poison) InflictPoison(stacks);
        else if (status == Status.Burn) InflictBurn(stacks);
        else if (status == Status.Chill) InflictChill(stacks);
        else if (status == Status.Curse) InflictCurse(stacks);
        else if (status == Status.Bleed) InflictBleed(stacks);
        else if (status == Status.Smite) InflictSmite();
        else if (status == Status.Push) InflictPush(stacks);
        else if (status == Status.StunVulnerable) InflictStunVulnerable(stacks);
    }

    public void InflictStun(int stacks)
    {
        stunTimer = 0;
        stunStacks += stacks;
    }

    public void InflictPoison(int stacks)
    {
        poisonTimer = 0;
        poisonStacks += stacks;
    }

    public void InflictBurn(int stacks)
    {
        burnTimer = 0;
        burnStacks += stacks;
    }

    public void InflictChill(int stacks)
    {
        chillTimer = 0;
        chillStacks = Mathf.Clamp(chillStacks + stacks, 0, chillMaxStacks);
    }

    public void InflictCurse(int stacks)
    {
        curseTimer = 0;
        if (curseStacks == 0) currentCurseDamage = curseBaseDamage;
        curseStacks += stacks;
    }

    public void InflictBleed(int stacks)
    {
        bleedStacks += stacks;
    }

    public void InflictSmite()
    {
        if (smiteStacks <= 0) smiteStacks = 1;
    }

    public void InflictPush(int force)
    {
        if (pushForce <= 0) pushForce += force;
    }

    public void InflictStunVulnerable(int amount)
    {
        stunVulnerableStacks += amount;
    }
}

public enum Status
{
    Stun,
    Poison,
    Burn,
    Chill,
    Curse,
    Bleed,
    Smite,
    Push,
    StunVulnerable
}