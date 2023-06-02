using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
internal struct TextColorList
{
    public Color MainColorText;
    public Color TextShadowColor;
    public Color TextOutlineColor;
}

public class StageButton : MonoBehaviour
{
    public float SceneTransTime = 1.0f;

    [SerializeField] private Image _childStars;

    [SerializeField] private Image _childMission;

    [SerializeField] private Text stageNumberText;

    [SerializeField] private Shadow stageNumberShadow;

    [SerializeField] private Outline stageNumberOutline;

    [SerializeField] private Transform _plusRewardBox;

    [SerializeField] private TextColorList beforeTextColorList;

    [SerializeField] private TextColorList nowTextColorList;

    [SerializeField] private TextColorList futureTextColorList;

    [SerializeField] private Text startStage = null;

    protected int _stageNum;

    public Text GetText => stageNumberText;

    public Shadow GetShadow => stageNumberShadow;

    public Shadow GetOutline => stageNumberOutline;

    public Transform SetRewardBox
    {
        set => _plusRewardBox = value;
        get => _plusRewardBox;
    }

    public Sprite SetStars
    {
        set
        {
            if (_childStars != null)
            {
                if (value == _childStars.sprite && value != null)
                {
                    _childStars.gameObject.SetActive(true);
                }
                else if (value != _childStars && value != null)
                {
                    _childStars.sprite = value;
                    _childStars.gameObject.SetActive(true);
                }
                else if (value == null)
                {
                    _childStars.gameObject.SetActive(false);
                }
            }
        }
    }

    public Sprite SetMission
    {
        set
        {
            if (_childMission != null)
            {
                _childMission.sprite = value;
                _childMission.SetNativeSize();
                var Min = Mathf.Min(25.0f / _childMission.GetComponent<RectTransform>().sizeDelta.x,
                    25.0f / _childMission.GetComponent<RectTransform>().sizeDelta.y);
                var Sizex = _childMission.GetComponent<RectTransform>().sizeDelta.x * Min;
                var Sizey = _childMission.GetComponent<RectTransform>().sizeDelta.y * Min;
                _childMission.GetComponent<RectTransform>().sizeDelta = new Vector2(Sizex, Sizey);
            }
        }
    }

    public int _getStageNum
    {
        get => _stageNum;
        set
        {
            _stageNum = value;
            stageNumberText.text = "" + _stageNum;
        }
    }

    private void Start()
    {
        if (_childStars == null || _childMission == null)
            if (PlayerData.GetInstance != null)
            {
                _stageNum = PlayerData.GetInstance.PresentLevel + 1;
                if (startStage != null)
                {
                    if (_stageNum > StaticGameSettings.TotalStage)
                    {
                        startStage.text = StaticGameSettings.TotalStage + " stage";
                    }
                    else
                    {
                        startStage.text = _stageNum + " stage";
                    }
                }
                if (_stageNum > StaticGameSettings.TotalStage) _stageNum = StaticGameSettings.TotalStage;
            }
    }

    public List<Color> GetTextColors(int num)
    {
        var colors = new List<Color>();

        switch (num)
        {
            case 0:
                colors.Add(beforeTextColorList.MainColorText);
                colors.Add(beforeTextColorList.TextShadowColor);
                colors.Add(beforeTextColorList.TextOutlineColor);
                break;

            case 1:
                colors.Add(nowTextColorList.MainColorText);
                colors.Add(nowTextColorList.TextShadowColor);
                colors.Add(nowTextColorList.TextOutlineColor);
                break;

            case 2:
                colors.Add(futureTextColorList.MainColorText);
                colors.Add(futureTextColorList.TextShadowColor);
                colors.Add(futureTextColorList.TextOutlineColor);
                break;

            default:
                colors.Add(futureTextColorList.MainColorText);
                colors.Add(futureTextColorList.TextShadowColor);
                colors.Add(futureTextColorList.TextOutlineColor);
                break;
        }

        return colors;
    }

    public void StageStart()
    {
        
        if (_childStars == null || _childMission == null)
        {
            if (EditorAutoModeControll._isAutoMode)
            {
                if (EditorAutoModeControll.TestStageList.Count == 0)
                {
                    return;
                }

                var obj = GameObject.Find("PopupManager");
                obj.GetComponent<PopupManager>().InputStageNumber(EditorAutoModeControll.TestStageList[0]);
                obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
                return;
            }

            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Play_button");
                var obj = GameObject.Find("PopupManager");
                if (_childStars == null || _childMission == null)
                    if (PlayerData.GetInstance != null)
                    {
                        FirebaseManager.GetInstance.FirebaseLogEvent("level_button");
                        _stageNum = PlayerData.GetInstance.PresentLevel + 1;
                        if (_stageNum > StaticGameSettings.TotalStage) _stageNum = StaticGameSettings.TotalStage;
                    }

                obj.GetComponent<PopupManager>().InputStageNumber(_stageNum);
                obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
                return;
            }
        }

        if (!EditorAutoModeControll._isAutoMode)
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Stage_button");
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().InputStageNumber(_stageNum);
            obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
        }
        else
        {
            var _isIn = false;
            for (var i = 0; i < EditorAutoModeControll.TestStageList.Count; i++)
                if (EditorAutoModeControll.TestStageList[i] == _stageNum)
                {
                    _isIn = true;
                    EditorAutoModeControll.TestStageList.RemoveAt(i);
                    break;
                }

            if (!_isIn) EditorAutoModeControll.TestStageList.Add(_stageNum);
        }
    }

    public void FastStageStart()
    {
        //if(PlayerData.GetInstance != null)
        //{
        //    StageInfo.StageNumber = PlayerData.GetInstance.PresentLevel + 1;
        //    if(StageInfo.StageNumber > StageInfo.TotalStage)
        //    {
        //        StageInfo.StageNumber = StageInfo.TotalStage;
        //    }
        //}
        //else
        //{
        //    StageInfo.StageNumber = 1;
        //}
        StartCoroutine("SceneChange");
    }

    public void GameSceneStart()
    {
        var obj = GameObject.Find("PopupManager");
        obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
    }

    private IEnumerator SceneChange()
    {
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator SceneStart()
    {
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ESceneChange()
    {
        yield return new WaitForEndOfFrame();
    }
}