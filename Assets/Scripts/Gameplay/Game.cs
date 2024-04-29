using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum GameplayState { NOT_PLAYING, HERO_SETUP, BATTLING, CASTING };

    public GameplayState gameplayState = GameplayState.NOT_PLAYING;

    public float currentMana = 0;
    public float maxMana;

    public int drawNum = 5;

    public Tile hoveredTile;

    public List<BaseSpell> drawPile = new List<BaseSpell>();
    public List<BaseSpell> hand = new List<BaseSpell>();
    public List<BaseSpell> discardPile = new List<BaseSpell>();

    public delegate void DiscardSpell(BaseSpell spell);
    public static event DiscardSpell onDiscardSpell;

    public delegate void DrawSpell(BaseSpell spell);
    public static event DrawSpell onDrawSpell;

    public delegate void OnChangeState(GameplayState newState);
    public static event OnChangeState onChangeStae;

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

        return team ?? Character.Team.Neutral;
    }

    public void startBattle()
    {
        foreach (BaseSpell spell in GameManager.Instance.playerGrimoire)
        {
            drawPile.Add(spell.copy());
        }
    }

    public void shuffleDiscard()
    {
        foreach (BaseSpell spell in discardPile)
        {
            drawPile.Add(spell);
        }

        drawPile.Shuffle();

        discardPile.Clear();
    }

    public void discardSpell(BaseSpell spell)
    {
        discardPile.Add(spell);
        hand.Remove(spell);
        onDiscardSpell?.Invoke(spell);
    }

    public void drawSpells(int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (drawPile.Count <= 0)
            {
                shuffleDiscard();
            }
            BaseSpell spell = drawPile[0];
            drawPile.RemoveAt(0);
            hand.Add(spell);

            onDrawSpell?.Invoke(spell);
        }
    }

    public void queueNextHand()
    {
        foreach (BaseSpell spell in hand)
        {
            discardPile.Add(spell);
        }

        hand.Clear();

        drawSpells(drawNum);
    }

    public void endTurn()
    {
        for (int i = hand.Count - 1; i > -1; i--)
        {
            discardSpell(hand[i]);
        }
        queueNextHand();
    }
}
