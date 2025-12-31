using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    public void OnTiggerAction()
    {
        Debug.Log("Teleport");

        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
