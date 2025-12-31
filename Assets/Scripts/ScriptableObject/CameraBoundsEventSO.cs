using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CameraBoundsEventSO")]
public class CameraBoundsEventSO : ScriptableObject
{
    public UnityAction onBoundsChange;
}