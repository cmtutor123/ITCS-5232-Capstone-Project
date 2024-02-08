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

    public PlayerData()
    {
        for (int i = 0; i < 6; i++)
        {
            level[i] = 1;
            string perkString = "";
            for (int j = 0; j < 60; j++)
            {
                if (j == 0 || j == 12 || j == 24)
                {
                    perkString += "1";
                }
                else
                {
                    perkString += "0";
                }
            }
            perks[i] = perkString;
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
            perks[i] = inputData.perks[i];
            int levelPoints = level[i] * 3;
            int perkCount = 0;
            foreach (char perk in perks[i])
            {
                if (perk == '1')
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
                        string perkString = "";
                        for (int j = 0; j < 60; j++)
                        {
                            if (j == 0 || j == 12 || j == 24)
                            {
                                perkString += "1";
                            }
                            else
                            {
                                perkString += "0";
                            }
                        }
                        perks[i] = perkString;
                        points[i] = levelPoints - 3;
                    }
                }
            }
            if (perks[i][0] == '0' || perks[i][12] == '0' || perks[i][24] == '0')
            {
                int missing = (perks[i][0] == '0' ? 1 : 0) + (perks[i][12] == '0' ? 1 : 0) + (perks[i][24] == '0' ? 1 : 0);
                if (missing <= points[i])
                {
                    char[] perksArray = perks[i].ToCharArray();
                    perksArray[0] = '1';
                    perksArray[12] = '1';
                    perksArray[24] = '1';
                    perks[i] = new string(perksArray);
                    points[i] -= missing;
                }
                else
                {
                    string perkString = "";
                    for (int j = 0; j < 60; j++)
                    {
                        if (j == 0 || j == 12 || j == 24)
                        {
                            perkString += "1";
                        }
                        else
                        {
                            perkString += "0";
                        }
                    }
                    perks[i] = perkString;
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