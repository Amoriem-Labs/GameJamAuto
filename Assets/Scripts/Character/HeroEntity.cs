using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroEntity : PlayerEntity
{

    protected new void Start() {
        base.Start();
        playerCharType = PlayerCharType.HERO;
    }
}
