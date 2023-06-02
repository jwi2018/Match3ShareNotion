using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotchHighlight : MonoBehaviour
{
    [SerializeField] private Image image = null;
    [SerializeField] private Image tutoBackground = null;
    [SerializeField] private GameObject tutoObj = null;

    private float tutoAlpha;
    private bool isPopupActive = false;

    private void Start()
    {
        //isPopupActive = false;
    }
	private void Update()
    {
        if (image == null || tutoBackground == null || tutoObj == null) return;

        Color32 color = image.color;
        color.a = //Mathf.Max(color.a, tutoBackground.color.a);
            191;

        if(!tutoObj.activeSelf)
        {
            color.a = 0;
        }

        if(isPopupActive)
        {
            color.a = 191;
        }
        else if(!tutoObj.activeSelf)
        {
            color.a = 0;
        }

        image.color = color;

    }

    public void SetPopupActive(bool value)
    {
            isPopupActive = value;
    }
}
