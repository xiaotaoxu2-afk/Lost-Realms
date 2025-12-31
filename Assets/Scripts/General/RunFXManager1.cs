using UnityEngine;

public class RunFXManager1 : StateMachineBehaviour
{
    public AudioClip clip;

    private AudioSource audioSource;
    private PlayerControl player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.GetComponent<AudioSource>();
        player = animator.GetComponent<PlayerControl>();

        // 进入时先执行一次“该播就播/该停就停”的逻辑
        RefreshRunSound();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 离开状态时，确保停止“跑步循环音”
        StopRunSound();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
        RefreshRunSound();

    private void RefreshRunSound()
    {
        if (audioSource == null || player == null || clip == null)
        {
            return;
        }

        // ✅ 你想要的规则：slide 或 hurt 时不播放，其它时候播放
        bool shouldPlay = !player.isSlide && !player.isHurt;

        if (shouldPlay)
        {
            // 如果刚刚因为 slide 停掉了，现在 slide 结束了，就自动补播回来
            if (!audioSource.isPlaying || audioSource.clip != clip || !audioSource.loop)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // slide/hurt 时停掉（注意别用 !player.isHurt 限制 stop）
            if (audioSource.isPlaying && audioSource.clip == clip)
            {
                audioSource.Stop();
            }

            // 如果当前就是跑步clip，顺手把 loop 关掉，避免别的地方复用出问题
            if (audioSource.clip == clip)
            {
                audioSource.loop = false;
            }
        }
    }

    private void StopRunSound()
    {
        if (audioSource == null)
        {
            return;
        }

        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            audioSource.Stop();
        }

        if (audioSource.clip == clip)
        {
            audioSource.loop = false;
        }
    }
}
