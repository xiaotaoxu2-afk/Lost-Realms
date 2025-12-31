using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BossAI : MonoBehaviour
{
    [Header("玩家")] public Transform target; // 玩家 Transform

    public Collider2D bodyCollider; //传送时禁用碰撞
    public SpriteRenderer spriteRenderer; //用于传送时隐藏

    [Header("自动寻找玩家")] public bool autoFindPlayerByTag = true;

    public string playerTag = "Player";
    public int autoFindAttempts = 5;

    [Header("移动速度")] public float runSpeed = 3.5f;

    public float escapeSpeed = 6f;

    [Header("检测")] public bool usePhysicsDetect;

    public float aggroRange = 8f;
    public Vector2 checkSize = new(3f, 1.5f);
    public Vector2 checkOffset = new(0.5f, 0f);
    public float checkDistance = 2f;
    public LayerMask checkLayer;

    public float lostTime = 2f;

    [Header("攻击检测")] public float hitHeightTolerance = 1.2f;

    [Header("近战")] public float meleeRange = 1.8f;

    public float attackCooldown = 1.2f;

    public int attack1Damage = 10;
    public int attack2Damage = 14;
    public int attack3Damage = 18;

    [Header("攻击时机(攻击间隔)")] [Tooltip("如果 totalTime 填 0，会尝试从 Animator Clip 自动读取长度（clip 名需与状态名一致）")]
    public bool autoUseClipLengthWhenTotalIsZero = true;

    public float attack1HitDelay = 0.25f;
    public float attack1TotalTime = 0.8f;

    public float attack2HitDelay = 0.25f;
    public float attack2TotalTime = 0.85f;

    public float attack3HitDelay = 0.30f;
    public float attack3TotalTime = 1.00f;

    [Header("连击概率")] [Range(0, 1)] public float comboChance12 = 0.65f;

    [Range(0, 1)] public float comboChance23 = 0.50f;

    [Header("传送")] public float teleportCooldown = 5f;

    public float teleportMinDistance = 4.5f;
    public float teleportBehindOffset = 2f;

    public float teleportStartTime = 0.45f;
    public float teleportEndTime = 0.35f;

    [Header("格挡")] public float blockCooldown = 3f;

    [Range(0, 1)] public float blockChanceWhenClose = 0.25f;
    public float blockRange = 2.2f;
    public float blockDuration = 0.8f;

    public string animatorBlockingBool = "isBlocking";

    [Header("跳跃")] public float jumpCooldown = 6f;

    public float jumpMinDistance = 2.5f;
    public float jumpMaxDistance = 6f;

    public float jumpWindupTime = 0.15f;
    public float jumpUpSpeed = 12f;
    public float jumpForwardSpeed = 8f;

    public float jumpLandingRadius = 2.2f;
    public int jumpDamage = 22;
    public float jumpRecoverTime = 0.35f;

    [Header("受伤")] public float hurtLockTime = 0.35f;

    public float hurtKnockbackForce = 6f;
    [Range(0, 1)] public float hurtToBlockChance = 0.15f;

    [Header("逃跑")] [Range(0.05f, 1f)] public float escapeHealthPercent = 0.2f;

    public float escapeTime = 3f;
    public float escapeStopDistance = 12f;

    [Header("动画过度时间")] public float crossFadeTime = 0.08f;

    public bool isHurt;
    public bool isDeath;

    private readonly Dictionary<string, float> clipLengthCache = new(32);

    private Coroutine actionRoutine;
    private Animator anim;
    private Attack attack; // 用于对玩家造成伤害（Attack.OnAttack）
    private float attackCd;
    private float blockCd;
    private string currentAnim;
    private float jumpCd;
    private float lostCounter;
    private Vector3 originalScale;
    private PhysicsCheck physicsCheck;
    private Property property;

    private Rigidbody2D rb;
    private State state = State.Idle;
    private float teleportCd;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        property = GetComponent<Property>();

        if (attack == null)
        {
            attack = GetComponent<Attack>();
        }

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        originalScale = transform.localScale;
        lostCounter = lostTime;

        CacheClipLength();

        // 自动监听受伤/死亡
        if (property != null)
        {
            if (property.onTakedamage != null)
            {
                property.onTakedamage.AddListener(OnTakeDamage);
            }

            if (property.onDeath != null)
            {
                property.onDeath.AddListener(OnDeath);
            }
        }
    }

    private void Start()
    {
        if (target == null && autoFindPlayerByTag)
        {
            StartCoroutine(DelayedFindPlayer(autoFindAttempts));
        }
    }

    private void Update()
    {
        if (state == State.Dead)
        {
            return;
        }

        float dt = Time.deltaTime;
        attackCd -= dt;
        teleportCd -= dt;
        blockCd -= dt;
        jumpCd -= dt;

        // 触发逃跑
        if (property != null && property.maxHealth > 0f)
        {
            float hp01 = property.currentHealth / property.maxHealth;
            if (hp01 <= escapeHealthPercent && state != State.Escape)
            {
                StartEscape();
                return;
            }
        }

        // 丢失目标计时
        if (state != State.Idle && state != State.Escape && !FoundPlayer())
        {
            lostCounter -= dt;
            if (lostCounter <= 0f)
            {
                lostCounter = lostTime;
                StartIdle();
                return;
            }
        }
        else
        {
            lostCounter = lostTime;
        }

        switch (state)
        {
            case State.Idle:
                PlayAnim("Idle");
                if (FoundPlayer())
                {
                    StartRun();
                }

                break;

            case State.Run:
                PlayAnim("Run");
                if (target != null)
                {
                    LookAt(target.position);
                }

                if (target == null)
                {
                    StartIdle();
                    break;
                }

                float distX = Mathf.Abs(target.position.x - transform.position.x);

                // 近距离：随机格挡
                if (distX <= blockRange && blockCd <= 0f && Random.value < blockChanceWhenClose)
                {
                    StartBlock();
                    break;
                }

                // 中距离：Jump 攻击
                if (distX >= jumpMinDistance && distX <= jumpMaxDistance && jumpCd <= 0f)
                {
                    StartJump();
                    break;
                }

                // 远距离：Teleport 贴脸
                if (distX >= teleportMinDistance && teleportCd <= 0f)
                {
                    StartTeleport();
                    break;
                }

                // 近距离：连击
                if (distX <= meleeRange && attackCd <= 0f)
                {
                    StartCombo();
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        if (state == State.Run)
        {
            if (target == null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }

            float dir = Mathf.Sign(target.position.x - transform.position.x);
            rb.velocity = new Vector2(dir * runSpeed, rb.velocity.y);
            return;
        }

        if (state == State.Escape)
        {
            if (target == null)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }

            float dir = Mathf.Sign(transform.position.x - target.position.x); // 远离玩家
            rb.velocity = new Vector2(dir * escapeSpeed, rb.velocity.y);
            LookAt(transform.position + Vector3.right * dir);
            return;
        }

        // Attack / Block / Teleport / Idle 时停住水平
        if (state == State.Attack || state == State.Block || state == State.Teleport || state == State.Idle)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        // Hurt / Jump 交给物理（不要在这里强行归零，否则击退/跳跃会被打断）
    }

    private void OnDestroy()
    {
        if (property != null)
        {
            if (property.onTakedamage != null)
            {
                property.onTakedamage.RemoveListener(OnTakeDamage);
            }

            if (property.onDeath != null)
            {
                property.onDeath.RemoveListener(OnDeath);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.DrawWireSphere(transform.position, jumpLandingRadius);
    }

    private IEnumerator DelayedFindPlayer(int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                target = player.transform;
                yield break;
            }

            yield return null;
        }
    }


    public void OnTakeDamage(Transform attackerTrans)
    {
        isHurt = true;
        if (state == State.Dead)
        {
            return;
        }

        // 格挡中不打断
        if (state == State.Block)
        {
            return;
        }

        // 受击有概率直接格挡
        if (blockCd <= 0f && Random.value < hurtToBlockChance)
        {
            StartBlock();
            return;
        }

        StartHurt(attackerTrans);
    }

    public void OnDeath()
    {
        isDeath = true;
        if (state == State.Dead)
        {
            return;
        }

        state = State.Dead;
        StopAction();

        rb.velocity = Vector2.zero;
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }
    }

    // ====== 状态切换（协程驱动） ======
    private void StartIdle()
    {
        StopAction();
        state = State.Idle;
    }

    private void StartRun()
    {
        StopAction();
        state = State.Run;
    }

    private void StartCombo()
    {
        StopAction();
        actionRoutine = StartCoroutine(ComboRoutine());
    }

    private void StartTeleport()
    {
        StopAction();
        actionRoutine = StartCoroutine(TeleportRoutine());
    }

    private void StartBlock()
    {
        StopAction();
        actionRoutine = StartCoroutine(BlockRoutine());
    }

    private void StartHurt(Transform attackerTrans)
    {
        StopAction();
        actionRoutine = StartCoroutine(HurtRoutine(attackerTrans));
    }

    private void StartJump()
    {
        StopAction();
        actionRoutine = StartCoroutine(JumpRoutine());
    }

    private void StartEscape()
    {
        StopAction();
        actionRoutine = StartCoroutine(EscapeRoutine());
    }

    private void StopAction()
    {
        if (actionRoutine != null)
        {
            StopCoroutine(actionRoutine);
            actionRoutine = null;
        }

        // 退出阻挡状态
        SetBoolSafe(animatorBlockingBool, false);
    }

    // ====== 连击 ======
    private IEnumerator ComboRoutine()
    {
        state = State.Attack;
        attackCd = attackCooldown;

        yield return AttackStep("Attack1", attack1Damage, attack1HitDelay, GetTotalTime("Attack1", attack1TotalTime));

        if (target != null && IsTargetInRangeX(meleeRange) && Random.value < comboChance12)
        {
            yield return AttackStep("Attack2", attack2Damage, attack2HitDelay,
                GetTotalTime("Attack2", attack2TotalTime));

            if (target != null && IsTargetInRangeX(meleeRange) && Random.value < comboChance23)
            {
                yield return AttackStep("Attack3", attack3Damage, attack3HitDelay,
                    GetTotalTime("Attack3", attack3TotalTime));
            }
        }

        state = FoundPlayer() ? State.Run : State.Idle;
        actionRoutine = null;
    }

    private IEnumerator AttackStep(string animState, int damage, float hitDelay, float totalTime)
    {
        if (target != null)
        {
            LookAt(target.position);
        }

        PlayAnim(animState);

        if (hitDelay > 0f)
        {
            yield return new WaitForSeconds(hitDelay);
        }

        TryDealDamage(damage, meleeRange, true);

        float remain = Mathf.Max(0f, totalTime - hitDelay);
        if (remain > 0f)
        {
            yield return new WaitForSeconds(remain);
        }
    }

    // ====== 传送 ======
    private IEnumerator TeleportRoutine()
    {
        state = State.Teleport;
        teleportCd = teleportCooldown;

        PlayAnim("TeleportStart");

        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(teleportStartTime);

        if (target != null)
        {
            transform.position = CalcTeleportDestination();
            LookAt(target.position);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        PlayAnim("TeleportEnd");

        yield return new WaitForSeconds(teleportEndTime);

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }

        // 贴脸就连击，否则追
        if (target != null && IsTargetInRangeX(meleeRange) && attackCd <= 0f)
        {
            actionRoutine = StartCoroutine(ComboRoutine());
            yield break;
        }

        state = FoundPlayer() ? State.Run : State.Idle;
        actionRoutine = null;
    }

    private Vector3 CalcTeleportDestination()
    {
        // 传送到玩家“背后”：取 Boss -> 玩家方向，再反向偏移
        float dirToPlayer = Mathf.Sign(target.position.x - transform.position.x);
        float offset = -dirToPlayer * teleportBehindOffset;

        return new Vector3(target.position.x + offset, transform.position.y, transform.position.z);
    }

    // ====== 格挡 ======
    private IEnumerator BlockRoutine()
    {
        state = State.Block;
        blockCd = blockCooldown;

        // 让 Property 能识别格挡
        SetBoolSafe(animatorBlockingBool, true);
        PlayAnim("Block");

        yield return new WaitForSeconds(blockDuration);

        SetBoolSafe(animatorBlockingBool, false);

        state = FoundPlayer() ? State.Run : State.Idle;
        actionRoutine = null;
    }

    // ====== 受伤 ======
    private IEnumerator HurtRoutine(Transform attackerTrans)
    {
        state = State.Hurt;
        isHurt = true;

        PlayAnim("Hurt");

        if (attackerTrans != null)
        {
            float dir = Mathf.Sign(transform.position.x - attackerTrans.position.x);
            rb.AddForce(new Vector2(dir * hurtKnockbackForce, 0f), ForceMode2D.Impulse);
            LookAt(attackerTrans.position);
        }

        yield return new WaitForSeconds(hurtLockTime);

        state = FoundPlayer() ? State.Run : State.Idle;
        actionRoutine = null;
        isHurt = false;
    }

    // ====== Jump：跳砸（落地AOE） ======
    private IEnumerator JumpRoutine()
    {
        state = State.Jump;
        jumpCd = jumpCooldown;

        if (target != null)
        {
            LookAt(target.position);
        }

        PlayAnim("Jump");

        if (jumpWindupTime > 0f)
        {
            yield return new WaitForSeconds(jumpWindupTime);
        }

        float dir = 1f;
        if (target != null)
        {
            dir = Mathf.Sign(target.position.x - transform.position.x);
        }

        rb.velocity = new Vector2(dir * jumpForwardSpeed, jumpUpSpeed);

        // 等待离地
        yield return new WaitForSeconds(0.05f);

        // 等落地
        if (physicsCheck != null)
        {
            while (physicsCheck.isGround)
            {
                yield return null; // 等到离地
            }

            while (!physicsCheck.isGround)
            {
                yield return null; // 等到落地
            }
        }
        else
        {
            // fallback：粗略等待（没 PhysicsCheck 也能跑起来）
            yield return new WaitForSeconds(0.6f);
        }

        // 落地伤害：AOE，不要求朝向
        TryDealDamage(jumpDamage, jumpLandingRadius, false);

        if (jumpRecoverTime > 0f)
        {
            yield return new WaitForSeconds(jumpRecoverTime);
        }

        state = FoundPlayer() ? State.Run : State.Idle;
        actionRoutine = null;
    }

    // ====== Escape：血量低于阈值就逃跑 ======
    private IEnumerator EscapeRoutine()
    {
        state = State.Escape;
        PlayAnim("Escape");

        float t = 0f;
        while (t < escapeTime)
        {
            t += Time.deltaTime;

            if (target != null)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (dist >= escapeStopDistance)
                {
                    break;
                }
            }

            yield return null;
        }

        StartIdle();
    }

    // ====== 伤害输出：复用 Attack.cs ======
    private void TryDealDamage(int damage, float range, bool requireFacing)
    {
        if (attack == null || target == null)
        {
            return;
        }

        // 横版更常用：用 X 距离 + Y 容差
        float dx = Mathf.Abs(target.position.x - transform.position.x);
        float dy = Mathf.Abs(target.position.y - transform.position.y);

        if (dx > range)
        {
            return;
        }

        if (dy > hitHeightTolerance)
        {
            return;
        }

        if (requireFacing)
        {
            float dirToTarget = Mathf.Sign(target.position.x - transform.position.x);
            float face = GetFaceDirX();
            if (Mathf.Sign(face) != Mathf.Sign(dirToTarget))
            {
                return;
            }
        }

        attack.damage = damage;
        attack.OnAttack(null);
    }

    // ====== 检测 ======
    private bool FoundPlayer()
    {
        if (target == null)
        {
            return false;
        }

        if (!usePhysicsDetect)
        {
            return Vector2.Distance(transform.position, target.position) <= aggroRange;
        }

        var dir = new Vector2(GetFaceDirX(), 0f);
        return Physics2D.BoxCast(transform.position + (Vector3)checkOffset, checkSize, 0f, dir, checkDistance,
            checkLayer);
    }

    private bool IsTargetInRangeX(float rangeX)
    {
        if (target == null)
        {
            return false;
        }

        return Mathf.Abs(target.position.x - transform.position.x) <= rangeX;
    }

    private void LookAt(Vector3 worldPos)
    {
        float absX = Mathf.Abs(originalScale.x);
        Vector3 scale = transform.localScale;

        if (worldPos.x > transform.position.x)
        {
            scale.x = absX; // 向右看
        }
        else if (worldPos.x < transform.position.x)
        {
            scale.x = -absX; // 向左看
        }

        transform.localScale = scale;
    }

    private float GetFaceDirX()
    {
        // scale.x < 0 => face right => dir +1
        return -Mathf.Sign(transform.localScale.x);
    }


    private void PlayAnim(string stateName)
    {
        if (anim == null)
        {
            return;
        }

        if (currentAnim == stateName)
        {
            return;
        }

        currentAnim = stateName;
        anim.CrossFadeInFixedTime(stateName, crossFadeTime);
    }

    private void CacheClipLength()
    {
        clipLengthCache.Clear();

        if (anim == null)
        {
            return;
        }

        if (anim.runtimeAnimatorController == null)
        {
            return;
        }

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        if (clips == null)
        {
            return;
        }

        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip clip = clips[i];
            if (clip == null)
            {
                continue;
            }

            // 注意：同名 clip 只记录一次
            if (!clipLengthCache.ContainsKey(clip.name))
            {
                clipLengthCache.Add(clip.name, clip.length);
            }
        }
    }

    private float GetTotalTime(string animStateName, float inspectorValue)
    {
        if (inspectorValue > 0f)
        {
            return inspectorValue;
        }

        if (!autoUseClipLengthWhenTotalIsZero)
        {
            return 0.8f;
        }

        if (clipLengthCache.TryGetValue(animStateName, out float len))
        {
            return len;
        }

        return 0.8f;
    }

    // ====== Animator 参数安全设置（避免参数不存在时报错） ======
    private void SetBoolSafe(string boolName, bool value)
    {
        if (anim == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(boolName))
        {
            return;
        }


        if (!HasParameter(boolName, AnimatorControllerParameterType.Bool))
        {
            return;
        }

        anim.SetBool(boolName, value);
    }

    private bool HasParameter(string name, AnimatorControllerParameterType type)
    {
        if (anim == null)
        {
            return false;
        }

        AnimatorControllerParameter[] ps = anim.parameters;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].name == name && ps[i].type == type)
            {
                return true;
            }
        }

        return false;
    }

    private enum State
    {
        Idle,
        Run,
        Attack,
        Teleport,
        Block,
        Jump,
        Hurt,
        Escape,
        Dead
    }
}
