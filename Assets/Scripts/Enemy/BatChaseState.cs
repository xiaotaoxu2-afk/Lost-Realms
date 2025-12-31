using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private bool isAttack;
    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        currentenemy.currentSpeed = currentenemy.pursuitSpeed;
        attack = currentenemy.GetComponent<Attack>();
        currentenemy.anim.SetBool("isRun", true);
        currentenemy.lostCounter = currentenemy.lostTime;
        currentenemy.attackRateCounter = attack.attackRate;
    }
    public override void LogicUpdate()
    {
        if (currentenemy.lostCounter <= 0)
            currentenemy.stateSwitch(EnemyState.patrol);

        target = new Vector3(currentenemy.attacker.position.x, currentenemy.attacker.position.y,0);

        //¹¥»÷¾àÀëÅÐ¶Ï
        if (Mathf.Abs(target.x - currentenemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentenemy.transform.position.y) <= attack.attackRange)
        {
            isAttack = true;
            if(!currentenemy.isHurt)
                currentenemy.rb.velocity = Vector2.zero;

            //¹¥»÷¼ä¸ô¼ÆÊ±
            currentenemy.attackRateCounter -= Time.deltaTime;
            if(currentenemy.attackRateCounter <= 0)
            {
                currentenemy.attackRateCounter = attack.attackRate;
                currentenemy.anim.SetTrigger("isAttack");
            }

        }
        else
        {
            isAttack = false;

        }

        moveDir = (target - currentenemy.transform.position).normalized;

        if (moveDir.x > 0)
        {
            currentenemy.transform.localScale = new Vector3((float)-3.5, (float)3.5, (float)3.5);
        }
        if (moveDir.x < 0)
        {
            currentenemy.transform.localScale = new Vector3((float)3.5, (float)3.5, (float)3.5);
        }

    }

    public override void PhysicsUpdate()
    {
        if (!currentenemy.isHurt && !currentenemy.isDeath && !isAttack)
        {
            currentenemy.rb.velocity = moveDir * currentenemy.currentSpeed * Time.deltaTime;
            
        }
    }

    public override void OnExit()
    {
        currentenemy.anim.SetBool("isRun", false);
    }

}
