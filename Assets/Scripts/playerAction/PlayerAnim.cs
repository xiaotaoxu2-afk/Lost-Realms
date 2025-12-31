using System;

using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    private PhysicsCheck physicsCheck;
    private PlayerControl playerControl;
    private Rigidbody2D rb;
    private PlayerSkill skill;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerControl = GetComponent<PlayerControl>();
        skill = GetComponent<PlayerSkill>();
    }


    private void Update() => SetAnimation();

    private void SetAnimation()
    {
        anim.SetFloat("isRun", Math.Abs(rb.velocity.x));
        anim.SetBool("isjumping", physicsCheck.isGround);
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isDeath", playerControl.isDead);
        anim.SetBool("Attack", playerControl.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerControl.isSlide);
        anim.SetBool("isSword", playerControl.isSword);
        anim.SetBool("isShiled", playerControl.isShiled);
        anim.SetBool("isBlocking", playerControl.isBlock);
        anim.SetBool("isSainHeal", skill.isSain);
        anim.SetBool("isHolyHeal", skill.isHolyHeal);
        anim.SetBool("isHolySlash", skill.isHolySlash);
        anim.SetBool("isLightCut", skill.isLightCut);
    }


    public void playerHurt()
    {
        anim.SetTrigger("isHurt");
        if (anim.GetBool("isBlocking"))
        {
            anim.SetTrigger("isBlock"); // 额外触发
        }
    }

    public void playerAttack() => anim.SetTrigger("isAttack");
}
