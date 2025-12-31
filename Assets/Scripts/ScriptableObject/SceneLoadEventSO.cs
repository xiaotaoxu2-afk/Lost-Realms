using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO,Vector3,bool> LoadRequestEvent;
    



    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="sceneSO">要加载的场景</param>
    /// <param name="position">Player的坐标</param>
    /// <param name="fadeScreen">是否要渐入渐出</param>
    public void RaiseLoadRequestEvent(GameSceneSO sceneSO, Vector3 position, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(sceneSO,position,fadeScreen);
    }

}
