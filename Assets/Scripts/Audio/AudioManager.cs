using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")] public PlayerAudioEventSO BGMEvent;

    public PlayerAudioEventSO FXEvent;
    public FloatEventSO VolumeEvent;
    public VoidSO pauseEvent;

    [Header("广播")] public FloatEventSO syncVolumeEvent;

    public AudioSource BGMSource;
    public AudioSource FXSource;

    public AudioMixer Mixer;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        VolumeEvent.OnEventRaised += OnVolumeEvent;
        pauseEvent.VoidEvent += OnPauseEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        VolumeEvent.OnEventRaised -= OnVolumeEvent;
        pauseEvent.VoidEvent -= OnPauseEvent;
    }

    private void OnPauseEvent()
    {
        float amout;
        Mixer.GetFloat("MasterVolume", out amout);
        syncVolumeEvent.RaiseEvent(amout);
    }

    private void OnVolumeEvent(float amout) => Mixer.SetFloat("Mastervolume", amout * 100 - 80);

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }
}
