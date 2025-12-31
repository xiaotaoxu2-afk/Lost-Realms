using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolstate = new SlimePatrolState();
        chasestate = new SlimeChaseState();
    }
}
