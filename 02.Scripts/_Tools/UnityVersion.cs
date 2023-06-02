using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityVersion : MonoBehaviour
{
    public Text versionText = null;
    void Start()
    {
        string bn = Application.version + "_" +Application.unityVersion;
        versionText.text = bn;
    }
}
