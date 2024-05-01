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
        spellType = SpellType.TARGET_ENEMY;
    }
    public override bool play()
    {
        Entity selected = GameManager.Instance.game.getSelectedCharacter();
        Entity.Team team = GameManager.Instance.game.getSelectedTeam();

        if (team == Entity.Team.ENEMY && selected != null) // basically, if not null and on enemy team, then pass, otherwise fail
        {
            selected.TakeDamage(statsDict["damage"]);
            return true;
        }
        return false;
    }
    public override List<Tile> highlight()
    {
        //Debug.Log($"{GameManager.Instance.game.hoveredTile == null} | {GameManager.Instance.game.hoveredTile.currentOccupant}");
        if (GameManager.Instance.game.hoveredTile != null && GameManager.Instance.game.hoveredTile.currentOccupant?.team == Entity.Team.ENEMY)
        {
            return new List<Tile> { GameManager.Instance.game.hoveredTile };
        }
        return new List<Tile> { };
    }
}
