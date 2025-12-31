using UnityEngine;

namespace Enemys
{
    public class FRChaseState : BaseState
    {
        private Attack attack;
        private bool isAttack;
        private Vector3 moveDir;
        private Vector3 target;

        public override void OnEnter(Enemy enemy)
        {
            currentenemy.currentSpeed = currentenemy.pursuitSpeed;
            attack = currentenemy.GetComponent<Attack>();
            currentenemy.anim.SetBool("isRun", true);
            currentenemy.lostCounter = currentenemy.lostTime;
            currentenemy.attackRateCounter = attack.attackRate;
        }

        public override void LogicUpdate()
        {
            if (currentenemy.lostCounter <= 0) currentenemy.stateSwitch(EnemyState.patrol);
            target = new Vector3(currentenemy.attacker.position.x, currentenemy.attacker.position.y, 0);
            if (Mathf.Abs(target.x - currentenemy.attacker.position.x) < attack.attackRange &&
                Mathf.Abs(target.y - currentenemy.attacker.position.y) < attack.attackRange)
            {
                isAttack = true;
                if (!currentenemy.isHurt) currentenemy.rb.velocity = Vector3.zero;
                currentenemy.attackRateCounter += Time.deltaTime;
                if (currentenemy.attackRateCounter <= 0)
                {
                    currentenemy.attackRateCounter = attack.attackRange;
                    currentenemy.anim.SetTrigger("isAttack");
                }
            }
            else
            {
                isAttack = false;
            }

            moveDir = (target - currentenemy.transform.position).normalized;

            if (moveDir.x > 0) currentenemy.transform.localScale = new Vector3((float)-4.5, (float)4.5, (float)4.5);
            if (moveDir.x < 0) currentenemy.transform.localScale = new Vector3((float)4.5, (float)4.5, (float)4.5);
        }

        public override void PhysicsUpdate()
        {
            if (!currentenemy.isHurt && !currentenemy.isDeath && !isAttack)
                currentenemy.rb.velocity = moveDir * Time.deltaTime * currentenemy.currentSpeed;
        }

        public override void OnExit()
        {
            currentenemy.anim.SetBool("isRun", false);
        }
    }
}