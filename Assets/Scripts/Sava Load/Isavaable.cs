public interface Isaveable
{
    DataDefinition GetDataID();

    void RegisterSaveData() => DataManager.instance.RegisterSaveData(this);

    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveData(this);

    void LoadSaveData(Data data);

    void GetSaveData(Data data);
}