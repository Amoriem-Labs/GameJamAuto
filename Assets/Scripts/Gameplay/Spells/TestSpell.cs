using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpell : BaseSpell
{

    protected override void Start()
    {
        base.Start();
        manaCost = 30;
        spellName = "Test Spell";
        description = "This is a test spell. Deals 100 damage to an enemy";
    }
    public override bool play()
    {
        return true;
    }
    public override List<Tile> highlight(Tile tile)
    {
        return null;
    }
}
