using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;

    public Tile[,] boardTiles;

    public delegate void BoardAction();

    public static event BoardAction onBoardLoaded;

    void Awake()
    {
        GameManager.Instance.board = this;
        boardTiles = new Tile[boardWidth, boardHeight];

        // board tiles should populate themselves
        onBoardLoaded?.Invoke();

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (boardTiles[x,y] == null)
                {
                    Debug.LogError("Tile " + x + "," + y + "not loaded");
                }
            }
        }
    }

    void Start()
    {
        List<Tile> testPath = GetPathToTile(boardTiles[3, 3], tile => tile.xCoord == 2 && tile.yCoord == 5);

        foreach (Tile t in testPath)
        {
            print(t.printSelf());
        }
    }

    public delegate bool CheckTileDelegate(Tile tile);

    public List<Tile> GetPathToTile(Tile startTile, CheckTileDelegate checkCompleteAction)
    {
        Queue<Tile> queue = new Queue<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Dictionary<Tile, Tile> parentMap = new Dictionary<Tile, Tile>(); // Map each tile to its parent

        queue.Enqueue(startTile);
        visited.Add(startTile);
        parentMap[startTile] = null; // Start tile has no parent

        while (queue.Count > 0)
        {
            Tile currentTile = queue.Dequeue();

            // Check if the current tile is the target
            if (checkCompleteAction(currentTile))
            {
                return ReconstructPath(currentTile, parentMap);
            }

            // Enqueue all adjacent tiles
            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = currentTile; // Set the parent for path reconstruction
                }
            }
        }

        return new List<Tile>(); 
    }

    private List<Tile> ReconstructPath(Tile endTile, Dictionary<Tile, Tile?> parentMap)
    {
        List<Tile> path = new List<Tile>();
        Tile current = endTile;
        while (current != null)
        {
            path.Add(current);
            current = parentMap[current];
        }
        path.Reverse(); // The path is constructed backwards, so we need to reverse it
        return path;
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        List<(int, int)> neighborOffsets = new List<(int,int)> { (0,1), (0,-1), (0,1), (0,-1)};
        if (tile.yCoord % 2 == 0)
        {
            neighborOffsets.Add((-1, 1));
            neighborOffsets.Add((-1, -1));
        }
        else
        {
            neighborOffsets.Add((1, 1));
            neighborOffsets.Add((1, -1));
        }

        foreach ((int, int) offset in neighborOffsets)
        {
            if (isValidCoordOnBoard(tile.xCoord + offset.Item1, tile.yCoord + offset.Item2))
            {
                neighbors.Add(boardTiles[tile.xCoord + offset.Item1, tile.yCoord + offset.Item2]);
            }
        }
        return neighbors;
    }

    public bool isValidCoordOnBoard(int x, int y)
    {
        if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
        {
            return false;
        }
        return true;
    }
}