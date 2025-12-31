using UnityEngine;

public class BossAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    private BossAI bossAI;

    private void Start()
    {
        bossAI = GetComponent<BossAI>();
        if (bossAI == null)
        {
            Debug.LogError("Enemy 组件缺失！请在对象上附加 Enemy 脚本。");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource 未赋值！请在 Inspector 中拖拽。");
        }

        if (clips == null || clips.Length < 4)
        {
            Debug.LogError("clips 数组未赋值或长度不足4！请检查 Inspector。");
        }
    }

    public void OnHurtAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.PlayOneShot(clips[0]);
        }
    }

    public void OnDeathAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.PlayOneShot(clips[1]);
        }
    }

    public void OnAttackAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.PlayOneShot(clips[2]);
        }
    }

    public void OnWalkAd()
    {
        if (audioSource == null || clips == null || clips.Length <= 3 || clips[3] == null || bossAI == null)
        {
            return;
        }

        if (!bossAI.isHurt && !bossAI.isDeath)
        {
            audioSource.clip = clips[3];
            audioSource.loop = true;
            audioSource.spatialBlend = 1f;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void OnTeleportSAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[4];
        }
    }

    public void OnTeleportEAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[5];
        }
    }

    public void OnJumpAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[6];
        }
    }

    public void OnBlockAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[7];
        }
    }

    public void OnEscapeAd()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[8];
        }
    }

    public void OnAttack1Ad()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[9];
        }
    }

    public void OnAttack2Ad()
    {
        if (audioSource != null && clips != null && clips.Length > 0 && clips[0] != null)
        {
            audioSource.clip = clips[10];
        }
    }
}
