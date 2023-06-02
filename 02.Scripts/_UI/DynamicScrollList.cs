using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollList : MonoBehaviour
{
    private int _LowLevel;
    private int _HighLevel;
    private int _BannerLevel;
    private int MyPos;
    private float mSpacingX;
    private float mPaddingX;

    // 총 스테이지 갯수를 여기에...
    public int totalStage;

    public int MyLevel = 1;

    private int[] missionInfo = null;

    [SerializeField]
    private GameObject _scrollBar;

    [SerializeField]
    private GameObject _stageButton;

    [SerializeField]
    private Transform _myViewPort;

    [SerializeField]
    private GameObject _stageButton_Tint;

    [SerializeField]
    private List<Transform> _moreRewardBox = null;

    [SerializeField]
    private GameObject _imageFrame;

    [SerializeField]
    private GameObject _bannerButton;

    [SerializeField]
    private GameObject _tobeContinue;

    [SerializeField]
    private GameObject _textBox;

    [SerializeField]
    private List<Sprite> StageImage;

    [SerializeField]
    private List<Sprite> StarImage;

    //[SerializeField]
    //private MissionSpriteContainer _missionContainer = null;

    [SerializeField]
    private bool StageOpen;     // obsolete go StageManager

    [SerializeField]
    private int ForceOpenStage = 0;

    private Vector2 StageButtonRect;
    private GridLayoutGroup Layout;
    private int StarCnt;
    private float mViewPosY;
    private float BannerRectYSpacing;
    private float ToBeContinueYSpacing;
    private SpriteContainer container;

    [ContextMenu("StageOpen")]
    public void FuncStageOpen()
    {
        var obj = GameObject.Find("PopupManager");
        obj.GetComponent<PopupManager>().InputStageNumber(ForceOpenStage);
        obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
    }

    private void Start()
    {
        this.GetComponent<ScrollRect>().content = _myViewPort.GetComponent<RectTransform>();
        if (DataContainer.GetInstance != null)
        {
            missionInfo = DataContainer.GetInstance.GetMissionID();
        }
        GameObject obj = GameObject.Find("SpriteContainer");
        if (obj != null)
        {
            container = obj.GetComponent<SpriteContainer>();
        }

        StageButtonRect = _stageButton.GetComponent<RectTransform>().sizeDelta;
        Layout = _myViewPort.GetComponent<GridLayoutGroup>();
        if (PlayerData.GetInstance != null)
        {
            MyLevel = PlayerData.GetInstance.PresentLevel + 1;
        }
        totalStage = StaticGameSettings.TotalStage;
        StarCnt = 1;
        StartCoroutine("SceneStart");
        PopulateList();
    }

    private void PopulateList()
    {
        if (MyLevel > StaticGameSettings.TotalStage)
        {
            _stageButton_Tint.SetActive(false);
        }
        _LowLevel = (int)((MyLevel - 1) * 0.25);
        _BannerLevel = (int)((MyLevel - 1) * 0.25);
        int StartLevelBlock = (_LowLevel * 4);

        mPaddingX = (transform.parent.GetComponent<RectTransform>().sizeDelta.x - (StageButtonRect.x * 4) - (Layout.spacing.x * 4)) / 2;
        mSpacingX = (transform.parent.GetComponent<RectTransform>().sizeDelta.x - (StageButtonRect.x * 4) - (mPaddingX * 2)) / 3;

        _myViewPort.GetComponent<RectTransform>().sizeDelta = new Vector2((StageButtonRect.x + mSpacingX) * 4 + mPaddingX,
                                                                          ((StageButtonRect.y + Layout.spacing.y) * ((totalStage / 4) + 2)) + Layout.padding.top);
        mViewPosY = _myViewPort.localPosition.y;

        float BannerRectXSpacing = (((StageButtonRect.x * 4) + (mSpacingX * 2)) - _imageFrame.GetComponent<RectTransform>().sizeDelta.x) * 0.5f;

        BannerRectYSpacing = (StageButtonRect.y - _imageFrame.GetComponent<RectTransform>().sizeDelta.y) * 0.5f;

        float ToBeContinueXSpacing = (((StageButtonRect.x * 4) + (mSpacingX * 2)) - _tobeContinue.GetComponent<RectTransform>().sizeDelta.x) * 0.5f;
        ToBeContinueYSpacing = (StageButtonRect.y - _tobeContinue.GetComponent<RectTransform>().sizeDelta.y) * 0.5f;

        _imageFrame.GetComponent<RectTransform>().SetParent(_myViewPort, false);
        _tobeContinue.GetComponent<RectTransform>().SetParent(_myViewPort, false);

        for (int i = StartLevelBlock; i < (StartLevelBlock + 32); i++)
        {
            GameObject obj = Instantiate(_stageButton, _myViewPort);

            float YPosition = (i / 4 * -(StageButtonRect.y + Layout.spacing.y)) - Layout.padding.top;
            if (i == StartLevelBlock)
            {
                _imageFrame.transform.localPosition = new Vector2(mPaddingX + mSpacingX + BannerRectXSpacing, YPosition);
            }
            if (i >= StartLevelBlock)
            {
                if (YPosition - BannerRectYSpacing <= _imageFrame.transform.localPosition.y + BannerRectYSpacing)
                {
                    YPosition += -(StageButtonRect.y + Layout.spacing.y);
                }
                else
                {
                    YPosition += -(_imageFrame.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y);
                }
            }
            obj.transform.localPosition = new Vector3(
                (i % 4 * (StageButtonRect.x + mSpacingX) + mPaddingX), YPosition, 0);
            ChangeBtnNum(obj.transform);
        }

        _myViewPort.localPosition = new Vector3(_myViewPort.localPosition.x,
                                                mViewPosY + ((StageButtonRect.y + Layout.spacing.y) * _LowLevel),
                                                _myViewPort.localPosition.z);

        MyPos = (int)((_myViewPort.localPosition.y - mViewPosY) / ((StageButtonRect.y + Layout.spacing.y))) + 1;

        float RectValue = (_myViewPort.localPosition.y - mViewPosY) / (_myViewPort.GetComponent<RectTransform>().sizeDelta.y - (mViewPosY * 2));
        _scrollBar.GetComponent<StageScrollBar>().ScrollRectValueChange(RectValue);
        _tobeContinue.transform.localPosition = new Vector2(mPaddingX + mSpacingX + ToBeContinueXSpacing, -_myViewPort.GetComponent<RectTransform>().sizeDelta.y + StageButtonRect.y + Layout.spacing.y);
    }

    private void ChangeBtnNum(Transform Btn)
    {
        StageButton ComponentButton = Btn.GetComponent<StageButton>();

        Vector3 BtnPos = Btn.GetComponent<RectTransform>().localPosition;
        int numX = (int)(((BtnPos.x - mPaddingX) / ((_stageButton.GetComponent<RectTransform>().sizeDelta.y))) + 1);
        int numY = 0;
        if (_stageButton.GetComponent<RectTransform>().sizeDelta.y <= _imageFrame.GetComponent<RectTransform>().sizeDelta.y)
        {
            numY = (int)((int)Mathf.Abs((BtnPos.y / ((_stageButton.GetComponent<RectTransform>().sizeDelta.y + BannerRectYSpacing)))) * 4);
        }
        else
        {
            numY = (int)((int)Mathf.Abs(((BtnPos.y - (_imageFrame.GetComponent<RectTransform>().sizeDelta.y - Layout.spacing.y - BannerRectYSpacing)) / ((_stageButton.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y)))) * 4);
        }
        if (numY > _BannerLevel * 4)
        {
            numY -= 4;
        }
        ComponentButton._getStageNum = numX + numY;
        if (ComponentButton._getStageNum % 15 == 0)
        {
            if (PlayerData.GetInstance.PresentLevel < ComponentButton._getStageNum)
            {
                for (int i = 0; i < _moreRewardBox.Count; i++)
                {
                    if (_moreRewardBox[i].parent == this.transform && ComponentButton.SetRewardBox == null)
                    {
                        ComponentButton.SetRewardBox = _moreRewardBox[i];
                        int number = ComponentButton.GetText.transform.GetSiblingIndex();
                        _moreRewardBox[i].SetParent(Btn);
                        _moreRewardBox[i].gameObject.SetActive(true);
                        _moreRewardBox[i].localPosition = new Vector3(Btn.GetComponent<RectTransform>().sizeDelta.x * 0.5f, (-Btn.GetComponent<RectTransform>().sizeDelta.y * 0.5f) + 7, 0);
                        _moreRewardBox[i].SetSiblingIndex(number + 1);
                        break;
                    }
                }
            }
        }
        else
        {
            if (ComponentButton.SetRewardBox != null)
            {
                ComponentButton.SetRewardBox.SetParent(this.transform);
                ComponentButton.SetRewardBox.gameObject.SetActive(false);
                ComponentButton.SetRewardBox = null;
            }
            else if (ComponentButton.transform.childCount > 3)
            {
                for (int i = 0; i < ComponentButton.transform.childCount; i++)
                {
                    foreach (var item in _moreRewardBox)
                    {
                        if (item == ComponentButton.transform.GetChild(i))
                        {
                            item.SetParent(this.transform);
                            item.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        if (numX + numY < 0 || numX + numY > totalStage)
        {
            Btn.gameObject.SetActive(false);
            return;
        }
        else Btn.gameObject.SetActive(true);
        int missionImageID = 0;

        if (DataContainer.GetInstance != null)
        {
            if (missionInfo != null)
            {
                if (ComponentButton._getStageNum < missionInfo.Length)
                    missionImageID = missionInfo[ComponentButton._getStageNum - 1];
            }
        }
        if (container != null)
        {
            ComponentButton.SetMission = container.GetMissionImage((EID)missionImageID);
        }

        if (ComponentButton._getStageNum < MyLevel)
        {
            Btn.GetComponent<Image>().sprite = StageImage[1];
            Btn.GetComponent<Button>().enabled = true;

            if (PlayerData.GetInstance != null)
            {
                StarCnt = PlayerData.GetInstance.GetLevelStartCount(ComponentButton._getStageNum);
                StarCnt = Mathf.Max(StarCnt, 1);
            }
            if (_stageButton_Tint.transform.parent == Btn)
            {
                _stageButton_Tint.SetActive(false);
            }
            ComponentButton.SetStars = StarImage[StarCnt - 1];
            List<Color> colors = ComponentButton.GetTextColors(0);
            ComponentButton.GetText.color = colors[0];
            ComponentButton.GetShadow.effectColor = colors[1];
            ComponentButton.GetOutline.effectColor = colors[2];
        }
        else if (ComponentButton._getStageNum == MyLevel)
        {
            if (false) { Btn.GetComponent<Image>().sprite = StageImage[2]; } //Hard
            else if (false) { Btn.GetComponent<Image>().sprite = StageImage[3]; } //VaryHard
            else { Btn.GetComponent<Image>().sprite = StageImage[1]; }

            Btn.GetComponent<Button>().enabled = true;
            _stageButton_Tint.transform.SetParent(Btn.transform);
            _stageButton_Tint.SetActive(true);
            _stageButton_Tint.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            _stageButton_Tint.transform.SetAsFirstSibling();
            ComponentButton.SetStars = null;
            List<Color> colors = ComponentButton.GetTextColors(1);
            ComponentButton.GetText.color = colors[0];
            ComponentButton.GetShadow.effectColor = colors[1];
            ComponentButton.GetOutline.effectColor = colors[2];
        }
        else
        {
            if (_stageButton_Tint.transform.parent == Btn)
            {
                if (_stageButton_Tint.activeSelf != false)
                {
                    _stageButton_Tint.SetActive(false);
                }
            }
            Btn.GetComponent<Image>().sprite = StageImage[0];
            if (StageOpen)
            {
                //Btn.GetComponent<Button>().enabled = true;
            }
            else
            {
                Btn.GetComponent<Button>().enabled = false;
            }
            ComponentButton.SetStars = null;
            List<Color> colors = ComponentButton.GetTextColors(2);
            ComponentButton.GetText.color = colors[0];
            ComponentButton.GetShadow.effectColor = colors[1];
            ComponentButton.GetOutline.effectColor = colors[2];
        }
    }

    public void CheckRecycle()
    {
        float RectValue = (_myViewPort.localPosition.y - mViewPosY) / (_myViewPort.GetComponent<RectTransform>().sizeDelta.y - (mViewPosY * 2));
        _scrollBar.GetComponent<StageScrollBar>().ScrollRectValueChange(RectValue);

        if (_scrollBar.activeSelf == false)
        {
            _scrollBar.SetActive(true);
            _scrollBar.GetComponent<StageScrollBar>().SetScrollActiveDelay = 0.0f;
        }
        else
        {
            _scrollBar.GetComponent<StageScrollBar>().SetScrollActiveDelay = 0.0f;
        }

        int temp = (int)((_myViewPort.localPosition.y - mViewPosY) / ((_stageButton.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y)));
        if (temp >= _BannerLevel)
        {
            temp -= 1;
            if (temp < 0)
            {
                temp = 0;
            }
        }
        if (temp != MyPos && temp >= 0)
        {
            RecycleBtn();
        }
    }

    private void RecycleBtn()
    {
        //MyPos * 4 + 1 = 가장 위의 레벨블럭의 숫자
        MyPos = (int)((_myViewPort.localPosition.y - mViewPosY) / ((_stageButton.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y)));
        int CountMax;
        int CountMin;
        if (MyPos >= _BannerLevel)
        {
            MyPos -= 1;
            if (MyPos < 0)
            {
                MyPos = 0;
            }
        }
        if (MyPos > _LowLevel)
        {
            if ((totalStage * 0.25) - _LowLevel > 8)
            {
                for (int i = 0; i < MyPos - _LowLevel;)
                {
                    Transform Lastchild = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 1);
                    if (Lastchild == _imageFrame.transform || Lastchild == _tobeContinue.transform)
                    {
                        Lastchild = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 2);
                        if (Lastchild == _imageFrame.transform || Lastchild == _tobeContinue.transform)
                        {
                            Lastchild = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 3);
                        }
                    }
                    float YPosition = Lastchild.transform.localPosition.y -
                                                  (_stageButton.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y);

                    float Max = Mathf.Max(Mathf.Abs(YPosition), Mathf.Abs(_imageFrame.transform.localPosition.y));
                    float Min = Mathf.Min(Mathf.Abs(YPosition), Mathf.Abs(_imageFrame.transform.localPosition.y));
                    //if(Max - Min < 10.0f + BannerRectYSpacing)

                    //if (Max - Min < _imageFrame.GetComponent<RectTransform>().sizeDelta.y + (Layout.spacing.y * 2))
                    if (_stageButton.GetComponent<RectTransform>().sizeDelta.y > Max - Min)
                    {
                        YPosition -= (_imageFrame.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y);
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        Transform child = _myViewPort.transform.GetChild(0);
                        if (child == _imageFrame.transform || child == _tobeContinue.transform)
                        {
                            child = _myViewPort.transform.GetChild(1);
                            if (child == _imageFrame.transform || child == _tobeContinue.transform)
                            {
                                child = _myViewPort.transform.GetChild(2);
                            }
                        }
                        child.localPosition = new Vector3(
                            child.localPosition.x,
                            YPosition, 0);
                        child.SetAsLastSibling();

                        CountMax = Mathf.Max(MyPos, _LowLevel);
                        CountMin = Mathf.Min(MyPos, _LowLevel);
                        ChangeBtnNum(child);
                    }
                    _LowLevel++;
                }
            }
        }
        else
        {
            if (_LowLevel != 0)
            {
                for (int i = 0; i > MyPos - _LowLevel;)
                {
                    Transform Firstchild = _myViewPort.transform.GetChild(0);
                    if (Firstchild == _imageFrame.transform || Firstchild == _tobeContinue.transform)
                    {
                        Firstchild = _myViewPort.transform.GetChild(1);
                        if (Firstchild == _imageFrame.transform || Firstchild == _tobeContinue.transform)
                        {
                            Firstchild = _myViewPort.transform.GetChild(2);
                        }
                    }
                    float YPosition = Firstchild.transform.localPosition.y +
                                                    (_stageButton.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y);
                    float Max = Mathf.Max(Mathf.Abs(YPosition), Mathf.Abs(_imageFrame.transform.localPosition.y));
                    float Min = Mathf.Min(Mathf.Abs(YPosition), Mathf.Abs(_imageFrame.transform.localPosition.y));
                    //if (Max - Min < 10.0f + BannerRectYSpacing)
                    if (Max - Min < _imageFrame.GetComponent<RectTransform>().sizeDelta.y)
                    {
                        YPosition += (_imageFrame.GetComponent<RectTransform>().sizeDelta.y + Layout.spacing.y);
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        Transform child = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 1);
                        if (child == _imageFrame.transform || child == _tobeContinue.transform)
                        {
                            child = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 2);
                            if (child == _imageFrame.transform || child == _tobeContinue.transform)
                            {
                                child = _myViewPort.transform.GetChild(_myViewPort.transform.childCount - 3);
                            }
                        }
                        child.localPosition = new Vector3(child.localPosition.x,
                                                          YPosition,
                                                          0);
                        child.SetAsFirstSibling();

                        CountMax = Mathf.Max(MyPos, _LowLevel);
                        CountMin = Mathf.Min(MyPos, _LowLevel);

                        ChangeBtnNum(child);
                    }
                    _LowLevel--;
                }
            }
        }
    }

    private IEnumerator SceneStart()
    {
        /*GameObject obj = GameObject.Find("GUI");
        obj.GetComponent<GUITexture>().color = new Vector4(0, 0, 0, 0.5f);
        while (true)
        {
            float aColor = obj.GetComponent<GUITexture>().color.a;
            aColor -= Time.deltaTime;
            obj.GetComponent<GUITexture>().color = new Vector4(0, 0, 0, aColor);
            yield return new WaitForSeconds(0.005f);
            if (obj.GetComponent<GUITexture>().color.a <= 0)
            {
                break;
            }
        }*/
        yield return new WaitForEndOfFrame();
    }

    public void ScrollBarValueChange(float ScrollBarValue)
    {
        _myViewPort.localPosition = new Vector3(_myViewPort.localPosition.x,
                                                (mViewPosY * (1 - (ScrollBarValue * 2))) + (_myViewPort.GetComponent<RectTransform>().sizeDelta.y * ScrollBarValue), 0);
        _scrollBar.GetComponent<StageScrollBar>().SetScrollActiveDelay = 0.0f;
    }

    public void ResettingButton()
    {
        if (PlayerData.GetInstance != null)
        {
            MyLevel = PlayerData.GetInstance.PresentLevel + 1;
            for (int i = _myViewPort.transform.childCount - 1; i >= 0; i--)
            {
                if (_myViewPort.GetChild(i).GetComponent<StageButton>() != null)
                {
                    ChangeBtnNum(_myViewPort.GetChild(i));
                }
            }
        }
        _myViewPort.localPosition = new Vector3(_myViewPort.localPosition.x,
                                                mViewPosY + ((Layout.spacing.y) + (Layout.cellSize.y)) * (int)((MyLevel - 1) * 0.25), 0);
    }

    public void OnClickCrossBanner()
    {
    }
}