using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, Isaveable
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;

    public VoidSO newGameEventSO;
    public VoidSO backToMenuEvent;

    [Header("广播")]
    public VoidSO afterSceneLoadEvent;

    public SceneLoadEventSO sceneUnLoadEvent;

    public FadeEventSO fadeEventSO;

    [Header("场景")]
    public GameSceneSO menuScene;

    public GameSceneSO firstLoadScene;
    private GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;

    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;

    public float fadeTime;

    public Transform playerTransform;
    public Vector3 firstTransform;
    public Vector3 menuTransform;

    private void Awake()
    {
    }

    private void Start()
    {
        //NewGame();
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuTransform, false);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEventSO.VoidEvent += NewGame;
        backToMenuEvent.VoidEvent += OnBackToMenuEvent;

        Isaveable isaveable = this;
        isaveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEventSO.VoidEvent -= NewGame;
        backToMenuEvent.VoidEvent += OnBackToMenuEvent;

        Isaveable isaveable = this;
        isaveable.RegisterSaveData();
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstTransform, true);
    }

    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuTransform, true);
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    // 卸载当前场景
    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //TODO: 渐变
            fadeEventSO.fadeIn(fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        sceneUnLoadEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, false);

        yield return currentLoadedScene.sceneReference.UnLoadScene();

        // 在加载过程中隐藏玩家
        playerTransform.gameObject.SetActive(false);

        //加载新场景
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        // 异步加载新场景，并监听完成事件
        var handle = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        handle.Completed += OnNewSceneLoaded;  // 关键：加载完成后传送玩家
    }

    /// <summary>
    /// 场景加载完成后的回调
    /// </summary>
    /// <param name="handle"></param>
    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            currentLoadedScene = sceneToLoad;

            if (playerTransform != null)
            {
                playerTransform.position = positionToGo;
                playerTransform.gameObject.SetActive(true);
            }

            if (fadeScreen)
            {
                //TODO: 渐变
                fadeEventSO.fadeOut(fadeTime);
            }

            isLoading = false;
        }
        if (currentLoadedScene.sceneType != SceneType.Menu)
            afterSceneLoadEvent.RaiseEvent();
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void LoadSaveData(Data data)
    {
        var playerID = playerTransform.GetComponent<DataDefinition>().id;
        if (data.characterPosDirt.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDirt[playerID].toVector3();
            sceneToLoad = data.GetSavedScene();
            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }

    public void GetSaveData(Data data)
    {
        //保存当前加载的场景
        data.sceneGameSave(currentLoadedScene);
    }
}