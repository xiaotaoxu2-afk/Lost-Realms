using System;
using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("事件监听")] public SceneLoadEventSO sceneLoadEventSO;

    public VoidSO afterSceneLoadEvent;
    public VoidSO loadDataEvent;
    public VoidSO backToMenuEvent;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Collider2D coll;
    [HideInInspector] public Vector2 inputDirection;

    [Header("粒子效果")] public ParticleSystem RunPar;

    [Header("物理材质")] public PhysicsMaterial2D smooth;

    public PhysicsMaterial2D friction;

    [Header("角色移动")] public float moveSpeed;

    [Header("角色跳跃")] public float jumpForce;

    public float wallJumpForce;

    [Header("受伤反弹")] public bool isHurt;

    public float hurtForce;

    [Header("角色死亡")] public bool isDead;

    [Header("角色攻击")] public bool isAttack;

    [Header("角色滑行")] public bool isSlide;

    public float slideSpeed;
    public float slideDistance;
    public float slidePowerCost;

    [Header("角色格挡")] public bool isBlock;
    public bool isShileding;


    [Header("角色技能")] public GameObject a1;

    public GameObject a2;
    public GameObject a3;
    public bool isSword;
    public float swordPowerCost;
    public bool isShiled;
    public float shiledPowerCost;
    public GameObject swordIcon;
    public GameObject shiledIcon;

    private int faceDir;
    public PlayerInputControl inputActions;

    private PhysicsCheck physicscheck;
    private PlayerAnim playerAnim;
    private PlayerSkill playerSkill; // 新增：技能组件引用
    private Property property;
    private bool wallJump;


    private void Awake()
    {
        physicscheck = GetComponent<PhysicsCheck>();
        playerAnim = GetComponent<PlayerAnim>();
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputControl();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        property = GetComponent<Property>();
        playerSkill = GetComponent<PlayerSkill>(); // 新增：获取技能组件
        inputActions.GamePlayer.Jump.started += jump;
        inputActions.GamePlayer.Attack.started += attack;
        inputActions.GamePlayer.Slide.started += slide;
        inputActions.GamePlayer.Sword.started += sword;
        inputActions.GamePlayer.Shiled.started += shiled;
        inputActions.GamePlayer.Block.started += OnBlockStarted; //按下k键格挡
        inputActions.GamePlayer.Block.canceled += OnBlockCanceled; //松开k键结束格挡
        inputActions.Enable();
    }

    private void Update()
    {
        inputDirection = inputActions.GamePlayer.Move.ReadValue<Vector2>();
        cheakState();
    }

    private void FixedUpdate()
    {
        // 新增：技能期间禁止移动


        if (!isHurt && !isAttack && !isSword && !isShiled && !isBlock && !playerSkill.isHolyHeal &&
            !playerSkill.isHolySlash && !playerSkill.isSain)
        {
            move();
        }
    }


    private void OnEnable()
    {
        sceneLoadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        afterSceneLoadEvent.VoidEvent += OnLoadRequestEvent;
        loadDataEvent.VoidEvent += OnLoadDataEvent;
        backToMenuEvent.VoidEvent += OnLoadDataEvent;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        sceneLoadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        afterSceneLoadEvent.VoidEvent -= OnLoadRequestEvent;
        loadDataEvent.VoidEvent -= OnLoadDataEvent;
        backToMenuEvent.VoidEvent -= OnLoadDataEvent;
    }

    // 场景加载时禁用玩家输入
    private void OnLoadRequestEvent(GameSceneSO arg0, Vector3 arg1, bool arg2) => inputActions.GamePlayer.Disable();

    private void OnLoadDataEvent()
    {
        isDead = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    // 场景加载完成后启用玩家输入
    private void OnLoadRequestEvent() => inputActions.GamePlayer.Enable();

    private void move()
    {
        if (!wallJump)
        {
            rb.velocity = new Vector2(inputDirection.x * moveSpeed, rb.velocity.y);
        }

        // 角色朝向
        faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0 && !isSlide)
        {
            faceDir = 4;
            PlayerPar();
        }
        else if (inputDirection.x < 0 && !isSlide)
        {
            faceDir = -4;
            PlayerPar();
        }

        transform.localScale = new Vector3(faceDir, 4, 4);
    }

    private void jump(InputAction.CallbackContext context)
    {
        if (physicscheck.isGround && !isSword && !isShiled && !isBlock && !playerSkill.isHolyHeal &&
            !playerSkill.isHolySlash && !playerSkill.isSain)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            PlayerPar();
            //如果处于滑行状态，则取消滑行
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicscheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void attack(InputAction.CallbackContext context)
    {
        if (!isSword && !isShiled && !isBlock && !playerSkill.isHolyHeal && !playerSkill.isHolySlash &&
            !playerSkill.isSain)
        {
            playerAnim.playerAttack();
            isAttack = true;
        }
    }

    private void slide(InputAction.CallbackContext context)
    {
        if (!isSlide && physicscheck.isGround && property.currentSlidePower >= slidePowerCost && !isSword &&
            !isShiled && !isBlock && !playerSkill.isHolyHeal && !playerSkill.isHolySlash && !playerSkill.isSain)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,
                transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");

            StartCoroutine(TriggerSlide(targetPos));
            property.onSlide((int)slidePowerCost);
        }
    }

    private void sword(InputAction.CallbackContext obj)
    {
        if (!isSword && physicscheck.isGround && property.currentSlidePower >= swordPowerCost && !isShiled &&
            !isBlock && !playerSkill.isHolyHeal && !playerSkill.isHolySlash && !playerSkill.isSain)
        {
            isSword = true;
            swordIcon.SetActive(true);
            a1.GetComponent<Attack>().damage = 20;
            a2.GetComponent<Attack>().damage = 20;
            a3.GetComponent<Attack>().damage = 40;

            property.OnSword((int)swordPowerCost);

            float lg = 2f;

            StartCoroutine(TriggerSword(lg));
        }
    }

    private void shiled(InputAction.CallbackContext obj)
    {
        if (!isShiled && physicscheck.isGround && property.currentSlidePower >= shiledPowerCost && !isSword &&
            !isBlock && !playerSkill.isHolyHeal && !playerSkill.isHolySlash && !playerSkill.isSain)
        {
            isShiled = true;
            isShileding = true;
            shiledIcon.SetActive(true);
            property.OnSword((int)shiledPowerCost);

            float lg = 1.35f;
            StartCoroutine(TriggerShiled(lg));
        }
    }

    private void OnBlockStarted(InputAction.CallbackContext obj) => isBlock = true;

    private void OnBlockCanceled(InputAction.CallbackContext obj)
    {
        isBlock = false;
        anim.SetTrigger("EndBlock");
    }


    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicscheck.isGround)
            {
                break;
            }

            if ((physicscheck.isLeftground && transform.localScale.x < 0f) ||
                (physicscheck.isRightground && transform.localScale.x > 0f))
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,
                transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private IEnumerator TriggerSword(float lg)
    {
        yield return new WaitForSecondsRealtime(lg);
        // 技能动画结束
        isSword = false;
        float time = 60f;
        StartCoroutine(swordSkillTimeControl(time));
    }

    private IEnumerator TriggerShiled(float lg)
    {
        yield return new WaitForSecondsRealtime(lg);
        // 技能动画结束
        isShiled = false;
        float time = 60f;
        StartCoroutine(shiledSkillTimeControl(time));
    }

    private IEnumerator swordSkillTimeControl(float time)
    {
        Debug.Log("技能倒计时");
        yield return new WaitForSecondsRealtime(time);
        // 技能持续时间结束后重置攻击力
        a1.GetComponent<Attack>().damage = 10;
        a2.GetComponent<Attack>().damage = 10;
        a3.GetComponent<Attack>().damage = 20;
        swordIcon.SetActive(false);
        Debug.Log("技能倒计时结束");
    }

    private IEnumerator shiledSkillTimeControl(float time)
    {
        Debug.Log("技能倒计时");
        yield return new WaitForSecondsRealtime(time);
        isShileding = false;
        shiledIcon.SetActive(false);
        Debug.Log("技能倒计时结束");
    }


    public void getHurt(Transform attack)
    {
        //如果在格挡中则播放格挡受伤动画
        if (anim.GetBool("isBlocking"))
        {
            anim.SetTrigger("isBlock");
        }
        else
        {
            isHurt = true;
            rb.velocity = Vector2.zero;
            Vector2 dir = new Vector2(transform.position.x - attack.position.x, 0).normalized;
            rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        }
    }

    public void playerDeath()
    {
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        inputActions.GamePlayer.Disable();
    }

    public void PlayerPar() => RunPar.Play();

    public void cheakState()
    {
        coll.sharedMaterial = physicscheck.isGround ? friction : smooth;

        if (physicscheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
    }
}
