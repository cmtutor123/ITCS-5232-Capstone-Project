using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int[] exp = new int[6];
    public int[] level = new int[6];
    public int[] points = new int[6];
    public string[] perks = new string[6];
    public string[] loadout = new string[6];

    public PlayerData()
    {

    }

    public PlayerData(PlayerData inputData)
    {

    }
}