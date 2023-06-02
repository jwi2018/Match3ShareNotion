using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private PopupManager popupManager;

    [SerializeField] private Text moveCountText;
    [SerializeField] private Text stageNumberText;
    [SerializeField] private Text stageNumberText_Test;
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject[] missionContainer;
    [SerializeField] private Image[] missionImages;
    [SerializeField] private Text[] missionCounts;
    [SerializeField] private GameObject[] missionChecks;

    [SerializeField] private GameObject[] itemOutlines;
    [SerializeField] private Text[] itemCounts;
    [SerializeField] private Text[] cashTutoItemCounts;
    [SerializeField] private GameObject[] itemCountBox;
    [SerializeField] private GameObject[] itemLockBox;
    [SerializeField] private Image[] itemLockBackColor;
    [SerializeField] private GameObject[] itemUnLockObj;

    [SerializeField] private Image scoreBar;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private Transform moveCount;

    [SerializeField] private List<GameObject> fadeoutWhenGameClear = new List<GameObject>();
    [SerializeField] private GameObject skipTextObject;

    [SerializeField] private GameObject reward_PlayItemObj;

    [SerializeField] private ItemGroup itemGroup;
    [SerializeField] private ItemGroup cashTutoItemGroup;

    [SerializeField] private SkeletonGraphic skelGameInfoChar;

    [SerializeField] private Text tutoMoveCountText;

    private bool isClearPopupOn = false;
    private bool isReward_PlayItem;

    private void Awake()
    {
        if (null != WCGoogleSheetMapAnalyze.instance)
        {
            WCGoogleSheetMapAnalyze.instance.Refresh();
        }
    }

    public void SetMoveCount(int count)
    {
        if (moveCountText != null)
        {
            moveCountText.gameObject.SetActive(true);
            moveCountText.text = count.ToString();
            if (tutoMoveCountText != null)
            {
                tutoMoveCountText.text = count.ToString();
            }
        }
    }

    public void ItemGroupRefresh()
    {
        if (itemGroup != null)
        {
            itemGroup.Refresh();
            cashTutoItemGroup.Refresh();
        }
    }

    public void SetStageNumber(int count)
    {
        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                if (stageNumberText != null)
                    stageNumberText.text = $"Event {count}";
                return;
            }
            else
            {
                if (stageNumberText != null)
                    stageNumberText.text = count.ToString();
            }
        }
        else
        {
            if (stageNumberText != null)
                stageNumberText.text = count.ToString();
        }

        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                stageNumberText.text = "★";
            }
            else
            {
                stageNumberText.text = count.ToString();
            }
        }

        // if (stageNumberText != null)
        //     stageNumberText.text = count.ToString();

        // if (stageNumberText_Test != null)
        //     stageNumberText_Test.text = count.ToString();
    }

    public void SetScore(int count)
    {
        if (scoreText != null)
            scoreText.text = count.ToString("#,##0");
    }

    public void SetMissionCount(int index, int count)
    {
        if (index < 0 || index >= missionCounts.Length) return;
        if (missionCounts[index] == null) return;

        if (missionContainer.Length != 0) missionContainer[index].SetActive(true);
        if (count != 0)
        {
            missionCounts[index].gameObject.SetActive(true);
            missionCounts[index].text = count.ToString();
        }
        else
        {
            missionCounts[index].text = count.ToString();
            SetMissionCheck(index);
        }
    }

    public void SetMissionImage(int index, Sprite sprite)
    {
        if (index < 0 || index >= missionImages.Length) return;
        if (missionImages[index] == null) return;

        missionImages[index].gameObject.SetActive(true);
        missionImages[index].sprite = sprite;
        missionCounts[index].gameObject.SetActive(true);

        if (index < 0 || index >= missionChecks.Length) return;
        missionChecks[index].SetActive(false);
    }

    public void SetMissionCheck(int index)
    {
        if (index < 0 || index >= missionChecks.Length) return;

        missionCounts[index].gameObject.SetActive(false);
        missionChecks[index].SetActive(true);
    }

    public void SetActiceFalse()
    {
        if (moveCountText != null) moveCountText.gameObject.SetActive(false);
        if (stageNumberText != null)
        {
            //stageNumberText.gameObject.SetActive(false);
        }

        if (scoreText != null) scoreText.gameObject.SetActive(false);
        foreach (var mission in missionCounts) mission.gameObject.SetActive(false);
        foreach (var mission in missionImages) mission.gameObject.SetActive(false);
        foreach (var mission in missionContainer) mission.gameObject.SetActive(false);
    }

    public void AnimationChangeForGameInfoCharacter(string setAni, float delayTime)
    {
        if (skelGameInfoChar != null)
        {
            if (skelGameInfoChar.AnimationState.GetCurrent(0).Animation.Name != setAni)
            {
                StartCoroutine(IE_AnimationChangeForGameInfoCharacter(setAni, delayTime));
            }
        }
    }

    private IEnumerator IE_AnimationChangeForGameInfoCharacter(string setAni, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (skelGameInfoChar != null)
        {
            skelGameInfoChar.AnimationState.SetAnimation(0, setAni, true);
        }
    }

    public void ShowVictoryPopup()
    {
        popupManager.OnCongratulationClear();
    }

    public void ShowFailPopup(int count)
    {
        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                ChallengeSystem.GetInstance.IsChallengeStage = false;
                popupManager.ShowChallengeFailPopup();
                return;
            }
        }

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                popupManager.ShowEventLevelFail();
                return;
            }
        }

        popupManager.OnBuyToRemainMove(count);
    }

    public void ShowAddMoveCountPopup(int type = 0)
    {
        DailyQuestManager.CollectMission(EDailyQuestType.USERMOVEADREWARD, 1);
        popupManager.OnClickMovePlus(type);
    }

    public void ShowItemPopup(int number)
    {
        popupManager.OnClickUseItem(number);
    }

    public void ClickBuyItem(int number)
    {
        popupManager.OnClickBuyItem(number);
    }

    public void ShowClearPopup(int starCount)
    {
        //BlockManager.GetInstance.
        if (starCount == 0) starCount = 1;

        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                ChallengeSystem.GetInstance.IsChallengeStage = false;
                popupManager.ShowChallengeClearPopup();

                return;
            }
        }

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                //EventLevelSystem.GetInstance.IsEventLevel = false;
                popupManager.ShowEventLevelClear();
                return;
            }
        }

        popupManager.OnMissionClear(starCount);
    }

    public void ShowReward_PlayAdButton()
    {
        reward_PlayItemObj.GetComponent<PlaySceneReward>().ItemChange();
        reward_PlayItemObj.GetComponent<Animator>().SetTrigger("Show");
    }

    public void ShowFiveCountPopup()
    {
        popupManager.OnMoveWarring();
        if (!isReward_PlayItem)
        {
            if (!reward_PlayItemObj.activeSelf)
            {
                reward_PlayItemObj.SetActive(true);
                reward_PlayItemObj.GetComponent<PlaySceneReward>().ItemChange();
                reward_PlayItemObj.GetComponent<Animator>().SetTrigger("Show");
            }

            reward_PlayItemObj.GetComponent<Animator>().SetTrigger("Impact");
            isReward_PlayItem = true;
        }
    }

    public void OffItemOutline()
    {
        foreach (var item in itemOutlines)
        {
            if (item != null) item.SetActive(false);
        }
    }

    public void OnItemOutline(int num)
    {
        OffItemOutline();
        if (itemOutlines[num] != null) itemOutlines[num].SetActive(true);
    }

    public void CancelItem()
    {
        popupManager.OnBackKey();
    }

    public void SetItemCount(int num, int num2, int num3, int num4)
    {
        if (itemCounts == null || cashTutoItemCounts == null) return;
        if (num == 0)
        {
            itemCounts[0].text = "+";
            cashTutoItemCounts[0].text = "+";
        }
        else
        {
            itemCounts[0].text = num.ToString();
            cashTutoItemCounts[0].text = num.ToString();
        }

        if (num2 == 0)
        {
            itemCounts[1].text = "+";
            cashTutoItemCounts[1].text = "+";
        }
        else
        {
            itemCounts[1].text = num2.ToString();
            cashTutoItemCounts[1].text = num2.ToString();
        }

        if (num3 == 0)
        {
            itemCounts[2].text = "+";
            cashTutoItemCounts[2].text = "+";
        }
        else
        {
            itemCounts[2].text = num3.ToString();
            cashTutoItemCounts[2].text = num3.ToString();
        }

        if (num4 == 0)
        {
            itemCounts[3].text = "+";
            cashTutoItemCounts[3].text = "+";
        }
        else
        {
            itemCounts[3].text = num4.ToString();
            cashTutoItemCounts[3].text = num4.ToString();
        }
    }

    public Transform GetMissionTransform(int num)
    {
        if (missionImages.Length <= num) return missionImages[0].transform;

        return missionImages[num].transform;
    }

    public Transform GetMoveCountTransform()
    {
        return moveCountText.gameObject.transform;
    }

    public void ShowShufflePopup()
    {
        popupManager.OnShufflePop();
    }

    public void SetScoreBar(float ratio)
    {
        if (scoreBar == null) return;
        scoreBar.fillAmount = ratio;
    }

    public void StarOn(int starNum)
    {
        if (stars.Length <= starNum || starNum < 0) return;
        ParticleManager.GetInstance.CreateStarPartice(stars[starNum].transform.position);
        stars[starNum].SetActive(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetStarInPlay");
    }

    public void ShowMissionFail()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                StageManager.StageNumber = EventLevelSystem.GetInstance.OriginLevel;
                popupManager.ShowEventLevelFail();
                return;
            }
        }

        popupManager.OnMissionFail();
    }

    public void ShowComboPopup(int comboCount)
    {
        popupManager.OnComboPop(comboCount);
    }

    public void CallLoadingTutorialPop(string SceneName, int SceneImage = 0)
    {
        popupManager.CallLoadingTutorialPop(SceneName, SceneImage);
    }

    public void GameClear()
    {
        foreach (var item in fadeoutWhenGameClear) item.SetActive(false);
    }

    public void SetSkipText(bool value)
    {
        if (skipTextObject != null) skipTextObject.SetActive(value);
    }

    public void moveStarOn()
    {
        ParticleManager.GetInstance.CreateStarPartice(moveCount.position);
    }

    public void UserItemUnLock(int stageNum)
    {
        if (PlayerData.GetInstance == null)
            return;

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                return;
            }
        }

        if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
        {
            PlayerData.GetInstance.IsItemUnlock[1] = true;
            PlayerData.GetInstance.IsItemUnlock[3] = true;
            PlayerData.GetInstance.IsItemUnlock[4] = true;
        }
        else
        {
            if (stageNum >= 24) //24
            {
                PlayerData.GetInstance.IsItemUnlock[1] = true;
                PlayerData.GetInstance.IsItemUnlock[3] = true;
                PlayerData.GetInstance.IsItemUnlock[4] = true;
            }
            else if (stageNum >= 16 && stageNum < 24) // 16 / 24
            {
                PlayerData.GetInstance.IsItemUnlock[1] = true;
                PlayerData.GetInstance.IsItemUnlock[3] = true;
            }
            else if (stageNum >= 9 && stageNum < 16) // 9 16
            {
                PlayerData.GetInstance.IsItemUnlock[1] = true;
            }

            if (itemGroup != null)
            {
                itemGroup.Refresh();
                cashTutoItemGroup.Refresh();
                PlayerData.GetInstance.SaveIsItemUnlock();
            }
        }
    }

    /* public void FirUserItemUnLock()
     {
         itemCountBox[0].SetActive(true);
         itemLockBox[0].SetActive(false);
         itemLockBackColor[0].gameObject.SetActive(false);
         if (PlayerData.GetInstance.TouchFirUnLockItem.Equals(true))
         {
             itemUnLockObj[0].SetActive(false);
         }
         else
         {
             itemUnLockObj[0].SetActive(true);
         }
     }

     public void SecUserItemUnLock()
     {
         itemCountBox[1].SetActive(true);
         itemLockBox[1].SetActive(false);
         itemLockBackColor[1].gameObject.SetActive(false);
         if (PlayerData.GetInstance.TouchSecUnLockItem.Equals(true))
         {
             itemUnLockObj[1].SetActive(false);
         }
         else
         {
             itemUnLockObj[1].SetActive(true);
         }
     }

     public void ThrUserItemUnLock()
     {
         itemCountBox[2].SetActive(true);
         itemLockBox[2].SetActive(false);
         itemLockBackColor[2].gameObject.SetActive(false);
         if (PlayerData.GetInstance.TouchThrUnLockItem.Equals(true))
         {
             itemUnLockObj[2].SetActive(false);
         }
         else
         {
             itemUnLockObj[2].SetActive(true);
         }
     }*/
}