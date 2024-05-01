using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;

    public Tile[,] boardTiles;

    public GameObject entityParent;

    public delegate void BoardAction();

    public static event BoardAction onBoardLoaded;

    void Awake()
    {
    }

    void Start()
    {
        if (!GameManager.Instance.board)
        {
            GameManager.Instance.board = this;
        }
        boardTiles = new Tile[boardWidth, boardHeight];

        // board tiles should populate themselves
        onBoardLoaded?.Invoke();

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (boardTiles[x, y] == null)
                {
                    Debug.LogError("Tile " + x + "," + y + "not loaded");
                }
            }
        }

        //List<Tile> testPath = GetPathToTile(boardTiles[3, 7], tile => tile.xCoord == 2 && tile.yCoord == 7);

        //foreach (Tile t in testPath)
        //{
        //    print(t.printSelf());   
        //}
    }

    public delegate bool CheckEntityDelegate(Entity entity);

    public List<Entity> getEntitiesOfTypeFromBoard(CheckEntityDelegate entityCheck) {
        List<Entity> entities = new List<Entity>();
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                if (entityCheck(boardTiles[x, y].currentOccupant)) {
                    entities.Add(boardTiles[x, y].currentOccupant);
                }
            }
        }
        return entities;
    }


    public delegate bool CheckTileDelegate(Tile tile);

    public List<Tile> getTilesOfTypeFromBoard(CheckTileDelegate tileCheck) {
        List<Tile> tiles = new List<Tile>();
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                if (tileCheck(boardTiles[x, y])) {
                    tiles.Add(boardTiles[x, y]);
                }
            }
        }
        return tiles;
    }

    public List<Tile> GetPathToTile(Tile startTile, CheckTileDelegate checkCompleteAction, bool ignoreTakenTiles = false)
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
                if (!visited.Contains(neighbor) && (!ignoreTakenTiles || neighbor.tileType == Tile.TileType.FREE || checkCompleteAction(neighbor)))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = currentTile; // Set the parent for path reconstruction
                }
            }
        }

        return new List<Tile>(); 
    }

    private List<Tile> ReconstructPath(Tile endTile, Dictionary<Tile, Tile> parentMap)
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
        List<(int, int)> neighborOffsets = new List<(int,int)> { (0,1), (0,-1), (1,0), (-1,0)};
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

    public List<Tile> getEmptyTileList() {
        List<Tile> tiles = new List<Tile>();
        for (int x = 0; x < boardWidth; x++) {
            for (int y = 0; y < boardHeight; y++) {
                if (boardTiles[x, y].currentOccupant == null && boardTiles[x, y].tileType == Tile.TileType.FREE) {
                    tiles.Add(boardTiles[x, y]);
                }
            }
        }
        return tiles;
    }

    public Tile getRandomEmptyTile() {
        List<Tile> emptyTiles = getEmptyTileList();
        if (emptyTiles.Count > 0) {
            System.Random rand = new System.Random();
            int randTile = rand.Next(emptyTiles.Count);
            return emptyTiles[randTile];
        }
        return null;
    }

    public static int[] convertToCube(Tile t) {
        int q = t.xCoord - (t.yCoord - (t.yCoord & 1)) / 2;
        int r = t.yCoord;
        return new int[3] { q, r, -q - r };
    }

    public static int[] cubeSubtract(int[] a, int[] b) {
        return new int[3] { a[0] - b[0], a[1] - b[1], a[2] - b[2] };
    }

    public static int getDistBetweenTiles(Tile t1, Tile t2) {
        if (t1 == null || t2 == null) {
            return 0;
        }
        int[] t1_conv = convertToCube(t1);
        int[] t2_conv = convertToCube(t2);

        int[] sub = cubeSubtract(t1_conv, t2_conv);
        return (Mathf.Abs(sub[0]) + Mathf.Abs(sub[1]) + Mathf.Abs(sub[2])) / 2;
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