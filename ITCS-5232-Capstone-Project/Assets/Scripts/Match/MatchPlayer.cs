using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPlayer : MonoBehaviour
{
    public List<PerkId> perkIds = new List<PerkId>();

    public bool chargeActive;
    

    public void ProcessPerks()
    {

        // Normal Active Perks

        // Berserker

        if (HasPerk(PerkId.BNThrow))
        {

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
