using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class Data", menuName = "ClassData")]
public class ClassData : ScriptableObject
{
    public string className;
    public string classDescription;
    public Color classColorLight;
    public Color classColorDark;
    public Sprite classSprite;
    [Header("Perks")]
    public PerkData[] normalAbility = new PerkData[12];
    public PerkData[] specialAbility = new PerkData[12];
    public PerkData[] chargedAbility = new PerkData[12];
    public PerkData[] passiveAbilityA = new PerkData[12];
    public PerkData[] passiveAbilityB = new PerkData[12];
}
