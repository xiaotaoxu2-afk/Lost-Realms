using System.Collections;

using UnityEngine;

public class BossVagabondAI : MonoBehaviour
{
    [Header("核心组件")] public Transform player;

    public Transform attackPoint;
    public LayerMask playerLayer;

    [Header("属性设置")] public float moveSpeed = 3.5f;

    public float maxHealth = 100f;
    public float attackRange = 1.5f;
    public float sightRange = 10.0f;
    public float attackCooldown = 1.5f;

    [Header("伤害数值")] public float normalDamage = 10f;

    public float heavyDamage = 25f;
    public float jumpDamage = 15f;
    public float attackRadius = 0.8f;

    private Animator anim;
    private bool canAttack = true;
    private float currentHealth;
    private BossState currentState = BossState.Idle;
    private bool isFacingRight = true;
    private Rigidbody2D rb;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // 自动寻找挂载了 PlayerControl 的对象
        if (player == null)
        {
            PlayerControl pObj = FindObjectOfType<PlayerControl>();
            if (pObj != null)
            {
                player = pObj.transform;
            }
        }


        // 新增：延迟自动查找 attacker（最多 5 帧，确保玩家加载）
        StartCoroutine(DelayedFindPlayer(5));
    }

    private void Update()
    {
        if (currentState == BossState.Dead || currentState == BossState.Hurt || player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Idle:
                LogicIdle(distanceToPlayer);
                break;
            case BossState.Chasing:
                LogicChase(distanceToPlayer);
                break;
            case BossState.Attacking:
            case BossState.Blocking:
                rb.velocity = Vector2.zero;
                break;
        }

        if (currentState != BossState.Attacking && currentState != BossState.Blocking)
        {
            LookAtPlayer();
        }
    }

    private IEnumerator DelayedFindPlayer(int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            // 如果已经有 player 引用，直接退出
            if (player != null)
            {
                Debug.Log("Boss: player 已经赋值，无需查找");
                yield break;
            }

            // 尝试查找玩家
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                Debug.Log("Boss: player 自动赋值成功 - " + player.name);
                yield break;
            }

            yield return null; // 等待一帧
        }

        Debug.LogError("未找到 Tag 为 'Player' 的对象，请手动赋值 player 或检查玩家 Tag。");
    }

    // 状态枚举
    private enum BossState
    {
        Idle,
        Chasing,
        Attacking,
        Blocking,
        Hurt,
        Dead
    }

    #region 状态逻辑

    private void LogicIdle(float distance)
    {
        anim.SetBool("isMoving", false);
        if (distance < sightRange)
        {
            if (Random.value < 0.05f)
            {
                PerformBlock();
            }
            else
            {
                currentState = BossState.Chasing;
                anim.SetBool("isMoving", true); // 新增：立即播放跑步动画
            }
        }
    }

    private void LogicChase(float distance)
    {
        if (distance > attackRange)
        {
            anim.SetBool("isMoving", true);
            var targetPos = new Vector2(player.position.x, rb.position.y);
            var newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            anim.SetBool("isMoving", false);
            currentState = BossState.Attacking;
            if (canAttack)
            {
                StartCoroutine(ChooseAttackPattern());
            }
        }
    }

    #endregion

    #region 战斗系统

    private IEnumerator ChooseAttackPattern()
    {
        canAttack = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);

        float rand = Random.value;

        if (rand < 0.4f)
        {
            anim.SetTrigger("tAttack");
        }
        else if (rand < 0.7f)
        {
            anim.SetTrigger("tHeavyAttack");
        }
        else if (rand < 0.9f)
        {
            StartCoroutine(PerformJumpAttackSequence());
        }
        else
        {
            PerformBlock();
        }

        yield return new WaitForSeconds(attackCooldown);

        if (currentState != BossState.Dead)
        {
            currentState = BossState.Idle;
            canAttack = true;
        }
    }

    private IEnumerator PerformJumpAttackSequence()
    {
        anim.SetTrigger("tJump");
        rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("tJumpAttack");
        Vector2 diveDir = (isFacingRight ? Vector2.right : Vector2.left) + Vector2.down;
        rb.velocity = diveDir * 5f;
    }

    private void PerformBlock()
    {
        currentState = BossState.Blocking;
        anim.SetTrigger("tBlock");
        StartCoroutine(ResetStateAfterDelay(1.0f));
    }

    private IEnumerator ResetStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentState != BossState.Dead)
        {
            currentState = BossState.Idle;
        }
    }

    #endregion

    #region 关键：伤害判定与玩家交互

    // 动画事件调用此方法：Parameter 填 0, 1, 或 2
    public void DealDamage(int attackType)
    {
        float damageAmount = normalDamage;
        if (attackType == 1)
        {
            damageAmount = heavyDamage;
        }

        if (attackType == 2)
        {
            damageAmount = jumpDamage;
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);

        if (hitPlayer != null)
        {
            // 1. 获取 PlayerControl 处理物理击退
            PlayerControl pc = hitPlayer.GetComponent<PlayerControl>();
            if (pc != null)
            {
                // 注意：这里我们传递 Boss 的 transform 作为攻击源，让玩家知道往哪个方向击退
                pc.getHurt(transform);
            }

            // 2. 获取 Property 处理血量扣减和无敌/格挡逻辑
            Property playerProp = hitPlayer.GetComponent<Property>();
            if (playerProp != null)
            {
                // 调用我们刚刚在 Property.cs 里写的新方法
                playerProp.TakeDamageGeneric(damageAmount, transform);
            }

            Debug.Log($"Boss 命中玩家！类型:{attackType} 伤害:{damageAmount}");
        }
    }

    // Boss 自身的受击逻辑
    public void TakeDamage(float damage)
    {
        if (currentState == BossState.Dead)
        {
            return;
        }

        if (currentState == BossState.Blocking)
        {
            damage *= 0.1f;
        }
        else if (damage > 5f)
        {
            anim.SetTrigger("tHit");
            currentState = BossState.Hurt;
            StartCoroutine(ResetStateAfterDelay(0.5f));
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentState = BossState.Dead;
        anim.SetBool("isDead", true);
        rb.simulated = false;
        enabled = false;
    }

    #endregion

    #region 辅助功能

    private void LookAtPlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    #endregion
}
