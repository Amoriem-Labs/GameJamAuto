using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xCoord;
    public int yCoord;

    private void OnEnable()
    {
        Board.onBoardLoaded += addSelfToBoard;
    }

    private void OnDisable()
    {
        Board.onBoardLoaded -= addSelfToBoard;
    }

    private void addSelfToBoard()
    {
        GameManager.Instance.board.boardTiles[xCoord, yCoord] = this;
    }

    public string printSelf()
    {
        return "(" + xCoord + ", " + yCoord + ")";
    }
}
