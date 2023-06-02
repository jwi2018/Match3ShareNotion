using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace LogicStates
{
    public class MapSettingLogic : LogicState
    {
        public override void Enter()
        {
            STATE_NAME = "MAPSETTING";
            if (BaseSystem.GetInstance != null)
            {
                if (AcornSystem.GetInstance != null)
                {
                    AcornSystem.GetInstance.SetAcornGetting(true);
                }
            }

            StageManager.GetInstance.Init();
            BlockManager.GetInstance.Init();

            StageManager.GetInstance.GameStart();
            TileManager.GetInstance.GameStart();
            TileManager.GetInstance.SettingTile();
            TileManager.GetInstance.RelicException();
            StateTime = 0f;

            if (TileManager.GetInstance.IsMultiMap()) TileManager.GetInstance.StartMultiCameraWork();
        }

        public override void Update()
        {
            if (TileManager.GetInstance.IsMultiCameraWorkEnd && AnimationManager.AnimCount == 0)
                LogicManager.GetInstance.ChangeLogicState(new PopLogic());
        }

        public override void Exit()
        {
            StateTime = 0f;
        }
    }

    public class PopLogic : LogicState
    {
        private bool isGoDropLogic;
        private bool isGoWaitUserInputLogic;
        private bool isPopComplete = false;

        public override void Enter()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            STATE_NAME = "POP";
            StateTime = 0f;
            BlockManager.GetInstance.ClearPopCondition();
            if (BlockManager.GetInstance.PopCheck())
            {
                StageManager.GetInstance.ComboCount++;
                StageManager.GetInstance.ComboIng();
                //if (StageManager.GetInstance.ComboCount > 2)
                //    if (SoundManager.GetInstance != null)
                //        SoundManager.GetInstance.Play(SoundManager.GetInstance.ComboMatch);
                BlockManager.GetInstance.Pop();
            }
            else
            {
                BlockManager.GetInstance.ClearPopCondition();
                if (TileManager.GetInstance.DropCheck() ||
                    TileManager.GetInstance.IsAbleToCreateBlock()
                    || BlockManager.GetInstance.IsDropMoving())
                    isGoDropLogic = true;
                else
                    isGoWaitUserInputLogic = true;
            }
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            BlockManager.GetInstance.CreateGiftBlock();
            BlockManager.GetInstance.CreateSpecialBlock();
            if (!TileManager.GetInstance.IsBombing())
            {
                if (isGoDropLogic)
                {
                    LogicManager.GetInstance.ChangeLogicState(new DropLogic());
                    return;
                }

                if (isGoWaitUserInputLogic && AnimationManager.RainbowCount == 0 && AnimationManager.AnimCount == 0)
                {
                    if (TileManager.GetInstance.DropCheck() ||
                        TileManager.GetInstance.IsAbleToCreateBlock() || !TileManager.GetInstance.CheckEndBigAnim() ||
                        AnimationManager.AutoTargetBombCount != 0)
                    {
                        //AutoTargetBomb[] butterfly = GameObject.FindObjectsOfType<AutoTargetBomb>();
                        //AnimationManager.AutoTargetBombCount = butterfly.Length;
                        LogicManager.GetInstance.ChangeLogicState(new DropLogic());
                    }
                    else
                    {
                        BlockManager.GetInstance.ClearPopCondition();
                        if (!BlockManager.GetInstance.IsDropMoving() && BlockManager.GetInstance.IsMovingEnd()
                                                                     && !TileManager.GetInstance.IsAbleToCreateBlock()
                                                                     && !BlockManager.GetInstance.PopCheck())
                        {
                            BlockManager.GetInstance.ClearPopCondition();
                            LogicManager.GetInstance.ChangeLogicState(new ObstacleLogic());
                        }
                        else
                        {
                            LogicManager.GetInstance.ChangeLogicState(new PopLogic());
                        }
                    }

                    return;
                }

                StateTime += Time.deltaTime;
                //if(StateTime > 0.3f)
                {
                    LogicManager.GetInstance.ChangeLogicState(new ItemCreateLogic());
                }
            }
        }

        public override void Exit()
        {
            TileManager.GetInstance.TileBombEnd();
            BlockManager.GetInstance.BombEnd();
            BlockManager.GetInstance.SetNotMasterBlock();
            StateTime = 0f;
            BlockManager.GetInstance.ClearPopCondition();
            TileManager.GetInstance.ClearBombCondition();
        }
    }

    public class ItemCreateLogic : LogicState
    {
        private bool isDropLogic;
        private float ToDropLogicTime;

        public override void Enter()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            STATE_NAME = "ITEM_CREATE";
            BlockManager.GetInstance.CreateSpecialBlock();
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            ToDropLogicTime += Time.deltaTime;
            //if (ToDropLogicTime > 0.1f && !isDropLogic)
            if (BlockManager.GetInstance.IsMovingEnd())
            {
                isDropLogic = true;
                LogicManager.GetInstance.ChangeLogicState(new DropLogic());
            }
        }

        public override void Exit()
        {
            BlockManager.GetInstance.ClearSwapBlock();
            //BlockManager.GetInstance.SetEndPopAnim();
        }
    }

    public class DropLogic : LogicState
    {
        private static float createTime;

        public override void Enter()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            STATE_NAME = "DROP";
            StateTime = 0f;
            BlockManager.GetInstance.CreateGiftBlock();
            BlockManager.GetInstance.CreateSpecialBlock();
            if (TileManager.GetInstance.DropCheck() || TileManager.GetInstance.IsAbleToCreateBlock()
                                                    || TileManager.GetInstance.CheckRelicDrop())
                TileManager.GetInstance.Drop();
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            createTime += Time.deltaTime;
            if (createTime > AnimationManager.GetInstance.createBlockTime)
            {
                createTime = 0f;
                TileManager.GetInstance.CreateBlock();
            }

            //if (BlockManager.GetInstance.IsMovingEnd())
            {
                TileManager.GetInstance.RelicRemove();
                LogicManager.GetInstance.ChangeLogicState(new PopLogic());
            }
        }

        public override void Exit()
        {
            StateTime = 0f;
            BlockManager.GetInstance.EndDrop();
        }
    }

    public class ObstacleLogic : LogicState
    {
        public static bool ObstacleActive = true;
        public static int LavaCount = 0;
        public static int BandageCount = 0;

        private bool isRainbowBoxOpen;

        public override void Enter()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            BlockManager.GetInstance.RelicCountCheck();

            STATE_NAME = "OBSTACLE";
            if (BlockManager.GetInstance.IsRainbowBoxBroken())
            {
                BlockManager.GetInstance.ActiveRainbowBox();
                isRainbowBoxOpen = true;
            }
            else
            {
                if (ObstacleActive)
                {
                    BlockManager.GetInstance.IsDoubleClick = false;
                    LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                }
                else
                {
                    ObstacleActive = true;
                    BlockManager.GetInstance.ActiveObstacle();
                }
            }

            StateTime = 0f;
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            if (BlockManager.GetInstance.IsEndObstacleActive)
            {
                if (isRainbowBoxOpen || TileManager.GetInstance.CheckRelicDrop())
                    LogicManager.GetInstance.ChangeLogicState(new DropLogic());
                else
                    LogicManager.GetInstance.ChangeLogicState(new PopLogic());
            }
        }

        public override void Exit()
        {
            StateTime = 0f;
        }
    }

    public class WaitUserInputLogic : LogicState
    {
        private readonly List<GameObject> _hintParticles = new List<GameObject>();
        private float gravityTime;
        private HintInfo hintInfo;
        private bool isFailWait;
        private bool isHintShow;
        private bool isShuffle = true;
        private bool isVictoryWait;

        public override void Enter()
        {
            STATE_NAME = "WAITUSERINPUT";
            StateTime = 0f;

            BlockManager.GetInstance.Clear_Combine();
            BlockManager.GetInstance.SetBlockPosition();
            TileManager.GetInstance.TileBombEnd();
            if (StageManager.GetInstance.VictoryPopupShow && !StageManager.GetInstance.GameClear)
            {
                if (BlockManager.GetInstance.IsActiveBomb())
                {
                    LogicManager.GetInstance.ChangeLogicState(new BonusBombLogic());
                }
                else
                {
                    StageManager.GetInstance.ShowClearPopup();

                    //StageManager.GetInstance.ShowClearPopup();
                    //StageManager.GetInstance.VictoryPopupShow = false;
                    StageManager.GetInstance.GameClear = true;
                }
            }
            else if (StageManager.GetInstance.VictoryLogicShow)
            {
                if (LogicManager.GetInstance != null)
                {
                    LogicManager.GetInstance.ChangeLogicState(new BonusBombLogic());
                }
            }
            else if (StageManager.GetInstance.IsStageClear())
            {
                if (BaseSystem.GetInstance != null)
                {
                    if (AcornSystem.GetInstance != null)
                    {
                        AcornSystem.GetInstance.SetAcornGetting(false);
                    }
                }
                if (StageManager.CollectAnimCount == 0)
                {
                    LogicManager.GetInstance.ChangeLogicState(new VictoryLogic());
                    //isVictoryWait = true;

                    #region !!!AutoRePlayMode!!!

                    if (EditorAutoModeControll._isAutoMode && EditorAutoModeControll._isAutoFull)
                    {
                        if (EditorAutoModeControll._isChallengeInfo.Count == 0)
                        {
                            EditorAutoModeControll._isStartStage = StageManager.StageNumber;
                            var challengeInfo = new StageChallengeInfo();
                            challengeInfo._isStage = StageManager.StageNumber;
                            challengeInfo._isClearCount = 1;
                            challengeInfo._isChallenge_Count = 1;
                            challengeInfo.RemainMoveCount = new List<int>();
                            challengeInfo.RemainMoveCount.Add(StageManager.GetInstance.PreMoveCount);
                            EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                            if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                        }
                        else
                        {
                            var Find = false;
                            foreach (var item in EditorAutoModeControll._isChallengeInfo)
                                if (item._isStage == StageManager.StageNumber)
                                {
                                    Find = true;
                                    item._isChallenge_Count++;
                                    item._isClearCount++;
                                    item.RemainMoveCount.Add(StageManager.GetInstance.PreMoveCount);
                                    if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                                }

                            if (!Find)
                            {
                                var challengeInfo = new StageChallengeInfo();
                                challengeInfo._isStage = StageManager.StageNumber;
                                challengeInfo._isClearCount = 1;
                                challengeInfo._isChallenge_Count = 1;
                                challengeInfo.RemainMoveCount = new List<int>();
                                challengeInfo.RemainMoveCount.Add(StageManager.GetInstance.PreMoveCount);
                                EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                                if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                            }
                        }
                    }

                    #endregion !!!AutoRePlayMode!!!
                }
                else
                {
                    isVictoryWait = true;
                }
            }
            else if (StageManager.GetInstance.IsMissionClearInPreMap() && TileManager.GetInstance.IsMultiMap())
            {
                BlockManager.GetInstance.DestroyInvisibleBox();
                TileManager.GetInstance.GoNextMap();
            }
            else
            {
                if (StageManager.GetInstance.PreMoveCount < 6)
                {
                    StageManager.GetInstance.ShowFiveCountPopup();
                }

                hintInfo = BlockManager.GetInstance.HintCheck();
                if (hintInfo == null)
                {
                    if (DoubleClickSystem.GetInstance != null)
                    {
                        int specialBlockCount = 0;

                        foreach (var item in DoubleClickSystem.GetInstance.GetBlockList())
                        {
                            specialBlockCount += BlockManager.GetInstance.GetMovingBlockList(item).Count;
                        }

                        if (specialBlockCount <= 0)
                        {
                            isShuffle = true;
                            TileManager.GetInstance.SetShuffleTile();
                            StageManager.GetInstance.SetShufflePopup();
                            BlockManager.GetInstance.IsSwapAble = false;
                        }
                    }
                    else
                    {
                        isShuffle = true;
                        TileManager.GetInstance.SetShuffleTile();
                        StageManager.GetInstance.SetShufflePopup();
                        BlockManager.GetInstance.IsSwapAble = false;
                    }
                    //셔플
                }
                else
                {
                    isShuffle = false;

                    if (StageManager.GetInstance.PreMoveCount < 1)
                    {
                        isFailWait = true;
                        /*
                        StageManager.GetInstance.ShowFailPopup();

                        #region !!!AutoRePlayMode!!!

                        if (EditorAutoModeControll._isAutoMode && EditorAutoModeControll._isAutoFull)
                        {
                            if (EditorAutoModeControll._isChallengeInfo.Count == 0)
                            {
                                EditorAutoModeControll._isStartStage = StageManager.StageNumber;
                                StageChallengeInfo challengeInfo = new StageChallengeInfo();
                                challengeInfo._isStage = StageManager.StageNumber;
                                challengeInfo._isClearCount = 0;
                                challengeInfo._isChallenge_Count = 1;
                                challengeInfo.RemainMoveCount = new List<int>();
                                EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                                if (MapEditor.GetInstance != null)
                                {
                                    MapEditor.GetInstance.AutoModeTestController();
                                }
                            }
                            else
                            {
                                bool Find = false;
                                foreach (var item in EditorAutoModeControll._isChallengeInfo)
                                {
                                    if (item._isStage == StageManager.StageNumber)
                                    {
                                        Find = true;
                                        item._isChallenge_Count++;
                                        if (MapEditor.GetInstance != null)
                                        {
                                            MapEditor.GetInstance.AutoModeTestController();
                                        }
                                    }
                                    if (!Find)
                                    {
                                        StageChallengeInfo challengeInfo = new StageChallengeInfo();
                                        challengeInfo._isStage = StageManager.StageNumber;
                                        challengeInfo._isClearCount = 0;
                                        challengeInfo._isChallenge_Count = 1;
                                        challengeInfo.RemainMoveCount = new List<int>();
                                        EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                                        if (MapEditor.GetInstance != null)
                                        {
                                            MapEditor.GetInstance.AutoModeTestController();
                                        }
                                    }
                                }
                            }
                        }

                        #endregion !!!AutoRePlayMode!!!

                        */
                    }
                    else
                    {
                        StageManager.GetInstance.TutorialConditionClear(ETutorialCondition.SWAP);

                        StageManager.GetInstance.ComboEnd();
                        BlockManager.GetInstance.IsSwapAble = true;
                        if (EditorAutoModeControll._isAutoMode)
                        {
                            DebugX.Log("AutoMove");
                            hintInfo.moveBlock.Tile.Swap(hintInfo.moveBlockDirection);
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            //if (CameraWork.IsMoving)
            //{
            //    Debug.Log("[버그 추적] CameraWork.IsMoving : " + CameraWork.IsMoving);
            //}
            //if (StageManager.GetInstance.GameClear)
            //{
            //    Debug.Log("[버그 추적] StageManager.GetInstance.GameClear : " + StageManager.GetInstance.GameClear);
            //}
            //if (CameraWork.IsMoving || StageManager.GetInstance.GameClear) return;
            //// 캐시 아이템 사용 관련 웨이트
            //if (AnimationManager.AnimCount != 0)
            //{
            //    Debug.Log("[버그 추적] AnimationManager.AnimCount : " + AnimationManager.AnimCount);
            //}

            //if (AnimationManager.RainbowCount != 0)
            //{
            //    Debug.Log("[버그 추적] AnimationManager.RainbowCount : " + AnimationManager.RainbowCount);
            //}

            //if (TileManager.GetInstance.IsBombing())
            //{
            //    Debug.Log("[버그 추적] TileManager.GetInstance.IsBombing() : " + TileManager.GetInstance.IsBombing());
            //}
            if (AnimationManager.AnimCount != 0 || AnimationManager.RainbowCount != 0
                                                || TileManager.GetInstance.IsBombing())
                return;

            if (isVictoryWait)
            {
                Debug.Log("[버그 추적] isVictoryWait 이프문 입장");
                if (StageManager.CollectAnimCount == 0)
                {
                    isVictoryWait = false;
                    LogicManager.GetInstance.ChangeLogicState(new VictoryLogic());
                }
            }
            else if (isFailWait)
            {
                Debug.Log("[버그 추적] isFailWait 이프문 입장");
                if (StageManager.CollectAnimCount == 0)
                {
                    isFailWait = false;
                    StageManager.GetInstance.ShowFailPopup();

                    #region !!!AutoRePlayMode!!!

                    if (EditorAutoModeControll._isAutoMode && EditorAutoModeControll._isAutoFull)
                    {
                        if (EditorAutoModeControll._isChallengeInfo.Count == 0)
                        {
                            EditorAutoModeControll._isStartStage = StageManager.StageNumber;
                            var challengeInfo = new StageChallengeInfo();
                            challengeInfo._isStage = StageManager.StageNumber;
                            challengeInfo._isClearCount = 0;
                            challengeInfo._isChallenge_Count = 1;
                            challengeInfo.RemainMoveCount = new List<int>();
                            EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                            if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                        }
                        else
                        {
                            var Find = false;
                            foreach (var item in EditorAutoModeControll._isChallengeInfo)
                            {
                                if (item._isStage == StageManager.StageNumber)
                                {
                                    Find = true;
                                    item._isChallenge_Count++;
                                    if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                                }

                                if (!Find)
                                {
                                    var challengeInfo = new StageChallengeInfo();
                                    challengeInfo._isStage = StageManager.StageNumber;
                                    challengeInfo._isClearCount = 0;
                                    challengeInfo._isChallenge_Count = 1;
                                    challengeInfo.RemainMoveCount = new List<int>();
                                    EditorAutoModeControll._isChallengeInfo.Add(challengeInfo);
                                    if (MapEditor.GetInstance != null) MapEditor.GetInstance.AutoModeTestController();
                                }
                            }
                        }
                    }

                    #endregion !!!AutoRePlayMode!!!
                }
            }
            else
            {
                gravityTime += Time.deltaTime;
                if (gravityTime > 7f)
                {
                    gravityTime = 0f;
                    if (!StageManager.GetInstance.IsTutorialActive()) TileManager.GetInstance.ShowGravity();
                }

                if (!TileManager.GetInstance.ShuffleOn && isShuffle)
                {
                    Debug.Log("[버그 추적] 셔플 온 이프문 추적");
                    BlockManager.GetInstance.IsSwapAble = true;
                    hintInfo = BlockManager.GetInstance.HintCheck();
                    isShuffle = false;
                    if (BlockManager.GetInstance.PopCheck())
                    {
                        BlockManager.GetInstance.ClearPopCondition();
                        LogicManager.GetInstance.ChangeLogicState(new PopLogic());
                        hintInfo = null;
                    }
                    else
                    {
                        BlockManager.GetInstance.ClearPopCondition();
                        BlockManager.GetInstance.IsSwapAble = true;

                        if (StageManager.GetInstance.PreMoveCount < 1 && !StageManager.GetInstance.IsStageClear())
                        {
                            StageManager.GetInstance.ShowFailPopup();
                        }
                        else
                        {
                            BlockManager.GetInstance.IsSwapAble = true;

                            if (EditorAutoModeControll._isAutoMode)
                            {
                                DebugX.Log("AutoMove");
                                hintInfo.moveBlock.Tile.Swap(hintInfo.moveBlockDirection);
                            }
                        }
                    }
                }
                else if (BlockManager.GetInstance.IsSwapEnd())
                {
                    if (BlockManager.GetInstance.IsCombineBlocks())
                    {
                        ObstacleLogic.ObstacleActive = false;
                        StageManager.GetInstance.Move();
                        LogicManager.GetInstance.ChangeLogicState(new CombineBombLogic());
                    }
                    else if (BlockManager.GetInstance.PopCheck())
                    {
                        ObstacleLogic.ObstacleActive = false;
                        StageManager.GetInstance.Move();
                        BlockManager.GetInstance.ClearPopCondition();
                        LogicManager.GetInstance.ChangeLogicState(new PopLogic());
                    }
                    else
                    {
                        BlockManager.GetInstance.ClearPopCondition();
                        BlockManager.GetInstance.ReverseSwap();
                    }
                }
                else if (BlockManager.GetInstance.IsDoubleClicktrue())
                {
                    if (BlockManager.GetInstance.clickedBlock != null)
                    {
                        BlockManager.GetInstance.clickedBlock.BombPop();
                        BlockManager.GetInstance.clickedBlock = null;
                    }
                    ObstacleLogic.ObstacleActive = false;
                    StageManager.GetInstance.Move();
                    BlockManager.GetInstance.ClearPopCondition();
                    LogicManager.GetInstance.ChangeLogicState(new PopLogic());
                }
            }

            if (StateTime > 3.0f && hintInfo != null)
            {
                if (!StageManager.GetInstance.IsTutorialActive())
                {
                    if (!isHintShow)
                    {
                        isHintShow = true;
                        if (hintInfo.combine != ECombine.NONE)
                        {
                            foreach (var item in hintInfo.blocks)
                            {
                                if (item.Tile == null) continue;
                                item.GetComponent<Animator>().SetTrigger("Hint");
                                for (var d = 0; d < 4; d++)
                                    if (item.Tile.GetConnectTileOrNull((EDirection)d) != null)
                                    {
                                        if (!hintInfo.blocks.Contains(item.Tile.GetConnectTileOrNull((EDirection)d)
                                            .NormalBlock))
                                            _hintParticles.Add(
                                                ParticleManager.GetInstance.CreateParticles(item.transform,
                                                    (EDirection)d));
                                    }
                                    else
                                    {
                                        _hintParticles.Add(
                                            ParticleManager.GetInstance.CreateParticles(item.transform,
                                                (EDirection)d));
                                    }
                            }
                        }
                        else
                        {
                            foreach (var item in hintInfo.blocks)
                            {
                                if (item.Tile == null) continue;
                                item.GetComponent<Animator>().SetTrigger("Hint");
                                if (item == hintInfo.moveBlock) continue;
                                for (var d = 0; d < 4; d++)
                                {
                                    if (item.Tile == null) continue;
                                    if (item == hintInfo.centerBlock && (EDirection)d == hintInfo.centerBlockDirection)
                                        continue;

                                    if (item.Tile.GetConnectTileOrNull((EDirection)d) != null)
                                    {
                                        if (!hintInfo.blocks.Contains(item.Tile.GetConnectTileOrNull((EDirection)d)
                                                .NormalBlock) &&
                                            item.Tile.GetConnectTileOrNull((EDirection)d).NormalBlock !=
                                            hintInfo.centerBlock)
                                            _hintParticles.Add(
                                                ParticleManager.GetInstance.CreateParticles(item.transform,
                                                    (EDirection)d));
                                    }
                                    else
                                    {
                                        _hintParticles.Add(
                                            ParticleManager.GetInstance.CreateParticles(item.transform,
                                                (EDirection)d));
                                    }
                                }
                            }

                            var block = hintInfo.centerBlock;
                            for (var d = 0; d < 4; d++)
                            {
                                if (block.Tile == null) continue;
                                if (block.Tile.GetConnectTileOrNull((EDirection)d) != null)
                                {
                                    if (!hintInfo.blocks.Contains(block.Tile.GetConnectTileOrNull((EDirection)d)
                                            .NormalBlock) ||
                                        (EDirection)d == hintInfo.centerBlockDirection)
                                        _hintParticles.Add(
                                            ParticleManager.GetInstance.CreateParticles(block.transform,
                                                (EDirection)d));
                                }
                                else
                                {
                                    _hintParticles.Add(
                                        ParticleManager.GetInstance.CreateParticles(block.transform, (EDirection)d));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in hintInfo.blocks)
                        {
                            if (item.Tile == null) continue;
                            item.GetComponent<Animator>().SetTrigger("Hint");
                        }
                    }
                }

                StateTime = 0.0f;
            }

            StateTime += Time.deltaTime;
        }

        public override void Exit()
        {
            //StageManager.GetInstance.TutorialConditionClear(ETutorialCondition.SWAP);
            StageManager.GetInstance.HideHand();
            StateTime = 0f;
            if (hintInfo != null)
            {
                foreach (var item in hintInfo.blocks)
                {
                    if (item.Tile == null) continue;
                    item.GetComponent<Animator>().SetTrigger("Move");
                }

                foreach (var item in _hintParticles) ParticleManager.GetInstance.DestroyParticle(item);
            }

            _hintParticles.Clear();
            BlockManager.GetInstance.IsSwapAble = false;
        }
    }

    public class CombineBombLogic : LogicState
    {
        public override void Enter()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            STATE_NAME = "COMBINE";
            StateTime = 0f;
            BlockManager.GetInstance.ActiveCombine();
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            BlockManager.GetInstance.CreateGiftBlock();
            BlockManager.GetInstance.CreateSpecialBlock();
            if (!TileManager.GetInstance.IsBombing()
                && AnimationManager.AnimCount == 0)
                LogicManager.GetInstance.ChangeLogicState(new ItemCreateLogic());
        }

        public override void Exit()
        {
            StateTime = 0f;
            BlockManager.GetInstance.ClearSwapBlock();
        }
    }

    public class VictoryLogic : LogicState
    {
        public override void Enter()
        {
            //지윽 : 여기다 체크?
            //Debug.Log("게임 클리어");
            STATE_NAME = "VICTORY_BOMB";
            StateTime = 0f;
            BlockManager.GetInstance.IsSwapAble = false;
            StageManager.GetInstance.VictoryLogicShow = true;
            BlockManager.GetInstance.DestroyInvisibleBox();
            StageManager.GetInstance.ShowVictoryPopup();
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            StateTime = 0f;
        }
    }

    public class BonusBombLogic : LogicState
    {
        public override void Enter()
        {
            BlockManager.GetInstance.IsSwapAble = false;
            STATE_NAME = "BONUS_BOMB";
            StateTime = 0f;
            StageManager.GetInstance.ShowBonusBomb();
            //StageManager.GetInstance.VictoryLogicShow = false;
        }

        public override void Update()
        {
            if (StageManager.GetInstance.IsSkipOn)
            {
                LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
                return;
            }

            if (StageManager.GetInstance.BonusBombSettingEnd && BlockManager.FireworkCount == 0)
            {
                BlockManager.GetInstance.ShowClearBomb();
                StageManager.GetInstance.BonusBombSettingEnd = false;
                StageManager.GetInstance.VictoryPopupShow = true;

                LogicManager.GetInstance.ChangeLogicState(new PopLogic());
            }
        }

        public override void Exit()
        {
            StateTime = 0f;
        }
    }
}