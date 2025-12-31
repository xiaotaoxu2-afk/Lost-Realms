using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/Player Audio Event")]
public class PlayerAudioEventSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRaised;

    public void RaisedEvent(AudioClip clip) => OnEventRaised?.Invoke(clip);
}
