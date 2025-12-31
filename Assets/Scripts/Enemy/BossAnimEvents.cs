using UnityEngine;

/// <summary>
///     所有 Animation Event 都从这里进，保证 Attack3 严格顺序 & hitbox 按帧开关
/// </summary>
public class BossAnimEvents : MonoBehaviour
{
    public BossController boss;

    private void Awake()
    {
        if (boss == null)
        {
            boss = GetComponent<BossController>();
        }
    }

    // ===== Attack1 hitbox =====
    public void AE_Attack1_HitboxOn()
    {
        if (boss && boss.hitboxAttack1)
        {
            boss.hitboxAttack1.SetActive(true);
        }
    }

    public void AE_Attack1_HitboxOff()
    {
        if (boss && boss.hitboxAttack1)
        {
            boss.hitboxAttack1.SetActive(false);
        }
    }

    // ===== Attack2 hitbox =====
    public void AE_Attack2_HitboxOn()
    {
        if (boss && boss.hitboxAttack2)
        {
            boss.hitboxAttack2.SetActive(true);
        }
    }

    public void AE_Attack2_HitboxOff()
    {
        if (boss && boss.hitboxAttack2)
        {
            boss.hitboxAttack2.SetActive(false);
        }
    }

    // ===== Attack3 hitbox =====
    public void AE_Attack3_HitboxOn()
    {
        if (boss && boss.hitboxAttack3)
        {
            boss.hitboxAttack3.SetActive(true);
        }
    }

    public void AE_Attack3_HitboxOff()
    {
        if (boss && boss.hitboxAttack3)
        {
            boss.hitboxAttack3.SetActive(false);
        }
    }

    // ===== Attack3 严格三段顺序（关键）=====
    public void AE_Attack3_StartFinished()
    {
        if (boss)
        {
            boss.DoAttack3_ToIng();
        }
    }

    public void AE_Attack3_IngFinished()
    {
        if (boss)
        {
            boss.DoAttack3_ToEnd();
        }
    }

    public void AE_Attack3_EndFinished()
    {
        if (boss)
        {
            boss.FinishAttack();
        }
    }

    // ===== Hit 结束 =====
    public void AE_HitFinished()
    {
        if (boss)
        {
            boss.FinishHit();
        }
    }

    // ===== Attack1/2 动画结束 =====
    public void AE_AttackFinished()
    {
        if (boss)
        {
            boss.FinishAttack();
        }
    }
}
