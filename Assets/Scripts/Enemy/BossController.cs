using System.Collections;

using UnityEngine;

/// <summary>
///     不继承 Enemy：Boss 独立实现（但复用 Attack/Property 的伤害体系）
///     Attack -> Property.Takedamage 参考你的现有实现 :contentReference[oaicite:5]{index=5} :contentReference[oaicite:6]{index=6}
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BossController : MonoBehaviour
{
    [Header("References")]
    public Transform player; // 可手动拖，也可自动找 Tag=Player（参考 Enemy 的延迟查找思路 :contentReference[oaicite:7]{index=7}）

    public Property property; // Boss 自己的血量组件
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Detection")] public LayerMask playerLayer;

    public Vector2 aggroBoxSize = new(8f, 3f);
    public Vector2 aggroBoxOffset = new(2f, 0f);

    [Header("Hurt")] public float hurtForce = 5f; // 受伤击退力


    [Header("Ranges")] public float attack1Range = 1.6f;

    public float attack2BackRange = 1.2f; // 背后贴近距离
    public float attack3MinRange = 2.5f; // 玩家稍远/拉扯时更倾向放 attack3

    [Header("Cooldowns")] public float attack1Cooldown = 1.2f;

    public float attack2Cooldown = 2.0f;
    public float attack3Cooldown = 6.0f;

    [Header("Smart")] [Range(0f, 1f)] public float phase2HpRatio = 0.5f; // 血量低于 50% 更激进

    public float phase2Attack3CooldownMul = 0.75f; // 二阶段 attack3 CD 更短
    public float attack3RollSpeed = 5f; // attack3 滚动速度
    public float attack3RollDuration = 0.5f; // attack3 滚动持续时间
    public float facePlayerDelay = 0.3f; // 延迟转身时间，避免立即转身打断 attack2


    [Header("Hitboxes (child objects)")] public GameObject hitboxAttack1;

    public GameObject hitboxAttack2;
    public GameObject hitboxAttack3;

    private float _atk1Timer;
    private float _atk2Timer;
    private float _atk3Timer;

    private float _faceSign = 1f; // 1 表示 localScale.x 为正（朝左/朝右取决于你美术）；我们用统一公式算 forward

    private bool _isRolling; // 是否正在执行 attack3 滚动

    private float _lastFacePlayerTime; // 上次转身的时间

    // runtime
    public bool IsDead { get; private set; }
    public bool IsHurt { get; private set; }
    public bool IsBusy { get; private set; } // 攻击/受击/死亡期间锁定
    public string CurrentAttack { get; private set; } // "Attack1"/"Attack2"/"Attack3"


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (property == null)
        {
            property = GetComponent<Property>();
        }

        // Boss 站桩：不巡逻、不走位
        rb.velocity = Vector2.zero;
    }

    private void Start()
    {
        // 新增：延迟自动查找 attacker（最多 5 帧，确保玩家加载）
        StartCoroutine(DelayedFindPlayer(5));
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }

        TickCooldowns();

        // Boss 站桩：不要移动
        if (!IsHurt)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // 记录当前朝向符号（用来判断"背后"）
        _faceSign = Mathf.Sign(transform.localScale.x);
        if (_faceSign == 0)
        {
            _faceSign = 1f;
        }
    }


    private void OnEnable()
    {
        if (property != null)
        {
            // 复用 Property 的事件：受击/死亡 :contentReference[oaicite:8]{index=8}
            property.onTakedamage.AddListener(OnTakeDamage);
            property.onDeath.AddListener(OnDeath);
        }

        // 初始关闭 hitbox
        SetAllHitboxes(false);
    }

    private void OnDisable()
    {
        if (property != null)
        {
            property.onTakedamage.RemoveListener(OnTakeDamage);
            property.onDeath.RemoveListener(OnDeath);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 画出 Aggro 检测框
        float forward = GetForwardSign();
        Vector2 origin = (Vector2)transform.position + aggroBoxOffset * new Vector2(forward, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(origin, aggroBoxSize);
    }
#endif


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

    private void TickCooldowns()
    {
        if (_atk1Timer > 0)
        {
            _atk1Timer -= Time.deltaTime;
        }

        if (_atk2Timer > 0)
        {
            _atk2Timer -= Time.deltaTime;
        }

        if (_atk3Timer > 0)
        {
            _atk3Timer -= Time.deltaTime;
        }
    }

    // =========================
    // ====== Condition API =====
    // =========================

    public bool PlayerInAggro()
    {
        if (player == null)
        {
            return false;
        }

        // 参考 Enemy.FoundPlayer 的 BoxCast 检测方式 :contentReference[oaicite:10]{index=10}
        Vector2 origin = (Vector2)transform.position + aggroBoxOffset * new Vector2(GetForwardSign(), 1f);
        Collider2D hit = Physics2D.OverlapBox(origin, aggroBoxSize, 0f, playerLayer);
        return hit != null && hit.transform == player;
    }

    public float DistanceToPlayer()
    {
        if (player == null)
        {
            return float.MaxValue;
        }

        return Vector2.Distance(transform.position, player.position);
    }

    public bool PlayerInAttack1Range() => PlayerInAggro() && DistanceToPlayer() <= attack1Range;

    /// <summary>
    ///     玩家在 boss 背后 + 贴近（attack2 条件）
    ///     背后判断：玩家在 forward 的反方向
    /// </summary>
    public bool PlayerBehindAndClose()
    {
        if (!PlayerInAggro())
        {
            return false;
        }

        float dist = DistanceToPlayer();
        if (dist > attack2BackRange)
        {
            return false;
        }

        // 修复：forward 现在与 localScale.x 同向
        float forward = GetForwardSign();
        float dx = player.position.x - transform.position.x;

        // dx 和 forward 反向 => 在背后
        bool behind = dx * forward < 0f;
        return behind;
    }

    public bool CanAttack1() => !IsBusy && _atk1Timer <= 0f && PlayerInAttack1Range();
    public bool CanAttack2() => !IsBusy && _atk2Timer <= 0f && PlayerBehindAndClose();

    public bool CanAttack3()
    {
        if (IsBusy)
        {
            return false;
        }

        if (_atk3Timer > 0f)
        {
            return false;
        }

        if (!PlayerInAggro())
        {
            return false;
        }

        // 更聪明：玩家拉扯（距离较远）时更倾向 attack3；或二阶段更频繁
        float d = DistanceToPlayer();
        bool farEnough = d >= attack3MinRange;

        // 二阶段更激进：即使不够远也可能放（看你要不要）
        bool phase2 = GetHpRatio() <= phase2HpRatio;

        return farEnough || phase2;
    }

    public float GetHpRatio()
    {
        if (property == null || property.maxHealth <= 0)
        {
            return 1f;
        }

        return property.currentHealth / property.maxHealth;
    }

    // =========================
    // ====== Action API ========
    // =========================

    public void FacePlayer()
    {
        if (player == null || IsDead)
        {
            return;
        }

        // 延迟转身：避免玩家滑到背后时立即转身，这样 attack2 才能触发
        if (Time.time - _lastFacePlayerTime < facePlayerDelay)
        {
            return;
        }

        float dx = player.position.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f)
        {
            return;
        }

        // 修复转身逻辑：与 Enemy 保持一致
        // 玩家在右边 -> localScale.x = 正值（向右看）
        // 玩家在左边 -> localScale.x = 负值（向左看）
        float absScale = Mathf.Abs(transform.localScale.x);
        if (dx > 0)
        {
            transform.localScale = new Vector3(absScale, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-absScale, transform.localScale.y, transform.localScale.z);
        }

        _lastFacePlayerTime = Time.time;
    }

    /// <summary>
    ///     Boss 移动：如果 walk=true 且不忙，则向玩家方向移动
    /// </summary>
    public void SetWalk(bool on)
    {
        if (IsDead)
        {
            return;
        }

        anim.SetBool("isWalk", on);
        // 如果你想让 Boss 能移动，取消注释下面的代码：
        if (on && !IsBusy && player != null)
        {
            float dx = player.position.x - transform.position.x;
            float moveSpeed = 2f; // 设置 Boss 移动速度
            rb.velocity = new Vector2(Mathf.Sign(dx + 1.2f) * moveSpeed, rb.velocity.y);
        }
    }

    public bool DoAttack1()
    {
        if (!CanAttack1())
        {
            return false;
        }

        LockBusy("Attack1");
        _atk1Timer = attack1Cooldown;

        SetWalk(false);
        anim.ResetTrigger("Attack2");
        anim.ResetTrigger("StartAttack3");
        anim.SetTrigger("Attack1");
        return true;
    }

    public bool DoAttack2()
    {
        // attack2：必须玩家在背后贴近才会进入（行为树会优先判这个）
        if (!CanAttack2())
        {
            return false;
        }

        LockBusy("Attack2");
        _atk2Timer = attack2Cooldown;

        SetWalk(false);
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("StartAttack3");
        anim.SetTrigger("Attack2");
        return true;
    }

    public bool DoAttack3_Start()
    {
        if (!CanAttack3())
        {
            return false;
        }

        LockBusy("Attack3");

        // 二阶段缩短 CD
        float cd = attack3Cooldown;
        if (GetHpRatio() <= phase2HpRatio)
        {
            cd *= phase2Attack3CooldownMul;
        }

        _atk3Timer = cd;

        SetWalk(false);
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");
        anim.SetTrigger("StartAttack3");

        // 开始滚动
        StartCoroutine(Attack3Roll());

        return true;
    }

    // 下面三个由动画事件按顺序调用，保证：start -> ing -> end
    public void DoAttack3_ToIng()
    {
        if (IsDead || CurrentAttack != "Attack3")
        {
            return;
        }

        anim.SetTrigger("Attack3Ing");
    }

    public void DoAttack3_ToEnd()
    {
        if (IsDead || CurrentAttack != "Attack3")
        {
            return;
        }

        anim.SetTrigger("EndAttack3");
    }

    public void FinishAttack()
    {
        if (IsDead)
        {
            return;
        }

        // 停止滚动
        _isRolling = false;

        UnlockBusy();
        SetAllHitboxes(false);
    }

    // =========================
    // ===== Damage/Death =======
    // =========================

    private void OnTakeDamage(Transform attacker)
    {
        if (IsDead)
        {
            return;
        }

        // 被打时：打断攻击更像“智能”（不让 Boss 无脑硬直免疫）
        StopAllCoroutines();
        SetAllHitboxes(false);

        IsHurt = true;
        IsBusy = true;
        CurrentAttack = "Hit";

        anim.SetTrigger("Hit");

        // 受伤被击退（参考 Enemy 的实现）
        if (attacker != null)
        {
            Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
            rb.velocity = new Vector2(0, rb.velocity.y);

            // 检查攻击者是否使用了 HolySlash 技能
            float actualHurtForce = hurtForce;
            PlayerSkill playerSkill = attacker.GetComponent<PlayerSkill>();
            if (playerSkill != null && playerSkill.isHolySlash)
            {
                actualHurtForce = playerSkill.slashForce; // 使用增强击退力
            }

            StartCoroutine(OnHurt(dir, actualHurtForce));
        }


        // 受击硬直结束由动画事件回调 FinishHit()
    }

    private void OnDeath()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        IsBusy = true;
        IsHurt = false;

        SetAllHitboxes(false);

        gameObject.layer = 2; // 和你 Enemy 死亡一致 :contentReference[oaicite:13]{index=13}
        anim.SetBool("isDeath", true);
    }

    public void FinishHit()
    {
        if (IsDead)
        {
            return;
        }

        IsHurt = false;
        UnlockBusy();
    }

    private void LockBusy(string attackName)
    {
        IsBusy = true;
        CurrentAttack = attackName;
    }

    private void UnlockBusy()
    {
        IsBusy = false;
        CurrentAttack = "";
    }

    private void SetAllHitboxes(bool on)
    {
        if (hitboxAttack1)
        {
            hitboxAttack1.SetActive(on);
        }

        if (hitboxAttack2)
        {
            hitboxAttack2.SetActive(on);
        }

        if (hitboxAttack3)
        {
            hitboxAttack3.SetActive(on);
        }
    }

    private float GetForwardSign()
    {
        // 修复：forward 应该与 localScale.x 同向（参考 Enemy.faceDir = transform.localScale.x）
        // 向右：localScale.x > 0 -> forward = 1
        // 向左：localScale.x < 0 -> forward = -1
        float scaleX = transform.localScale.x;
        if (scaleX == 0)
        {
            scaleX = 1f;
        }

        return Mathf.Sign(scaleX);
    }

    /// <summary>
    ///     attack3 滚动协程：向前滚动一小段距离
    /// </summary>
    private IEnumerator Attack3Roll()
    {
        _isRolling = true;
        float elapsed = 0f;
        float direction = GetForwardSign();

        while (elapsed < attack3RollDuration && CurrentAttack == "Attack3")
        {
            rb.velocity = new Vector2(direction * attack3RollSpeed, rb.velocity.y);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isRolling = false;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    /// <summary>
    ///     受伤击退协程（参考 Enemy 的实现）
    /// </summary>
    private IEnumerator OnHurt(Vector2 dir, float force)
    {
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        IsHurt = false;
    }
}
