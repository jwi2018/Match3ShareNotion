using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextContentType
{
    Stage,
    Gold,
}

[RequireComponent(typeof(Text))]
public class TextContentConnector : MonoBehaviour
{
    private Text textTarget = null;
    [SerializeField] private string strFormat = "";
    [SerializeField] private TextContentType textContentType = TextContentType.Stage;

    private IEnumerator DelayUpdate(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (textContentType)
        {
            case TextContentType.Stage:

                int icurStage;
                if (PlayerData.GetInstance.PresentLevel == StaticGameSettings.TotalStage) 
                    icurStage = PlayerData.GetInstance.PresentLevel;
                else 
                    icurStage = PlayerData.GetInstance.PresentLevel + 1;


                if (textTarget != null)
                {
                    textTarget.text = string.Format(strFormat, icurStage.ToString("#,##0"));
                }
                break;
        }
    }

    private void Awake()
    {
        textTarget = GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(DelayUpdate(0.005f));
    }

    public void LoadGameData()
    {
        StartCoroutine(DelayUpdate(0.005f));
    }
}