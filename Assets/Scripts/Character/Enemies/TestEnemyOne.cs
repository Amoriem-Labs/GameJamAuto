using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyOne : EnemyEntity {
    protected new void Start() {
        maxHealth = 1000f;
        attackDamage = 100f;
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
