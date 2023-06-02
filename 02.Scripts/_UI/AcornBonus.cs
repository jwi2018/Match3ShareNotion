using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcornBonus : MonoBehaviour
{
    [SerializeField] private Text AcornValueText;

    private void Start()
    {
        AcornValueText.text = PlayerData.GetInstance.Acorn.ToString();
    }
}
