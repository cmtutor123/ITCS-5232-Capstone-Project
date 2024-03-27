using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public static int CLASS_COUNT = 6;
    public static int STAGE_COUNT = 3;

    public int[] exp = new int[CLASS_COUNT];
    public int[] level = new int[CLASS_COUNT];
    public int[] points = new int[CLASS_COUNT];
    public int[,] perks = new int[CLASS_COUNT, 60];
    public int[,] stages = new int[CLASS_COUNT, STAGE_COUNT];

    public PlayerData()
    {
        for (int i = 0; i < CLASS_COUNT; i++)
        {
            level[i] = 1;
            for (int j = 0; j < 60; j++)
            {
                if (j == 0 || j == 12 || j == 24)
                {
                    perks[i, j] = 1;
                }
            }
            for (int j = 0; j < STAGE_COUNT; j++)
            {
                stages[i, j] = 0;
            }
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
            for (int j = 0; j < STAGE_COUNT; j++)
            {
                stages[i, j] = inputData.stages[i, j];
            }
        }
    }

    public int GetLevelFromExp(int exp)
    {
        return 1;
    }
}