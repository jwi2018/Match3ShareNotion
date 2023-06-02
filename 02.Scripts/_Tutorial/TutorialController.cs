using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ETutorialCondition
{
    NONE = 0,
    SWAP = 1,
    TOUCH = 2
}

public class TutorialChapter
{
    public string charAnimTrigger;
    public Vector2 charPosition = new Vector2();
    public ETutorialCondition condition;
    public string explanation;

    public List<Vector2Int> highlightPosition = new List<Vector2Int>();

    public bool isDarkBackground;
    public bool isShowCharacter;
    public bool isShowHand;
    public bool isTextboxDown;

    public EDirection swapDirection = EDirection.NONE;
    public Vector2Int swapPosition;
}

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Image darkBackground;
    [SerializeField] private Animator charAnimator;
    [SerializeField] private RectTransform charRectTransform;
    [SerializeField] private Text explanationText;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private RectTransform handRectTransform;
    [SerializeField] private RectTransform textboxRectTransform;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject nextTextObject;
    [SerializeField] private Localize tutoComponent;
    [SerializeField] private GameObject TutorialBlackImage;
    [SerializeField] private GameObject MoveCount;
    [SerializeField] private GameObject HandArrowUp;
    [SerializeField] private GameObject HandArrowDown;
    [SerializeField] private GameObject HandArrowLeft;
    [SerializeField] private GameObject HandArrowRight;

    [SerializeField] private List<GameObject> disableObjects = new List<GameObject>();
    [SerializeField] private List<Image> darkImages = new List<Image>();
    public bool isFirstTurn = true;
    public Vector2 TextBoxYTopPosition = new Vector2(0, 225);
    public Vector2 TextBoxYDownPosition = new Vector2(0, -300);

    private readonly List<TutorialChapter> chapters = new List<TutorialChapter>();

    private int cNumber;
    private float touchDelayTime;
    [SerializeField] private bool isControlAnchorsPos = true;
    [SerializeField] private bool textBoxNotMoving;

    public ETutorialCondition Condition { get; private set; } = ETutorialCondition.NONE;

    public EDirection SwapDirection { get; private set; } = EDirection.NONE;

    public Vector2Int SwapPosition { get; private set; }

    private void FixedUpdate()
    {
        if (touchDelayTime > 0f)
        {
            touchDelayTime -= Time.deltaTime;
            touchDelayTime = Mathf.Max(0, touchDelayTime);
        }
    }

    private void OnMouseDown()
    {
        ConditionClear(ETutorialCondition.TOUCH);
    }

    public void Init()
    {
        /*
        TutorialChapter chapter = new TutorialChapter();
        chapter.condition = ETutorialCondition.TOUCH;
        chapter.isDarkBackground = true;
        chapter.isShowCharacter = false;
        chapter.isShowHand = false;
        chapter.explanation = "나머지 오리도 가져오세요";
        chapter.charPosition = new Vector2(150, 232);

        chapter.highlightPosition.Add(new Vector2Int(3, 7));
        chapter.highlightPosition.Add(new Vector2Int(4, 7));
        chapter.highlightPosition.Add(new Vector2Int(5, 7));

        TutorialChapter chapter2 = new TutorialChapter();
        chapter2.condition = ETutorialCondition.SWAP;
        chapter2.isDarkBackground = true;
        chapter2.isShowCharacter = false;
        chapter2.isShowHand = true;
        chapter2.explanation = "오리 주변에서 블록을 매치하면 오리를 가져 올 수 있습니다";
        chapter2.charPosition = new Vector2(150, 232);

        chapter2.highlightPosition.Add(new Vector2Int(4, 2));
        chapter2.highlightPosition.Add(new Vector2Int(4, 4));
        chapter2.highlightPosition.Add(new Vector2Int(4, 5));

        chapter2.highlightPosition.Add(new Vector2Int(3, 3));
        chapter2.highlightPosition.Add(new Vector2Int(3, 4));
        chapter2.highlightPosition.Add(new Vector2Int(3, 5));
        chapter2.highlightPosition.Add(new Vector2Int(5, 3));
        chapter2.highlightPosition.Add(new Vector2Int(5, 4));
        chapter2.highlightPosition.Add(new Vector2Int(5, 5));

        chapter2.swapDirection = EDirection.DOWN;
        chapter2.swapPosition = new Vector2Int(4, 2);

        chapters.Add(chapter2);
        chapters.Add(chapter);
        */

        var pData =
            DataContainer.GetInstance.GetTutorialDataListOrNull(StageManager.StageNumber);

        if (pData == null) return;

        for (var i = 0; i < pData.pracTutoDatas.Length; i++)
        {
            var chapter = new TutorialChapter();
            chapter.condition = (ETutorialCondition)pData.pracTutoDatas[i].condition;
            chapter.explanation = pData.pracTutoDatas[i].explanation;

            chapter.isShowHand = pData.pracTutoDatas[i].isShowHand;
            chapter.isShowCharacter = pData.pracTutoDatas[i].isShowCharacter;
            chapter.isDarkBackground = pData.pracTutoDatas[i].isDarkBackground;

            for (var j = 0; j < pData.pracTutoDatas[i].highlightPosition.Length; j++)
            {
                var highPosition = new Vector2Int();
                highPosition.x = pData.pracTutoDatas[i].highlightPosition[j] / 100;
                highPosition.y = pData.pracTutoDatas[i].highlightPosition[j] - highPosition.x * 100;
                chapter.highlightPosition.Add(highPosition);
            }

            var sPosition = new Vector2Int();
            sPosition.x = pData.pracTutoDatas[i].swapPosition / 100;
            sPosition.y = pData.pracTutoDatas[i].swapPosition - sPosition.x * 100;

            chapter.swapPosition = sPosition;
            chapter.swapDirection = (EDirection)pData.pracTutoDatas[i].swapDirection;

            var cPosition = new Vector2Int();
            cPosition.x = pData.pracTutoDatas[i].charPosition / 1000;
            cPosition.y = pData.pracTutoDatas[i].charPosition - cPosition.x * 1000;

            chapter.charAnimTrigger = pData.pracTutoDatas[i].charAnimTrigger;
            chapter.isTextboxDown = pData.pracTutoDatas[i].isTextboxDown;

            chapters.Add(chapter);
        }
    }

    public void TutorialStart()
    {
        if (chapters.Count == 1)
        {
            skipButton.SetActive(true);
            nextTextObject.SetActive(false);
        }
        else
        {
            skipButton.SetActive(true);
            nextTextObject.SetActive(true);
        }

        TutorialBlackImage.SetActive(true);
        gameObject.SetActive(true);
        Enter();
        foreach (var item in disableObjects) item.SetActive(false);
    }

    public void SetHandStatusExternal(bool value)
    {
        if (value)
            SetHand(chapters[cNumber].isShowHand);
        else
            SetHand(false);
    }

    public void Enter()
    {
        if (StageManager.StageNumber.Equals(20) && cNumber == 2)
        {
            if (MoveCount != null)
            {
                MoveCount.SetActive(true);
            }
        }

        SetCharacter(chapters[cNumber].isShowCharacter);
        SetDarkBackgorund(chapters[cNumber].isDarkBackground);
        SetHighlight(true);
        SetHand(chapters[cNumber].isShowHand);

        if (chapters[cNumber].explanation.Equals("null"))
        {
            textboxRectTransform.gameObject.SetActive(false);
        }
        else
        {
            textboxRectTransform.gameObject.SetActive(true);
            //explanationText.text = chapters[cNumber].explanation;
            tutoComponent.SetTerm(chapters[cNumber].explanation);
        }

        TutorialBlackImage.SetActive(true);
        skipButton.SetActive(true);
        Condition = chapters[cNumber].condition;
        SwapDirection = chapters[cNumber].swapDirection;
        SwapPosition = chapters[cNumber].swapPosition;

        if (isControlAnchorsPos == true)
        {
            if (textboxRectTransform != null)
            {
                if (chapters[cNumber].isTextboxDown)
                {
                    if (!textBoxNotMoving)
                    {
                        textboxRectTransform.anchoredPosition = TextBoxYDownPosition;
                    }

                    if (charRectTransform != null)
                    {
                        //charRectTransform.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!textBoxNotMoving)
                    {
                        textboxRectTransform.anchoredPosition = TextBoxYTopPosition;
                    }
                }
            }
        }

        foreach (var item in disableObjects) item.SetActive(false);
    }

    public void Exit()
    {
        SetCharacter(false);
        SetDarkBackgorund(false);
        SetHighlight(false);
        SetHand(false);

        TutorialBlackImage.SetActive(false);
        Condition = ETutorialCondition.NONE;
        SwapDirection = EDirection.NONE;
        SwapPosition = Vector2Int.zero;
    }

    public void ConditionClear(ETutorialCondition _condition)
    {
        if (_condition == ETutorialCondition.NONE) return;
        if (_condition == ETutorialCondition.SWAP && isFirstTurn)
        {
            isFirstTurn = false;
            return;
        }

        if (Condition == _condition)
        {
            if (Condition == ETutorialCondition.TOUCH && touchDelayTime > 0f) return;
            touchDelayTime = 0.5f;
            NextChapter();
        }
    }

    public void ConditionClear(int _condition)
    {
        if (Condition == (ETutorialCondition)_condition) NextChapter();
    }

    public void NextChapter()
    {
        Exit();
        cNumber++;
        if (cNumber >= chapters.Count)
        {
            EndTutorial();
        }
        else
            Enter();
    }
    public void EndTutorial()
    {
        Exit();
        if (StageManager.StageNumber.Equals(81))
        {
            StopCoroutine(GravityCo());
        }

        BlockManager.GetInstance.HighlightOff();
        SetDarkBackgorund(false);
        HideHand();
        gameObject.SetActive(false);
        Condition = ETutorialCondition.NONE;
        foreach (var item in disableObjects) item.SetActive(true);

        if (MoveCount != null)
        {
            MoveCount.SetActive(false);
        }
        
        TutorialBlackImage.SetActive(false);
        //2022 02 04 - rail & 20스테이지 이슈로 잠시 주석 처리.
        // if (cNumber.Equals(1))
        // {
        //     //cNumber--;
        //     SetHighlight(false);
        //     //cNumber++;
        // }
        // else if (cNumber.Equals(2))
        // {
        //     cNumber = 0;
        //     SetHighlight(false);
        //     cNumber = 2;
        // }
        // else if (cNumber.Equals(3))
        // {
        //     cNumber = 0;
        //     SetHighlight(false);
        //     cNumber = 3;
        // }
        // else
        // {
        //     SetHighlight(false);
        // }

        //SetHighlight(false);
        //chapters.Clear();
        //StageManager.GetInstance.ShowReward_PlayAdButton();
    }

    public void SetDarkBackgorund(bool value)
    {
        if (darkBackground != null)
        {
            darkBackground.gameObject.SetActive(value);
            darkBackground.enabled = value;

            foreach (var item in darkImages)
                if (value)
                    item.color = new Color32(66, 66, 66, 255);
                else
                    item.color = new Color32(255, 255, 255, 255);
        }
    }

    public void SetCharacter(bool value)
    {
        if (value)
        {
            if (charRectTransform != null)
                charRectTransform.gameObject.SetActive(true);
            //charRectTransform.anchoredPosition = chapters[cNumber].charPosition;

            //charAnimator.SetTrigger(chapters[cNumber].charAnimTrigger);
        }
        else
        {
            if (charRectTransform != null) charRectTransform.gameObject.SetActive(false);
        }
    }

    public void SetHand(bool value)
    {
        if (value)
        {
            if (handAnimator != null && handRectTransform != null)
            {
                handRectTransform.gameObject.SetActive(true);
                handRectTransform.anchoredPosition =
                    TileManager.GetInstance.GetTilePosition(chapters[cNumber].swapPosition) * 100;

                handAnimator.SetBool("Up", false);
                handAnimator.SetBool("Down", false);
                handAnimator.SetBool("Left", false);
                handAnimator.SetBool("Right", false);
                //StaticScript.SetActiveCheckNULL(HandArrowUp, false);
                //StaticScript.SetActiveCheckNULL(HandArrowDown, false);
                //StaticScript.SetActiveCheckNULL(HandArrowLeft, false);
                //StaticScript.SetActiveCheckNULL(HandArrowRight, false);
                switch (chapters[cNumber].swapDirection)
                {
                    case EDirection.UP:
                        handAnimator.SetBool("Up", true);
                        //StaticScript.SetActiveCheckNULL(HandArrowUp, true);
                        break;

                    case EDirection.DOWN:
                        handAnimator.SetBool("Down", true);
                        //StaticScript.SetActiveCheckNULL(HandArrowDown, true);
                        break;

                    case EDirection.LEFT:
                        handAnimator.SetBool("Left", true);
                        //StaticScript.SetActiveCheckNULL(HandArrowLeft, true);
                        break;

                    case EDirection.RIGHT:
                        handAnimator.SetBool("Right", true);
                        //StaticScript.SetActiveCheckNULL(HandArrowRight, true);
                        break;
                }
            }
        }
        else
        {
            if (handAnimator != null && handRectTransform != null)
            {
                handAnimator.SetBool("Up", false);
                handAnimator.SetBool("Down", false);
                handAnimator.SetBool("Left", false);
                handAnimator.SetBool("Right", false);
                //StaticScript.SetActiveCheckNULL(HandArrowUp, false);
                //StaticScript.SetActiveCheckNULL(HandArrowDown, false);
                //StaticScript.SetActiveCheckNULL(HandArrowLeft, false);
                //StaticScript.SetActiveCheckNULL(HandArrowRight, false);

                handRectTransform.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator GravityCo()
    {
        while (true)
        {
            TileManager.GetInstance.ShowGravity();

            yield return new WaitForSeconds(3f);
        }
    }

    public void SetHighlight(bool value)
    {
        if (value)
        {
            if (StageManager.StageNumber.Equals(81))
            {
                StartCoroutine(GravityCo());
            }
        }

        if (cNumber >= chapters.Count) return;
        
        foreach (var position in chapters[cNumber].highlightPosition)
        {
            TileManager.GetInstance.SetSpecialHighlight(value, position);

            var tile = TileManager.GetInstance.GetTileOrNull(position);
            if (BaseSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance != null)
                {
                    if (value)
                    {
                        tile.TileRenderer.maskInteraction = SpriteMaskInteraction.None;
                        tile.TileRenderer.gameObject.layer = 5;
                        tile.TileRenderer.sortingOrder = 1001;
                    }
                    else
                    {
                        tile.TileRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                        tile.TileRenderer.gameObject.layer = 0;
                        tile.TileRenderer.sortingOrder = 0;
                    }
                }
            }

            if (tile == null) continue;

            var block = tile.GetBlockOrNULL(EDepth.FLOOR);
            if (block != null) block.SetHighlight(value);
            block = tile.GetBlockOrNULL(EDepth.NORMAL);
            if (block != null) block.SetHighlight(value);
            block = tile.GetBlockOrNULL(EDepth.TOP);
            if (block != null) block.SetHighlight(value);

            var arrow = tile.transform.GetComponentInChildren<Arrow>();
            if (arrow != null)
            {
                if (value)
                {
                    arrow.SetHighlightArrow(true);
                }
                else
                {
                    arrow.SetHighlightArrow(false);
                }
            }

        }
    }

    public void HideHand()
    {
        if (handRectTransform != null) handRectTransform.gameObject.SetActive(false);
    }

    public void CloseHighlight()
    {
        TutorialBlackImage.SetActive(false);

        if (!gameObject.activeSelf) return;

        if (chapters.Count > 1)
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
        skipButton.SetActive(false);
        nextTextObject.SetActive(false);
        SetDarkBackgorund(false);
        textboxRectTransform.gameObject.SetActive(false);
        var value = false;
        if (charRectTransform != null) charRectTransform.gameObject.SetActive(false);
        foreach (var position in chapters[cNumber].highlightPosition)
        {
            TileManager.GetInstance.SetSpecialHighlight(value, position);

            var tile = TileManager.GetInstance.GetTileOrNull(position);

            if (BaseSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance != null)
                {
                    tile.TileRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    tile.TileRenderer.gameObject.layer = 0;
                    tile.TileRenderer.sortingOrder = 0;
                }
            }

            if (tile == null) continue;

            var block = tile.GetBlockOrNULL(EDepth.FLOOR);
            if (block != null) block.SetHighlight(value);
            block = tile.GetBlockOrNULL(EDepth.NORMAL);
            if (block != null) block.SetHighlight(value);
            block = tile.GetBlockOrNULL(EDepth.TOP);
            if (block != null) block.SetHighlight(value);
            /*
            var arrow = tile.transform.GetComponentInChildren<Arrow>();

            if (arrow != null)
            {
                arrow.SetHighlightArrow(false);
            }
            */
        }
    }
}