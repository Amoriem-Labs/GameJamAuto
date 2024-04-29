using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock propBlock;
    private Color originalColor;
    private Color highlightColor = Color.yellow;

    public GameObject body;

    public bool hovered = false;

    public int xCoord;
    public int yCoord;

    public enum TileType { FREE, OCCUPIED, RESERVED};

    public TileType tileType = TileType.FREE;

    public Character currentOccupant = null;

    void Awake()
    {
        _renderer = body.GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(propBlock);
        originalColor = Color.white;
    }
    private void OnEnable()
    {
        Board.onBoardLoaded += addSelfToBoard;
    }

    private void OnDisable()
    {
        Board.onBoardLoaded -= addSelfToBoard;
    }

    public void setHover()
    {
        if (!hovered)
        {
            hovered = true;
            highlight();
        }
    }

    public void clearHover()
    {
        hovered = false;
        clearHighlight();
    }

    public void highlight()
    {
        if (_renderer == null)
        {
            return;
        }
        _renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", highlightColor);
        _renderer.SetPropertyBlock(propBlock);
    }

    public void clearHighlight()
    {
        if (_renderer == null)
        {
            return;
        }
        _renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", originalColor);
        _renderer.SetPropertyBlock(propBlock);
    }

    private void addSelfToBoard()
    {
        GameManager.Instance.board.boardTiles[xCoord, yCoord] = this;
    }

    public string printSelf()
    {
        return "(" + xCoord + ", " + yCoord + ")";
    }

    public bool equals(Tile otherTile)
    {
        return otherTile.xCoord == xCoord && otherTile.yCoord == yCoord;
    }
}
