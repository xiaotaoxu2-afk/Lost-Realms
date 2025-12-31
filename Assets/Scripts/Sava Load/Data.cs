using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public string sceneToSave;
    public Dictionary<string, SerializeVector3> characterPosDirt = new Dictionary<string, SerializeVector3>();
    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();

    public void sceneGameSave(GameSceneSO savedscene)
    {
        sceneToSave = JsonUtility.ToJson(savedscene);
    }

    public GameSceneSO GetSavedScene()
    {
        var newscene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newscene);
        return newscene;
    }
}

public class SerializeVector3
{
    public float x, y, z;

    public SerializeVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 toVector3()
    {
        return new Vector3(x, y, z);
    }
}