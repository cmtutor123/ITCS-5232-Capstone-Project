using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingHelper : MonoBehaviour
{
    TileGrid grid = new TileGrid();

    public void AddTile(TileData tileData)
    {
        grid.AddTile(tileData);
    }

    public Vector2 PathToPlayer(Vector2 currentPosition)
    {
        return grid.FindPathToPlayer(currentPosition);
    }
}

class TileGrid
{
    public float currentOff = 0.4f;
    public float nextOff = 0.6f;

    Dictionary<(int, int), TilePath> grid = new Dictionary<(int, int), TilePath>();

    float tileSize;

    Dictionary<((int, int), (int, int)), (int, int)> bestNextTile = new Dictionary<((int, int), (int, int)), (int, int)>();

    public TileGrid()
    {
        tileSize = TileGenerator.tileSize;
    }

    public void AddTile(TileData tileData)
    {
        grid.Add(tileData.tileOffset, new TilePath(!tileData.wallLeft, !tileData.wallRight, !tileData.wallUp || tileData.hasExit, !tileData.wallDown || tileData.hasEntrance));
    }

    public Vector2 FindPathToPlayer(Vector2 objPos)
    {
        if (GameManager.instance.matchPlayer.invisible) return objPos;
        Vector2 playerPosition = GameManager.instance.matchPlayer.transform.position;
        Vector2 toPlayer = playerPosition - objPos;
        RaycastHit2D hit = Physics2D.BoxCast(objPos, new Vector2(.9f, 0.1f), Vector2.SignedAngle(objPos, playerPosition), toPlayer, toPlayer.magnitude, GameManager.instance.layerMaskSightBlock);
        if (hit.collider == null) return playerPosition;
        return FindPathToPoint(objPos, playerPosition);
    }

    public Vector2 FindPathToPoint(Vector2 objPos, Vector2 pointPos)
    {
        (int, int) start = (Mathf.RoundToInt(objPos.x / tileSize), Mathf.RoundToInt(objPos.y / tileSize));
        (int, int) end = (Mathf.RoundToInt(pointPos.x / tileSize), Mathf.RoundToInt(pointPos.y / tileSize));
        if (start == end) return pointPos;
        if (!bestNextTile.ContainsKey((start, end)))
        {
            CalculateNextTile(start, end);
        }
        (int, int) next = bestNextTile[(start, end)];
        Vector2 startPos = new Vector2(start.Item1 * tileSize, start.Item2 * tileSize);
        Vector2 tilePos = new Vector2(next.Item1 * tileSize, next.Item2 * tileSize);
        return (currentOff * startPos) + (nextOff * tilePos);
    }

    public void CalculateNextTile((int, int) start, (int, int) end)
    {
        PriorityQueue<(int, int)> frontier = new PriorityQueue<(int, int)>();
        frontier.Enqueue(start, 0);
        Dictionary<(int, int), (int, int)> cameFrom = new Dictionary<(int, int), (int, int)>();
        Dictionary<(int, int), int> costSoFar = new Dictionary<(int, int), int>();
        cameFrom[start] = start;
        costSoFar[start] = 0;
        while (frontier.Count > 0)
        {
            (int, int) current = frontier.Dequeue();
            if (current == end)
            {
                break;
            }
            foreach ((int, int) next in GetNeighbors(current))
            {
                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Distance(next, end);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        (int, int) currentTile = end;
        while (currentTile != start)
        {
            if (cameFrom[currentTile] == start)
            {
                bestNextTile[(start, end)] = currentTile;
                break;
            }
            currentTile = cameFrom[currentTile];
        }
    }

    public List<(int, int)> GetNeighbors((int, int) pos)
    {
        List<(int, int)> neighbors = new List<(int, int)>();
        int xPos = pos.Item1;
        int yPos = pos.Item2;
        TilePath tilePath = grid[pos];
        if (tilePath.pathLeft)
        {
            neighbors.Add((xPos - 1, yPos));
        }
        if (tilePath.pathRight)
        {
            neighbors.Add((xPos + 1, yPos));
        }
        if (tilePath.pathUp)
        {
            neighbors.Add((xPos, yPos + 1));
        }
        if (tilePath.pathDown)
        {
            neighbors.Add((xPos, yPos - 1));
        }
        return neighbors;
    }
    public int Distance((int, int) start, (int, int) end)
    {
        return Mathf.Abs(start.Item1 - end.Item1) + Mathf.Abs(start.Item2 - end.Item2);
    }
}

class TilePath
{
    public bool pathLeft, pathRight, pathUp, pathDown;

    public TilePath(bool pathLeft, bool pathRight, bool pathUp, bool pathDown)
    {
        this.pathLeft = pathLeft;
        this.pathRight = pathRight;
        this.pathUp = pathUp;
        this.pathDown = pathDown;
    }
}

class PriorityQueue<TElement>
{
    List<TElement> elements = new List<TElement>();
    List<int> priorities = new List<int>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, int priority)
    {
        int index = GetAddPosition(priority);
        elements.Insert(index, element);
        priorities.Insert(index, priority);
    }

    public TElement Dequeue()
    {
        TElement element = elements[0];
        elements.RemoveAt(0);
        priorities.RemoveAt(0);
        return element;
    }

    public int GetAddPosition(int priority)
    {
        int index = 0;
        while (index < priorities.Count)
        {
            if (priority > priorities[index])
            {
                index++;
            }
            else
            {
                return index;
            }
        }
        return index;
    }

    public bool Contains(TElement element)
    {
        return elements.Contains(element);
    }
}