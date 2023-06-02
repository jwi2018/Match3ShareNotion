using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class TriggerDetector : MonoBehaviour
{
    [HideInInspector] public Action<GameObject> actTriggerEnter = null;
    
    [SerializeField] private RectTransform PinPivot;
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        PinPivot.DORotate(new Vector3(0, 0, 160f), 1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PinPivot.DORotate(new Vector3(0, 0, 45f), 1f);
    }
}