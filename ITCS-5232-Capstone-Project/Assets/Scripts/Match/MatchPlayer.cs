using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPlayer : MonoBehaviour
{
    public List<PerkId> perkIds = new List<PerkId>();

    public PlayerState currentState = PlayerState.Idle;

    public AbilityType currentAbilityType;

    public Rigidbody2D rb;

    public float windUpTime, castTime, windDownTime;
    public float windUpTimer, castTimer, windDownTimer;

    public GameObject playerProjectile => GameManager.instance.prefabPlayerProjectile;

    public PlayerProjectile NewProjectile => Instantiate(playerProjectile, transform).GetComponent<PlayerProjectile>();

    public bool StateMovable => currentState == PlayerState.Idle || currentState == PlayerState.Moving;
    public bool StateAbilityUsable => currentState == PlayerState.Idle || currentState == PlayerState.Moving || currentState == PlayerState.WindingDown;
    public bool StateDashUsable => currentState == PlayerState.Idle || currentState == PlayerState.Moving || currentState == PlayerState.WindingDown || currentState == PlayerState.WindingUp;

    public float currentMoveSpeed;

    public Vector2 aimDirection;
    public float moveSpeed;
    public float dashTimer;
    public float dashDuration;
    public float dashDistance;

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

    public DamageType damageTypeNormal1, damageTypeSpecial1, damageTypeCharged1;
    public ProjectileShape shapeNormal1, shapeSpecial1, shapeCharged1;
    public float sizeXNormal1, sizeYNormal1, sizeXSpecial1, sizeYSpecial1, sizeXCharged1, sizeYCharged1;
    public bool grows;
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

    public int stunStackNormal1, stunStackSpecial1, stunStackCharged1;
    public float damageNormal1, damageSpecial1, damageCharged1;

    public BaseStats baseStats => GameManager.instance.GetBaseStats();

    private void FixedUpdate()
    {
        if (currentState == PlayerState.Charging)
        {
            chargeAbilityTimer += Time.fixedDeltaTime;
        }
        else if (currentState == PlayerState.Dashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
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
        else if (currentState == PlayerState.Moving)
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

        dashDuration = baseStats.dashDuration;
        dashDistance = baseStats.dashDistance;
        windUpTime = baseStats.windUpTime;
        windDownTime = baseStats.windDownTime;
        castTime = baseStats.castTime;
        GetComponent<SpriteRenderer>().sprite = baseStats.playerSprite;

        // Normal Active Perks

        // Berserker

        if (HasPerk(PerkId.BNThrow))
        {
            damageTypeNormal1 = DamageType.Physical;
            shapeNormal1 = ProjectileShape.Rectangle;
            sizeXNormal1 = .25f;
            sizeYNormal1 = 1;
            grows = false;
            float range = 4;
            durationNormal1 = 1.5f;
            projectileSpeedNormal1 = range / durationNormal1;

        }

        // Druid

        // Necromancer

        // Paladin

        // Rogue

        // Wizard

        // Special Active Perks

        // Berserker

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
        PlayerProjectile projectile = NewProjectile;
        projectile.PrepareProjectile(shapeNormal1, durationNormal1, sizeXNormal1, sizeYNormal1, grows ? sizeXGrowNormal1 : sizeXNormal1, grows ? sizeYGrowNormal1 : sizeYNormal1, grows ? growDurationNormal1 : 0, projectileSpeedNormal1, homingStrengthNormal1, rotateSpeedNormal1, periodLengthNormal1, pierceNormal1, bounceNormal1, followPlayerNormal1, returningNormal1, false, AbilityType.Normal, aimDirection, spriteNormal1, chargedActive);
        projectile.StartProjectile();
    }

    public void TriggerSpecialAbility(float chargePercent = 0)
    {
        PlayerProjectile projectile = NewProjectile;
        projectile.PrepareProjectile(shapeSpecial1, durationSpecial1, sizeXSpecial1, sizeYSpecial1, grows ? sizeXGrowSpecial1 : sizeXSpecial1, grows ? sizeYGrowSpecial1 : sizeYSpecial1, growDurationSpecial1, projectileSpeedSpecial1, homingStrengthSpecial1, rotateSpeedSpecial1, periodLengthSpecial1, pierceSpecial1, bounceSpecial1, followPlayerSpecial1, returningSpecial1, false, AbilityType.Special, aimDirection, spriteSpecial1, chargedActive);
        projectile.StartProjectile();
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
            PlayerProjectile projectile = NewProjectile;
            projectile.PrepareProjectile(shapeCharged1, durationCharged1, sizeXCharged1, sizeYCharged1, grows ? sizeXGrowCharged1 : sizeXCharged1, grows ? sizeYGrowCharged1 : sizeYCharged1, growDurationCharged1, projectileSpeedCharged1, homingStrengthCharged1, rotateSpeedCharged1, periodLengthCharged1, pierceCharged1, bounceCharged1, followPlayerCharged1, returningCharged1, true, AbilityType.Charged, aimDirection, spriteCharged1, chargedActive);
            projectile.StartProjectile();
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
            moveSpeed = dashDistance / dashDuration;
        }
    }

    public void TriggerMove(Vector2 direction)
    {
        if (StateMovable)
        {
            if (direction != new Vector2(0, 0))
            {
                aimDirection = direction.normalized;
                currentMoveSpeed = moveSpeed;
                currentState = PlayerState.Moving;
            }
            else
            {
                currentMoveSpeed = 0;
                currentState = PlayerState.Idle;
            }
        }
    }

    public bool TriggerHit(MatchEnemy enemy, AbilityType abilityType, bool chargeActive)
    {
        if (enemy != null)
        {
            bool invincibilityFrames = enemy.GetInvincibilityFrames();
            if (invincibilityFrames) return true;
        }
        bool hitCrit = TriggeredCrit(enemy, abilityType, chargeActive);
        // determine damage
        // determine hit effects
        // if crit determine crit damage and crit effects
        // damage enemy
        // apply effects to enemy
        return false;
    }

    public void TriggerPeriodic(List<MatchEnemy> enemies, AbilityType abilityType, bool chargeActive)
    {

    }

    public void TriggerCrit(MatchEnemy enemy, AbilityType abilityType, bool chargeActive)
    {

    }

    public bool TriggeredCrit(MatchEnemy enemy, AbilityType abilityType, bool chargeActive)
    {
        // calculate crit chance and randomly determine if crit
        return false;
    }

    public void TriggerDestroy(Vector2 position, AbilityType abilityType, bool chargeActive)
    {

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