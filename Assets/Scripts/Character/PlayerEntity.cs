using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEntity : Entity
{
    public enum PlayerCharType { SUMMON, HERO };

    public PlayerCharType playerCharType;
    protected new void Start() {
        base.Start();
        team = Team.PLAYER;
        targetTeam = Team.ENEMY;
        playerCharType = PlayerCharType.SUMMON;
        GameManager.Instance.game.playerUnits.Add(this);
    }

    public override void die() {
        GameManager.Instance.game.playerUnits.Remove(this);
        base.die();
    }
}
