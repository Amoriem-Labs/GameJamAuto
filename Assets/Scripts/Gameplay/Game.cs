using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum GameplayState { NOT_PLAYING, HERO_SETUP, BATTLING, CASTING };

    public GameplayState gameplayState = GameplayState.NOT_PLAYING;

    public float currentMana = 0;
    public float maxMana;

    public Tile hoveredTile;

    void Start()
    {
        if (!GameManager.Instance.game)
        {
            GameManager.Instance.game = this;    
        }
    }

    void Update()
    {
        Tile hoveredTile = GetHoveredTile();

        hoveredTile?.setHover();

        for (int x = 0; x < GameManager.Instance.board.boardWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.board.boardHeight; y++)
            {
                Tile boardTile = GameManager.Instance.board.boardTiles[x, y];
                if (boardTile.hovered && (hoveredTile == null || !hoveredTile.equals(boardTile)))
                {
                    boardTile.hovered = false;
                    boardTile.clearHover();
                }
            }
        }
    }

    private Tile GetHoveredTile()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("TileBody"))
            {
                return hit.collider.gameObject.transform.parent.parent.GetComponent<Tile>();
            }
        }
        return null;
    }

    public Character getSelectedCharacter()
    {
        return hoveredTile?.currentOccupant;
    }

    public Character.Team getSelectedTeam()
    {
        Character selected = getSelectedCharacter();
        Character.Team? team = selected?.team;

        return team ?? Character.Team.Enemy;
    }

    public void startBattle()
    {

    }
}
