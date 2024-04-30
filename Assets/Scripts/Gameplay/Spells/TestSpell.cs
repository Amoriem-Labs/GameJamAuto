using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TestSpell : BaseSpell
{

    public TestSpell() : base() {
        statsDict["manaCost"] = 30;
        statsDict["damage"] = 100;
        spellName = "Test Spell";
        description = "This is a test spell. Deals <damage> damage to an enemy";
        spellType = SpellType.SINGLE_TARGET;
    }
    public override bool play()
    {
        Entity selected = GameManager.Instance.game.getSelectedCharacter();
        Entity.Team team = GameManager.Instance.game.getSelectedTeam();

        if (team == Entity.Team.ENEMY && selected != null) // basically, if not null and on enemy team, then pass, otherwise fail
        {
            takeManaCost();
            selected.TakeDamage(statsDict["damage"]);
            return true;
        }
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
