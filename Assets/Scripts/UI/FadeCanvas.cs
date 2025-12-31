using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("事件监听")]
    public FadeEventSO fadeEventSO;

    public Image fadeImage;

    public void OnEnable()
    {
        fadeEventSO.OnEventRaised += OnFadeEvent;
    }

    public void OnDisable()
    {
        fadeEventSO.OnEventRaised -= OnFadeEvent;
    }

    public void OnFadeEvent(Color target, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}