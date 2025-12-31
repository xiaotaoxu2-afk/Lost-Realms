using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/Float Event")]
public class FloatEventSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float amount) => OnEventRaised?.Invoke(amount);
}
