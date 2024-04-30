using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestSpellTwo : BaseSpell {

    public TestSpellTwo() : base() {
        statsDict["manaCost"] = 40;
        statsDict["shield"] = 300;
        spellName = "Test Spell Two";
        description = "This is a test spell. Shields hero for <shield> shielding";
        spellType = SpellType.NO_TARGET;
    }
    public override bool play() {
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
    public override List<Tile> highlight(Tile tile) {
        return null;
    }
}
