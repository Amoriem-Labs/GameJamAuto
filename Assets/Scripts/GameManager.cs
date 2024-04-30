using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public delegate void HeroStateUpdate();

    public static event HeroStateUpdate onHeroStateUpdated;

    public List<GameObject> allSpells = new List<GameObject>();

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
    }

    void Start()
    {
        startGame();
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

        for (int i = 0; i < 10; i++)
        {
            playerGrimoire.Add(new TestSpell());
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
