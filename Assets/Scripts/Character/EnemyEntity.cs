using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEntity : Entity {
    protected new void Start() {
        base.Start();
        team = Team.ENEMY;
        targetTeam = Team.PLAYER;
        GameManager.Instance.game.enemyUnits.Add(this);
    }

    public override void die() {
        GameManager.Instance.game.enemyUnits.Remove(this);
        base.die();
    }

}
