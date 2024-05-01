using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestSpellTwo : BaseSpell {

    public TestSpellTwo() : base() {
        statsDict["manaCost"] = 40;
        statsDict["shield"] = 300;
        spellName = "Test Spell Two";
        description = "This is a test spell. Shields all heroes for <shield> shielding";
        spellType = SpellType.NO_TARGET;
    }
    public override bool play() {
        List<Entity> heroes = GameManager.Instance.board.getEntitiesOfTypeFromBoard(entity => entity is HeroEntity);

        foreach (Entity hero in heroes) {
            hero?.AddShield(statsDict["shield"]);
        }
        return true;
    }
    public override List<Tile> highlight() {
        return GameManager.Instance.board.getTilesOfTypeFromBoard(tile => tile.currentOccupant is HeroEntity);
    }
}
