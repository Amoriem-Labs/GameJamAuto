using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharOne : HeroEntity {
    protected new void Start() {
        maxHealth = 1000f;
        attackDamage = 100f;
        shield = 300f;
        base.Start();
    }

    public override void tick() {
        base.tick();
    }

    public override void doAbility() {
        if (target != null) {
            attack(target, 100f);
        }
    }
}
