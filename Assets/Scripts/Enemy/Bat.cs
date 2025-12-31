using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [Header("移动范围")]
    public float parrolRadius;

    protected override void Awake()
    {
        base.Awake();
        patrolstate = new BatPatrolState();
        chasestate = new BatChaseState();

    }

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkdistance, checklayer);
        if (obj)
        {
            attacker = obj.transform;
        }
        return obj;
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkdistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, parrolRadius);

    }

    public override Vector3 GetNewPoint()
    {
        var targetx = Random.Range(-parrolRadius, parrolRadius);
        var targety = Random.Range(-parrolRadius, parrolRadius);

        return spwanPoint + new Vector3(targetx, targety);
    }

    public override void Move()
    {
        
    }
}
