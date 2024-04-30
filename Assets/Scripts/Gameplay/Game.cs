using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum GameplayState { NOT_PLAYING, HERO_SETUP, BATTLING, CASTING };

    public GameplayState gameplayState = GameplayState.NOT_PLAYING;

    public float heroHealth = 100;
    public float heroMaxHealth = 100;
    public float heroShield = 30;

    public float currentMana = 0;
    public float maxMana = 100f;
    public float manaGain = 5f;

    public float turnMaxTime = 5f;
    public float currTurnTimer;
    public int drawNum = 5;

    public bool turnReady = false;

    public Tile hoveredTile;

    public List<BaseSpell> drawPile = new List<BaseSpell>();
    public List<BaseSpell> hand = new List<BaseSpell>();
    public List<BaseSpell> discardPile = new List<BaseSpell>();

    public delegate void DiscardSpell(BaseSpell spell);
    public static event DiscardSpell onDiscardSpell;

    public delegate void DrawSpell(BaseSpell spell);
    public static event DrawSpell onDrawSpell;

    public delegate void OnChangeState(GameplayState newState);
    public static event OnChangeState onChangeState;

    public delegate void OnChangeResource();
    public static event OnChangeResource onChangeResource;

    void Start()
    {
        if (!GameManager.Instance.game)
        {
            GameManager.Instance.game = this;    
        }

        maxMana = GameManager.Instance.maxMana;
        heroHealth = GameManager.Instance.heroHealth;
        heroMaxHealth = GameManager.Instance.heroMaxHealth;
        heroShield = heroMaxHealth * .3f;

        onChangeResource?.Invoke();

        currTurnTimer = 0;

        startBattle();
    }

    void Update()
    {
        if (gameplayState == GameplayState.HERO_SETUP)
        {
            changeGameplayState(GameplayState.BATTLING);
        }
        if (gameplayState == GameplayState.NOT_PLAYING || gameplayState == GameplayState.HERO_SETUP)
        {
            return;
        }
        currTurnTimer += Time.deltaTime;
        if (currTurnTimer >= turnMaxTime)
        {
            currTurnTimer = turnMaxTime;
            turnReady = true;
        }

        currentMana += Time.deltaTime * manaGain;
        if (currentMana >= maxMana)
        {
            currentMana = maxMana;
        }

        onChangeResource?.Invoke();

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

    private void changeGameplayState(GameplayState newState)
    {
        gameplayState = newState;
        onChangeState?.Invoke(gameplayState);
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
        changeGameplayState(GameplayState.HERO_SETUP);
        foreach (BaseSpell spell in GameManager.Instance.playerGrimoire)
        {
            drawPile.Add(spell.copy());
        }
        turnReady = false;
        currTurnTimer = 0;
        queueNextHand();
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
                if (drawPile.Count == 0)
                {
                    return;
                }
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
        turnReady = false;
        currTurnTimer = 0;
        queueNextHand();
    }
}
