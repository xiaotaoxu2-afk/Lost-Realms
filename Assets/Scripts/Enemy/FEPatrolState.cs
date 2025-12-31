using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FEPatrolState : BaseState
{
    private Vector3 target;
    private Vector3 moveDir;

    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        currentenemy.currentSpeed = currentenemy.walkSpeed;
        target = currentenemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentenemy.FoundPlayer())
        {
            currentenemy.stateSwitch(EnemyState.chase);
        }

        if (Mathf.Abs(target.x - currentenemy.transform.position.x) < 0.1f && Mathf.Abs(target.y - currentenemy.transform.position.y) < 0.1f)
        {
            currentenemy.wait = true;
            target = currentenemy.GetNewPoint();
        }

        moveDir = (target - currentenemy.transform.position).normalized;

        if (moveDir.x > 0)
        {
            currentenemy.transform.localScale = new Vector3((float)-4.5, (float)4.5, (float)4.5);
        }
        if (moveDir.x < 0)
        {
            currentenemy.transform.localScale = new Vector3((float)4.5, (float)4.5, (float)4.5);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!currentenemy.isHurt && !currentenemy.isDeath && !currentenemy.wait)
        {
            currentenemy.rb.velocity = moveDir * currentenemy.currentSpeed * Time.deltaTime;
            currentenemy.anim.SetBool("isWalk", true);
        }
        else
        {
            currentenemy.rb.velocity = Vector3.zero;
            currentenemy.anim.SetBool("isWalk", false);
        }
    }

    public override void OnExit()
    {
        currentenemy.anim.SetBool("isWalk", false);
    }
}