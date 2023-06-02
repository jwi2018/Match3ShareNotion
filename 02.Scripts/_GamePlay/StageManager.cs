using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[Serializable]
public class BlockInfo
{
    public EID ID;
    public EColor color;
    public int HP;
    public int ETC;

    public BlockInfo(EID _id, EColor _color, int _hp, int _etc)
    {
        ID = _id;
        color = _color;
        HP = _hp;
        ETC = _etc;
    }
}

public class MissionKind
{
    public EColor color;
    public int count;
    public EID ID;

    public MissionKind(EID _id, EColor _color, int _count)
    {
        ID = _id;
        color = _color;
        count = _count;
    }
}

public class StageInfo
{
    private readonly Dictionary<BlockInfo, float> basicBlocks = new Dictionary<BlockInfo, float>();
    private readonly List<MissionKind> missions = new List<MissionKind>();
    public int moveCount;
    public int stageNumber;

    public void AddBasicBlock(BlockInfo info, float percent)
    {
        if (!basicBlocks.ContainsKey(info))
            basicBlocks.Add(info, percent);
    }

    public void AddMissions(MissionKind mission)
    {
        if (!missions.Contains(mission))
            missions.Add(mission);
    }

    public BlockInfo GetRandomBlock()
    {
        float maxValue = 0;
        var basicColors = new List<EColor>();

        foreach (var basicBlock in basicBlocks)
        {
            maxValue += basicBlock.Value;
            if (basicBlock.Key.ID == EID.NORMAL)
                if (!basicColors.Contains(basicBlock.Key.color))
                    basicColors.Add(basicBlock.Key.color);
        }

        var randomValue = Random.Range(0, maxValue);
        float range = 0;
        foreach (var blocks in basicBlocks)
        {
            range += blocks.Value;
            if (range >= randomValue)
            {
                var data = new BlockInfo(blocks.Key.ID, blocks.Key.color, blocks.Key.HP, blocks.Key.ETC);
                if (blocks.Key.ID == EID.GLASS_COLOR || blocks.Key.ID == EID.TIMEBOMB_ICE || blocks.Key.ID == EID.NORMAL
                    || blocks.Key.ID == EID.HORIZONTAL || blocks.Key.ID == EID.VERTICAL || blocks.Key.ID == EID.X
                    || blocks.Key.ID == EID.RHOMBUS
                    || blocks.Key.ID == EID.TIMEBOMB_LAVA || blocks.Key.ID == EID.GIFTBOX ||
                    blocks.Key.ID == EID.CHAMELEON)
                {
                    var ranColor = Random.Range(0, basicColors.Count);
                    data.color = basicColors[ranColor];

                    if (blocks.Key.ID == EID.TIMEBOMB_ICE || blocks.Key.ID == EID.TIMEBOMB_LAVA)
                    {
                        data.ETC = 16;
                        var revisionValue = (50 - moveCount) / 10;
                        switch (moveCount)
                        {
                            case 9:
                                revisionValue = 5;
                                break;

                            case 8:
                                revisionValue = 6;
                                break;

                            case 7:
                                revisionValue = 7;
                                break;

                            case 6:
                                revisionValue = 8;
                                break;

                            case 5:
                                revisionValue = 9;
                                break;
                        }

                        data.ETC -= revisionValue;
                    }
                }

                return data;
            }
        }

        return null;
    }

    public Dictionary<BlockInfo, float> GetBlocks()
    {
        return basicBlocks;
    }

    public List<MissionKind> GetMissions()
    {
        return missions;
    }

    public void ClearBasicBlock()
    {
        basicBlocks.Clear();
    }

    public int GetColorCount()
    {
        var basicColors = new List<EColor>();

        foreach (var basicBlock in basicBlocks)
            if (basicBlock.Key.ID == EID.NORMAL)
                if (!basicColors.Contains(basicBlock.Key.color))
                    basicColors.Add(basicBlock.Key.color);

        return basicColors.Count;
    }
}

public class StageManager : Singleton<StageManager>
{
    public static int StageNumber;
    public static int CollectAnimCount;
    public bool IsTestMode;
    public static int LoadingBackground = 0;

    [SerializeField] private GameUI gameUI;
    [SerializeField] private MissionCollectContainer missionCollectContainer;
    [SerializeField] private TutorialController tutorialController;
    [SerializeField] public CashItemTutorial cashItemTutorial;

    public int BeforeStars { get; set; }

    private int ClearMoveCount;
    private readonly List<EID> collectIDs = new List<EID>();

    private readonly List<MissionKind> initMissions = new List<MissionKind>();

    private int maxScore;
    private float maxScoreCache;
    private List<MissionKind> preMissions = new List<MissionKind>();
    private int scoreLevel;
    private int stageFailNumber;
    private bool EventNumCheckClear;

    private StageInfo stageInfo;

    public StageInfo StageInfo
    {
        get { return stageInfo; }
        set { stageInfo = value; }
    }

    private int star1Score;
    private int star2Score;
    private int star3Score;
    private readonly List<int> uiMissionCounts = new List<int>();

    public int PreScore { get; set; }
    public int PreMoveCount { get; set; }
    public EUseItem SelectItem { get; set; }
    public EUseItem SelectItemAgo { get; set; }
    public int ComboCount { get; set; }

    public bool VictoryLogicShow { get; set; }
    public bool BonusBombSettingEnd { get; set; }
    public bool VictoryPopupShow { get; set; }
    public bool GameClear { get; set; }
    public bool IsShowFiveCount { get; set; }
    public bool IsSkipOn { get; set; }
    public bool IsSkipAble { get; set; }

    public bool IsEditorMode { get; set; } = false;

    public bool AutoMode { get; set; } = false;

    public bool isRewardADItemShownd = false;
    public int MoveCountForRewardItemAD { get; set; }

    private int scoreStandard = 1500;

    public void Init()
    {
        CollectAnimCount = 0;
        MoveCountForRewardItemAD = 0;
        stageInfo = new StageInfo();

        if (missionCollectContainer != null)
        {
            foreach (var missionSP in missionCollectContainer.missionSprites)
            {
                collectIDs.Add(missionSP.ID);
            }
        }

        //collectIDs.Add(EID.NORMAL);
        //collectIDs.Add(EID.GOLD);
        //collectIDs.Add(EID.ACTINIARIA);
        //collectIDs.Add(EID.BIG_SIDE);
        //collectIDs.Add(EID.BIG_SIDE_COLOR);
        ////collectIDs.Add(EID.CHAMELEON);
        //collectIDs.Add(EID.CLAM);
        //collectIDs.Add(EID.DROP_RELIC1);
        //collectIDs.Add(EID.DROP_RELIC2);
        //collectIDs.Add(EID.DROP_RELIC3);
        //collectIDs.Add(EID.GIFTBOX);
        //collectIDs.Add(EID.GLASS_COLOR);
        //collectIDs.Add(EID.RELIC_IN_INVISIBLE_BOX);
        //collectIDs.Add(EID.TABLET);
        //collectIDs.Add(EID.TIMEBOMB_ICE);
        //collectIDs.Add(EID.TIMEBOMB_LAVA);
        //collectIDs.Add(EID.TURN_BOX);
        //collectIDs.Add(EID.DOUBLE);

        VictoryLogicShow = false;
        BonusBombSettingEnd = false;
        VictoryPopupShow = false;
        GameClear = false;
        IsShowFiveCount = false;

        if (tutorialController != null) tutorialController.Init();
        if (cashItemTutorial != null) cashItemTutorial.Init();

        if (PlayerData.GetInstance != null)
            BeforeStars = PlayerData.GetInstance.GetLevelStartCount(StageNumber);

    }

    public int GetBlockColorCount()
    {
        return stageInfo.GetColorCount();
    }

    public void ComboIng()
    {
        if (ComboCount > 3)
        {
            gameUI.AnimationChangeForGameInfoCharacter("smile", 0);
        }
    }

    public void ComboEnd()
    {
        if (isRewardADItemShownd == false)
        {
            if (MoveCountForRewardItemAD > 0)
            {
                ShowReward_PlayAdButton();
                isRewardADItemShownd = true;
            }
        }

        /*
        isRewardADItemShownd
        do
        {
            if (isRewardADItemShownd == true)
                break;

            if(MoveCountForRewardItemAD < 0)
                break;

            ShowReward_PlayAdButton();
            isRewardADItemShownd = true;
        } while (false);
        */

        gameUI.ShowComboPopup(ComboCount);
        gameUI.AnimationChangeForGameInfoCharacter("normal", 1.5f);

        ComboCount = 0;
    }

    public void GameStart()
    {
        if (FirebaseManager.GetInstance != null)
            FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"Stage_{StageNumber}_Start"));
        IsSkipAble = true;
        IsSkipOn = false;
        CollectAnimCount = 0;
        var number = StageNumber;
        if (IsEditorMode) number = -1;
        ComboCount = 0;
        int PlusMoveCount = 0;

        var stageData = DataContainer.GetInstance.GetMapDataOrNULL(number);

        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance.IsChallengeStage)
                {
                    stageData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                }
            }

            if (EventLevelSystem.GetInstance != null)
            {
                if (EventLevelSystem.GetInstance.IsEventLevel)
                {
                    switch (EventLevelSystem.GetInstance.EventLevelNum)
                    {
                        case 1:
                            number = 200 + (PlayerData.GetInstance.GetEventLevelList()[0] * 20);
                            PlusMoveCount = 10;
                            break;

                        case 2:
                            number = 200 + (PlayerData.GetInstance.GetEventLevelList()[1] * 20);
                            PlusMoveCount = 8;
                            break;

                        case 3:
                            number = 200 + (PlayerData.GetInstance.GetEventLevelList()[2] * 20);
                            PlusMoveCount = 7;
                            break;

                        case 4:
                            number = 200 + (PlayerData.GetInstance.GetEventLevelList()[3] * 20);
                            PlusMoveCount = 5;
                            break;

                        case 5:
                            number = 200 + (PlayerData.GetInstance.GetEventLevelList()[4] * 20);
                            PlusMoveCount = 5;
                            break;
                    }

                    stageData = DataContainer.GetInstance.GetChallengeMapDataOrNULL(number);
                    EventLevelSystem.GetInstance.OriginLevel = StageNumber;
                    EventLevelSystem.GetInstance.EventMapStageNum = number;
                    StageNumber = number;
                }
            }
        }

        if (stageData != null)
        {
            stageInfo.moveCount = stageData.moveCount;
            if (EventLevelSystem.GetInstance != null && EventLevelSystem.GetInstance.IsEventLevel) stageInfo.moveCount += PlusMoveCount;
            maxScore = scoreStandard * stageInfo.moveCount;
            star3Score = scoreStandard * stageInfo.moveCount;
            star2Score = (scoreStandard / 2) * stageInfo.moveCount;
            star1Score = (scoreStandard / 4) * stageInfo.moveCount;
            maxScoreCache = 1 / (float)maxScore;

            foreach (var item in stageData.basicBlocks)
            {
                int percent, id, color = 0;
                percent = item / 10000;
                id = (item - percent * 10000) / 10;
                color = item - percent * 10000 - id * 10;

                //TODO
                stageInfo.AddBasicBlock(new BlockInfo((EID)id, (EColor)color, 1, 0), percent);
            }

            foreach (var item in stageData.missions)
            {
                int count, mID, mColor = 0;

                count = item / 10000;
                mID = (item - count * 10000) / 10;
                mColor = item - count * 10000 - mID * 10;

                stageInfo.AddMissions(new MissionKind((EID)mID, (EColor)mColor, count));
            }
        }

        PreScore = 0;
        PreMoveCount = stageInfo.moveCount;

        preMissions = stageInfo.GetMissions();
        foreach (var mission in preMissions) uiMissionCounts.Add(mission.count);

        if (gameUI != null)
        {
            gameUI.SetScore(PreScore);
        }

        SetUI();
        EventNumCheckClear = true;
    }

    public void CollectMission(EID id, EColor color, Transform startTransform, int value = 0)
    {
        var missionBeing = false;
        var count = 0;
        foreach (var mission in preMissions)
        {
            if (mission.ID == id)
            {
                if (mission.ID == EID.NORMAL && mission.color != color)
                {
                    count++;
                    continue;
                }

                if (mission.count == 0) return;

                missionBeing = true;
                mission.count--;
                mission.count = Mathf.Max(0, mission.count);
                break;
            }

            count++;
        }

        if (IsEditorMode)
        {
            if (uiMissionCounts.Count > count) uiMissionCounts[count]--;
            SetUI();
        }
        else if (missionBeing)
        {
            if (collectIDs.Contains(id))
            {
                ShowCollectBlockAnim(id, color, value, count, startTransform);
            }
            else
            {
                if (uiMissionCounts.Count > count) uiMissionCounts[count]--;
                SetUI();
            }
        }
    }

    public void ShowCollectBlockAnim(EID id, EColor color, int value, int count, Transform startTransform)
    {
        var collectObj = DynamicObjectPool.GetInstance.GetObjectForType("MissionItemRootingOb", false);
        if (collectObj == null) return;

        var collectScript = collectObj.GetComponent<MissionItemRooting>();
        if (collectScript == null) return;

        int isRelic = 0, colorValue = -1;
        if (id == EID.DROP_RELIC1 || id == EID.DROP_RELIC2 || id == EID.DROP_RELIC3 || id == EID.TABLET)
            isRelic = 1;
        if (color != EColor.NONE)
            colorValue = (int)color;
        else
            colorValue = value;

        var endTransform = gameUI.GetMissionTransform(count);

        CollectAnimCount++;
        var sprite = missionCollectContainer.GetSpriteOrNull(id, colorValue);

        if (id != EID.ACTINIARIA)
            collectScript.StartAnim(sprite, startTransform.position, endTransform.position, count, isRelic);
        else
            collectScript.StartAnim(sprite, startTransform.position, endTransform.position, count, isRelic, true, 1.5f);
    }

    public bool IsStageClear()
    {
        var isStageClear = true;

        foreach (var mission in preMissions)
            if (mission.count != 0)
                isStageClear = false;

        return isStageClear;
    }

    public bool IsMissionBeing(EID id)
    {
        foreach (var mission in preMissions)
            if (mission.ID == id)
                return true;
        return false;
    }

    public bool IsMissionClearInPreMap()
    {
        if (preMissions[0].ID == EID.GOLD)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.GOLD);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        if (preMissions[0].ID == EID.RELIC_IN_INVISIBLE_BOX)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.RELIC_IN_INVISIBLE_BOX);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        if (preMissions[0].ID == EID.JAM)
        {
            var result = 0;
            result += TileManager.GetInstance.GetPreViewTile(ETileKind.END);
            result += TileManager.GetInstance.GetPreViewTile(ETileKind.START);
            result += TileManager.GetInstance.GetPreViewTile(ETileKind.NORMAL);

            result -= BlockManager.GetInstance.GetCountToUseIDandPreview(EID.JAM);

            if (result != 0) return false;
        }

        if (preMissions[0].ID == EID.TABLET)
            if (TileManager.GetInstance.IsTabletInPreViewTile())
                return false;
        if (preMissions[0].ID == EID.DROP_RELIC1 || preMissions[0].ID == EID.DROP_RELIC2
                                                 || preMissions[0].ID == EID.DROP_RELIC3)
            return TileManager.GetInstance.CheckRelicMultimap(preMissions[0].ID);
        if (preMissions[0].ID == EID.GLASS_COLOR)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.GLASS_COLOR);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        if (preMissions[0].ID == EID.BIG_SIDE)
            if (TileManager.GetInstance.IsBigObjInPreViewTile(EID.BIG_SIDE))
                return false;
        if (preMissions[0].ID == EID.CLAM)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.CLAM);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        if (preMissions[0].ID == EID.TURN_BOX)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.TURN_BOX);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        if (preMissions[0].ID == EID.DOUBLE)
        {
            var blocks = BlockManager.GetInstance.GetBlocksToUseID(EID.DOUBLE);

            foreach (var block in blocks)
            {
                if (block.Tile == null) continue;
                if (TileManager.GetInstance.IsPreViewTile(block.Tile)) return false;
            }
        }

        return true;
    }

    public void SetShufflePopup()
    {
        gameUI.ShowShufflePopup();
    }

    public bool IsMissionClear(EID id)
    {
        foreach (var mission in preMissions)
            if (mission.ID == id)
                if (mission.count == 0)
                    return true;

        return false;
    }

    /// <summary>
    /// 무브 카운터 깎는 함수
    /// </summary>
    public void Move()
    {
        PreMoveCount--;

        TileManager.GetInstance.GetDropRelicCount = TileManager.GetInstance.GetDropRelicCount + 1;
        SetUI();
    }

    public void ShowBonusBomb()
    {
        StartCoroutine(ShowBonusBombCoroutine());
    }

    public void GameOver()
    {
        if (IsEditorMode)
            MapEditor.GetInstance.ShowFailPopup();
        else
            gameUI.ShowMissionFail();
    }

    private IEnumerator ShowBonusBombCoroutine()
    {
        while (PreMoveCount > 0)
        {
            PreMoveCount--;
            SetUI();
            var startTransform = gameUI.GetMoveCountTransform();
            BlockManager.GetInstance.CreateChangeBlockMissile(startTransform);
            yield return new WaitForSeconds(0.1f);
        }

        BonusBombSettingEnd = true;
    }

    public void ShowVictoryPopup()
    {
        ClearMoveCount = PreMoveCount;
        if (IsEditorMode)
        {
            MapEditor.GetInstance.ShowVictoryPopup();
        }
        else
        {
            DailyQuestManager.CollectMission(EDailyQuestType.GAMEPLAY, 1);
            DailyQuestManager.CollectMission(EDailyQuestType.GAMECLEAR, 1);
            DailyQuestManager.Save();
            gameUI.GameClear();
            gameUI.ShowVictoryPopup();
        }
    }

    public void ShowClearPopup()
    {
        gameUI.ShowClearPopup(scoreLevel);

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
                StageNumber = EventLevelSystem.GetInstance.OriginLevel;
        }

        if (PlayerData.GetInstance != null)
            if (PlayerData.GetInstance.PresentLevel < StageNumber)
            {
                WeeklyRetentionIndicator.Tracking(StageNumber.ToString());
                WeeklyRetentionIndicator.StageClearTracking(StageNumber);
                if (EventLevelSystem.GetInstance != null)
                    if (StageNumber == StaticGameSettings.iLimitStageEventLevel)
                        PopupManager.instance.ShowEventLevelOpen();
                if (StageNumber % 15 == 0) PlayerData.GetInstance.Gold += 30;
                PlayerData.GetInstance.Gold += 20;

                PlayerData.GetInstance.PresentLevel = StageNumber;
                if (GpgsManager.GetInstance != null) GpgsManager.GetInstance.UpdateRecord();

                var StageInfo = new Dictionary<string, object>();
                StageInfo.Add("StageNumber", StageNumber.ToString());
                StageInfo.Add("MoveCount", ClearMoveCount.ToString());
                Analytics.CustomEvent("StageClear", StageInfo);
            }

        if (EditorAutoModeControll._isAutoMode)
        {
            //FirebaseManager.GetInstance.FirebaseLogEvent("Auto_Stage_Clear", StageInfo);
            if (EditorAutoModeControll.NowCount >= EditorAutoModeControll.TestCount)
            {
                EditorAutoModeControll.NowCount = 0;
                if (EditorAutoModeControll.TestStageList.Count == 1)
                    StageNumber++;
                else
                    for (var i = 0; i < EditorAutoModeControll.TestStageList.Count; i++)
                        if (EditorAutoModeControll.TestStageList[i] == StageNumber)
                        {
                            i += 1;
                            if (i == EditorAutoModeControll.TestStageList.Count)
                            {
                                EditorAutoModeControll.TestStageList.Clear();
                                gameUI.CallLoadingTutorialPop("MainScene", 100);
                                return;
                            }

                            StageNumber = EditorAutoModeControll.TestStageList[i];
                        }
            }
            else
            {
                EditorAutoModeControll.NowCount++;
            }

            gameUI.CallLoadingTutorialPop("GameScene");
        }
        else
        {
    
        }
    }

    public void ShowFailPopup()
    {
        if (IsEditorMode)
            MapEditor.GetInstance.ShowFailPopup();
        else
        {
            if (EventLevelSystem.GetInstance != null)
            {
                if (EventLevelSystem.GetInstance.IsEventLevel)
                    StageNumber = EventLevelSystem.GetInstance.OriginLevel;
            }
            
            DailyQuestManager.CollectMission(EDailyQuestType.GAMEPLAY, 1);
            DailyQuestManager.Save();
            gameUI.ShowFailPopup(stageFailNumber);
        }
    }

    public BlockInfo GetBlockInfo()
    {
        return stageInfo.GetRandomBlock();
    }

    public void GiveUpStage()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.PauseBGM();

        /*
        GameObject obj = _popupManager.GetComponent<PopupManager>().OnMissionFail();

        _failMissionImages = obj.GetComponent<MissionFailPopup>().FailMissionImage;
        _failMissionTexts = obj.GetComponent<MissionFailPopup>().FailMissionTexts;

        foreach (var item in _failMissionImages)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in _failMissionTexts)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in _missionCounts)
        {
            if (item.Value > 0)
            {
                foreach (var failImage in _failMissionImages)
                {
                    if (!failImage.gameObject.activeSelf)
                    {
                        failImage.gameObject.SetActive(true);
                        failImage.sprite = _missionSpriteContainer.GetSprite((int)item.Key);
                        break;
                    }
                }

                foreach (var failtext in _failMissionTexts)
                {
                    if (!failtext.gameObject.activeSelf)
                    {
                        failtext.gameObject.SetActive(true);
                        failtext.text = item.Value.ToString();
                    }
                }
            }
        }
        */
    }

    public void SetScoreUI()
    {
        if (gameUI == null) return;

        gameUI.SetScore(PreScore);
        var ratio = PreScore * maxScoreCache;
        if (scoreLevel == 0 && PreScore >= star1Score)
        {
            gameUI.StarOn(scoreLevel);
            scoreLevel = 1;
        }
        else if (scoreLevel == 1 && PreScore >= star2Score)
        {
            gameUI.StarOn(scoreLevel);
            scoreLevel = 2;
        }
        else if (scoreLevel == 2 && PreScore >= star3Score)
        {
            gameUI.StarOn(scoreLevel);
            scoreLevel = 3;
        }

        switch (scoreLevel)
        {
            case 0:
                //ratio *= 2.4f;
                //ratio = Mathf.Min(ratio, 0.2f);
                break;

            case 1:
                //ratio *= 1.6f;
                //ratio = Mathf.Min(ratio, 0.65f);
                //ratio = Mathf.Max(ratio, 0.2f);
                break;

            case 2:
                //ratio *= 1.4f;
                //ratio = Mathf.Min(ratio, 0.9f);
                //ratio = Mathf.Max(ratio, 0.65f);
                break;

            case 3:
                //ratio = Mathf.Max(ratio, 0.9f);
                break;
        }

        gameUI.SetScoreBar(ratio);
    }

    public void SetSkipText(bool value)
    {
        gameUI.SetSkipText(value);
    }

    public void CollectMovingMissionItem(int index)
    {
        if (index < uiMissionCounts.Count) uiMissionCounts[index]--;
    }

    public void SetUI()
    {
        if (gameUI == null) return;

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                if (!EventNumCheckClear)
                    gameUI.SetStageNumber(EventLevelSystem.GetInstance.EventLevelNum);
            }
            else
            {
                gameUI.SetStageNumber(StageNumber);
            }
        }
        else
        {
            gameUI.SetStageNumber(StageNumber);
        }

        //Debug.LogWarningFormat("KKI{0}", PreScore);
        //gameUI.SetScore(PreScore);
        gameUI.SetMoveCount(PreMoveCount);
        gameUI.UserItemUnLock(StageNumber);

        for (var i = 0; i < preMissions.Count; i++)
            if (preMissions[i].ID == EID.NORMAL)
                gameUI.SetMissionImage(i, BlockManager.GetInstance.GetBlockSprite(EID.NORMAL,
                    preMissions[i].color, 1));
            else
                gameUI.SetMissionImage(i, BlockManager.GetInstance.GetMissionSprite(preMissions[i].ID));
        /*
        for (int i = 0; i < preMissions.Count; i++)
        {
            gameUI.SetMissionCount(i, preMissions[i].count);
        }
        */
        for (var i = 0; i < uiMissionCounts.Count; i++) gameUI.SetMissionCount(i, uiMissionCounts[i]);

        if (PlayerData.GetInstance != null)
            gameUI.SetItemCount(PlayerData.GetInstance.ItemHammer, PlayerData.GetInstance.ItemCross,
                PlayerData.GetInstance.ItemBomb, PlayerData.GetInstance.ItemColor);

        if (gameUI != null)
        {
            var ratio = PreScore * maxScoreCache;
            if (scoreLevel == 0 && PreScore >= star1Score)
            {
                gameUI.StarOn(scoreLevel);
                scoreLevel = 1;
            }
            else if (scoreLevel == 1 && PreScore >= star2Score)
            {
                gameUI.StarOn(scoreLevel);
                scoreLevel = 2;
            }
            else if (scoreLevel == 2 && PreScore >= star3Score)
            {
                gameUI.StarOn(scoreLevel);
                scoreLevel = 3;
            }

            switch (scoreLevel)
            {
                case 0:
                    //ratio *= 2.4f;
                    //ratio = Mathf.Min(ratio, 0.4f);
                    break;

                case 1:
                    //ratio *= 2.0f;
                    //ratio = Mathf.Min(ratio, 0.65f);
                    //ratio = Mathf.Max(ratio, 0.4f);
                    break;

                case 2:
                    //ratio *= 1.7f;
                    //ratio = Mathf.Min(ratio, 0.9f);
                    //ratio = Mathf.Max(ratio, 0.65f);
                    break;

                case 3:
                    //ratio = Mathf.Max(ratio, 0.9f);
                    break;
            }

            gameUI.SetScoreBar(ratio);
        }
    }

    public void DecreasePreMoveCount()
    {
        PreMoveCount--;
        SetUI();
    }

    public void AddPreMoveCount()
    {
        PreMoveCount++;
        gameUI.moveStarOn();
        SetUI();
    }

    public void AddScore(int score)
    {
        PreScore += score;
        SetScoreUI();
    }

    public void ShowScoreText(int score, Vector2Int matrix, EColor color)
    {
        var scoreObj = DynamicObjectPool.GetInstance.GetObjectForType("ScoreText", true);
        if (scoreObj == null) return;
        var movement = scoreObj.GetComponent<ScoreMovement>();
        if (movement == null) return;

        movement.Setting(matrix, EDirection.UP, score, color);
    }

    public void ContinueStage()
    {
        if (PlayerData.GetInstance != null)
        {
            if (stageFailNumber == 0)
            {
                BlockManager.GetInstance.IsSwapAble = true;
                BlockManager.GetInstance.SetBlockCollider(true);
                gameUI.ShowAddMoveCountPopup();
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("GetFiveCount");
            }
            else if (stageFailNumber == 1)
            {
                BlockManager.GetInstance.IsSwapAble = true;
                BlockManager.GetInstance.SetBlockCollider(true);
                gameUI.ShowAddMoveCountPopup();
                BlockManager.GetInstance.SetBlockToItem(EID.RHOMBUS);
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("GetFiveCount");
            }
            else
            {
                BlockManager.GetInstance.IsSwapAble = true;
                BlockManager.GetInstance.SetBlockCollider(true);
                gameUI.ShowAddMoveCountPopup();
                BlockManager.GetInstance.SetBlockToItem(EID.RHOMBUS);
                BlockManager.GetInstance.SetBlockToItem(EID.COLOR_BOMB);
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("GetFiveCount");
            }

            stageFailNumber++;
        }
    }

    public void ClickItemButton(int item)
    {
        if (AnimationManager.AnimCount != 0) return;
        if (!PlayerData.GetInstance.IsItemUnlock[item]) return;

        if (!PlayerData.GetInstance.IsItemClicked[item])
        {
            PlayerData.GetInstance.IsItemClicked[item] = true;
            gameUI.ItemGroupRefresh();
            PlayerData.GetInstance.SaveIsItemClicked();
        }

        if (SelectItem == (EUseItem)item)
        {
            CancelItem();
            CloseItemPopup();
        }
        else
        {
            if (SelectItem != EUseItem.NONE) CloseItemPopup();
            var isItemExist = true;
            if (PlayerData.GetInstance != null)
                switch ((EUseItem)item)
                {
                    case EUseItem.HAMMER:
                        
                        if (PlayerData.GetInstance.ItemHammer <= 0) isItemExist = false;
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item1_use");
                        break;

                    case EUseItem.CROSS:
                        if (PlayerData.GetInstance.ItemCross <= 0) isItemExist = false;
                        break;

                    case EUseItem.BOMB:
                        
                        if (PlayerData.GetInstance.ItemBomb <= 0) isItemExist = false;
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item2_use");
                        break;

                    case EUseItem.COLOR:
                        
                        if (PlayerData.GetInstance.ItemColor <= 0)
                        {
                            isItemExist = false;
                        }
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item3_use");

                        break;
                }

            if (IsTestMode) isItemExist = true;

            if (isItemExist)
            {
                SelectItem = (EUseItem)item;
                SelectItemAgo = SelectItem;

                gameUI.ShowItemPopup(item - 1);
                gameUI.OnItemOutline(item - 1);
            }
            else
            {
                gameUI.ClickBuyItem(item - 1);
            }
        }

        SetUI();
    }

    public void ShowReward_PlayAdButton()
    {
        if (PlayerData.GetInstance != null)
        {
            gameUI.ShowReward_PlayAdButton();
        }
    }

    public void rewardContinue()
    {
        BlockManager.GetInstance.IsSwapAble = true;
        gameUI.ShowAddMoveCountPopup();
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetFiveCount");
        stageFailNumber++;
    }

    public void CancelItem()
    {
        SelectItem = EUseItem.NONE;

        gameUI.OffItemOutline();
    }

    public void CloseItemPopup()
    {
        gameUI.CancelItem();
    }

    public List<MissionKind> GetMissions()
    {
        return preMissions;
    }

    public void ShowFiveCountPopup()
    {
        if (IsShowFiveCount || IsEditorMode) return;

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetFiveCount");
        gameUI.ShowFiveCountPopup();
        IsShowFiveCount = true;
    }

    public void TutorialStart()
    {
        if (DataContainer.GetInstance != null)
            if (DataContainer.GetInstance.GetTutorialNumber(StageNumber) == -1)
                return;
        if (DataContainer.GetInstance.GetCashItemTutorialStart(StageNumber))
        {
            cashItemTutorial.StartTutorial();
        }
        else if (tutorialController != null) tutorialController.TutorialStart();
    }

    public bool TutorialCheck()
    {
        var returnValue = true;
        if (DataContainer.GetInstance != null)
            if (DataContainer.GetInstance.GetTutorialNumber(StageNumber) == -1)
                return false;
        return returnValue;
    }

    public void TutorialConditionClear(ETutorialCondition condition)
    {
        if (tutorialController != null) tutorialController.ConditionClear(condition);
    }

    public bool IsTutorialActive()
    {
        if (tutorialController != null) return tutorialController.gameObject.activeSelf;
        return false;
    }

    public void TutorialEnd()
    {
        if (tutorialController != null)
        {
            tutorialController.Exit();
            tutorialController.EndTutorial();
        }
    }

    public void CashTutorialEnd()
    {
        if (cashItemTutorial != null)
        {
            cashItemTutorial.SetCashItemTutorialShowObject(false);
        }
    }

    public void HideHand()
    {
        if (tutorialController != null)
        {
            tutorialController.HideHand();
            tutorialController.CloseHighlight();
        }
    }

    public bool IsSwapAbleInTuto(Vector2Int position, EDirection direction)
    {
        if (tutorialController != null)
            if (tutorialController.gameObject.activeSelf)
            {
                if (tutorialController.Condition == ETutorialCondition.SWAP)
                {
                    if (tutorialController.SwapPosition == position && tutorialController.SwapDirection == direction)
                        return true;
                    return false;
                }

                return false;
            }

        return true;
    }

    public void AddStartMoveCount()
    {
        gameUI.ShowAddMoveCountPopup(1);
    }

    public void SkipClearBomb()
    {
        if (!IsSkipAble) return;

        AddScore(PreMoveCount * 2000);
        SetScoreUI();
        SetScoreUI();
        SetScoreUI();
        //PreMoveCount = 0;
        IsSkipOn = true;

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                ShowClearPopup();
                return;
            }
        }

        if (ChallengeSystem.GetInstance != null)
        {
            /*
            if (ChallengeSystem.GetInstance.ConfirmChallengeStage(StageNumber))
            {
                PopupManager.instance.ShowChallengeStartPopup();
                return;
            }
            */

            //if (ChallengeSystem.GetInstance.ConfirmChallengeStage(StageNumber))
            //{
            //PopupManager.instance.ShowChallengeStartPopup();
            //return;

            //if (StageNumber.Equals(50))
            //{
            //    PopupManager.instance.ShowChallengeTutorialPopup();
            //    return;
            //}
            //else
            //{
            //PopupManager.instance.ShowChallengeStartPopup();
            //return;

            //}

            //}
        }

        ShowClearPopup();
    }

    public void Clear()
    {
        CollectAnimCount = 0;
        StopAllCoroutines();
        stageInfo = null;
        initMissions.Clear();
        preMissions.Clear();
        VictoryPopupShow = false;
        GameClear = false;
        uiMissionCounts.Clear();

        if (gameUI != null) gameUI.SetActiceFalse();
    }
}