using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public delegate void HeroStateUpdate();

    public static event HeroStateUpdate onHeroStateUpdated;

    [HideInInspector]
    public Board board;
    [HideInInspector]
    public Game game;

    public bool battleOngoing = true;
    public int coins;

    public float heroHealth = 100f;
    public float heroMaxHealth = 100f;
    public float maxMana = 100f;

    public List<BaseSpell> playerGrimoire = new List<BaseSpell>();

    public enum EntityTypes { TEST_CHAR_ONE, TEST_ENEMY_ONE };

    [SerializeField]
    private List<EntityEnumMatch> entityDictSetup = new List<EntityEnumMatch>();
    public Dictionary<EntityTypes, Entity> entityObjectDict = new Dictionary<EntityTypes, Entity>();
    public enum SpellTypes { TEST_SPELL_ONE, TEST_SPELL_TWO };

    public Dictionary<SpellTypes, Type> spellDict = new Dictionary<SpellTypes, Type> {
        { SpellTypes.TEST_SPELL_ONE, typeof(TestSpell) },
        { SpellTypes.TEST_SPELL_TWO, typeof(TestSpellTwo) }
    };

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }

        foreach (EntityEnumMatch match in entityDictSetup) {
            entityObjectDict.Add(match.entityName, match.entityObject.GetComponent<Entity>());
        }

        startGame();
    }

    void Start()
    {
    }

    public void WinRound(){
        if (battleOngoing == true){
            battleOngoing = false;
        }
    }

    public void LoseRound(){
        if (battleOngoing == true){
            battleOngoing = false;
        }
    }
    
    // for now just implement a base deck, maybe expand upon this some other time
    public void startGame()
    {
        updateHeroMaxHealth(1000);
        updateHeroHealth(heroMaxHealth);

        for (int i = 0; i < 5; i++) { 

            playerGrimoire.Add(new TestSpell());
            playerGrimoire.Add(new TestSpellTwo());
        }
    }

    public void updateHeroHealth(float _health)
    {
        heroHealth = _health;
        onHeroStateUpdated?.Invoke();
    }

    public void updateHeroMaxHealth(float _maxHealth)
    {
        heroMaxHealth = _maxHealth;
        onHeroStateUpdated?.Invoke();
    }
}
