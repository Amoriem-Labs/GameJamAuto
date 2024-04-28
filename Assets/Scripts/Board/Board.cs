using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
