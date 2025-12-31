using UnityEngine;
using UnityEngine.Events;

public class Property : MonoBehaviour, Isaveable
{
    public UnityEvent<Property> onHealthChange;
    public UnityEvent<Transform> onTakedamage;
    public UnityEvent onDeath;

    [Header("事件监听")] public VoidSO newGameEvent;

    [Header("基本属性")] public float maxHealth;

    public float currentHealth;
    public float maxSlidePower;
    public float currentSlidePower;
    public float powerRecoverSpeed;

    [Header("无敌")] public float invincibleTime;

    public float invincibleTimer;
    public bool invincible;
    private PlayerControl playerControl;
    private PlayerSkill playerSkill;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        playerSkill = GetComponent<PlayerSkill>();
    }

    private void Start() => currentHealth = maxHealth;

    private void Update()
    {
        invincibleTimerUpdate();

        if (currentSlidePower < maxSlidePower)
        {
            currentSlidePower += Time.deltaTime * powerRecoverSpeed * 0.6f;
        }
    }

    private void OnEnable()
    {
        newGameEvent.VoidEvent += newGame;
        Isaveable isaveable = this;
        isaveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.VoidEvent -= newGame;
        Isaveable isaveable = this;
        isaveable.UnRegisterSaveData();
    }

    public DataDefinition GetDataID() => GetComponent<DataDefinition>();

    public void GetSaveData(Data data)
    {
        if (data.characterPosDirt.ContainsKey(GetDataID().id))
        {
            data.characterPosDirt[GetDataID().id] = new SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().id + "血量"] = currentHealth;
            data.floatSaveData[GetDataID().id + "能量"] = currentSlidePower;
        }
        else
        {
            data.characterPosDirt.Add(GetDataID().id, new SerializeVector3(transform.position));
            data.floatSaveData.Add(GetDataID().id + "血量", currentHealth);
            data.floatSaveData.Add(GetDataID().id + "能量", currentSlidePower);
        }
    }

    public void LoadSaveData(Data data)
    {
        if (data.characterPosDirt.ContainsKey(GetDataID().id))
        {
            transform.position = data.characterPosDirt[GetDataID().id].toVector3();
            currentHealth = data.floatSaveData[GetDataID().id + "血量"];
            currentSlidePower = data.floatSaveData[GetDataID().id + "能量"];

            //通知UI更新
            onHealthChange?.Invoke(this);
        }
    }

    public void newGame()
    {
        currentHealth = maxHealth;
        currentSlidePower = maxSlidePower;
        onHealthChange.Invoke(this);
    }

    public void Takedamage(Attack attack)
    {
        if (attack == null)
        {
            Debug.LogError("Attack 参数为 null！检查 Attack 组件是否正确附加。");
            return;
        }

        // 将逻辑转发给通用的处理方法
        TakeDamageGeneric(attack.damage, attack.transform);
    }

    // 2. 新增一个通用的伤害处理方法（供 Boss 调用）
    public void TakeDamageGeneric(float damageAmount, Transform attackerTransform)
    {
        if (invincible)
        {
            return;
        }

        // 获取必要的引用
        Animator animator = GetComponent<Animator>();
        // 注意：这里需要确保 playerControl 已经被获取 (在Awake中已获取)

        bool isBlocking = animator != null && animator.GetBool("isBlocking");
        bool isShiled = playerControl != null && playerControl.isShileding;

        float damageMultiplier = 1f;

        // === 核心防御逻辑 ===
        // 1. 盾牌反击/完美防御
        if (isShiled && isBlocking)
        {
            damageMultiplier = 0f;
            if (currentHealth > 0 && animator != null)
            {
                animator.SetTrigger("isBlock"); // 播放格挡成功动画
            }

            return; // 无伤直接返回
        }

        // 2. 普通格挡
        if (isBlocking)
        {
            damageMultiplier = 0.5f; // 减伤 50%
            if (currentHealth > 0 && animator != null)
            {
                animator.SetTrigger("isBlock");
            }
        }

        // === 结算伤害 ===
        float actualDamage = damageAmount * damageMultiplier;

        if (currentHealth - actualDamage > 0)
        {
            currentHealth -= actualDamage;
            Triggerinvincible(); // 触发无敌时间

            // 触发受击事件（这里可能会调用 PlayerControl 的 getHurt）
            onTakedamage?.Invoke(attackerTransform);
        }
        else
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                onDeath?.Invoke(); // 触发死亡
            }
        }

        onHealthChange?.Invoke(this); // 更新UI
    }


    public void Triggerinvincible()
    {
        if (!invincible)
        {
            invincible = true;
            invincibleTimer = invincibleTime;
        }
    }

    private void invincibleTimerUpdate()
    {
        if (invincible && (playerSkill == null || !playerSkill.isSain))
        {
            invincibleTimer -= Time.deltaTime;
        }

        if (invincibleTimer <= 0 && (playerSkill == null || !playerSkill.isSaining))
        {
            invincible = false;
        }
    }

    public void onSlide(int cost)
    {
        currentSlidePower -= cost;
        onHealthChange?.Invoke(this);
    }

    public void OnSword(int cost)
    {
        currentSlidePower -= cost;
        onHealthChange?.Invoke(this);
    }

    public void OnShiled(int cost)
    {
        currentSlidePower -= cost;
        onHealthChange?.Invoke(this);
    }
}
