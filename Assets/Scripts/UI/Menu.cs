using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject newGame;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(newGame);
    }

    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}