using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Perk Data", menuName = "Perk Data")]
public class PerkData : ScriptableObject
{
    public string id, perkName, perkDescription;
    public Sprite perkIcon;
}
