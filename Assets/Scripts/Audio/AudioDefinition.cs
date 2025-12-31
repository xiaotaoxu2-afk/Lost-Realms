using UnityEngine;

public class AudioDefinition : MonoBehaviour
{
    public PlayerAudioEventSO playerAudioEventSO;
    public AudioClip audioClip;
    public bool isEnable;

    private void OnEnable()
    {
        if (isEnable)
        {
            PlayerAudio();
        }
    }

    private void PlayerAudio() => playerAudioEventSO.RaisedEvent(audioClip);
}
