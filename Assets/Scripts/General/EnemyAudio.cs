using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
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
        if (audioSource == null || clips == null || clips.Length <= 3 || clips[3] == null || enemy == null)
        {
            return;
        }

        if (!enemy.isHurt && !enemy.isDeath)
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
}
