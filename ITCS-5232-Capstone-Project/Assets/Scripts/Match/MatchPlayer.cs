using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPlayer : MonoBehaviour
{
    public float newlySpawnedTimer = 3;

    public List<PerkId> perkIds = new List<PerkId>();

    public PlayerState currentState = PlayerState.Idle;

    public AbilityType currentAbilityType;

    public Rigidbody2D rb;

    public float windUpTime, castTime, windDownTime;
    public float windUpTimer, castTimer, windDownTimer;

    public GameObject playerProjectile => GameManager.instance.prefabPlayerProjectile;

    public PlayerProjectile NewProjectile => Instantiate(playerProjectile).GetComponent<PlayerProjectile>();

    public bool StateMovable => currentState == PlayerState.Idle || currentState == PlayerState.Moving;
    public bool StateAbilityUsable => currentState == PlayerState.Idle || currentState == PlayerState.Moving || currentState == PlayerState.WindingDown;
    public bool StateDashUsable => currentState == PlayerState.Idle || currentState == PlayerState.Moving || currentState == PlayerState.WindingDown || currentState == PlayerState.WindingUp;
    public bool StateAimChangable => currentState == PlayerState.Idle || currentState == PlayerState.Moving || currentState == PlayerState.Charging || currentState == PlayerState.WindingDown;

    public float currentMoveSpeed;

    public float currentHealth, maxHealth, currentBarrier;

    public Vector2 aimDirection, newDirection;
    public bool inputMoving;
    public float moveSpeed;
    public float dashTimer;
    public float dashDuration;
    public float dashDistance;
    public float dashSpeed;

    public int currentSpecialCharges;
    public int maxSpecialCharges;
    public float specialChargeTime;
    public float specialChargeTimer;

    public bool ChargedAbilityTogglable => chargedActive ? chargedCancelable : (chargedActivatibleEarly ? chargedMana > 0 : chargedMana >= chargedMaxMana);
    public bool chargedActivatibleEarly, chargedCancelable, chargedActive;
    public float chargedMana, chargedMaxMana;

    public bool chargeAbilityNormal, chargeAbilitySpecial;
    public float chargeLengthNormal, chargeLengthSpecial;
    public float chargeAbilityTimer;
    public float chargeAbilityTimeNormal, chargeAbilityTimeSpecial;
    public float chargeAbilityDamageNormal1, chargeAbilityDamageSpecial1;
    public float chargeAbilitySizeNormal1, chargeAbilitySizeSpecial1;

    public DamageType damageTypeNormal1, damageTypeSpecial1, damageTypeCharged1;

    public ProjectileShape shapeNormal1, shapeSpecial1, shapeCharged1;

    public float sizeXNormal1, sizeYNormal1, sizeXSpecial1, sizeYSpecial1, sizeXCharged1, sizeYCharged1;

    public bool growsNormal1, growsSpecial1, growsCharged1;
    public float sizeXGrowNormal1, sizeYGrowNormal1, sizeXGrowSpecial1, sizeYGrowSpecial1, sizeXGrowCharged1, sizeYGrowCharged1;
    public float growDurationNormal1, growDurationSpecial1, growDurationCharged1;

    public float projectileSpeedNormal1, projectileSpeedSpecial1, projectileSpeedCharged1;
    public float durationNormal1, durationSpecial1, durationCharged1;

    public int bounceNormal1, bounceSpecial1, bounceCharged1;
    public bool followPlayerNormal1, followPlayerSpecial1, followPlayerCharged1;
    public float homingStrengthNormal1, homingStrengthSpecial1, homingStrengthCharged1;
    public float periodLengthNormal1, periodLengthSpecial1, periodLengthCharged1;
    public int pierceNormal1, pierceSpecial1, pierceCharged1;
    public bool returningNormal1, returningSpecial1, returningCharged1;
    public float rotateSpeedNormal1, rotateSpeedSpecial1, rotateSpeedCharged1;
    public Sprite spriteNormal1, spriteSpecial1, spriteCharged1;

    public bool lunging;

    public float damageNormal1, damageSpecial1, damageCharged1;
    public float critChanceNormal1, critChanceSpecial1, critChanceCharged1;
    public float critDamageNormal1, critDamageSpecial1, critDamageCharged1;

    public int stunStackNormal1, stunStackSpecial1, stunStackCharged1;
    public int bleedStackNormal1;

    public bool hasDestroyedNormal1;
    public ProjectileShape shapeDestroyedNormal1;
    public float durationDestroyedNormal1;
    public float sizeXDestroyedNormal1, sizeYDestroyedNormal1;
    public bool growsDestroyedNormal1;
    public float sizeXGrowDestroyedNormal1, sizeYGrowDestroyedNormal1;
    public float growDurationDestroyedNormal1;
    public float projectileSpeedDestroyedNormal1;
    public DamageType damageTypeDestroyedNormal1;
    public float damageDestroyedNormal1;
    public float critChanceDestroyedNormal1;
    public float critDamageDestroyedNormal1;
    public int pierceDestroyedNormal1;
    public int bounceDestroyedNormal1;
    public bool followPlayerDestroyedNormal1;
    public float homingStrengthDestroyedNormal1;
    public float periodLengthDestroyedNormal1;
    public bool returningDestroyedNormal1;
    public float rotateSpeedDestroyedNormal1;
    public Sprite spriteDestroyedNormal1;

    public float poisonDamage = 5f;
    public float chillSlowMin = 0.3f;
    public float chillSlowIncrease = 0.05f;
    public float burnDamage = 10;
    public bool burnCrit = false;
    public float burnCritChance = 0.2f;
    public float burnCritDamage = 1.0f;
    public float curseBaseDamage = 10f;
    public float curseDamageMult = 1.5f;

    public bool invisible;

    public BaseStats baseStats => GameManager.instance.GetBaseStats();
    public ProjectileSpriteManager ProjectileSprite => GameManager.instance.projectileSpriteManager;
    public bool gameWon => GameManager.instance.bossActive && GameManager.instance.matchEnemies.Count == 0;

    public bool setupComplete = false;
    public bool matchComplete = false;

    private void FixedUpdate()
    {
        if (!setupComplete || matchComplete) return;
        if (currentHealth > 0 && gameWon)
        {
            matchComplete = true;
            GameManager.instance.EndGame(true);
        }
        else if (currentHealth <= 0)
        {
            matchComplete = true;
            GameManager.instance.EndGame(false);
        }
        if (currentSpecialCharges < maxSpecialCharges)
        {
            specialChargeTimer += Time.fixedDeltaTime;
            if (specialChargeTimer >= specialChargeTime)
            {
                specialChargeTimer -= specialChargeTime;
                currentSpecialCharges++;
                if (currentSpecialCharges == maxSpecialCharges)
                {
                    specialChargeTimer = 0;
                }
            }
        }

        if (currentState == PlayerState.Charging)
        {
            chargeAbilityTimer += Time.fixedDeltaTime;
        }
        else if (currentState == PlayerState.Dashing)
        {
            rb.velocity = aimDirection * dashSpeed;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
                currentMoveSpeed = 0;
                currentState = PlayerState.Idle;
            }
        }
        else if (currentState == PlayerState.WindingUp)
        {
            windUpTimer -= Time.fixedDeltaTime;
            if (windUpTimer <= 0)
            {
                TriggerCast(currentAbilityType);
            }
        }
        else if (currentState == PlayerState.WindingDown)
        {
            windDownTimer -= Time.fixedDeltaTime;
            if (windDownTimer <= 0)
            {
                currentState = PlayerState.Idle;
            }
        }
        else if (currentState == PlayerState.Casting)
        {
            castTimer -= Time.fixedDeltaTime;
            if (castTimer <= 0)
            {
                TriggerWindDown();
            }
        }
        if (StateAimChangable)
        {
            aimDirection = newDirection;
        }
        if (currentState == PlayerState.Idle && inputMoving)
        {
            currentState = PlayerState.Moving;
        }
        if (currentState == PlayerState.Moving && inputMoving)
        {
            rb.velocity = aimDirection * moveSpeed;
        }
        if (currentState != PlayerState.Moving && currentState != PlayerState.Dashing && !lunging)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void ProcessPerks()
    {
        // Base Stats

        maxHealth = baseStats.maxHealth;
        dashDuration = baseStats.dashDuration;
        dashDistance = baseStats.dashDistance;
        dashSpeed = dashDistance / dashDuration;
        windUpTime = baseStats.windUpTime;
        windDownTime = baseStats.windDownTime;
        castTime = baseStats.castTime;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = baseStats.playerSprite;
        spriteRenderer.size = new Vector2(1, 2);
        moveSpeed = baseStats.moveSpeed;

        // Base Temp

        float baseDamage = baseStats.damage;
        float baseCritChance = baseStats.critChance;
        float baseCritDamage = baseStats.critDamage;

        damageNormal1 = baseDamage;
        damageDestroyedNormal1 = baseDamage;
        damageSpecial1 = baseDamage;
        damageCharged1 = baseDamage;

        critChanceNormal1 = baseCritChance;
        critChanceDestroyedNormal1 = baseCritChance;
        critChanceSpecial1 = baseCritChance;
        critChanceCharged1 = baseCritChance;

        critDamageNormal1 = baseCritDamage;
        critDamageDestroyedNormal1 = baseCritDamage;
        critDamageSpecial1 = baseCritDamage;
        critDamageCharged1 = baseCritDamage;

        // Normal Active Perks

        // Berserker

        if (HasPerk(PerkId.BNThrow))
        {
            shapeNormal1 = ProjectileShape.Rectangle;
            durationNormal1 = 0.5f;
            sizeXNormal1 = .25f;
            sizeYNormal1 = 1;
            growsNormal1 = false;
            float range = 4;
            projectileSpeedNormal1 = range / durationNormal1;
            damageTypeNormal1 = DamageType.Physical;
            stunStackNormal1 = 1;
            spriteNormal1 = ProjectileSprite.rectangleTemp;

            if (HasPerk(PerkId.BNThrowWind))
            {
                chargeAbilityNormal = true;
                chargeAbilityTimeNormal = 3;
                chargeAbilityDamageNormal1 = 2;
                chargeAbilitySizeNormal1 = 1;
            }

            if (HasPerk(PerkId.BNThrowExplode))
            {
                damageNormal1 *= 0.2f;
                hasDestroyedNormal1 = true;
                shapeDestroyedNormal1 = ProjectileShape.Circle;
                durationDestroyedNormal1 = 2;
                growsDestroyedNormal1 = true;
                sizeXDestroyedNormal1 = 3;
                sizeYDestroyedNormal1 = 3;
                growDurationDestroyedNormal1 = 1;
                projectileSpeedDestroyedNormal1 = 0;
                damageTypeDestroyedNormal1 = DamageType.Fire;
                damageDestroyedNormal1 *= 0.6f;
                pierceDestroyedNormal1 = int.MaxValue;
                spriteDestroyedNormal1 = ProjectileSprite.circleTemp;
            }

            if (HasPerk(PerkId.BNThrowReturn))
            {
                damageNormal1 *= 0.5f;
                returningNormal1 = true;
                pierceNormal1 = int.MaxValue;
            }
        }
        if (HasPerk(PerkId.BNGreat))
        {
            shapeNormal1 = ProjectileShape.Slash;
            durationNormal1 = 0.3f;
            sizeXNormal1 = 1;
            sizeYNormal1 = 0.3f;
            growsNormal1 = false;
            pierceNormal1 = int.MaxValue;
            damageTypeNormal1 = DamageType.Physical;
            damageNormal1 *= 1.2f;
            if (HasPerk(PerkId.BNGreatBlood))
            {
                bleedStackNormal1 = 1;
            }
            if (HasPerk(PerkId.BNGreatBrutal))
            {
                critChanceNormal1 += 0.1f;
                critDamageNormal1 += 0.5f;
            }
            if (HasPerk(PerkId.BNGreatWrath))
            {
                damageNormal1 *= 1.25f;
                sizeXNormal1 = 1.25f;
                sizeYNormal1 = 0.4f;
            }
        }
        if (HasPerk(PerkId.BNWar))
        {
            shapeNormal1 = ProjectileShape.Circle;
            durationNormal1 = 0.2f;
            sizeXNormal1 = 1.5f;
            sizeYNormal1 = 1.5f;
            growsNormal1 = false;
            pierceNormal1 = int.MaxValue;
            damageTypeNormal1 = DamageType.Physical;
            stunStackNormal1 = 2;
            if (HasPerk(PerkId.BNWarElec))
            {
                damageTypeNormal1 = DamageType.Lightning;
                stunStackNormal1 += 1;
                damageNormal1 *= 1.2f;
            }
            if (HasPerk(PerkId.BNWarStun))
            {
                sizeXNormal1 = 2;
                sizeYNormal1 = 2;
                stunStackNormal1 += 2;
            }
            if (HasPerk(PerkId.BNWarHeavy))
            {
                damageNormal1 *= 1.75f;
                stunStackNormal1 -= 1;
                sizeXNormal1 = 1.75f;
                sizeYNormal1 = 1.75f;
            }
        }

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Special Active Perks

        // Berserker

        if (HasPerk(PerkId.BSScreech))
        {
            shapeSpecial1 = ProjectileShape.Circle;
            durationSpecial1 = 0.2f;
            sizeXSpecial1 = 1;
            sizeYSpecial1 = 1;
            growsSpecial1 = true;
            sizeXGrowSpecial1 = 5;
            sizeYGrowSpecial1 = 5;
            growDurationSpecial1 = 0.1f;
            pierceSpecial1 = int.MaxValue;
            damageTypeSpecial1 = DamageType.Physical;
            damageSpecial1 *= 2.5f;
            stunStackSpecial1 = 1;
            specialChargeTime = 10;
            if (HasPerk(PerkId.BSScreechPain))
            {

            }
            if (HasPerk(PerkId.BSScreechStun))
            {

            }
            if (HasPerk(PerkId.BSScreenStutter))
            {

            }
        }

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Charged Active Perks

        // Berserker

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Passive Perks


        // Final Setup

        currentHealth = maxHealth;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        setupComplete = true;
    }

    public void AddPerk(PerkId perkId)
    {
        perkIds.Add(perkId);
    }

    public bool HasPerk(PerkId perkId)
    {
        return perkIds.Contains(perkId);
    }

    public void TriggerAbilityUse(AbilityType abilityType)
    {
        bool abilityRequirement = false;
        if (abilityType == AbilityType.Normal && !chargeAbilityNormal) abilityRequirement = true;
        else if (abilityType == AbilityType.Special && !chargeAbilitySpecial) abilityRequirement = currentSpecialCharges > 0;
        else if (abilityType == AbilityType.Charged) abilityRequirement = ChargedAbilityTogglable;
        if (abilityRequirement && StateAbilityUsable)
        {
            TriggerWindUp(abilityType);
        }
    }

    public void TriggerWindUp(AbilityType abilityType)
    {
        windUpTimer = windUpTime;
        currentState = PlayerState.WindingUp;
        currentAbilityType = abilityType;
    }

    public void TriggerCast(AbilityType abilityType)
    {
        castTimer = castTime;
        currentState = PlayerState.Casting;
        if (currentAbilityType == AbilityType.Normal) TriggerNormalAbility();
        else if (currentAbilityType == AbilityType.Special) TriggerSpecialAbility();
        else if (currentAbilityType == AbilityType.Charged) ToggleChargedAbility();
    }

    public void TriggerChargedCast(AbilityType abilityType, float chargePercent)
    {
        castTimer = castTime;
        currentState = PlayerState.Casting;
        if (currentAbilityType == AbilityType.Normal) TriggerNormalAbility(chargePercent);
        else if (currentAbilityType == AbilityType.Special) TriggerSpecialAbility(chargePercent);
    }

    public void TriggerWindDown()
    {
        windDownTimer = windDownTime;
        currentState = PlayerState.WindingDown;
    }

    public void TriggerNormalAbility(float chargePercent = 0)
    {
        SpawnProjectile(shapeNormal1, durationNormal1, sizeXNormal1, sizeYNormal1, growsNormal1 ? sizeXGrowNormal1 : sizeXNormal1, growsNormal1 ? sizeYGrowNormal1 : sizeYNormal1, growDurationNormal1, projectileSpeedNormal1, homingStrengthNormal1, rotateSpeedNormal1, periodLengthNormal1, pierceNormal1, bounceNormal1, followPlayerNormal1, returningNormal1, false, AbilityType.Normal, aimDirection, spriteNormal1, chargedActive, 1, 1 + chargePercent);
    }

    public void TriggerSpecialAbility(float chargePercent = 0)
    {
        currentSpecialCharges--;
        SpawnProjectile(shapeSpecial1, durationSpecial1, sizeXSpecial1, sizeYSpecial1, growsSpecial1 ? sizeXGrowSpecial1 : sizeXSpecial1, growsSpecial1 ? sizeYGrowSpecial1 : sizeYSpecial1, growDurationSpecial1, projectileSpeedSpecial1, homingStrengthSpecial1, rotateSpeedSpecial1, periodLengthSpecial1, pierceSpecial1, bounceSpecial1, followPlayerSpecial1, returningSpecial1, false, AbilityType.Special, aimDirection, spriteSpecial1, chargedActive, 1, 1 + chargePercent);
    }

    public void ToggleChargedAbility()
    {
        if (chargedActive)
        {
            chargedActive = false;
        }
        else
        {
            chargedActive = true;
            SpawnProjectile(shapeCharged1, durationCharged1, sizeXCharged1, sizeYCharged1, growsCharged1 ? sizeXGrowCharged1 : sizeXCharged1, growsCharged1 ? sizeYGrowCharged1 : sizeYCharged1, growDurationCharged1, projectileSpeedCharged1, homingStrengthCharged1, rotateSpeedCharged1, periodLengthCharged1, pierceCharged1, bounceCharged1, followPlayerCharged1, returningCharged1, true, AbilityType.Charged, aimDirection, spriteCharged1, chargedActive, 1, 1);
        }
    }

    public void TriggerChargeStart(AbilityType abilityType)
    {
        bool chargable = false;
        if (abilityType == AbilityType.Normal && chargeAbilityNormal) chargable = true;
        if (abilityType == AbilityType.Special && chargeAbilitySpecial) chargable = true;
        if (chargable && StateAbilityUsable)
        {
            currentState = PlayerState.Charging;
            chargeAbilityTimer = 0;

        }
    }

    public void TriggerChargeEnd(AbilityType abilityType)
    {
        if (currentState == PlayerState.Charging && currentAbilityType == abilityType)
        {
            float chargeAbilityTime = abilityType == AbilityType.Normal ? chargeAbilityTimeNormal : chargeAbilityTimeSpecial;
            TriggerChargedCast(abilityType, Mathf.Clamp(chargeAbilityTimer / chargeAbilityTime, 0, 1));
        }
    }

    public void TriggerDash()
    {
        if (StateDashUsable)
        {
            currentState = PlayerState.Dashing;
            dashTimer = dashDuration;
        }
    }

    public void TriggerMove(Vector2 direction)
    {
        if (direction != new Vector2(0, 0)) newDirection = direction.normalized;
        inputMoving = direction != new Vector2(0, 0);
        if (StateMovable)
        {
            if (direction != new Vector2(0, 0))
            {
                currentState = PlayerState.Moving;
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
    }

    public bool TriggerHit(MatchEnemy enemy, AbilityType abilityType, int index, bool chargeActive, float damageMult = 1)
    {
        if (enemy != null)
        {
            bool invincibilityFrames = enemy.HasInvincibilityFrames();
            if (invincibilityFrames) return true;
        }
        bool hitCrit = TriggeredCrit(enemy, abilityType, index, chargeActive);
        float damage = 0;
        float critDamage = 0;
        int stunStacks = 0;
        int bleedStacks = 0;
        int poisonStacks = 0;
        int chillStacks = 0;
        int burnStacks = 0;
        int curseStacks = 0;
        bool smited = false;
        if (abilityType == AbilityType.Normal)
        {
            if (index == 1)
            {
                damage = damageNormal1;
                critDamage = critDamageNormal1;
                stunStacks = stunStackNormal1;
                bleedStacks = bleedStackNormal1;
            }
            else if (index == 2)
            {
                damage = damageDestroyedNormal1;
                critDamage = critDamageDestroyedNormal1;
            }
        }
        else if (abilityType == AbilityType.Special)
        {
            if (index == 1)
            {
                damage = damageSpecial1;
                critDamage = critDamageSpecial1;
            }
        }
        else if (abilityType == AbilityType.Charged)
        {
            if (index == 1)
            {
                damage = damageCharged1;
                critDamage = critDamageCharged1;
            }
        }
        if (hitCrit)
        {
            damage *= 1 + critDamage;
        }
        damage *= damageMult;
        enemy.Damage(damage);
        if (stunStacks > 0) enemy.InflictStatus(Status.Stun, stunStacks);
        if (bleedStacks > 0) enemy.InflictStatus(Status.Bleed, bleedStacks);
        return false;
    }

    public void TriggerPeriodic(List<MatchEnemy> enemies, AbilityType abilityType, int index, bool chargeActive)
    {

    }

    public void TriggerCrit(MatchEnemy enemy, AbilityType abilityType, int index, bool chargeActive)
    {

    }

    public bool TriggeredCrit(MatchEnemy enemy, AbilityType abilityType, int index, bool chargeActive)
    {
        float critChance = 0;
        if (abilityType == AbilityType.Normal)
        {
            if (index == 1)
            {
                critChance = critChanceNormal1;
            }
            else if (index == 2)
            {
                critChance = critChanceDestroyedNormal1;
            }
        }
        else if (abilityType == AbilityType.Special)
        {
            critChance = critChanceSpecial1;
        }
        else if (abilityType == AbilityType.Charged)
        {
            critChance = critChanceCharged1;
        }
        float randChance = Random.value;
        return randChance > critChance;
    }

    public void TriggerDestroy(Vector2 position, Vector2 direction, AbilityType abilityType, int index, bool chargeActive)
    {
        if (index == 1 && hasDestroyedNormal1)
        {
            SpawnProjectile(shapeDestroyedNormal1, durationDestroyedNormal1, sizeXDestroyedNormal1, sizeYDestroyedNormal1, growsDestroyedNormal1, sizeXGrowDestroyedNormal1, sizeYGrowDestroyedNormal1, growDurationDestroyedNormal1, projectileSpeedDestroyedNormal1, homingStrengthDestroyedNormal1, rotateSpeedDestroyedNormal1, periodLengthDestroyedNormal1, pierceDestroyedNormal1, bounceDestroyedNormal1, followPlayerDestroyedNormal1, returningDestroyedNormal1, false, abilityType, direction, spriteDestroyedNormal1, chargeActive, 2, 1);
        }
    }

    public void SpawnProjectile(ProjectileShape shape, float duration, float sizeX, float sizeY, bool grows, float sizeXGrow, float sizeYGrow, float growDuration, float moveSpeed, float homingStrength, float rotateSpeed, float periodLength, int pierce, int bounces, bool followPlayer, bool returning, bool chargeDuration, AbilityType abilityType, Vector2 initialMoveDirection, Sprite sprite, bool chargeActive, int index, float damageMult)
    {
        SpawnProjectile(shape, duration, sizeX, sizeY, grows ? sizeXGrow : sizeX, grows ? sizeYGrow : sizeY, growDuration, moveSpeed, homingStrength, rotateSpeed, periodLength, pierce, bounces, followPlayer, returning, chargeDuration, abilityType, initialMoveDirection, sprite, chargeActive, index, damageMult);
    }

    public void SpawnProjectile(ProjectileShape shape, float duration, float sizeX, float sizeY, float sizeXGrow, float sizeYGrow, float growDuration, float moveSpeed, float homingStrength, float rotateSpeed, float periodLength, int pierce, int bounces, bool followPlayer, bool returning, bool chargeDuration, AbilityType abilityType, Vector2 initialMoveDirection, Sprite sprite, bool chargeActive, int index, float damageMult)
    {
        PlayerProjectile projectile = NewProjectile;
        projectile.PrepareProjectile(shape, duration, sizeX, sizeY, sizeXGrow, sizeYGrow, growDuration, moveSpeed, homingStrength, rotateSpeed, periodLength, pierce, bounces, followPlayer, returning, chargeDuration, abilityType, initialMoveDirection, sprite, chargeActive, index, damageMult);
        projectile.StartProjectile();
    }

    public void UpdatePerkDisplay()
    {
        GameManager.instance.UpdatePerkDisplayNormal(StateAbilityUsable);
        GameManager.instance.UpdatePerkDisplaySpecial(StateAbilityUsable, currentSpecialCharges, maxSpecialCharges, specialChargeTimer, specialChargeTime);
        GameManager.instance.UpdatePerkDisplayCharged(StateAbilityUsable, chargedActive, ChargedAbilityTogglable, chargedMana, chargedMaxMana);
        GameManager.instance.UpdateHealthDisplay(currentHealth, maxHealth, currentBarrier);
    }
}

public enum PerkId
{
    None,
    BNThrow ,
    BNThrowWind,
    BNThrowExplode,
    BNThrowReturn,
    BNGreat,
    BNGreatBlood,
    BNGreatBrutal,
    BNGreatWrath,
    BNWar,
    BNWarElec,
    BNWarStun,
    BNWarHeavy,
    BSScreech,
    BSScreechPain,
    BSScreechStun,
    BSScreenStutter,
    BSWhirl,
    BSWhirlBig,
    BSWhirlSharp,
    BSWhirlMount,
    BSReck,
    BSReckPower,
    BSReckFury,
    BSReckCrit,
    BCRage,
    BCRageUncon,
    BCRageFrenzy,
    BCRageCon,
    BCDefend,
    BCDefendBrace,
    BCDefendArmor,
    BCDefendShield,
    BCElement,
    BCElementBlast,
    BCElementAura,
    BCElementBurst,
    DNVine,
    DNVineEntangle,
    DNVineThorn,
    DNVinePoison,
    DNFlame,
    DNFlameSpark,
    DNFlameBurn,
    DNFlameEmber,
    DNChill,
    DNChillBreeze,
    DNChillDeep,
    DNChillQuick,
    DSPoison,
    DSPoisonPower,
    DSPoisonLinger,
    DSPoisonAura,
    DSWild,
    DSWildForce,
    DSWildEmber,
    DSWildSear,
    DSLightning,
    DSLightningStun,
    DSLightningStorm,
    DSLightningSmite,
    DCAura,
    DCAuraChill,
    DCAuraWind,
    DCAuraBlast,
    DCBoon,
    DCBoonMult,
    DCBoonBless,
    DCBoonUnmove,
    DCGift,
    DCGiftMult,
    DCGiftBless,
    DCGiftUnmove,
    NNSummon,
    NNSummonDeath,
    NNSummonSwarm,
    NNSummonDark,
    NNLightning,
    NNLightningBig,
    NNLightningChain,
    NNLightningVolt,
    NNCurse,
    NNCurseBlast,
    NNCurseSplit,
    NNCurseHex,
    NSBall,
    NSBallPulse,
    NSBallVolatile,
    NSBallShock,
    NSFear,
    NSFearOver,
    NSFearPet,
    NSFearTerror,
    NSGrave,
    NSGraveCurse,
    NSGraveChill,
    NSGraveSpecter,
    NCTouch,
    NCTouchLife,
    NCTouchDisease,
    NCTouchStrong,
    NCTrap,
    NCTrapFright,
    NCTrapRestless,
    NCTrapFalse,
    NCDark,
    NCDarkElement,
    NCDarkCurse,
    NCDarkChaos,
    PNSword,
    PNSwordBoom,
    PNSwordWave,
    PNSwordLunge,
    PNPray,
    PNPrayBless,
    PNPrayCurse,
    PNPrayMult,
    PNSacred,
    PNSacredGuide,
    PNSacredBounce,
    PNSacredExplode,
    PSHeal,
    PSHealPulse,
    PSHealDrain,
    PSHealFlat,
    PSJavelin,
    PSJavelinFlurry,
    PSJavelinReturn,
    PSJavelinLightning,
    PSSmite,
    PSSmiteBlast,
    PSSmiteLightning,
    PSSmiteFreeze,
    PCAura,
    PCAuraInflict,
    PCAuraBurn,
    PCAuraBig,
    PCInvin,
    PCInvinRush,
    PCInvinSurge,
    PCInvinArmor,
    PCBlade,
    PCBladeRush,
    PCBladeSharp,
    PCBladeLightning,
    RNDag,
    RNDagSharp,
    RNDagPoison,
    RNDagQuick,
    RNThrow,
    RNThrowBounce,
    RNThrowSharp,
    RNThrowFan,
    RNArch,
    RNArchPoison,
    RNArchSharp,
    RNArchHome,
    RSArrow,
    RSArrowSwarm,
    RSArrowPoison,
    RSArrowStun,
    RSPoison,
    RSPoisonSpray,
    RSPoisonBig,
    RSPoisonEffect,
    RSStun,
    RSStunSpray,
    RSStunBig,
    RSStunEffect,
    RCInvis,
    RCInvisRest,
    RCInvisStealth,
    RCInvisSneak,
    RCBack,
    RCBackWeak,
    RCBackCrit,
    RCBackPower,
    RCFrenzy,
    RCFrenzyOppor,
    RCFrenzyDefen,
    RCFrenzyQuick,
    WNMiss,
    WNMissMass,
    WNMissMany,
    WNMissExplos,
    WNBolt,
    WNBoltFire,
    WNBoltFrost,
    WNBoltLightning,
    WNBlast,
    WNBlastBoom,
    WNBlastShot,
    WNBlastBurst,
    WSFire,
    WSFireGround,
    WSFireFlame,
    WSFireIcy,
    WSRune,
    WSRuneBig,
    WSRunePlent,
    WSRuneDual,
    WSDrain,
    WSDrainOver,
    WSDrainStasis,
    WSDrainArmor,
    WCGolem,
    WCGolemMult,
    WCGolemInfuse,
    WCGolemCharge,
    WCWhirl,
    WCWhirlStorm,
    WCWhirlPower,
    WCWhirlSurge,
    WCEther,
    WCEtherEncase,
    WCEtherEscape,
    WCEtherDark,
    StunStack,
    Damage,
    AOE,
    Cooldown,
    CritBleed,
    CritChance,
    BleedCritChanceDamage,
    StunDamage,
    LightningStun,
    HurtBarrier,
    CritDamage,
    HurtCharge,
    PushStun,
    StatusDamage,
    BurnDamage,
    BurnStack,
    Linger,
    HitCharge,
    MinusDamageCritChance,
    MinusDamageCritDamage,
    FireBurn,
    LightningCritDamage,
    StunCritChance,
    KillStatusHeal,
    BurnCritChanceDamage,
    PoisonDamage,
    PoisonBurst,
    StunBleed,
    ChillDamage,
    CurseKillHeal,
    Speed,
    StatusStack,
    HurtBarrierThorn,
    FireCritChance,
    CritBarrier,
    CritBurn,
    CritCharge,
    CritChanceSelfDamage,
    MaxCharge,
    HurtLightningThorn,
    ChillSlow,
    StatusCritChanceDamage,
    BurnBurst,
    ChillShatter,
    ChillBarrier,
    HitCurse,
    MinusHealthBarrierEffective,
    MinusHealthDamage,
    LightningRange,
    LightningBurn,
    CurseKillCharge,
    CurseDamage,
    CurseSpirit,
    SpiritDamage,
    SpiritChill,
    SpiritCurse,
    CurseDefense,
    LowHealthBarrierEffective,
    HurtBarrierCurse,
    HealBarrier,
    MinusCritDamageCritChance,
    ChillKillBarrier,
    CritHeal,
    KillStatusBarrier,
    HurtBarrierResist,
    LessCritChanceCritDamage,
    AttackSpeed,
    DamageMoveSpeed,
    BleedDamage,
    PoisonSlow,
    SuperCritical,
    Dodge,
    HurtInvisible,
    ArcaneStatus,
    ArcaneCurse,
    IceChill,
    ArcaneDamage,
    ChillHitCharge,
    KillBarrier,
    Random,
    HurtBarrierChance,
    BurnChill
}

public enum DamageType
{
    Physical,
    Arcane,
    Fire,
    Ice,
    Lightning,
    Light,
    Dark,
    Spirit
}

public enum PlayerState
{
    Idle,
    Moving,
    WindingUp,
    Casting,
    WindingDown,
    Charging,
    Dashing
}

/*

1 Initial Projectile
2 Initial's Destroyed

*/