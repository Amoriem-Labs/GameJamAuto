using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseSpell : ICopyable<BaseSpell>
{
    public Game gameState;

    public Dictionary<string, float> statsDict = new Dictionary<string, float>();

    public string spellName = "Unnamed Spell";
    protected string description = "Undefined";

    public enum SpellType { NO_TARGET, SINGLE_TARGET, AOE, TARGET_ALLY};

    public SpellType spellType = SpellType.NO_TARGET;

    public BaseSpell()
    {
        statsDict.Add("manaCost", 0);
        gameState = GameManager.Instance.GetComponent<Game>();
        spellName = "Base Spell";
        description = "This is the base spell. You should not be seeing this >:(";
    }

    public abstract bool play();

    public abstract List<Tile> highlight(Tile tile);

    public void takeManaCost()
    {
        GameManager.Instance.game.currentMana -= statsDict["manaCost"];
    }

    public string getDescription()
    {
        return $"Mana Cost: {statsDict["manaCost"]:0}\n{description}";
    }

    public BaseSpell copy()
    {
        Type thisType = this.GetType();

        BaseSpell copy = (BaseSpell)Activator.CreateInstance(thisType);

        copy.spellName = this.spellName;
        copy.description = this.description;
        copy.spellType = this.spellType;

        copy.statsDict = new Dictionary<string, float>(this.statsDict);

        return copy;
    }

}
