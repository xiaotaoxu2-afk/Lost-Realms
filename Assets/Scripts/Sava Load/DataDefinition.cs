using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataDefinition : MonoBehaviour
{
    public PersistentType persistentType;
    public string id;

    private void OnValidate()
    {
        if (persistentType == PersistentType.ReadWrite)
        {
            if (id == string.Empty)
            {
                id = System.Guid.NewGuid().ToString();
            }
        }
        else
        {
            id = string.Empty;
        }
    }
}