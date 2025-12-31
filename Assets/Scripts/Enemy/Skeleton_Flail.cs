using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Flail : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolstate = new Skeleton_Flail_PatrolState();
        chasestate = new Skeleton_Flail_ChaseState();
    }

}
