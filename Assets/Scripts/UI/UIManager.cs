using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerhealthBar;


    [Header("事件监听")] public CharacterEventSO healthEvent;

    public SceneLoadEventSO unLoadedSceneEvent;
    public VoidSO loadDataEvent;
    public VoidSO gameOverEvent;
    public VoidSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("广播")] public VoidSO pauseEvent;

    [Header("组件")] public GameObject gameOverPanel;

    public GameObject T2;
    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake() => settingsBtn.onClick.AddListener(TogglePausePanel);

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unLoadedSceneEvent.LoadRequestEvent += OnLoadRequestEvent;
        loadDataEvent.VoidEvent += OnLoadDataEvent;
        gameOverEvent.VoidEvent += OnGameOverEvent;
        backToMenuEvent.VoidEvent += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unLoadedSceneEvent.LoadRequestEvent -= OnLoadRequestEvent;
        loadDataEvent.VoidEvent -= OnLoadDataEvent;
        gameOverEvent.VoidEvent -= OnGameOverEvent;
        backToMenuEvent.VoidEvent -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void OnSyncVolumeEvent(float amout) => volumeSlider.value = (amout + 80) / 100;

    private void TogglePausePanel()
    {
        // 如果暂停面板显示则关闭，反之开启
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnLoadDataEvent() => gameOverPanel.SetActive(false);

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(T2);
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 duration, bool fadeIn)
    {
        bool isMenu = sceneToGo.sceneType == SceneType.Menu;

        playerhealthBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Property property)
    {
        float percentage = property.currentHealth / property.maxHealth;
        playerhealthBar.OnHealthChange(percentage);

        playerhealthBar.OnPowerChange(property);
    }
}
