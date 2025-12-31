using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicscheck;

    public Transform attacker;

    public float walkSpeed;
    public float pursuitSpeed;
    public float currentSpeed;
    public float hurtForce;

    public Vector3 faceDir;
    public Vector3 spwanPoint;

    [Header("检测")] public Vector2 checksize;

    public Vector2 checkoffset;
    public float checkdistance;
    public LayerMask checklayer;

    [Header("等待计时")] public float waitTime;

    public float waitCounter;
    public bool wait;
    public float lostTime;
    public float lostCounter;
    public float attackRateCounter;

    [Header("状态")] public bool isHurt;

    public bool isDeath;
    public bool Attack;
    protected BaseState chasestate;
    private BaseState currentstate;
    protected BaseState patrolstate;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicscheck = GetComponent<PhysicsCheck>();

        currentSpeed = walkSpeed;
        waitCounter = waitTime;
        lostCounter = lostTime;
        spwanPoint = transform.position;
    }

    private void Start()
    {
        // 新增：延迟自动查找 attacker（最多 5 帧，确保玩家加载）
        StartCoroutine(DelayedFindPlayer(5));
    }

    public virtual void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        if (currentstate != null)
        {
            currentstate.LogicUpdate();
        }

        WaitTime();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isDeath && !wait && !Attack)
        {
            Move();
        }

        currentstate.PhysicsUpdate();
    }

    private void OnEnable()
    {
        currentstate = patrolstate;
        if (currentstate != null)
        {
            currentstate.OnEnter(this);
        }
    }

    private void OnDisable()
    {
        if (currentstate != null)
        {
            currentstate.OnExit();
        }
    }

    public void OnDestroy() => Destroy(gameObject);

    public virtual void OnDrawGizmosSelected()
    {
        //绘制检测
        Gizmos.DrawWireSphere(transform.position + (Vector3)checkoffset, 0.2f);
    }

    private IEnumerator DelayedFindPlayer(int maxAttempts)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                attacker = player.transform;
                Debug.Log("attacker 自动赋值成功: " + player.name);
                yield break;
            }

            yield return null; // 等待一帧
        }

        Debug.LogError("未找到 Tag 为 'Player' 的对象，请手动赋值 attacker 或检查玩家 Tag。");
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x, rb.velocity.y);
        //Debug.Log("Time Scale: " + Time.timeScale);
    }

    public void WaitTime()
    {
        //撞墙和悬崖等待计时
        if (wait)
        {
            waitCounter -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            if (waitCounter <= 0)
            {
                wait = false;
                waitCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, (float)4.5, (float)4.5);
            }
        }

        //丢失玩家计时
        if (!FoundPlayer() && lostCounter > 0)
        {
            lostCounter -= Time.deltaTime;
        }
        else if (!FoundPlayer())
        {
            Attack = false;
            lostCounter = lostTime;
        }
    }

    public virtual bool FoundPlayer() => Physics2D.BoxCast(transform.position + (Vector3)checkoffset, checksize, 0,
        faceDir, checkdistance, checklayer);

    public void stateSwitch(EnemyState state)
    {
        BaseState newstate = state switch
        {
            EnemyState.patrol => patrolstate,
            EnemyState.chase => chasestate,
            _ => null
        };

        // 空值检查，防止空引用
        if (currentstate != null)
        {
            currentstate.OnExit();
        }

        currentstate = newstate;

        if (currentstate != null)
        {
            currentstate.OnEnter(this);
        }
    }

    public virtual Vector3 GetNewPoint() => transform.position;

    public virtual void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //受伤转身（攻击者在右边向右看，攻击者在左边向左看）
        if (attacker.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3((float)-4.5, (float)4.5, (float)4.5); // 向右看
        }

        if (attacker.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3((float)4.5, (float)4.5, (float)4.5); // 向左看
        }

        //受伤被击退
        isHurt = true;
        anim.SetTrigger("isHurt");
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 检查政击者是否使用了 HolySlash 技能
        float actualHurtForce = hurtForce;
        PlayerSkill playerSkill = attacker.GetComponent<PlayerSkill>();
        if (playerSkill != null && playerSkill.isHolySlash)
        {
            actualHurtForce = playerSkill.slashForce; // 使用增强击退力
        }

        StartCoroutine(OnHurt(dir, actualHurtForce));
    }

    private IEnumerator OnHurt(Vector2 dir, float force)
    {
        rb.AddForce(dir * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public virtual void OnDeath()
    {
        gameObject.layer = 2;
        anim.SetBool("isDeath", true);
        isDeath = true;
    }
}
