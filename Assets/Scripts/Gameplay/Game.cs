using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public List<EnemyEntity> enemyUnits = new List<EnemyEntity>();

    public List<PlayerEntity> playerUnits = new List<PlayerEntity>();

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

    public delegate void OnControlTooltip(bool active, string text = "", string body = "");
    public static event OnControlTooltip onControlTooltip;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            print("a");
            foreach (BaseSpell spell in drawPile) {
                print($"draw: {spell.spellName}");
            }
            foreach (BaseSpell spell in hand) {
                print($"hand: {spell.spellName}");
            }
            foreach (BaseSpell spell in discardPile) {
                print($"discard: {spell.spellName}");
            }
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            print("b");
            if (gameplayState == GameplayState.NOT_PLAYING) {
                startBattle();
            }
            createEntity(GameManager.EntityTypes.TEST_CHAR_ONE);
            createEntity(GameManager.EntityTypes.TEST_ENEMY_ONE);
        }

        switch (gameplayState) {
            case GameplayState.NOT_PLAYING:
                break;
            case GameplayState.HERO_SETUP:
            changeGameplayState(GameplayState.BATTLING);
                break;
            case GameplayState.CASTING:
                break;
            case GameplayState.BATTLING:
                for (int i = playerUnits.Count - 1; i > -1; i--) {
                    playerUnits[i].tick();
                }
                for (int i = enemyUnits.Count - 1; i > -1; i--) {
                    enemyUnits[i].tick();
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
                break;
        }

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

    public Entity getSelectedCharacter()
    {
        return hoveredTile?.currentOccupant;
    }

    public Entity.Team getSelectedTeam()
    {
        Entity selected = getSelectedCharacter();
        Entity.Team? team = selected?.team;

        return team ?? Entity.Team.NULL;
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

    public void createEntity(GameManager.EntityTypes entityType, Tile tile = null) {
        if (tile == null) {
            tile = GameManager.Instance.board.getRandomEmptyTile();
        }

        GameObject newEntity = Instantiate(GameManager.Instance.entityObjectDict[entityType].gameObject, tile.transform.position, Quaternion.identity);
        newEntity.transform.parent = GameManager.Instance.board.entityParent.transform;
        newEntity.GetComponent<Entity>().commence(tile);
    }

    public static void tooltipControl(bool _active, string _title = "", string _body = "") {
        onControlTooltip?.Invoke(_active, _title, _body);
    }
}
