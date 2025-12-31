using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;

    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        //Debug.Log("chase");
        currentenemy.currentSpeed = currentenemy.pursuitSpeed;
        currentenemy.anim.SetBool("isRun", true);
        attack = currentenemy.GetComponent<Attack>();
    }

    public override void LogicUpdate()
    {
        if (currentenemy.lostCounter <= 0)
        {
            currentenemy.stateSwitch(EnemyState.patrol);
        }
        if (!currentenemy.physicscheck.isGround || (currentenemy.physicscheck.isLeftground && currentenemy.faceDir.x < 0) || (currentenemy.physicscheck.isRightground && currentenemy.faceDir.x > 0))
        {
            currentenemy.transform.localScale = new Vector3(currentenemy.faceDir.x, (float)4.5, (float)4.5);
        }

        target = new Vector3(currentenemy.attacker.position.x, currentenemy.transform.position.y);

        //判断攻击距离
        if (Mathf.Abs(target.x - currentenemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentenemy.transform.position.y) <= attack.attackRange)
        {
            currentenemy.Attack = true;
            if (!currentenemy.isHurt)
                currentenemy.rb.velocity = Vector2.zero;

            currentenemy.attackRateCounter -= Time.deltaTime;
            if (currentenemy.attackRateCounter <= 0)
            {
                currentenemy.attackRateCounter = attack.attackRate;
                currentenemy.anim.SetTrigger("isAttack");
            }
        }
        else
        {
            currentenemy.Attack = false;
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentenemy.anim.SetBool("isRun", false);
    }
}