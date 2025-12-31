using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Flail_PatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentenemy = enemy;
        currentenemy.currentSpeed = currentenemy.walkSpeed;
         
    }
    public override void LogicUpdate()
    {
        if (currentenemy.FoundPlayer())
        {
            currentenemy.stateSwitch(EnemyState.chase);
        }

        if (!currentenemy.physicscheck.isGround || (currentenemy.physicscheck.isLeftground && currentenemy.faceDir.x < 0) || (currentenemy.physicscheck.isRightground && currentenemy.faceDir.x > 0))
        {
            currentenemy.wait = true;

            currentenemy.anim.SetBool("isWalk", false);
        }
        else
        {
            currentenemy.anim.SetBool("isWalk", true);
        }
    }



    public override void PhysicsUpdate()
    {
        
    }
    public override void OnExit()
    {
        currentenemy.anim.SetBool("isWalk", false);
        Debug.Log("ÍË³ö");
    }
}
