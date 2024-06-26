using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public static int CLASS_COUNT = 1;
    public static int STAGE_COUNT = 3;

    public int[] exp = new int[CLASS_COUNT];
    public int[] level = new int[CLASS_COUNT];
    public int[] points = new int[CLASS_COUNT];
    public int[] perks = new int[60];
    public int[] stages = new int[STAGE_COUNT];

    public PlayerData()
    {
        for (int i = 0; i < CLASS_COUNT; i++)
        {
            level[i] = 1;
            for (int j = 0; j < 60; j++)
            {
                if (j == 0 || j == 12 || j == 24)
                {
                    perks[j] = 1;
                }
            }
            for (int j = 0; j < STAGE_COUNT; j++)
            {
                stages[j] = -1;
            }
            stages[0] = 0;
        }
    }

    public PlayerData(PlayerData inputData)
    {
        for (int i = 0; i < CLASS_COUNT; i++)
        {
            exp[i] = inputData.exp[i];
            if (exp[i] < 0)
            {
                exp[i] = 0;
            }
            level[i] = GetLevelFromExp(exp[i]);
            points[i] = inputData.points[i];
            perks = inputData.perks;
            int levelPoints = level[i] * 3;
            int perkCount = 0;
            for (int j = 0; j < 60; j++)
            {
                if (perks[j] == 1)
                {
                    perkCount++;
                }
            }
            if (perkCount + points[i] != levelPoints)
            {
                if (perkCount + points[i] < levelPoints)
                {
                    points[i] += levelPoints - (perkCount + points[i]);
                }
                else
                {
                    if (perkCount <= levelPoints)
                    {
                        points[i] = levelPoints - perkCount;
                    }
                    else
                    {
                        for (int j = 0; j < 60; j++)
                        {
                            if (j == 0 || j == 12 || j == 24)
                            {
                                perks[j] = 1;
                            }
                            else
                            {
                                perks[j] = 0;
                            }
                        }
                        points[i] = levelPoints - 3;
                    }
                }
            }
            if (perks[0] == 0 || perks[12] == 0 || perks[24] == 0)
            {
                int missing = (perks[0] == 0 ? 1 : 0) + (perks[12] == 0 ? 1 : 0) + (perks[24] == 0 ? 1 : 0);
                if (missing <= points[i])
                {
                    perks[0] = 1;
                    perks[12] = 1;
                    perks[24] = 1;
                    points[i] -= missing;
                }
                else
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (j == 0 || j == 12 || j == 24)
                        {
                            perks[j] = 1;
                        }
                        else
                        {
                            perks[j] = 0;
                        }
                    }
                    points[i] = levelPoints - 3;
                }
            }
            stages = inputData.stages;
        }
    }

    public static int GetLevelFromExp(int exp)
    {
        for (int lvl = 20; lvl >= 1; lvl--)
        {
            if (exp >= GetExpFromLevel(lvl)) return lvl;
        }
        return 1;
    }

    public static int GetExpFromLevel(int level)
    {
        int exp = 0;
        for (int i = 1; i < level; i++)
        {
            exp += GetExpNextLevel(i);
        }
        return exp;
    }

    public static int GetExpNextLevel(int level)
    {
        return level * level * 100;
    }

    public void UpdatePoints()
    {
        int count = 0;
        for (int i = 0; i < CLASS_COUNT; i++)
        {
            foreach (int perk in perks)
            {
                if (perk == 1)
                {
                    count++;
                }
            }
            points[i] = level[i] * 3 - count;
        }
    }
}