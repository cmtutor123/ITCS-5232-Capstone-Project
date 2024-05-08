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
    public int maxSpecialCharges = 1;
    public float specialChargeTime;
    public float specialChargeTimer;

    public bool ChargedAbilityTogglable => chargedActive ? chargedCancelable : (chargedActivatibleEarly ? chargedMana > 0 : chargedMana >= chargedMaxMana);
    public bool chargedActivatibleEarly, chargedCancelable, chargedActive, chargedAutoActivates;
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
    public int bleedStackNormal1, bleedStackSpecial1;
    public int stunVulnerableStackNormal1;
    public int pushForceNormal1, pushForceSpecial1;

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

    public bool hurtTriggerSpecial1;
    public float hurtTriggerAmountSpecial1 = 1;

    public bool noProjectileSpecial = false;
    public int overchargeGainSpecial, overchargeMaxSpecial, vulnerableGainSpecial, vulnerableMaxSpecial;
    public float overchargeCritChanceGain = 0;
    public float overchargeCritDamageGain = 0;
    public float overchargeStacks, vulnerableStacks;

    public bool noProjectileCharged = false;
    public float chargedBarrierGain;
    public bool chargedEndBarrierDeplete = false;
    public bool chargedStartBarrierDeplete = false;
    public bool chargedFullDeplete = false;

    public float damageGainCharged;
    public float critChanceGainCharged;
    public float critDamageGainCharged;

    public int manaGainHit, manaGainHurt;

    public bool chargedCancelBlast = false;

    public int bleedStacksCrit;

    public float barrierGainCrit;
    public int burnStackFire;
    public int burnStackCrit;
    public float critChanceFire;
    public float critDamageLightning;
    public float hurtBarrierGain;
    public float hurtLightningThorn;
    public int manaGainCrit;
    public float selfDamageNormal;
    public int stunStackLightning;
    public int stunStackCrit;
    public int stunVulnerableStack;
    public int bleedCritBonusStack;

    public float manaLossTimer = 0.5f;

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
        if (chargedActive)
        {
            manaLossTimer -= Time.fixedDeltaTime;
            if (manaLossTimer <= 0)
            {
                manaLossTimer = 0.5f;
                chargedMana = Mathf.Clamp(chargedMana - 1, 0, chargedMaxMana);
                if (chargedMana <= 0)
                {
                    ToggleChargedAbility();
                }
            }
            if (chargedMana >= chargedMaxMana && chargedAutoActivates && !chargedActive)
            {
                ToggleChargedAbility();
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
            UpdatePerkDisplay();
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

        maxSpecialCharges = 1;

        // Passive Perks Pre

        if (HasPerk(PerkId.Damage))
        {
            baseDamage *= 1.5f;
        }
        if (HasPerk(PerkId.CritChance))
        {
            baseCritChance += 0.1f;
        }
        if (HasPerk(PerkId.HurtBarrier))
        {
            hurtBarrierGain = 1;
        }
        if (HasPerk(PerkId.CritDamage))
        {
            baseCritDamage += 0.5f;
        }
        if (HasPerk(PerkId.MinusDamageCritChance))
        {
            baseDamage *= 0.5f;
            baseCritChance += 0.25f;
        }
        if (HasPerk(PerkId.MinusDamageCritDamage))
        {
            baseDamage *= 0.5f;
            baseCritDamage += 1.5f;
        }
        if (HasPerk(PerkId.CritChanceSelfDamage))
        {
            baseCritChance += 0.4f;
        }

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
            spriteNormal1 = ProjectileSprite.Sword;

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
                spriteDestroyedNormal1 = ProjectileSprite.Fire;
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
            spriteNormal1 = ProjectileSprite.Slash;
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
            spriteNormal1 = ProjectileSprite.Smash;
            if (HasPerk(PerkId.BNWarElec))
            {
                damageTypeNormal1 = DamageType.Lightning;
                spriteNormal1 = ProjectileSprite.Lightning;
                stunStackNormal1 += 1;
                damageNormal1 *= 1.2f;
            }
            if (HasPerk(PerkId.BNWarStun))
            {
                sizeXNormal1 = 2;
                sizeYNormal1 = 2;
                stunStackNormal1 += 1;
                stunVulnerableStackNormal1 = 1;
            }
            if (HasPerk(PerkId.BNWarHeavy))
            {
                damageNormal1 *= 1.75f;
                stunStackNormal1 -= 1;
                sizeXNormal1 = 1.75f;
                sizeYNormal1 = 1.75f;
                pushForceNormal1 = 2;
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
            pushForceSpecial1 = 4;
            spriteSpecial1 = ProjectileSprite.Ring;
            if (HasPerk(PerkId.BSScreechPain))
            {
                hurtTriggerSpecial1 = true;
                hurtTriggerAmountSpecial1 = 0.5f;
            }
            if (HasPerk(PerkId.BSScreechStun))
            {
                specialChargeTime *= 1.5f;
                stunStackSpecial1 += 3;
            }
            if (HasPerk(PerkId.BSScreenStutter))
            {
                maxSpecialCharges = 3;
                specialChargeTime *= 0.75f;
                sizeXGrowSpecial1 = 4;
                sizeYGrowSpecial1 = 4;
            }
        }
        if (HasPerk(PerkId.BSWhirl))
        {
            shapeSpecial1 = ProjectileShape.Rectangle;
            durationSpecial1 = 3;
            sizeXSpecial1 = 0.5f;
            sizeYSpecial1 = 5;
            growsSpecial1 = false;
            int rotations = 2 * 360;
            rotateSpeedSpecial1 = rotations / durationSpecial1;
            pierceSpecial1 = int.MaxValue;
            damageTypeSpecial1 = DamageType.Physical;
            damageSpecial1 *= 1.5f;
            specialChargeTime = 15;
            followPlayerSpecial1 = true;
            spriteSpecial1 = ProjectileSprite.Knife;
            if (HasPerk(PerkId.BSWhirlBig))
            {
                sizeYSpecial1 *= 1.2f;
                damageSpecial1 *= 1.2f;
                specialChargeTime *= 1.2f;
            }
            if (HasPerk(PerkId.BSWhirlSharp))
            {
                bleedStackSpecial1 = 3;
                critChanceSpecial1 += 0.2f;
            }
            if (HasPerk(PerkId.BSWhirlMount))
            {
                followPlayerSpecial1 = false;
                rotateSpeedSpecial1 *= 1.5f;
                sizeYSpecial1 *= 0.75f;
                damageSpecial1 *= 1.2f;
            }
        }
        if (HasPerk(PerkId.BSReck))
        {
            noProjectileSpecial = true;
            overchargeGainSpecial = 1;
            overchargeMaxSpecial = 1;
            vulnerableGainSpecial = 1;
            vulnerableMaxSpecial = 5;
            specialChargeTime = 3;
            if (HasPerk(PerkId.BSReckPower))
            {
                overchargeGainSpecial = 3;
                overchargeMaxSpecial = 3;
                vulnerableGainSpecial = 3;
                vulnerableMaxSpecial = 3;
            }
            if (HasPerk(PerkId.BSReckFury))
            {
                overchargeMaxSpecial = 10;
                specialChargeTime *= 0.75f;
            }
            if (HasPerk(PerkId.BSReckCrit))
            {
                overchargeCritChanceGain = 0.15f;
                overchargeCritDamageGain = 1;
            }
        }

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Charged Active Perks

        // Berserker

        if (HasPerk(PerkId.BCRage))
        {
            noProjectileCharged = true;
            manaGainHit = 2;
            manaGainHurt = 4;
            damageGainCharged = 1f;
            chargedCancelable = false;
            chargedActivatibleEarly = false;
            chargedAutoActivates = false;
            chargedFullDeplete = false;
            chargedMaxMana = 30;
            if (HasPerk(PerkId.BCRageUncon))
            {
                manaGainHit++;
                manaGainHurt++;
                chargedMaxMana = 15;
                chargedAutoActivates = true;
            }
            if (HasPerk(PerkId.BCRageFrenzy))
            {
                damageGainCharged = 0;
                critChanceGainCharged = 0.25f;
                critDamageGainCharged = 1f;
            }
            if (HasPerk(PerkId.BCRageCon))
            {
                chargedCancelable = true;
                chargedMaxMana = 40;
            }
        }
        if (HasPerk(PerkId.BCDefend))
        {
            noProjectileCharged = true;
            manaGainHit = 2;
            manaGainHurt = 4;
            chargedCancelable = false;
            chargedActivatibleEarly = false;
            chargedAutoActivates = false;
            chargedFullDeplete = true;
            chargedBarrierGain = .15f;
            chargedMaxMana = 50;
            if (HasPerk(PerkId.BCDefendBrace))
            {
                chargedMaxMana = 25;
                chargedAutoActivates = true;
                chargedBarrierGain = 0.05f;
            }
            if (HasPerk(PerkId.BCDefendArmor))
            {
                chargedBarrierGain = 1;
                chargedFullDeplete = false;
                chargedEndBarrierDeplete = true;
                chargedMaxMana = 20;
            }
            if (HasPerk(PerkId.BCDefendShield))
            {
                chargedBarrierGain = 0.2f;
                chargedStartBarrierDeplete = true;
                noProjectileCharged = false;
                shapeCharged1 = ProjectileShape.Circle;
                durationCharged1 = 0.5f;
                sizeXCharged1 = 0.5f;
                sizeYCharged1 = 0.5f;
                growsCharged1 = true;
                sizeXGrowCharged1 = 4;
                sizeYGrowCharged1 = 4;
                growDurationCharged1 = 0.25f;
                followPlayerCharged1 = true;
                pierceCharged1 = int.MaxValue;
                damageTypeCharged1 = DamageType.Physical;
                damageCharged1 = baseDamage * 2;
                critChanceCharged1 = baseCritChance;
                critDamageCharged1 = baseCritDamage;
                spriteCharged1 = ProjectileSprite.Ring;
            }
        }
        if (HasPerk(PerkId.BCElement))
        {
            manaGainHit = 2;
            manaGainHurt = 4;
            chargedCancelable = false;
            chargedActivatibleEarly = false;
            chargedAutoActivates = false;
            chargedFullDeplete = false;
            shapeCharged1 = ProjectileShape.Circle;
            sizeXCharged1 = 6;
            sizeYCharged1 = 6;
            growsCharged1 = false;
            followPlayerCharged1 = true;
            pierceCharged1 = int.MaxValue;
            damageTypeCharged1 = DamageType.Fire;
            damageCharged1 = baseDamage * 1.5f;
            critChanceCharged1 = baseCritChance;
            critDamageCharged1 = baseCritDamage;
            periodLengthCharged1 = 0.5f;
            chargedMaxMana = 20;
            spriteCharged1 = ProjectileSprite.Fire;
            if (HasPerk(PerkId.BCElementBlast))
            {
                chargedCancelable = true;
                chargedCancelBlast = true;
            }
            if (HasPerk(PerkId.BCElementAura))
            {
                damageTypeCharged1 = DamageType.Lightning;
                damageCharged1 *= 1.5f;
                periodLengthCharged1 = 0.75f;
                spriteCharged1 = ProjectileSprite.Lightning;
            }
            if (HasPerk(PerkId.BCElementBurst))
            {
                manaGainHit--;
                manaGainHurt --;
                chargedMaxMana = 10;
                chargedAutoActivates = true;
                damageCharged1 *= 0.75f;
            }
        }

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Passive Perks Post

        if (HasPerk(PerkId.LightningStun))
        {
            stunStackLightning = 2;
        }
        if (HasPerk(PerkId.StunStack))
        {
            stunStackNormal1 *= 2;
            stunStackSpecial1 *= 2;
            stunStackCharged1 *= 2;
            stunStackLightning *= 2;
        }
        if (HasPerk(PerkId.Cooldown))
        {
            specialChargeTime *= 0.75f;
        }
        if (HasPerk(PerkId.CritBleed))
        {
            bleedStacksCrit = 3;
        }
        if (HasPerk(PerkId.BleedCritChanceDamage))
        {
            bleedCritBonusStack = 1;
        }
        if (HasPerk(PerkId.StunDamage))
        {
            stunVulnerableStack = 2;
        }
        if (HasPerk(PerkId.HurtCharge))
        {
            manaGainHurt++;
        }
        if (HasPerk(PerkId.HitCharge))
        {
            manaGainHit += 2;
        }
        if (HasPerk(PerkId.FireBurn))
        {
            burnStackFire = 2;
        }
        if (HasPerk(PerkId.LightningCritDamage))
        {
            critDamageLightning = 1f;
        }
        if (HasPerk(PerkId.StunCritChance))
        {
            stunStackCrit = 2;
        }
        if (HasPerk(PerkId.FireCritChance))
        {
            critChanceFire = 0.25f;
        }
        if (HasPerk(PerkId.CritBarrier))
        {
            barrierGainCrit = 1f;
        }
        if (HasPerk(PerkId.CritBurn))
        {
            burnStackCrit = 2;
        }
        if (HasPerk(PerkId.CritCharge))
        {
            manaGainCrit = 2;
        }
        if (HasPerk(PerkId.CritChanceSelfDamage))
        {
            selfDamageNormal = baseDamage * 0.1f;
        }
        if (HasPerk(PerkId.MaxCharge))
        {
            chargedMaxMana *= 1.2f;
        }
        if (HasPerk(PerkId.HurtLightningThorn))
        {
            hurtLightningThorn = baseDamage;
        }

        // Final Setup

        currentHealth = maxHealth;
        chargedMana = 0;
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
        if (selfDamageNormal > 0)
        {
            Hurt(null, selfDamageNormal);
        }
    }

    public void TriggerSpecialAbility(float chargePercent = 0)
    {
        currentSpecialCharges--;
        if (noProjectileSpecial)
        {
            overchargeStacks = Mathf.Clamp(overchargeStacks + overchargeGainSpecial, 0, overchargeMaxSpecial);
            vulnerableStacks = Mathf.Clamp(vulnerableStacks + vulnerableGainSpecial, 0, vulnerableMaxSpecial);
        }
        else SpawnProjectile(shapeSpecial1, durationSpecial1, sizeXSpecial1, sizeYSpecial1, growsSpecial1 ? sizeXGrowSpecial1 : sizeXSpecial1, growsSpecial1 ? sizeYGrowSpecial1 : sizeYSpecial1, growDurationSpecial1, projectileSpeedSpecial1, homingStrengthSpecial1, rotateSpeedSpecial1, periodLengthSpecial1, pierceSpecial1, bounceSpecial1, followPlayerSpecial1, returningSpecial1, false, AbilityType.Special, aimDirection, spriteSpecial1, chargedActive, 1, 1 + chargePercent);
    }

    public void ToggleChargedAbility()
    {
        if (chargedActive)
        {
            chargedActive = false;
            if (chargedEndBarrierDeplete)
            {
                currentBarrier = 0;
            }
            if (chargedCancelBlast)
            {
                float damageMult = 2;
                damageMult *= chargedMana / chargedMaxMana;
                chargedMana = 0;
                SpawnProjectile(shapeCharged1, durationCharged1, sizeXCharged1, sizeYCharged1, growsCharged1 ? sizeXGrowCharged1 : sizeXCharged1, growsCharged1 ? sizeYGrowCharged1 : sizeYCharged1, growDurationCharged1, projectileSpeedCharged1, homingStrengthCharged1, rotateSpeedCharged1, periodLengthCharged1, pierceCharged1, bounceCharged1, followPlayerCharged1, returningCharged1, true, AbilityType.Charged, aimDirection, spriteCharged1, chargedActive, 1, 1 + damageMult);
            }
        }
        else
        {
            chargedActive = true;
            float damageMult = 0;
            if (chargedStartBarrierDeplete)
            {
                damageMult = currentBarrier;
                currentBarrier = 0;
            }
            if (chargedBarrierGain > 0)
            {
                currentBarrier = Mathf.Clamp(currentBarrier + chargedBarrierGain, 0, currentHealth);
            }
            if (chargedFullDeplete)
            {
                chargedMana = 0;
            }
            if (noProjectileCharged)
            {

            }
            else SpawnProjectile(shapeCharged1, durationCharged1, sizeXCharged1, sizeYCharged1, growsCharged1 ? sizeXGrowCharged1 : sizeXCharged1, growsCharged1 ? sizeYGrowCharged1 : sizeYCharged1, growDurationCharged1, projectileSpeedCharged1, homingStrengthCharged1, rotateSpeedCharged1, periodLengthCharged1, pierceCharged1, bounceCharged1, followPlayerCharged1, returningCharged1, true, AbilityType.Charged, aimDirection, spriteCharged1, chargedActive, 1, chargedStartBarrierDeplete ? damageMult : 1);
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
        chargedMana = Mathf.Clamp(chargedMana + manaGainHit, 0, chargedMaxMana);
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
        int pushForce = 0;
        int stunVulnerableStacks = stunVulnerableStack;
        int bleedCritBonusStacks = bleedCritBonusStack;
        DamageType currentDamageType = DamageType.Physical;
        if (abilityType == AbilityType.Normal)
        {
            if (index == 1)
            {
                damage = damageNormal1;
                critDamage = critDamageNormal1;
                stunStacks = stunStackNormal1;
                bleedStacks = bleedStackNormal1;
                pushForce = pushForceNormal1;
                stunVulnerableStacks = stunVulnerableStackNormal1;
                currentDamageType = damageTypeNormal1;
            }
            else if (index == 2)
            {
                damage = damageDestroyedNormal1;
                critDamage = critDamageDestroyedNormal1;
                currentDamageType = damageTypeDestroyedNormal1;
            }
        }
        else if (abilityType == AbilityType.Special)
        {
            if (index == 1)
            {
                damage = damageSpecial1;
                critDamage = critDamageSpecial1;
                stunStacks = stunStackSpecial1;
                pushForce = pushForceSpecial1;
                bleedStacks = bleedStackSpecial1;
                currentDamageType = damageTypeSpecial1;
            }
        }
        else if (abilityType == AbilityType.Charged)
        {
            if (index == 1)
            {
                damage = damageCharged1;
                critDamage = critDamageCharged1;
                currentDamageType = damageTypeCharged1;
            }
        }
        if (overchargeStacks > 0)
        {
            damage *= 1 + 2 * overchargeStacks / 10;
            overchargeStacks--;
            critDamage += overchargeCritDamageGain;
        }
        if (chargedActive)
        {
            damage *= 1 + damageGainCharged;
            critDamage += critDamageGainCharged;
        }
        if (currentDamageType == DamageType.Fire)
        {
            burnStacks += burnStackFire;
        }
        if (currentDamageType == DamageType.Lightning)
        {
            critDamage += critDamageLightning;
            stunStacks += stunStackLightning;
        }
        if (enemy.bleedCritBonusStacks > 0)
        {
            critDamage += enemy.bleedCritBonusStacks / 10f;
        }
        if (hitCrit)
        {
            damage *= 1 + critDamage;
            bleedStacks += bleedStacksCrit;
            burnStacks += burnStackCrit;
            currentBarrier = Mathf.Clamp(currentBarrier + barrierGainCrit, 0, currentHealth);
            chargedMana += manaGainCrit;
            stunStacks += stunStackCrit;
        }
        damage *= damageMult;
        if (enemy != null) enemy.Damage(damage);
        if (stunStacks > 0 && enemy != null) enemy.InflictStatus(Status.Stun, stunStacks);
        if (bleedStacks > 0 && enemy != null) enemy.InflictStatus(Status.Bleed, bleedStacks);
        if (stunVulnerableStacks > 0 && enemy != null) enemy.InflictStatus(Status.StunVulnerable, stunVulnerableStacks);
        if (pushForce > 0 && enemy != null) enemy.InflictStatus(Status.Push, pushForce);
        if (bleedCritBonusStacks > 0 && enemy != null) enemy.InflictStatus(Status.BleedCritBonus, bleedCritBonusStacks);
        return false;
    }

    public void TriggerPeriodic(List<MatchEnemy> enemies, AbilityType abilityType, int index, bool chargeActive)
    {
        foreach (MatchEnemy enemy in enemies)
        {
            TriggerHit(enemy, abilityType, index, chargeActive);
        }
    }

    public bool TriggeredCrit(MatchEnemy enemy, AbilityType abilityType, int index, bool chargeActive)
    {
        float critChance = 0;
        DamageType currentDamageType = DamageType.Physical;
        if (abilityType == AbilityType.Normal)
        {
            if (index == 1)
            {
                critChance = critChanceNormal1;
                currentDamageType = damageTypeNormal1;
            }
            else if (index == 2)
            {
                critChance = critChanceDestroyedNormal1;
                currentDamageType = damageTypeDestroyedNormal1;
            }
        }
        else if (abilityType == AbilityType.Special)
        {
            critChance = critChanceSpecial1;
            currentDamageType = damageTypeSpecial1;
        }
        else if (abilityType == AbilityType.Charged)
        {
            critChance = critChanceCharged1;
            currentDamageType = damageTypeCharged1;
        }
        if (overchargeStacks > 0) critChance += overchargeCritChanceGain;
        if (chargeActive)
        {
            critChance += critChanceGainCharged;
        }
        if (currentDamageType == DamageType.Fire)
        {
            critChance += critChanceFire;
        }
        if (enemy.bleedCritBonusStacks > 0)
        {
            critChance += enemy.bleedCritBonusStacks / 10f;
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

    public void Hurt(MatchEnemy enemy, float damage = 0)
    {
        if (enemy != null)
        {
            enemy.TriggerAttackCooldown();
            enemy.InflictStatus(Status.Push, 2);
            damage = enemy.damage;
            if (hurtLightningThorn > 0) enemy.Damage(hurtLightningThorn);
        }
        chargedMana = Mathf.Clamp(chargedMana + manaGainHurt, 0, chargedMaxMana);
        if (hurtTriggerSpecial1)
        {
            TriggerSpecialAbility(hurtTriggerAmountSpecial1 - 1);
        }
        if (vulnerableStacks > 0)
        {
            damage *= 1 + vulnerableStacks / 10f;
            vulnerableStacks--;
        }
        if (currentBarrier > 0)
        {
            float damageReduction = Mathf.Clamp(damage * 0.5f, 0, currentBarrier);
            damage -= damageReduction;
            currentBarrier -= damageReduction;
        }
        currentHealth -= damage;
        currentBarrier = Mathf.Clamp(currentBarrier + hurtBarrierGain, 0, currentHealth);
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
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