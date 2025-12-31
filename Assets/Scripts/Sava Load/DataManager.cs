using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("事件监听")] public VoidSO saveEvent;

    public VoidSO loadDataEvent;

    private string jsonFolder;

    private readonly List<Isaveable> saveables = new();

    private Data savedata;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        savedata = new Data();

        jsonFolder = Application.persistentDataPath + "/SAVE DATA";

        ReadSaveData();
    }

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            Load();
        }
    }

    private void OnEnable()
    {
        saveEvent.VoidEvent += Save;
        loadDataEvent.VoidEvent += Load;
    }

    private void OnDisable()
    {
        saveEvent.VoidEvent -= Save;
        loadDataEvent.VoidEvent -= Load;
    }

    public void RegisterSaveData(Isaveable saveable)
    {
        if (!saveables.Contains(saveable))
        {
            saveables.Add(saveable);
        }
    }

    public void UnRegisterSaveData(Isaveable saveable) => saveables.Remove(saveable);

    public void Save()
    {
        foreach (Isaveable item in saveables)
        {
            item.GetSaveData(savedata);
        }

        //序列化文件
        string resultPath = jsonFolder + "data.sav";
        string jsonData = JsonConvert.SerializeObject(savedata);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }

        File.WriteAllText(resultPath, jsonData);
    }

    public void Load()

    {
        foreach (Isaveable item in saveables)
        {
            item.LoadSaveData(savedata);
        }
    }

    private void ReadSaveData()
    {
        //反向序列化文件
        string resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            string stringData = File.ReadAllText(resultPath);
            Data jsonData = JsonConvert.DeserializeObject<Data>(stringData);
            savedata = jsonData;
        }
    }
}
