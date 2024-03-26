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
    public int[,] perks = new int[6, 60];

    public PlayerData()
    {
        for (int i = 0; i < 6; i++)
        {
            level[i] = 1;
            for (int j = 0; j < 60; j++)
            {
                if (j == 0 || j == 12 || j == 24)
                {
                    perks[i, j] = 1;
                }
            }
        }
    }

    public PlayerData(PlayerData inputData)
    {
        for (int i = 0; i < 6; i++)
        {
            exp[i] = inputData.exp[i];
            if (exp[i] < 0)
            {
                exp[i] = 0;
            }
            level[i] = GetLevelFromExp(exp[i]);
            points[i] = inputData.points[i];
            for (int j = 0; j < 60; j++)
            {
                perks[i, j] = inputData.perks[i, j];
            }
            int levelPoints = level[i] * 3;
            int perkCount = 0;
            for (int j = 0; j < 60; j++)
            {
                if (perks[i, j] == 1)
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
                                perks[i, j] = 1;
                            }
                            else
                            {
                                perks[i, j] = 0;
                            }
                        }
                        points[i] = levelPoints - 3;
                    }
                }
            }
            if (perks[i, 0] == 0 || perks[i, 12] == 0 || perks[i, 24] == 0)
            {
                int missing = (perks[i, 0] == 0 ? 1 : 0) + (perks[i, 12] == 0 ? 1 : 0) + (perks[i, 24] == 0 ? 1 : 0);
                if (missing <= points[i])
                {
                    perks[i, 0] = 1;
                    perks[i, 12] = 1;
                    perks[i, 24] = 1;
                    points[i] -= missing;
                }
                else
                {
                    for (int j = 0; j < 60; j++)
                    {
                        if (j == 0 || j == 12 || j == 24)
                        {
                            perks[i, j] = 1;
                        }
                        else
                        {
                            perks[i, j] = 0;
                        }
                    }
                    points[i] = levelPoints - 3;
                }
            }
        }
    }

    public int GetLevelFromExp(int exp)
    {
        return 1;
    }
}