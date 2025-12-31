using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("技能状态")] public bool isHolyHeal;

    public bool isHolySlash;
    public float slashForce;
    public bool isSain;
    public bool isSaining;
    public bool isLightCut;
    public float lightCutDir;
    public float lightSpeed;
    private PlayerInputControl inputActions;
    private PlayerControl playerControl;

    private Property property;
    private bool skill;


    private void Awake()
    {
        property = GetComponent<Property>();
        inputActions = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        playerControl = GetComponent<PlayerControl>();
        skill = playerControl.isAttack && playerControl.isShiled && playerControl.isSword && playerControl.isBlock &&
                playerControl.isSlide;
        inputActions.GamePlayer.Sain.started += Sain;
        inputActions.GamePlayer.HolyHeal.started += HolyHeal;
        inputActions.GamePlayer.HolySlash.started += HolySlash;
        inputActions.GamePlayer.LightCut.started += LightCut;
    }


    private void OnEnable() => inputActions.Enable();

    private void OnDisable() => inputActions.Disable();


    private void Sain(InputAction.CallbackContext obj)
    {
        if (!isSain && !skill)
        {
            isSain = true;
            property.invincible = true;
            isSaining = true;
            float anim = 2f;
            StartCoroutine(OnSkillTime(anim));
        }
    }

    private void HolyHeal(InputAction.CallbackContext obj)
    {
        if (!isHolyHeal && !skill)
        {
            if (property.currentSlidePower >= 15)
            {
                isHolyHeal = true;
                property.currentHealth = property.maxHealth;
                property.currentSlidePower -= 15;
                //及时更新ui
                property.onHealthChange?.Invoke(property);
                float anim = 2f;
                StartCoroutine(OnSkillTime(anim));
            }
        }
    }

    private void HolySlash(InputAction.CallbackContext obj)
    {
        if (!isHolySlash && !skill)
        {
            if (property.currentSlidePower >= 15)
            {
                isHolySlash = true;
                property.currentSlidePower -= 15;
                property.onHealthChange?.Invoke(property);
                // 击退力增强会在攻击判定时生效
                float anim = 2.3f;
                StartCoroutine(OnSkillTime(anim));
            }
        }
    }

    private void LightCut(InputAction.CallbackContext obj)
    {
        if (!isLightCut && !skill)
        {
            isLightCut = true;
            playerControl.inputActions.GamePlayer.Disable();

            var targetPos = new Vector3(transform.position.x + lightCutDir * transform.localScale.x,
                transform.position.y);

            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(OnTriggerLightCut(targetPos));
        }
    }

    private IEnumerator OnSkillTime(float anim)
    {
        if (isSain)
        {
            yield return new WaitForSecondsRealtime(anim);
            isSain = false;
            float time = 30f;
            StartCoroutine(OnTriggerSain(time));
        }

        if (isHolyHeal)
        {
            yield return new WaitForSecondsRealtime(anim);
            isHolyHeal = false;
        }

        if (isHolySlash)
        {
            yield return new WaitForSecondsRealtime(anim);
            isHolySlash = false;
            // 技能结束，击退力恢复正常
        }
    }

    private IEnumerator OnTriggerSain(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        isSaining = false;
        property.invincible = false;
    }

    private IEnumerator OnTriggerLightCut(Vector2 targetPos)
    {
        //蓄力1.5秒
        float chargeTime = 1.5f;
        yield return new WaitForSecondsRealtime(chargeTime);

        do
        {
            yield return null;

            rb.MovePosition(new Vector2(transform.localScale.x * lightSpeed + transform.position.x,
                transform.position.y));
        } while (Mathf.Abs(targetPos.x - transform.position.x) > 0.1f);

        //等待0.55秒退出
        yield return new WaitForSecondsRealtime(0.55f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        isLightCut = false;
        playerControl.inputActions.GamePlayer.Enable();
    }
}
