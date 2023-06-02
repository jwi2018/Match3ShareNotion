using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonFolder : MonoBehaviour
{
    private static SingletonFolder instance = null;
    
    public static SingletonFolder Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
