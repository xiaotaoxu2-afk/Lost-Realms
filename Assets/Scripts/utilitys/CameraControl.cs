using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;

    [Header("事件监听")]
    public CameraBoundsEventSO boundsEvent;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    //TODO:场景切换时调用
    public void OnEnable()
    {
        boundsEvent.onBoundsChange += GetNewCameraBounds;
    }

    public void OnDisable()
    {
        boundsEvent.onBoundsChange -= GetNewCameraBounds;
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }
}