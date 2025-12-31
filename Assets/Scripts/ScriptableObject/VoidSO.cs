using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/VoidSO")]
public class VoidSO : ScriptableObject
{
    public UnityAction VoidEvent;

    public void RaiseEvent()
    {
        VoidEvent?.Invoke();
    }
}