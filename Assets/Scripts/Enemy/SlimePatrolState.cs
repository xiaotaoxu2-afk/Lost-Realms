using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;

        currentenemy.currentSpeed = currentenemy.walkSpeed;

        Debug.Log("进入巡逻状态");
    }

    public override void LogicUpdate()
    {
        if (currentenemy.FoundPlayer())
        {
            currentenemy.stateSwitch(EnemyState.chase);
        }
        if (!currentenemy.physicscheck.isGround || (currentenemy.physicscheck.isLeftground && currentenemy.faceDir.x < 0) || (currentenemy.physicscheck.isRightground && currentenemy.faceDir.x > 0))
        {
            // 转身：翻转 faceDir 并改变朝向
            currentenemy.faceDir = new Vector3(-currentenemy.faceDir.x, 0, 0);
            currentenemy.transform.localScale = new Vector3(-currentenemy.transform.localScale.x, currentenemy.transform.localScale.y, currentenemy.transform.localScale.z);

            currentenemy.anim.SetBool("isWalk", false);
        }
        else
        {
            currentenemy.wait = false;
            currentenemy.anim.SetBool("isWalk", true);
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentenemy.anim.SetBool("isWalk", false);
        Debug.Log("退出");
    }
}