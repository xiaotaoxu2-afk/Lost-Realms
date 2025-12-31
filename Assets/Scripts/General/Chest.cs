using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private Animator anim;
    private bool isOpen;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }



    public void OnTiggerAction()
    {
        if (!isOpen)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        anim.SetBool("isopen", true);
        isOpen = true;
        // 新增：动画播放完后消失（假设动画名为 "Open"，长度通过 clip 获取）
        float animLength = 2f;  // 默认1秒，如果没找到
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "OpenChest")  // 改成你的打开动画 clip 名字
            {
                animLength = clip.length;
                break;
            }
        }
        StartCoroutine(DestroyAfterDelay(animLength));
    }

    // 新增协程
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}