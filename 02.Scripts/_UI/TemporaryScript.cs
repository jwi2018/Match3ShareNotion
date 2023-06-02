using LogicStates;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryScript : MonoBehaviour
{
    [SerializeField] private Text text;

    private int Count;

    public void Start()
    {
        EditorAutoModeControll.TestStageList.Clear();
        Count = EditorAutoModeControll.TestStageList.Count;
        text.text = "Auto ";
        text.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (EditorAutoModeControll.TestStageList.Count != Count)
        {
            Count = EditorAutoModeControll.TestStageList.Count;
            var textstring = "Auto ";

            for (var i = 0; i < Count; i++)
                if (i == Count - 1)
                    textstring += EditorAutoModeControll.TestStageList[i].ToString();
                else
                    textstring += EditorAutoModeControll.TestStageList[i] + ", ";
            text.text = textstring;
        }
    }

    public void OnClickAuto()
    {
        if (EditorAutoModeControll._isAutoMode == false)
        {
            EditorAutoModeControll._isAutoMode = true;
            text.gameObject.SetActive(true);
            if (LogicManager.GetInstance != null)
                if (LogicManager.GetInstance.GetStateName() == "WAITUSERINPUT")
                    LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
        }
        else
        {
            EditorAutoModeControll._isAutoMode = false;
            text.gameObject.SetActive(false);
        }
    }
}