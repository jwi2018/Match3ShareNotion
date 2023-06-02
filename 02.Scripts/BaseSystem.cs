using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseSystem : Singleton<BaseSystem>
{
    private Dictionary<string, bool> systemList = new Dictionary<string, bool>();
    void Start()
    {
        var loadText = Resources.Load<TextAsset>("SystemListData");
        systemList = JsonConvert.DeserializeObject<Dictionary<string, bool>>(loadText.text);

        foreach (var key in systemList.Keys)
        {
            if (systemList[key])
            {
                gameObject.AddComponent(Type.GetType(key));
            }
        }
    }

    public bool GetSystemList(string SystemName)
    {
        foreach (var system in systemList)
        {
            if (system.Key.Equals(SystemName) && system.Value.Equals(true))
            {
                return true;
            }
        }
        return false;
    }
}
