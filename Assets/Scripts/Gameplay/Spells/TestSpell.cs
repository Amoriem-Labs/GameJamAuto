using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpell : BaseSpell
{

    protected override void Start()
    {
        base.Start();
        statsDict["manaCost"] = 30;
        statsDict["damage"] = 100;
        spellName = "Test Spell";
        description = "This is a test spell. Deals <damage> damage to an enemy";
        spellType = SpellType.SINGLE_TARGET;
    }
    public override bool play()
    {
        Character selected = GameManager.Instance.game.getSelectedCharacter();
        Character.Team team = GameManager.Instance.game.getSelectedTeam();

        if (team == Character.Team.Enemy && selected != null) // basically, if not null and on enemy team, then pass, otherwise fail
        {
            takeManaCost();
            // deal damage to character
            return true;
        }*/
        return false;
    }
    public override List<Tile> highlight(Tile tile)
    {
        if (GameManager.Instance.game.hoveredTile != null)
        {
            return new List<Tile> { GameManager.Instance.game.hoveredTile };
        }
        return null;
    }
}
