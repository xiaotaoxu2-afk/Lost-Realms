using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CE : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolstate = new CEPatrolState();
    }
}