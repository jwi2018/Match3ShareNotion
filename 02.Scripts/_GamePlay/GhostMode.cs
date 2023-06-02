using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostMode : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    [SerializeField] private Image modeImgae;
    [SerializeField] private Text modeText;

    [SerializeField] private GameObject ghostObject;
    [SerializeField] private GameObject nestObject;
    [SerializeField] private GameObject butterflyObject;

    private EditorMode nowMode = EditorMode.NormalMode;

    private void Start()
    {
        ChangeMode();
    }

    public void ChangeMode()
    {
        if (nowMode == EditorMode.NormalMode)
        {
            nowMode = EditorMode.GhostMode;

            modeText.text = "포함";
            
            modeImgae.sprite = onSprite;
            
            ghostObject.SetActive(true);
            nestObject.SetActive(true);
            butterflyObject.SetActive(true);

            StaticGameSettings.IsAbleFish = true;
        }
        else
        {
            nowMode = EditorMode.NormalMode;
            
            modeText.text = "미포함";
            
            modeImgae.sprite = offSprite;
            
            ghostObject.SetActive(false);
            nestObject.SetActive(false);
            butterflyObject.SetActive(false);
            
            StaticGameSettings.IsAbleFish = false;
        }
    }
}
