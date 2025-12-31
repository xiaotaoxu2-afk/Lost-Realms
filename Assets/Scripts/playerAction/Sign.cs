using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private Animator anim;
    private PlayerInputControl playerinput;
    public GameObject sign;
    public bool canPass;
    private IInteractable targetItem;

    public GameObject playerObject;
    public Animator playeranim;

    private void Awake()
    {
        anim = sign.GetComponent<Animator>();

        playerinput = new PlayerInputControl();
        playerinput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerinput.GamePlayer.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        playerinput.GamePlayer.Confirm.started -= OnConfirm;
        playerinput.Disable();
    }

    private void Update()
    {
        sign.GetComponent<SpriteRenderer>().enabled = canPass;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;
            switch (d.device)
            {
                case Keyboard:
                    anim.Play("Idle");
                    break;
            }
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPass)
        {
            targetItem.OnTiggerAction();
            // 新增：触发角色动画（假设 Animator 有 Trigger "OpenChest"）
            // 安全获取 playeranim（万一没获取到）
            if (playeranim == null)
            {
                playeranim = playerObject.GetComponent<Animator>();
            }

            if (playeranim != null)
            {
                playeranim.SetTrigger("isOpen");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPass = true;
            targetItem = GetComponent<IInteractable>();
            playerObject = other.gameObject;  // 自动赋值 playerObject

            // 新增：自动赋值 playeranim 从 playerObject 获取
            playeranim = playerObject.GetComponent<Animator>();
            if (playeranim == null)
            {
                Debug.LogError("Player 对象上未找到 Animator 组件！", playerObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPass = false;
        playerObject = null;
        playeranim = null;
    }
}