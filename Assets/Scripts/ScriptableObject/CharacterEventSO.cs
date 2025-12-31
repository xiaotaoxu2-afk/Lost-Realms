using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<Property> OnEventRaised; 

    public void RaiseEvent(Property property)
    {
        OnEventRaised?.Invoke(property);
    }
}
