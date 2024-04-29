using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpell : MonoBehaviour
{
    public Game gameState;

    public float manaCost = 0;
    public string spellName = "Unnamed Spell";
    public string description = "Undefined";

    protected virtual void Start()
    {
        manaCost = 10;
        gameState = GameManager.Instance.GetComponent<Game>();
        spellName = "Base Spell";
        description = "This is the base spell. You should not be seeing this >:(";
    }

    public abstract bool play();

    public abstract List<Tile> highlight(Tile tile);

    public void takeManaCost()
    {
        GameManager.Instance.game.currentMana -= manaCost;
    }

    public string getDescription()
    {
        return "Mana Cost: " + manaCost.ToString("0") + "\n" + description;
    }
}
