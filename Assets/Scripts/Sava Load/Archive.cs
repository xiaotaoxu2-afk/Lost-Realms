using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Archive : MonoBehaviour
{
    [Header("广播")]
    public VoidSO saveDataEvent;

    private Animator anim;
    private PlayerInputControl playerinput;
    public GameObject sign;
    public bool canPass;

    private void Awake()
    {
        anim = sign.GetComponent<Animator>();

        playerinput = new PlayerInputControl();
        playerinput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerinput.GamePlayer.Confirm.started += OnSaveData;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        playerinput.GamePlayer.Confirm.started -= OnSaveData;
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

    private void OnSaveData(InputAction.CallbackContext context)
    {
        if (canPass)
        {
            //TODO:保存数据
            saveDataEvent.RaiseEvent();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPass = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPass = false;
    }
}