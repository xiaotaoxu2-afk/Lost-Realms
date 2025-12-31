using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using UnityEngine;

public class BoundsTrigger : MonoBehaviour
{
    public CameraBoundsEventSO cameraBounds;

    private void Start()
    {
        cameraBounds?.onBoundsChange?.Invoke();
    }
}
