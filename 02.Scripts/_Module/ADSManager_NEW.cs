using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSManager_NEW : MonoBehaviour
{
    public static ADSManager_NEW instance = null;
    private void Awake()
    {
        instance = this;
    }


}
