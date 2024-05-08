using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public List<EnemyData> roomEnemies = new List<EnemyData>();
    public List<(int, int)> points = new List<(int, int)>();
    public List<TileData> tileData = new List<TileData>();
    public (int, int) roomOffset, nextOffset;
    public int index;

    public void SetRoomShape(int size)
    {
        points.Add((0, 0));
        while (points.Count < size)
        {
            Dictionary<(int, int), int> probs = new Dictionary<(int, int), int>();
            foreach ((int, int) point in points)
            {
                List<(int, int)> surrounding = new List<(int, int)>();
                surrounding.Add((point.Item1 - 1, point.Item2));
                surrounding.Add((point.Item1 + 1, point.Item2));
                surrounding.Add((point.Item1, point.Item2 + 1));
                foreach ((int, int) key in surrounding)
                {
                    if (!points.Contains(key))
                    {
                        if (probs.ContainsKey(key))
                        {
                            probs[key] += 1;
                        }
                        else
                        {
                            probs[key] = 1;
                        }
                    }
                }
            }
            float sum = 0;
            foreach ((int, int) key in probs.Keys)
            {
                sum += probs[key];
            }
            float rand = Random.Range(0, sum);
            (int, int) pos = (0, 0);
            foreach ((int, int) key in probs.Keys)
            {
                rand -= probs[key];
                if (rand <= 0)
                {
                    pos = key;
                    break;
                }
            }
            points.Add(pos);
        }
        GenerateTileData();
        SetEntrance();
        SetExit();
    }

    public void SetStartRoom()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                points.Add((x, y));
            }
        }
        GenerateTileData();
        roomOffset = (0, -1);
        SetExit(true);
    }

    public void SetBossRoom()
    {
        index = 11;
        for (int x = -2; x <= 2; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                points.Add((x, y));
            }
        }
        GenerateTileData();
        SetEntrance();
    }

    public void SetRoomEnemies(List<EnemyData> enemyData)
    {
        foreach (EnemyData data in enemyData)
        {
            roomEnemies.Add(data);
        }
    }

    public void SetBossEnemy(EnemyData[] enemyData)
    {
        roomEnemies.Add(enemyData[Random.Range(0, enemyData.Length)]);
    }

    public void GenerateTileData()
    {
        foreach ((int, int) point in points)
        {
            bool left = !points.Contains((point.Item1 - 1, point.Item2));
            bool right = !points.Contains((point.Item1 + 1, point.Item2));
            bool up = !points.Contains((point.Item1, point.Item2 + 1));
            bool down = !points.Contains((point.Item1, point.Item2 - 1));
            tileData.Add(new TileData(point, left, right, up, down, index));
        }
    }

    public void SetEntrance()
    {
        if (tileData[0].tileOffset == (0, 0))
        {
            tileData[0].hasEntrance = true;
        }
        else
        {
            foreach (TileData data in tileData)
            {
                (int, int) entranceOffset = (0, 0);
                if (data.tileOffset == entranceOffset)
                {
                    data.hasEntrance = true;
                    return;
                }
            }
        }
    }

    public void SetExit(bool start = false)
    {
        if (start)
        {
            foreach (TileData data in tileData)
            {
                (int, int) startOffset = (0, 2);
                if (data.tileOffset == startOffset)
                {
                    data.hasExit = true;
                    nextOffset = (startOffset.Item1, startOffset.Item2 + 1);
                    return;
                }
            }
        }
        else
        {
            List<TileData> highest = new List<TileData>();
            int height = 0;
            foreach (TileData data in tileData)
            {
                int y = data.tileOffset.Item2;
                if (y > height)
                {
                    height = y;
                    highest.Clear();
                }
                if (y == height)
                {
                    highest.Add(data);
                }
            }
            TileData exit = highest[Random.Range(0, highest.Count)];
            exit.hasExit = true;
            nextOffset = (exit.tileOffset.Item1, exit.tileOffset.Item2 + 1);
        }
    }
}