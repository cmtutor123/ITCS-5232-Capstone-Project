using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Perk Data", menuName = "Perk Data")]
public class PerkData : ScriptableObject
{
    public PerkId perkId;
    public string perkName, perkDescription;
    public int cost = 0;
    public Sprite perkIcon;
}
