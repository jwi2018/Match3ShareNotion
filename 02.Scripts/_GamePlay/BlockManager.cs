using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LogicStates;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemInfo
{
    public GameBlock block;
    public EColor color;
    public EID ID;
    public Vector2Int matrix;

    public ItemInfo(GameBlock _block)
    {
        color = _block.Color;
        ID = _block.MasterItemKind;
        matrix = _block.Tile.Matrix;
        block = _block;
    }
}

public class HintInfo
{
    public List<GameBlock> blocks;
    public GameBlock centerBlock;
    public EDirection centerBlockDirection;
    public ECombine combine;
    public EItem eItem;
    public GameBlock moveBlock;
    public EDirection moveBlockDirection;
}

public class BlockManager : Singleton<BlockManager>
{
    public static int FireworkCount;

    [SerializeField] private SpriteContainer spriteContainer;
    [SerializeField] private float dragRange = 0.5f;
    [SerializeField] private Sprite[] bigSideColorPartSprite;
    [SerializeField] private RewardAdsGetItem rewardChangeItem;
    [SerializeField] private AutoTargetGetter autoTargetGetter;
    [SerializeField] private GameObject BoosterChangeItem;
    public int RelicCoolTime1;
    public int RelicCoolTime2;
    public bool IsSwapHorizontal;
    public Vector2 SwapPosition;
    private readonly List<EID> bigObjList = new List<EID>();
    private readonly Dictionary<EID, Vector2Int> bigSizeDictionary = new Dictionary<EID, Vector2Int>();
    private readonly Dictionary<EBlockKind, List<EID>> blockKindList = new Dictionary<EBlockKind, List<EID>>();
    private readonly List<GameBlock> blockList = new List<GameBlock>();
    private ECombine combine = ECombine.NONE;

    private readonly Dictionary<int, ECombine> combines = new Dictionary<int, ECombine>();

    private GameBlock firstBlock;
    private readonly Dictionary<GameTile, BlockInfo> giftList = new Dictionary<GameTile, BlockInfo>();
    private readonly List<ItemInfo> itemList = new List<ItemInfo>();
    private readonly List<List<GameBlock>> popList = new List<List<GameBlock>>();
    private readonly List<BigObject> rainbowBoxList = new List<BigObject>();
    private GameBlock secondBlock;

    public GameBlock clickedBlock;

    //더블클릭 아이템 사용
    public bool IsDoubleClick = false;

    public float DragRange => dragRange;

    private bool isSwapable = false;

    public bool IsSwapAble
    {
        get => isSwapable;
        set
        {
            //Debug.LogWarningFormat("KKI IsSwapAble : {0}", value);
            isSwapable = value;
        }
    }

    public int SwapCount { get; private set; }

    public bool HasJamInCombine { get; private set; }

    public bool IsEndObstacleActive { get; set; }

    private void Start()
    {
        SwapCount = 0;
    }

    public void Init()
    {
        DOTween.SetTweensCapacity(3000, 200);

        if (spriteContainer == null)
            spriteContainer = GameObject.Find("SpriteContainer").GetComponent<SpriteContainer>();

        RelicCoolTime1 = 1;
        RelicCoolTime2 = 1;

        InitCombine();
        InitBlockKind();

        bigObjList.Add(EID.BIG_SIDE);
        bigObjList.Add(EID.BIG_SIDE_COLOR);
        bigObjList.Add(EID.BIG_SIDE_DIRECTION);
        bigObjList.Add(EID.GEAR);
        bigSizeDictionary.Add(EID.BIG_SIDE, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.BIG_SIDE_COLOR, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.BIG_SIDE_DIRECTION, new Vector2Int(2, 2));
        bigSizeDictionary.Add(EID.GEAR, new Vector2Int(3, 3));

        IsEndObstacleActive = true;

        FireworkCount = 0;

        autoTargetGetter.Init();
    }

    private void InitCombine()
    {
        combines.Add((int)EItem.HORIZONTAL_BOMB + (int)EItem.HORIZONTAL_BOMB, ECombine.CROSS);
        combines.Add((int)EItem.VERTICAL_BOMB + (int)EItem.VERTICAL_BOMB, ECombine.CROSS);
        combines.Add((int)EItem.HORIZONTAL_BOMB + (int)EItem.VERTICAL_BOMB, ECombine.CROSS);

        combines.Add((int)EItem.X_BOMB + (int)EItem.HORIZONTAL_BOMB, ECombine.ASTERISK);
        combines.Add((int)EItem.X_BOMB + (int)EItem.VERTICAL_BOMB, ECombine.ASTERISK);

        combines.Add((int)EItem.X_BOMB + (int)EItem.X_BOMB, ECombine.X_X);

        combines.Add((int)EItem.RHOMBUS_BOMB + (int)EItem.HORIZONTAL_BOMB, ECombine.BIG_CROSS);
        combines.Add((int)EItem.RHOMBUS_BOMB + (int)EItem.VERTICAL_BOMB, ECombine.BIG_CROSS);

        combines.Add((int)EItem.X_BOMB + (int)EItem.RHOMBUS_BOMB, ECombine.BIG_X);

        combines.Add((int)EItem.RHOMBUS_BOMB + (int)EItem.RHOMBUS_BOMB, ECombine.BIG_RHOMBUS);

        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.NONE, ECombine.RAINBOW_NORMAL);

        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.VERTICAL_BOMB, ECombine.RAINBOW_DIRECTION);
        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.HORIZONTAL_BOMB, ECombine.RAINBOW_DIRECTION);

        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.X_BOMB, ECombine.RAINBOW_X);

        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.RHOMBUS_BOMB, ECombine.RAINBOW_RHOMBUS);

        combines.Add((int)EItem.COLOR_BOMB + (int)EItem.COLOR_BOMB, ECombine.RAINBOW_RAINBOW);

        combines.Add((int)EItem.FISH + (int)EItem.HORIZONTAL_BOMB, ECombine.FISH_HORIZONTAL);
        combines.Add((int)EItem.FISH + (int)EItem.VERTICAL_BOMB, ECombine.FISH_VERTICAL);
        combines.Add((int)EItem.FISH + (int)EItem.RHOMBUS_BOMB, ECombine.FISH_RHOMBUS);
        combines.Add((int)EItem.FISH + (int)EItem.COLOR_BOMB, ECombine.RAINBOW_FISH);
        combines.Add((int)EItem.FISH + (int)EItem.FISH, ECombine.FISH_FISH);
        combines.Add((int)EItem.FISH + (int)EItem.X_BOMB, ECombine.FISH_X);

        combines.Add((int)EItem.BOOSTER_ASTERISK + (int)EItem.NONE, ECombine.ACTIVE_ASTERISK);

        combines.Add((int)EItem.BOOSTER_GREATBOMB + (int)EItem.NONE, ECombine.ACTIVE_GREATBOMB);
    }

    private void InitBlockKind()
    {
        var normalID = new List<EID>();
        var bombID = new List<EID>();
        var obstacleID = new List<EID>();

        normalID.Add(EID.NORMAL);

        bombID.Add(EID.HORIZONTAL);
        bombID.Add(EID.VERTICAL);
        bombID.Add(EID.X);
        bombID.Add(EID.RHOMBUS);
        bombID.Add(EID.COLOR_BOMB);
        bombID.Add(EID.FISH);

        obstacleID.Add(EID.OAK);
        obstacleID.Add(EID.BOX);
        obstacleID.Add(EID.BOX_COLOR);
        obstacleID.Add(EID.GLASS_COLOR);
        obstacleID.Add(EID.LAVA);
        obstacleID.Add(EID.ICE);
        obstacleID.Add(EID.BIG_SIDE);
        obstacleID.Add(EID.BIG_SIDE_COLOR);
        obstacleID.Add(EID.BIG_SIDE_DIRECTION);
        obstacleID.Add(EID.JAIL);
        obstacleID.Add(EID.TIMEBOMB_ICE);
        obstacleID.Add(EID.TIMEBOMB_LAVA);
        obstacleID.Add(EID.SHIELD);
        obstacleID.Add(EID.ACTINIARIA);
        obstacleID.Add(EID.CLAM);
        obstacleID.Add(EID.BANDAGE);
        obstacleID.Add(EID.CREATOR_LAVA);
        obstacleID.Add(EID.CREATOR_BOMB);
        obstacleID.Add(EID.METAL_OAK);
        obstacleID.Add(EID.FACTION);
        obstacleID.Add(EID.CLIMBER_NEST);

        blockKindList.Add(EBlockKind.NORMAL, normalID);
        blockKindList.Add(EBlockKind.BOMB, bombID);
        blockKindList.Add(EBlockKind.OBSTACLE, obstacleID);
    }

    public void ChangeColorToMapSetting(List<Vector2Int> etc)
    {
        foreach (var list in popList)
            foreach (var block in list)
            {
                var data = StageManager.GetInstance.GetBlockInfo();

                if (data.color == EColor.NONE) continue;
                if (data.color == block.Color) continue;

                var isSettingBlock = false;
                foreach (var matrix in etc)
                    if (matrix == block.Tile.Matrix)
                        //if(GetBlockKind(block.ID) != EBlockKind.NONE)
                        isSettingBlock = true;

                if (isSettingBlock) continue;

                block.Setting(data.color, block.ID, block.HP, block.ETC);
                block.ApplySprite();
            }

        popList.Clear();
    }

    public void AddStartItem(int ranNum)
    {
        var targetBlock = GetRandomNormalBlockOrNull();
        if (targetBlock == null) return;

        var id = EID.NONE;
        switch (ranNum)
        {
            case 0:
                id = EID.VERTICAL;
                break;

            case 1:
                id = EID.HORIZONTAL;
                break;

            case 2:
                id = EID.RHOMBUS;
                break;

            case 3:
                id = EID.COLOR_BOMB;
                break;
        }
        
        var blockColor = EColor.NONE;
        //var blockColor = targetBlock.Color;
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy") || BaseSystem.GetInstance.GetSystemList("Castle"))
            {
                blockColor = targetBlock.Color;
            }
            else
            {
                blockColor = EColor.NONE;
            }
        }
        else
        {
            blockColor = EColor.NONE;
        }

        if (id == EID.COLOR_BOMB) blockColor = EColor.NONE;
        var sprite = GetBlockSprite(id, blockColor, 1);
        if (sprite == null) return;
        if (rewardChangeItem == null) return;

        //rewardChangeItem.SetProperty(targetBlock, id, EColor.NONE);
        //rewardChangeItem.SetProperty(targetBlock, id, targetBlock.Color);
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy") || BaseSystem.GetInstance.GetSystemList("Castle"))
            {
                rewardChangeItem.SetProperty(targetBlock, id, targetBlock.Color);
            }
            else
            {
                rewardChangeItem.SetProperty(targetBlock, id, EColor.NONE);
            }
        }
        else
        {
            rewardChangeItem.SetProperty(targetBlock, id, EColor.NONE);
        }
        rewardChangeItem.StartAnim(sprite, new Vector2(0, 0.4f), targetBlock.Tile.GetPosition());
    }

    public GameBlock GetRandomNormalBlockOrNull()
    {
        var targetBlock = new List<GameBlock>();

        foreach (var block in blockList)
            if (block.ID == EID.NORMAL)
            {
                if (!TileManager.GetInstance.IsPreViewTile(block.Tile)) continue;
                if (block.Tile.GetTopBlockOrNull() == null) targetBlock.Add(block);
            }

        if (targetBlock.Count == 0) return null;

        var ranNum = Random.Range(0, targetBlock.Count);

        return targetBlock[ranNum];
    }

    public void SetBlockToItem(EID ID)
    {
        var randomBlock = GetRandomNormalBlockOrNull();

        if (randomBlock == null) return;

        randomBlock.Setting(randomBlock.Color, ID);
        randomBlock.ApplySprite();
        if (randomBlock.Tile != null)
        {
            var particle = DynamicObjectPool.GetInstance.GetObjectForType("Particle_ChangeItem", false);
            particle.transform.position = randomBlock.Tile.GetPosition();
        }
    }

    public void ShowClearBomb()
    {
        StartCoroutine(ClearBombCoroutine());
    }

    private IEnumerator ClearBombCoroutine()
    {
        var bombList = new List<GameBlock>();

        foreach (var block in blockList)
        {
            if (!TileManager.GetInstance.IsPreViewTile(block.Tile)) continue;
            if (block.ID == EID.COLOR_BOMB) bombList.Add(block);
        }

        firstBlock = null;
        secondBlock = null;
        var rainColor = EColor.NONE;
        foreach (var bomb in bombList)
        {
            rainColor = (EColor)Random.Range(0, 5);
            bomb.Tile.RegisterCombinePop(ECombine.RAINBOW_NORMAL, rainColor);
            //bomb.Tile.RemoveBlock(bomb);
        }

        bombList.Clear();

        yield return new WaitUntil(() => AnimationManager.AnimCount == 0);

        foreach (var block in blockList)
        {
            if (!TileManager.GetInstance.IsPreViewTile(block.Tile)) continue;
            if (block.ID == EID.HORIZONTAL || block.ID == EID.VERTICAL
                                           || block.ID == EID.RHOMBUS || block.ID == EID.X || block.ID == EID.FISH)
                bombList.Add(block);
        }

        foreach (var bomb in bombList) bomb.Pop();

        LogicManager.GetInstance.ChangeLogicState(new PopLogic());
    }

    public void CreateChangeBlockMissile(Transform startTransform)
    {
        var targetBlocks = new List<GameBlock>();

        foreach (var block in blockList)
        {
            if (!TileManager.GetInstance.IsPreViewTile(block.Tile)) continue;
            if (block.ID == EID.NORMAL) targetBlocks.Add(block);
        }

        if (targetBlocks.Count == 0)
        {
            StageManager.GetInstance.AddScore(3000);
            return;
        }

        var particleObj = DynamicObjectPool.GetInstance.GetObjectForType("RemainMoveParticle", true);
        if (particleObj == null) return;

        var moving = particleObj.GetComponent<Moving>();
        if (moving == null) return;

        FireworkCount++;
        var randomValue = Random.Range(0, targetBlocks.Count);

        var destination = targetBlocks[randomValue].transform;
        EColor color;
        if (DoubleClickSystem.GetInstance != null)
        {
            color = //(EColor)Random.Range(0, (int)EColor.NONE);
                EColor.NONE;//targetBlocks[randomValue].Color;
        }
        else
        {
            color = (EColor)Random.Range(0, (int)EColor.NONE);
        }

        var item = randomValue % 2 == 0 ? EID.HORIZONTAL : EID.VERTICAL;
        targetBlocks[randomValue].Setting(color, item);
        StageManager.GetInstance.AddScore(3000);

        Action action = () =>
        {
            targetBlocks[randomValue].ApplySprite();
            //BlockManager.GetInstance.CreateScore(blockList[randomValue].Matrix, 100);
            DynamicObjectPool.GetInstance.PoolObject(moving.gameObject, false);
            FireworkCount--;
            var par = DynamicObjectPool.GetInstance.GetObjectForType("Particle_ChangeItem", true);
            par.transform.position = destination.position;
        };

        moving.StartAnim(startTransform, destination, action);
    }

    public bool IsActiveBomb()
    {
        var returnValue = false;
        foreach (var block in blockList)
            if (block.ID == EID.HORIZONTAL || block.ID == EID.VERTICAL || block.ID == EID.X || block.ID == EID.RHOMBUS
                || block.ID == EID.COLOR_BOMB || block.ID == EID.FISH)
                if (block.Tile != null)
                    if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                        returnValue = true;
        return returnValue;
    }

    public void ActiveAutoTargetBomb(GameTile startTile, EColor color, ECombine combine)
    {
        var autoTargetBombObj = DynamicObjectPool.GetInstance.GetObjectForType("AutoTargetBomb", false);
        if (autoTargetBombObj != null)
        {
            var autoTargetBomb = autoTargetBombObj.GetComponent<AutoTargetBomb>();
            if (autoTargetBomb != null)
            {
                autoTargetBomb.Setting(startTile.GetPosition(), color, combine, startTile.IsFreeJam());
                var vector = autoTargetGetter.GetAutoTargetPosition(blockList, startTile.IsFreeJam());
                var targetTile = TileManager.GetInstance.GetTileOrNull(vector);

                if (targetTile != null)
                    autoTargetBomb.SetTarget(targetTile);
            }
        }
    }

    public Vector2Int GetAutoTargetVector2Int(bool isFreejam)
    {
        return autoTargetGetter.GetAutoTargetPosition(blockList, isFreejam);
    }

    public EBlockKind GetBlockKind(EID id)
    {
        foreach (var item in blockKindList)
            if (item.Value.Contains(id))
                return item.Key;

        return EBlockKind.NONE;
    }

    public Vector2Int GetBigObjSize(EID id)
    {
        if (bigSizeDictionary.ContainsKey(id)) return bigSizeDictionary[id];
        return Vector2Int.zero;
    }

    public GameObject GetBigObject(EID id)
    {
        GameObject bigObj = null;
        if (id == EID.BIG_SIDE)
            bigObj = DynamicObjectPool.GetInstance.GetObjectForType("BigSideObj", false);
        if (id == EID.BIG_SIDE_COLOR)
            bigObj = DynamicObjectPool.GetInstance.GetObjectForType("BigSideColorObj", false);
        if (id == EID.BIG_SIDE_DIRECTION)
            bigObj = DynamicObjectPool.GetInstance.GetObjectForType("BigSideDirectionObj", false);
        if (id == EID.GEAR)
            bigObj = DynamicObjectPool.GetInstance.GetObjectForType("Gear", false);
        if (id == EID.TABLET)
            bigObj = DynamicObjectPool.GetInstance.GetObjectForType("Tablet22", false);
        return bigObj;
    }

    public Sprite GetBigSideColorSprite(EColor color)
    {
        if (color != EColor.NONE) return bigSideColorPartSprite[(int)color];
        return null;
    }

    public GameBlock GetGameBlockOrNull(EID id)
    {
        GameObject obj = null;
        if (DataContainer.GetInstance.GetObstacleDataOrNULL(id) == null)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("NormalBlock", true);
        else if (id != EID.CHAMELEON && id != EID.GEAR_CORE && id != EID.ACTINIARIA && id != EID.CLAM
                 && id != EID.TURN_BOX && id != EID.FIZZ && id != EID.DOUBLE && id != EID.CREATOR_LAVA &&
                 id != EID.CREATOR_BOMB && id != EID.FACTION)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("ObstacleBlock", false);
        else if (id == EID.CHAMELEON)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("ChameleonBlock", true);
        else if (id == EID.GEAR_CORE)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("GearCore", true);
        else if (id == EID.ACTINIARIA)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("Bush", true);
        else if (id == EID.CLAM)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("Clam", true);
        else if (id == EID.TURN_BOX)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("TurnBox", true);
        else if (id == EID.FIZZ)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("Fizz", true);
        else if (id == EID.DOUBLE)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("DoubleBlock", true);
        else if (id == EID.CREATOR_LAVA)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("ObLavaCreator", true);
        else if (id == EID.CREATOR_BOMB)
            obj = DynamicObjectPool.GetInstance.GetObjectForType("ObBombCreator", true);
        else if (id == EID.FACTION) obj = DynamicObjectPool.GetInstance.GetObjectForType("Faction", true);
        if (obj != null)
        {
            var block = obj.GetComponent<GameBlock>();
            if (block != null)
            {
                block.Init();
                blockList.Add(block);
                if (id == EID.TIMEBOMB_ICE || id == EID.TIMEBOMB_LAVA)
                {
                    var timeText = DynamicObjectPool.GetInstance.GetObjectForType("TimeBombText", false);
                    if (timeText != null) timeText.transform.SetParent(block.transform);
                }

                return block;
            }
        }

        return null;
    }

    public void BombEnd()
    {
        foreach (var block in blockList)
            if (block.IsBombed)
                block.IsBombed = false;
    }

    public Sprite GetBlockSprite(EID _id, EColor _color, int _hp)
    {
        return spriteContainer.GetSpriteOrNull(_id, _color, _hp);
    }

    public Sprite GetMissionSprite(EID _id)
    {
        return spriteContainer.GetMissionImage(_id);
    }

    public GameBlock GetConnectBlockOrNull(GameBlock block, EDirection direction)
    {
        var tile = block.Tile;
        if (tile == null) return null;
        var connectTile = tile.GetConnectTileOrNull(direction);
        if (connectTile == null) return null;
        var returnBlock = connectTile.NormalBlock;
        if (returnBlock == null) return null;

        return returnBlock;
    }

    public bool NormalPopCheck(List<GameBlock> floodBlocks, bool isFirstSetting = false)
    {
        var returnValue = false;

        var fishCheck = false;
        if (StaticGameSettings.IsAbleFish)//if (false)          //FishCheck
        {
            if (floodBlocks.Count < 4)
                fishCheck = false;
            else
                fishCheck = FishCheck(floodBlocks);
        }
        var MatchCheck = false;

        for (var d = 0; d < 4; d++)
        {
            if (fishCheck || MatchCheck) break;

            foreach (var block in floodBlocks)
            {
                var tile = block.Tile;
                if (block.IsDropAbleMaybe()) return false;
                if (tile == null) continue;
                if (!isFirstSetting)
                    if (!TileManager.GetInstance.IsPreViewTile(tile))
                        continue;
                if (!tile.IsPopAble()) continue;
                if (tile.IsDropAble(tile.DropDirection)) continue;

                var nextTile = tile.GetConnectTileOrNull((EDirection)d);
                if (nextTile == null) continue;
                if (!nextTile.IsPopAble()) continue;
                if (!isFirstSetting)
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile))
                        continue;
                var nextBlock = nextTile.NormalBlock;
                if (nextBlock == null) continue;
                if (nextBlock.IsDropAbleMaybe()) return false;
                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;

                var isColorMatch = true;
                var matchCount = 1;

                while (isColorMatch)
                    if (block.Color == nextBlock.Color)
                    {
                        matchCount++;
                        nextTile = nextTile.GetConnectTileOrNull((EDirection)d);
                        if (nextTile == null)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!nextTile.IsPopAble())
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!isFirstSetting)
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile))
                            {
                                isColorMatch = false;
                                break;
                            }

                        nextBlock = nextTile.NormalBlock;
                        if (nextBlock == null)
                        {
                            isColorMatch = false;
                        }
                        else if (nextBlock.IsDropAbleMaybe())
                        {
                            return false;
                            isColorMatch = false;
                            matchCount = 0;
                            break;
                        }
                    }
                    else
                    {
                        isColorMatch = false;
                    }

                if (matchCount > 2)
                    if (!fishCheck)
                        MatchCheck = true;
            }
        }

        if (fishCheck || MatchCheck)
            if (!popList.Contains(floodBlocks))
            {
                popList.Add(floodBlocks);
                returnValue = true;
            }

        return returnValue;
    }

    public bool PopCheck(bool isFirstSetting = false)
    {
        if (DoubleClickSystem.GetInstance != null)
        {
            bool useDoubleClickBlock = false;
            if (firstBlock != null)
            {
                foreach (var item in DoubleClickSystem.GetInstance.GetBlockList())
                {
                    if (firstBlock.ID == item)
                    {
                        useDoubleClickBlock = true;
                        break;
                    }
                }

                if (useDoubleClickBlock && secondBlock.ID == EID.NORMAL)
                {
                    //firstBlock.BombPop();
                    secondBlock.isUseSp = firstBlock;
                    firstBlock = null;
                    //secondBlock = null;
                    //return true;
                }
            }
        }

        var returnValue = false;
        foreach (var block in blockList)
        {
            if (block == null) continue;
            var tile = block.Tile;
            if (tile == null) continue;
            if (!isFirstSetting)
                if (!TileManager.GetInstance.IsPreViewTile(tile))
                    continue;
            if (!tile.IsPopAble()) continue;
            if (block.IsDropAbleMaybe()) continue;
            if (block.IsMatchMark) continue;
            if (block.IsCombine) continue;
            if (block.ID == EID.COLOR_BOMB) continue;
            if (block.IsDropAble() && tile.IsDropAble(tile.DropDirection)) continue;
            //블럭 리스트 생성
            var floodBlocks = new List<GameBlock>();
            floodBlocks.Add(block);
            block.IsMatchMark = true;
            for (var i = 0; i < floodBlocks.Count; i++)
                for (var d = 0; d < 4; d++)
                {
                    if (floodBlocks[i].Tile == null) continue;
                    var nextTile = floodBlocks[i].Tile.GetConnectTileOrNull((EDirection)d);
                    if (nextTile == null) continue;
                    if (!nextTile.IsPopAble()) continue;
                    if (!isFirstSetting)
                        if (!TileManager.GetInstance.IsPreViewTile(nextTile))
                            continue;
                    var nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) continue;
                    if (nextBlock.IsCombine) continue;
                    if (nextBlock.IsDropAbleMaybe()) break;
                    if (nextBlock.ID == EID.COLOR_BOMB) continue;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                    if (block.Color != EColor.NONE)
                    {
                        if (block.Color == nextBlock.Color)
                            if (!nextBlock.IsMatchMark)
                            {
                                floodBlocks.Add(nextBlock);
                                nextBlock.IsMatchMark = true;
                            }
                    }
                }

            //블럭 리스트 생성 완료
            if (floodBlocks.Count < 3)
            {
                for (int i = 0; i < floodBlocks.Count; i++)
                {
                    floodBlocks[0].IsMatchMark = false;
                }
                continue;
            }

            if (NormalPopCheck(floodBlocks, isFirstSetting))
            {
                returnValue = true;
            }
        }

        if (!returnValue && secondBlock != null)
        {
            if (secondBlock.isUseSp != null)
            {
                secondBlock.isUseSp.BombPop();
                secondBlock.isUseSp = null;
                secondBlock = null;

                return true;
            }
        }

        return returnValue;
    }

    public List<GameBlock> GetBlockList(EID blockEID)
    {
        List<GameBlock> listClimber = new List<GameBlock>();
        foreach (var block in blockList)
        {
            if (block.ID == blockEID)
            {
                listClimber.Add(block);
            }
        }

        return listClimber;
    }

    public List<GameBlock> GetMovingBlockList(EID blockEID)
    {
        List<GameBlock> listClimber = new List<GameBlock>();
        foreach (var block in blockList)
        {
            if (block.ID == blockEID)
            {
                if (block.Tile.IsDropAble())
                {
                    listClimber.Add(block);
                }
            }
        }

        return listClimber;
    }

    private bool IsFishAvoidHint(params GameBlock[] _block)
    {
        bool r_isGlassColor = false;
        foreach (GameBlock obj in _block)
        {
            switch (obj.ID)
            {
                case EID.GLASS_COLOR:
                case EID.DROP_RELIC1:
                case EID.DROP_RELIC2:
                case EID.DROP_RELIC3:
                    {
                        return true;
                    }
            }
        }

        return r_isGlassColor;
    }

    public HintInfo HintCheck()
    {
        var HintBlockList = new List<HintInfo>();

        var returnValue = new HintInfo();
        var item = EItem.NONE;
        var Combine = ECombine.NONE;
        var _isItemShaking = false;
        var ItemShakeList = new HintInfo();
        foreach (var block in blockList)
        {
            Combine = ECombine.NONE;
            for (var d = 0; d < 4; d++)
            {
                if (block == null) continue;
                var tile = block.Tile;
                if (tile == null) continue;
                if (!tile.IsSwapAble()) continue;
                if (!TileManager.GetInstance.IsPreViewTile(tile)) continue;
                var nextTile = block.Tile;
                GameBlock nextBlock = null;

                item = EItem.NONE;
                var hintList = new List<GameBlock>();
                var reverseBlocks = new List<GameBlock>();
                var leftBlocks = new List<GameBlock>();
                var rightBlocks = new List<GameBlock>();
                var reverse = EDirection.NONE;
                var left = EDirection.NONE;
                var right = EDirection.NONE;
                if ((EDirection)d == EDirection.UP)
                {
                    reverse = EDirection.DOWN;
                    left = EDirection.RIGHT;
                    right = EDirection.LEFT;
                }
                else if ((EDirection)d == EDirection.LEFT)
                {
                    reverse = EDirection.RIGHT;
                    left = EDirection.UP;
                    right = EDirection.DOWN;
                }
                else if ((EDirection)d == EDirection.DOWN)
                {
                    reverse = EDirection.UP;
                    left = EDirection.LEFT;
                    right = EDirection.RIGHT;
                }
                else if ((EDirection)d == EDirection.RIGHT)
                {
                    reverse = EDirection.LEFT;
                    left = EDirection.DOWN;
                    right = EDirection.UP;
                }

                if (block.ID == EID.COLOR_BOMB || block.ID == EID.FISH || block.ID == EID.HORIZONTAL ||
                    block.ID == EID.VERTICAL || block.ID == EID.X || block.ID == EID.RHOMBUS)
                {
                    var ItemShake = false;
                    for (var d2 = 0; d2 < 4; d2++)
                    {
                        var reversed2 = EDirection.NONE;
                        if ((EDirection)d2 == EDirection.UP) reversed2 = EDirection.DOWN;
                        else if ((EDirection)d2 == EDirection.DOWN) reversed2 = EDirection.UP;
                        else if ((EDirection)d2 == EDirection.LEFT) reversed2 = EDirection.RIGHT;
                        else if ((EDirection)d2 == EDirection.RIGHT) reversed2 = EDirection.LEFT;

                        nextTile = block.Tile.GetConnectTileOrNull((EDirection)d2);
                        if (nextTile == null) continue;
                        if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                        //if (!nextTile.IsSwapAble()) continue;
                        if (!nextTile.IsSwapAble(reversed2)) continue;
                        nextBlock = nextTile.NormalBlock;
                        if (nextBlock == null) continue;
                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                        if (nextBlock.ID == EID.COLOR_BOMB || nextBlock.ID == EID.FISH ||
                            nextBlock.ID == EID.HORIZONTAL ||
                            nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.X || nextBlock.ID == EID.RHOMBUS ||
                            nextBlock.ID == EID.NORMAL)
                        {
                            var hintInfo = new HintInfo();
                            hintInfo.centerBlock = block;
                            hintInfo.moveBlock = nextBlock;
                            hintInfo.centerBlockDirection = (EDirection)d2;
                            hintInfo.moveBlockDirection = reversed2;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.Add(block);
                            hintInfo.blocks.Add(nextBlock);
                            ItemShake = true;

                            if (block.ID == EID.COLOR_BOMB)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_RAINBOW;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.RAINBOW_X;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.RAINBOW_RHOMBUS;
                                else if (nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.HORIZONTAL)
                                    Combine = ECombine.RAINBOW_DIRECTION;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.RAINBOW_FISH;
                                else if (nextBlock.ID == EID.NORMAL) Combine = ECombine.RAINBOW_NORMAL;
                            }
                            else if (block.ID == EID.X)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_X;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.ASTERISK;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.BIG_X;
                                else if (nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.HORIZONTAL)
                                    Combine = ECombine.ASTERISK;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.FISH_X;
                                else if (nextBlock.ID == EID.NORMAL) ItemShake = false;
                            }
                            else if (block.ID == EID.RHOMBUS)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_RHOMBUS;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.BIG_X;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.BIG_RHOMBUS;
                                else if (nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.HORIZONTAL)
                                    Combine = ECombine.BIG_CROSS;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.FISH_RHOMBUS;
                                else if (nextBlock.ID == EID.NORMAL) ItemShake = false;
                            }
                            else if (block.ID == EID.HORIZONTAL)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_DIRECTION;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.ASTERISK;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.BIG_CROSS;
                                else if (nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.HORIZONTAL)
                                    Combine = ECombine.CROSS;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.FISH_HORIZONTAL;
                                else if (nextBlock.ID == EID.NORMAL) ItemShake = false;
                            }
                            else if (block.ID == EID.VERTICAL)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_DIRECTION;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.ASTERISK;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.BIG_CROSS;
                                else if (nextBlock.ID == EID.VERTICAL || nextBlock.ID == EID.HORIZONTAL)
                                    Combine = ECombine.CROSS;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.FISH_VERTICAL;
                                else if (nextBlock.ID == EID.NORMAL) ItemShake = false;
                            }
                            else if (block.ID == EID.FISH)
                            {
                                if (nextBlock.ID == EID.COLOR_BOMB) Combine = ECombine.RAINBOW_FISH;
                                else if (nextBlock.ID == EID.X) Combine = ECombine.FISH_X;
                                else if (nextBlock.ID == EID.RHOMBUS) Combine = ECombine.FISH_RHOMBUS;
                                else if (nextBlock.ID == EID.VERTICAL) Combine = ECombine.FISH_VERTICAL;
                                else if (nextBlock.ID == EID.HORIZONTAL) Combine = ECombine.FISH_HORIZONTAL;
                                else if (nextBlock.ID == EID.FISH) Combine = ECombine.FISH_FISH;
                                else if (nextBlock.ID == EID.NORMAL) ItemShake = false;
                            }

                            if (ItemShake)
                            {
                                hintInfo.combine = Combine;
                                var IntValue = (int)hintInfo.combine;
                                if (IntValue == (int)ECombine.RAINBOW_NORMAL) IntValue = -1;

                                if (_isItemShaking == false)
                                {
                                    _isItemShaking = true;
                                    ItemShakeList = hintInfo;
                                }
                                else if ((int)ItemShakeList.combine < IntValue)
                                {
                                    ItemShakeList = hintInfo;
                                }
                            }
                        }
                    }
                }

                nextTile = block.Tile;
                if (!nextTile.IsSwapAble()) continue;
                for (var i = 0; i < 2; i++)
                {
                    nextTile = nextTile.GetConnectTileOrNull((EDirection)d);
                    if (nextTile == null) break;
                    if (!nextTile.IsPopAble()) break;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                    nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) break;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;

                    //if (nextBlock.ID != EID.NORMAL) break;
                    if (i == 0 && nextBlock.Color != block.Color) hintList.Add(nextBlock);
                    if (i == 1)
                        if (hintList.Count != 0)
                        {
                            if (nextBlock.Color == hintList[0].Color) hintList.Add(nextBlock);
                            else break;
                        }
                }

                if (hintList.Count == 0) continue;

                nextTile = block.Tile;
                for (var i = 0; i < 2; i++)
                {
                    nextTile = nextTile.GetConnectTileOrNull(reverse);
                    if (nextTile == null) break;
                    if (!nextTile.IsPopAble()) break;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                    nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) break;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                    if (nextBlock.Color == hintList[0].Color && i == reverseBlocks.Count) reverseBlocks.Add(nextBlock);
                }

                //LeftCheck
                nextTile = block.Tile;
                for (var i = 0; i < 2; i++)
                {
                    nextTile = nextTile.GetConnectTileOrNull(left);
                    if (nextTile == null) break;
                    if (!nextTile.IsPopAble()) break;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                    nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) break;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                    if (nextBlock.Color == hintList[0].Color && i == leftBlocks.Count) leftBlocks.Add(nextBlock);
                }

                //RightCheck
                nextTile = block.Tile;
                for (var i = 0; i < 2; i++)
                {
                    nextTile = nextTile.GetConnectTileOrNull(right);
                    if (nextTile == null) break;
                    if (!nextTile.IsPopAble()) break;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                    nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) break;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                    if (nextBlock.Color == hintList[0].Color && i == rightBlocks.Count) rightBlocks.Add(nextBlock);
                }

                if (hintList.Count == 2)
                {
                    if (reverseBlocks.Count == 2)
                    {
                        if (leftBlocks.Count != 0 && leftBlocks[0].Tile.IsSwapAble(right))
                        {
                            item = EItem.COLOR_BOMB;
                            hintList.AddRange(reverseBlocks);
                            hintList.Add(leftBlocks[0]);
                            var hintInfo = new HintInfo();
                            hintInfo.eItem = item;
                            hintInfo.centerBlock = block;
                            hintInfo.centerBlockDirection = left;
                            hintInfo.moveBlock = leftBlocks[0];
                            hintInfo.moveBlockDirection = right;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.AddRange(hintList);
                            HintBlockList.Add(hintInfo);
                        }
                    }
                    else if (leftBlocks.Count == 2)
                    {
                        if (leftBlocks.Count == 2)
                        {
                            if (reverseBlocks.Count != 0 && reverseBlocks[0].Tile.IsSwapAble((EDirection)d))
                            {
                                item = EItem.RHOMBUS_BOMB;
                                hintList.AddRange(leftBlocks);
                                hintList.Add(reverseBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = reverse;
                                hintInfo.moveBlock = reverseBlocks[0];
                                hintInfo.moveBlockDirection = (EDirection)d;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                            else if (rightBlocks.Count != 0 && rightBlocks[0].Tile.IsSwapAble(left))
                            {
                                item = EItem.RHOMBUS_BOMB;
                                hintList.AddRange(leftBlocks);
                                hintList.Add(rightBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = right;
                                hintInfo.moveBlock = rightBlocks[0];
                                hintInfo.moveBlockDirection = left;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                            else
                            {
                                if (leftBlocks[0].Tile.IsSwapAble(right))
                                {
                                    hintList.Add(leftBlocks[0]);
                                    var hintInfo = new HintInfo();
                                    hintInfo.eItem = item;
                                    hintInfo.centerBlock = block;
                                    hintInfo.centerBlockDirection = left;
                                    hintInfo.moveBlock = leftBlocks[0];
                                    hintInfo.moveBlockDirection = right;
                                    hintInfo.blocks = new List<GameBlock>();
                                    hintInfo.blocks.AddRange(hintList);
                                    HintBlockList.Add(hintInfo);
                                }
                            }
                        }
                    }
                    else if (leftBlocks.Count != 0 && rightBlocks.Count != 0 && reverseBlocks.Count != 0 &&
                             reverseBlocks[0].Tile.IsSwapAble((EDirection)d))
                    {
                        item = EItem.X_BOMB;

                        hintList.Add(leftBlocks[0]);
                        hintList.Add(rightBlocks[0]);
                        hintList.Add(reverseBlocks[0]);
                        var hintInfo = new HintInfo();
                        hintInfo.eItem = item;
                        hintInfo.centerBlock = block;
                        hintInfo.centerBlockDirection = reverse;
                        hintInfo.moveBlock = reverseBlocks[0];
                        hintInfo.moveBlockDirection = (EDirection)d;
                        hintInfo.blocks = new List<GameBlock>();
                        hintInfo.blocks.AddRange(hintList);
                        HintBlockList.Add(hintInfo);
                    }
                    else if (reverseBlocks.Count != 0)
                    {
                        if (leftBlocks.Count != 0 && leftBlocks[0].Tile.IsSwapAble(right))
                        {
                            if (left == EDirection.DOWN || left == EDirection.UP)
                            {
                                item = EItem.VERTICAL_BOMB;
                                hintList.Add(reverseBlocks[0]);
                                hintList.Add(leftBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = left;
                                hintInfo.moveBlock = leftBlocks[0];
                                hintInfo.moveBlockDirection = right;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                            else
                            {
                                item = EItem.HORIZONTAL_BOMB;
                                hintList.Add(reverseBlocks[0]);
                                hintList.Add(leftBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = left;
                                hintInfo.moveBlock = leftBlocks[0];
                                hintInfo.moveBlockDirection = right;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                        }
                        else if (rightBlocks.Count != 0 && rightBlocks[0].Tile.IsSwapAble(left))
                        {
                            if (right == EDirection.DOWN || right == EDirection.UP)
                            {
                                item = EItem.VERTICAL_BOMB;
                                hintList.Add(reverseBlocks[0]);
                                hintList.Add(rightBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = right;
                                hintInfo.moveBlock = rightBlocks[0];
                                hintInfo.moveBlockDirection = left;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                            else
                            {
                                item = EItem.HORIZONTAL_BOMB;
                                hintList.Add(reverseBlocks[0]);
                                hintList.Add(rightBlocks[0]);
                                var hintInfo = new HintInfo();
                                hintInfo.eItem = item;
                                hintInfo.centerBlock = block;
                                hintInfo.centerBlockDirection = right;
                                hintInfo.moveBlock = rightBlocks[0];
                                hintInfo.moveBlockDirection = left;
                                hintInfo.blocks = new List<GameBlock>();
                                hintInfo.blocks.AddRange(hintList);
                                HintBlockList.Add(hintInfo);
                            }
                        }
                    }
                    else if (leftBlocks.Count != 0 && rightBlocks.Count != 0) //FishCheck -> remove false
                    {
                        var leftCheck = false;
                        nextBlock = null;
                        var rD = EDirection.NONE;

                        switch ((EDirection)d)
                        {
                            case EDirection.DOWN:
                                rD = EDirection.UP;
                                break;

                            case EDirection.UP:
                                rD = EDirection.DOWN;
                                break;

                            case EDirection.RIGHT:
                                rD = EDirection.LEFT;
                                break;

                            case EDirection.LEFT:
                                rD = EDirection.RIGHT;
                                break;
                        }

                        while (!leftCheck)
                        {
                            nextTile = leftBlocks[0].Tile.GetConnectTileOrNull((EDirection)d);
                            if (nextTile == null) break;
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                            if (!nextTile.IsSwapAble(rD)) break;
                            nextBlock = nextTile.NormalBlock;
                            if (nextBlock == null) break;
                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                            if (nextBlock.Color != leftBlocks[0].Color) break;
                            leftCheck = true;
                        }

                        if (leftCheck && rightBlocks[0].Tile.IsSwapAble(left))
                        {
                            if (StaticGameSettings.IsAbleFish)
                            {
                                if (false == IsFishAvoidHint(leftBlocks[0], rightBlocks[0], nextBlock))
                                {
                                    item = EItem.FISH;
                                    hintList.Add(leftBlocks[0]);
                                    hintList.Add(rightBlocks[0]);
                                    hintList.Add(nextBlock);
                                    var hintInfo = new HintInfo();
                                    hintInfo.eItem = item;
                                    hintInfo.centerBlock = block;
                                    hintInfo.centerBlockDirection = right;
                                    hintInfo.moveBlock = rightBlocks[0];
                                    hintInfo.moveBlockDirection = left;
                                    hintInfo.blocks = new List<GameBlock>();
                                    hintInfo.blocks.AddRange(hintList);
                                    HintBlockList.Add(hintInfo);
                                }
                            }
                        }
                        else
                        {
                            var rightCheck = false;
                            while (!rightCheck)
                            {
                                nextTile = rightBlocks[0].Tile.GetConnectTileOrNull((EDirection)d);
                                if (nextTile == null) break;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                if (!nextTile.IsSwapAble(rD)) break;
                                nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) break;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                if (nextBlock.Color != leftBlocks[0].Color) break;
                                rightCheck = true;
                            }

                            if (rightCheck && leftBlocks[0].Tile.IsSwapAble(right))
                            {
                                if (StaticGameSettings.IsAbleFish)
                                {
                                    if (false == IsFishAvoidHint(leftBlocks[0], rightBlocks[0], nextBlock))
                                    {
                                        item = EItem.FISH;
                                        hintList.Add(leftBlocks[0]);
                                        hintList.Add(rightBlocks[0]);
                                        hintList.Add(nextBlock);
                                        var hintInfo = new HintInfo();
                                        hintInfo.eItem = item;
                                        hintInfo.centerBlock = block;
                                        hintInfo.centerBlockDirection = left;
                                        hintInfo.moveBlock = leftBlocks[0];
                                        hintInfo.moveBlockDirection = right;
                                        hintInfo.blocks = new List<GameBlock>();
                                        hintInfo.blocks.AddRange(hintList);
                                        HintBlockList.Add(hintInfo);
                                    }
                                }
                            }
                        }
                    }

                    if (item == EItem.NONE)
                    {
                        if (leftBlocks.Count != 0 && leftBlocks[0].Tile.IsSwapAble(right))
                        {
                            hintList.Add(leftBlocks[0]);
                            var hintInfo = new HintInfo();
                            hintInfo.eItem = item;
                            hintInfo.centerBlock = block;
                            hintInfo.centerBlockDirection = left;
                            hintInfo.moveBlock = leftBlocks[0];
                            hintInfo.moveBlockDirection = right;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.AddRange(hintList);
                            HintBlockList.Add(hintInfo);
                        }
                        else if (rightBlocks.Count != 0 && rightBlocks[0].Tile.IsSwapAble(left))
                        {
                            hintList.Add(rightBlocks[0]);
                            var hintInfo = new HintInfo();
                            hintInfo.eItem = item;
                            hintInfo.centerBlock = block;
                            hintInfo.centerBlockDirection = right;
                            hintInfo.moveBlock = rightBlocks[0];
                            hintInfo.moveBlockDirection = left;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.AddRange(hintList);
                            HintBlockList.Add(hintInfo);
                        }
                        else if (reverseBlocks.Count != 0 && reverseBlocks[0].Tile.IsSwapAble((EDirection)d))
                        {
                            hintList.Add(reverseBlocks[0]);
                            var hintInfo = new HintInfo();
                            hintInfo.eItem = item;
                            hintInfo.centerBlock = block;
                            hintInfo.centerBlockDirection = reverse;
                            hintInfo.moveBlock = reverseBlocks[0];
                            hintInfo.moveBlockDirection = (EDirection)d;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.AddRange(hintList);
                            HintBlockList.Add(hintInfo);
                        }
                    }
                }
                else
                {
                    if (rightBlocks.Count != 0 && rightBlocks[0].Tile.IsSwapAble(reverse))
                    {
                        if (StaticGameSettings.IsAbleFish)//if (false)           //FishCheck remove if(false)
                        {
                            var fishCheck = false;
                            nextBlock = null;
                            while (!fishCheck)
                            {
                                nextTile = rightBlocks[0].Tile.GetConnectTileOrNull((EDirection)d);
                                if (nextTile == null) break;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                if (!nextTile.IsSwapAble(reverse)) break;
                                nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) break;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                if (nextBlock.Color != rightBlocks[0].Color) break;

                                fishCheck = true;
                            }

                            if (fishCheck)
                                if (reverseBlocks.Count != 0 || leftBlocks.Count != 0)
                                {
                                    if (reverseBlocks.Count != 0)
                                    {
                                        if (StaticGameSettings.IsAbleFish)
                                        {
                                            if (reverseBlocks[0].Tile.IsSwapAble((EDirection)d))
                                            {
                                                if (false == IsFishAvoidHint(reverseBlocks[0], rightBlocks[0], nextBlock))
                                                {
                                                    item = EItem.FISH;
                                                    hintList.Add(reverseBlocks[0]);
                                                    hintList.Add(rightBlocks[0]);
                                                    hintList.Add(nextBlock);
                                                    var hintInfo = new HintInfo();
                                                    hintInfo.eItem = item;
                                                    hintInfo.centerBlock = block;
                                                    hintInfo.centerBlockDirection = reverse;
                                                    hintInfo.moveBlock = reverseBlocks[0];
                                                    hintInfo.moveBlockDirection = (EDirection)d;
                                                    hintInfo.blocks = new List<GameBlock>();

                                                    hintInfo.blocks.AddRange(hintList);
                                                    HintBlockList.Add(hintInfo);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (StaticGameSettings.IsAbleFish)
                                        {
                                            if (leftBlocks[0].Tile.IsSwapAble((EDirection)d))
                                            {
                                                if (false == IsFishAvoidHint(leftBlocks[0], rightBlocks[0], nextBlock))
                                                {
                                                    item = EItem.FISH;
                                                    hintList.Add(leftBlocks[0]);
                                                    hintList.Add(rightBlocks[0]);
                                                    hintList.Add(nextBlock);
                                                    var hintInfo = new HintInfo();
                                                    hintInfo.eItem = item;
                                                    hintInfo.centerBlock = block;
                                                    hintInfo.centerBlockDirection = left;
                                                    hintInfo.moveBlock = leftBlocks[0];
                                                    hintInfo.moveBlockDirection = right;
                                                    hintInfo.blocks = new List<GameBlock>();
                                                    hintInfo.blocks.AddRange(hintList);
                                                    HintBlockList.Add(hintInfo);
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                    }
                    else
                    {
                        if (leftBlocks.Count != 0 && leftBlocks[0].Tile.IsSwapAble(right) && reverseBlocks.Count != 0)
                        {
                            hintList.Add(leftBlocks[0]);
                            hintList.Add(reverseBlocks[0]);
                            var hintInfo = new HintInfo();
                            hintInfo.eItem = item;
                            hintInfo.centerBlock = block;
                            hintInfo.centerBlockDirection = left;
                            hintInfo.moveBlock = leftBlocks[0];
                            hintInfo.moveBlockDirection = right;
                            hintInfo.blocks = new List<GameBlock>();
                            hintInfo.blocks.AddRange(hintList);
                            HintBlockList.Add(hintInfo);
                        }
                    }
                }
            }
        }

        if (_isItemShaking)
            if (ItemShakeList.combine != ECombine.RAINBOW_NORMAL)
            {
                returnValue = ItemShakeList;
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.COLOR_BOMB)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.RHOMBUS_BOMB)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.X_BOMB)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.VERTICAL_BOMB)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.HORIZONTAL_BOMB)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        for (var i = 0; i < HintBlockList.Count; i++)
            if (HintBlockList[i].eItem == EItem.FISH)
            {
                returnValue = HintBlockList[i];
                return returnValue;
            }

        if (_isItemShaking)
            if (ItemShakeList.combine == ECombine.RAINBOW_NORMAL)
            {
                returnValue = ItemShakeList;
                return returnValue;
            }

        var NormalPopScore = -1;
        for (var i = 0; i < HintBlockList.Count; i++)
        {
            var Score = 0;

            var JamBlocks = new List<GameBlock>();
            if (returnValue.moveBlock == null)
            {
                returnValue = HintBlockList[i];

                foreach (var temp in returnValue.blocks)
                {
                    if (temp.ID != EID.COLOR_BOMB) Score += 5;
                    var BreakTile = temp.Tile;
                    if (BreakTile == null) continue;
                    if (temp == returnValue.moveBlock) BreakTile = returnValue.centerBlock.Tile;
                    GameTile nextTile = null;
                    GameBlock _tempblock = null;
                    for (var j = 0; j < 4; j++)
                    {
                        nextTile = BreakTile.GetConnectTileOrNull((EDirection)j);
                        if (nextTile == null) continue;
                        _tempblock = nextTile.GetTopBlockOrNull();

                        if (_tempblock != null)
                        {
                            if (_tempblock.Property.popProperty.SidePop || _tempblock.Property.popProperty.SideColorPop)
                                Score += 10;

                            if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)
                                Score += 10;
                        }
                        else
                        {
                            continue;
                        }

                        _tempblock = nextTile.GetBlockOrNULL(EDepth.NORMAL);
                        if (_tempblock != null)
                            if (GetBlockKind(_tempblock.ID) == EBlockKind.OBSTACLE)
                            {
                                if (_tempblock.Property.popProperty.SidePop ||
                                    _tempblock.Property.popProperty.SideColorPop) Score += 10;

                                if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)
                                    Score += 10;
                            }
                    }

                    _tempblock = BreakTile.GetBlockOrNULL(EDepth.FLOOR);
                    if (_tempblock != null)
                    {
                        if (_tempblock.Property.popProperty.Break) Score += 10;
                        if (_tempblock.ID == EID.JAM) JamBlocks.Add(_tempblock);
                    }
                }

                if (JamBlocks.Count > 0 && returnValue.blocks.Count > JamBlocks.Count)
                    Score += (returnValue.blocks.Count - JamBlocks.Count) * 10;
                NormalPopScore = Score;
                Score = 0;
            }
            else
            {
                foreach (var temp in HintBlockList[i].blocks)
                {
                    if (temp == null) continue;
                    if (temp.ID != EID.NORMAL) Score += 5;
                    var BreakTile = temp.Tile;
                    if (BreakTile == null) continue;
                    if (temp == HintBlockList[i].moveBlock) BreakTile = HintBlockList[i].centerBlock.Tile;
                    GameTile nextTile = null;
                    GameBlock _tempblock = null;
                    for (var j = 0; j < 4; j++)
                    {
                        nextTile = BreakTile.GetConnectTileOrNull((EDirection)j);
                        if (nextTile == null) continue;
                        _tempblock = nextTile.GetTopBlockOrNull();
                        if (_tempblock != null)
                        {
                            if (_tempblock.Property.popProperty.SidePop || _tempblock.Property.popProperty.SideColorPop)
                                Score += 10;

                            if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)
                                Score += 10;
                        }
                        else
                        {
                            continue;
                        }

                        _tempblock = nextTile.GetBlockOrNULL(EDepth.NORMAL);
                        if (_tempblock != null)
                            if (GetBlockKind(_tempblock.ID) == EBlockKind.OBSTACLE)
                            {
                                if (_tempblock.Property.popProperty.SidePop ||
                                    _tempblock.Property.popProperty.SideColorPop) Score += 10;

                                if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)
                                    Score += 10;
                            }
                    }

                    _tempblock = BreakTile.GetBlockOrNULL(EDepth.FLOOR);
                    if (_tempblock != null)
                    {
                        if (_tempblock.Property.popProperty.Break) Score += 10;
                        if (_tempblock.ID == EID.JAM) JamBlocks.Add(_tempblock);
                    }
                    else
                    {
                    }
                }

                if (JamBlocks.Count > 0 && HintBlockList[i].blocks.Count > JamBlocks.Count)
                    Score += (HintBlockList[i].blocks.Count - JamBlocks.Count) * 10;
                if (Score > NormalPopScore)
                {
                    NormalPopScore = Score;
                    returnValue = HintBlockList[i];
                }

                Score = 0;
            }
        }

        /*         for (int i = 0; i < HintBlockList.Count; i++)         {             int Score = 0;             List<GameBlock> JamBlocks = new List<GameBlock>();             List<MissionKind> missions = StageManager.GetInstance.GetMissions();             List<GameTile> ScoreTiles = new List<GameTile>();             foreach (var MissionId in missions)             {                 if (MissionId.ID == EID.DROP_RELIC1 || MissionId.ID == EID.DROP_RELIC2 || MissionId.ID == EID.DROP_RELIC3)                 {                     foreach (var SerchRelic in blockList)                     {                         if (SerchRelic.ID == EID.DROP_RELIC1 || MissionId.ID == EID.DROP_RELIC2 || MissionId.ID == EID.DROP_RELIC3)                         {                             GameTile _tile = SerchRelic.Tile.GetConnectTileOrNull(SerchRelic.Tile.DropDirection);                             while (true)                             {                                 _tile = _tile.GetConnectTileOrNull(SerchRelic.Tile.DropDirection);                                 if (_tile == null) continue;                                 if (!TileManager.GetInstance.IsPreViewTile(_tile)) continue;                                 if (_tile.TileKind == ETileKind.VOID || _tile.TileKind == ETileKind.LADDER) continue;                                 else                                 {                                     if (_tile.TileKind == ETileKind.END)                                     {                                         ScoreTiles.Add(_tile);                                         break;                                     }                                     else ScoreTiles.Add(_tile);                                 }                             }                         }                     }                 }             }             foreach (var temp in HintBlockList[i].blocks)             {                 if (temp.ID != EID.NORMAL)                 {                     Score += 5;                 }                 GameTile BreakTile = temp.Tile;                 if (temp == HintBlockList[i].moveBlock) BreakTile = HintBlockList[i].centerBlock.Tile;                 GameTile nextTile;                 GameBlock _tempblock;                 if (ScoreTiles.Contains(BreakTile))                 {                     Score += 15;                 }                 for (int j = 0; j < 4; j++)                 {                     nextTile = BreakTile.GetConnectTileOrNull((EDirection)j);                     if (nextTile == null) continue;                     _tempblock = nextTile.GetTopBlockOrNull();                     if (_tempblock != null)                     {                         if (_tempblock.Property.popProperty.SidePop || _tempblock.Property.popProperty.SideColorPop)                         {                             Score += 10;                             if (ScoreTiles.Contains(_tempblock.Tile))                             {                                 Score += 5;                             }                         }                         if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)                         {                             Score += 10;                             if (ScoreTiles.Contains(_tempblock.Tile))                             {                                 Score += 5;                             }                         }                     }                     _tempblock = nextTile.GetBlockOrNULL(EDepth.NORMAL);                     if (_tempblock != null)                     {                         if (GetBlockKind(_tempblock.ID) == EBlockKind.OBSTACLE)                         {                             if (_tempblock.Property.popProperty.SidePop || _tempblock.Property.popProperty.SideColorPop)                             {                                 Score += 10;                                 if (ScoreTiles.Contains(_tempblock.Tile))                                 {                                     Score += 5;                                 }                             }                             if (_tempblock.Property.popProperty.SideColorPop && _tempblock.Color == temp.Color)                             {                                 Score += 10;                                 if (ScoreTiles.Contains(_tempblock.Tile))                                 {                                     Score += 5;                                 }                             }                         }                     }                 }                 _tempblock = BreakTile.GetBlockOrNULL(EDepth.FLOOR);                 if (_tempblock != null)                 {                     if (_tempblock.Property.popProperty.Break)                     {                         Score += 10;                     }                     if (_tempblock.ID == EID.JAM)                     {                         JamBlocks.Add(_tempblock);                     }                 }             }             if (JamBlocks.Count > 0 && HintBlockList[i].blocks.Count > JamBlocks.Count)             {                 Score += ((HintBlockList[i].blocks.Count - JamBlocks.Count) * 10);             }             if (Score > NormalPopScore)             {                 NormalPopScore = Score;                 returnValue = HintBlockList[i];             }             Score = 0;         }         */
        if (returnValue.moveBlock != null) return returnValue;
        return null;
    }

    public void MakePopList()
    {
        foreach (var block in blockList)
            if (block.IsPopConditionClear)
            {
                var blocks = new List<GameBlock>();
                block.MakePopList(ref blocks, block.Color);

                popList.Add(blocks);
            }
    }

    /*
    public int example()
    {
        var result = 0;

        do
        {
            if(// 어디 클래스에서 값을 체크함 )
            {
                result = 1;
                break;
            }
            if(// 뭔 값을 체크 )
            break;
        } while (false);

        return result;
    }
    */

    public void Pop()
    {
        var lists = new List<ItemBlockList>();
        foreach (var blocks in popList) lists.AddRange(blocks[0].ItemCreateCheck(blocks));
        for (var i = 0; i < lists.Count; i++)
        {
            if (lists[i].eItem == EItem.NONE)
            {
                var isJamTime = false;
                if (TileManager.GetInstance.IsJamTiles(lists[i].removeBlocks)) isJamTime = true;
                foreach (var block in lists[i].removeBlocks)
                {
                    if (isJamTime) block.JamPop();
                    block.Pop();
                }
            }
            else
            {
                var isInterrupt = false;
                foreach (var block in lists[i].removeBlocks)
                {
                    if (block.Tile == null)
                        isInterrupt = true;
                }

                if (isInterrupt)
                {
                }
                else
                {
                    var isJamTime = false;
                    if (TileManager.GetInstance.IsJamTiles(lists[i].removeBlocks)) isJamTime = true;

                    var masterBlock = lists[i].masterBlock;

                    if (lists[i].removeBlocks.Contains(firstBlock))
                        if (firstBlock.ID == EID.NORMAL)
                            masterBlock = firstBlock;
                    if (lists[i].removeBlocks.Contains(secondBlock))
                        if (secondBlock.ID == EID.NORMAL)
                            masterBlock = secondBlock;
                    /*                     foreach (var b in lists[i].removeBlocks)                     {                         if(b.Tile.Matrix == firstBlock.Tile.Matrix)                         {                             DebugX.Log("FIRST MATCH");                             masterBlock = firstBlock;                         }                         if (b.Tile.Matrix == secondBlock.Tile.Matrix)                         {                             DebugX.Log("SECOND MATCH");                             masterBlock = secondBlock;                         }                     }                     */
                    if (masterBlock != null)
                    {
                        masterBlock.SetMasterBlock(lists[i].eItem);
                    }

                    foreach (var block in lists[i].removeBlocks)
                    {
                        if (isJamTime) block.JamPop();
                        block.ItemCreatePop(masterBlock);
                    }
                }
            }
        }

        popList.Clear();
    }

    public void RegisterRainbow(EColor color, EID targetID, GameBlock block, bool isJam)
    {
        if (color == EColor.NONE)
        {
            List<GameBlock> blocks;
            /*if (block.ID == EID.COLOR_BOMB)
            {
                blocks = GetBlocksToUsingColor(GetMostColor());
            }
            else
            {
                blocks = GetBlocksToUsingColor(block.MadeColor);
            }*/
            blocks = GetBlocksToUsingColor(GetMostColor());
            block.StartRainbow(blocks, targetID, isJam);
        }
        else
        {
            var blocks = GetBlocksToUsingColor(color);
            block.StartRainbow(blocks, targetID, isJam);
        }
    }

    public EColor GetMostColor()
    {
        //TODO: 얼음 안에 있는 장애물은 제외
        var mostColor = EColor.NONE;
        var mostCount = 0;
        for (var i = 0; i < (int)EColor.NONE; i++)
        {
            var count = 0;
            var color = (EColor)i;
            foreach (var block in blockList)
            {
                if (block.Tile != null)
                    if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                        if (block.Color == color)
                            count++;
                if (mostCount < count)
                {
                    mostColor = color;
                    mostCount = count;
                }
            }
        }

        return mostColor;
    }

    public List<GameBlock> GetBlocksToUsingColor(EColor color)
    {
        //TODO: 얼음 안에 있는 장애물은 제외
        var blocks = new List<GameBlock>();

        foreach (var block in blockList)
            if (block.Tile != null)
                if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                    if (block.Color == color)
                        if (block.Tile.IsPopAble() && block.Property.popProperty.ColorPop)
                            blocks.Add(block);

        return blocks;
    }

    public void ClearPopCondition()
    {
        popList.Clear();
        foreach (var block in blockList)
        {
            block.IsPopConditionClear = false;
            block.IsMatchMark = false;
        }
    }

    public void CreateSpecialBlock()
    {
        foreach (var item in itemList)
        {
            if (DoubleClickSystem.GetInstance != null)
            {
                bool createClickBlock = false;
                foreach (var clickBlock in DoubleClickSystem.GetInstance.GetBlockList())
                {
                    if (item.ID == clickBlock)
                    {
                        createClickBlock = true;
                        break;
                    }
                }
                if (createClickBlock)
                {
                    item.block.Setting(EColor.NONE, item.ID);
                    item.block.SetMadeColor(item.color);
                }
                else
                {
                    item.block.Setting(item.color, item.ID);
                }
            }
            else
            {
                item.block.Setting(item.color, item.ID);
            }

            item.block.ApplySprite();
            //item.block.IsMasterBlock = false;
            SetPrefabBlock(item.block, item.ID);
        }

        itemList.Clear();
    }

    public void SetPrefabBlock(GameBlock __block, EID __eid)
    {
        if (__block.Tile != null)
        {
            var particle = DynamicObjectPool.GetInstance.GetObjectForType("Particle_ChangeItem", false);
            particle.transform.position = __block.Tile.GetPosition();

            var dummy = __block.GetComponentInChildren<DummyObject>();
            if (dummy != null)
            {
                dummy.Clear();
                DynamicObjectPool.GetInstance.PoolObject(dummy.gameObject, false);
            }

            if (__eid == EID.COLOR_BOMB)
            {
                var DummyObj = DynamicObjectPool.GetInstance.GetObjectForType("RainbowDummy", false);
                DummyObj.transform.SetParent(__block.transform);
                DummyObj.transform.position = __block.transform.position;
                __block.Renderer.sprite = null;
            }
            else
            {
                string prefabName = string.Format("SpecialBlock_{0}", __eid);
                GameObject specialBlock = PrefabRegister.GetInstance.GetPrefab(prefabName, __block.transform);
                if (null != specialBlock)
                {
                    DummyObject dummyComponent = specialBlock.GetComponent<DummyObject>();
                    if (null != dummyComponent)
                    {
                        dummyComponent.dummyRenderer.sprite = __block.Renderer.sprite;
                        dummyComponent.dummyRendererAdditive.sprite = __block.Renderer.sprite;
                    }

                    __block.Renderer.sprite = null;
                }
            }
        }
    }

    public void SetNotMasterBlock()
    {
        foreach (var item in blockList)
            if (item.IsMasterBlock)
                item.IsMasterBlock = false;
    }

    public void InputSwapBlock(GameBlock first, GameBlock second)
    {
        firstBlock = first;
        secondBlock = second;
    }

    public void StartBlockSwap()
    {
        SwapCount++;
        Debug.Log("[버그 추적] 스왑 카운트 증가 : " + SwapCount);
    }

    public void EndBlockSwap()
    {
        SwapCount--;
        TileManager.GetInstance.SwapReset();
        //swapCount = Mathf.Max(0, swapCount);
    }

    public void ReverseSwap()
    {
        if (firstBlock != null && secondBlock != null)
        {
            var firstTile = firstBlock.Tile;
            var secondTile = secondBlock.Tile;

            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("BlockSwap");
            firstBlock.Swap(secondTile);
            secondBlock.Swap(firstTile);

            firstBlock = null;
            secondBlock = null;
        }
    }

    public void RemoveBlockFromList(GameBlock block)
    {
        if (blockList.Contains(block)) blockList.Remove(block);
    }

    public bool IsSwapEnd()
    {
        if (SwapCount == 0 && firstBlock != null && secondBlock != null)
            return true;
        return false;
    }

    public bool IsDoubleClicktrue()
    {
        if (IsDoubleClick) return true;
        return false;
    }

    public void ClearSwapBlock()
    {
        //Debug.LogWarningFormat("KKI ClearSwapBlock");
        SwapCount = 0;
        firstBlock = null;
        secondBlock = null;
    }

    public bool FishCheck(List<GameBlock> floodBlocks)
    {
        var returnValue = false;

        foreach (var block in floodBlocks)
        {
            var tileDown = block.Tile.GetConnectTileOrNull(EDirection.DOWN);
            if (tileDown == null) continue;
            var tileRight = block.Tile.GetConnectTileOrNull(EDirection.RIGHT);
            if (tileRight == null) continue;
            var tileDown_Right = block.Tile.GetConnectTileOrNull(EDirection.DOWN_RIGHT);
            if (tileDown_Right == null) continue;

            if (tileDown.IsPopAble() && tileRight.IsPopAble() && tileDown_Right.IsPopAble())
                if (floodBlocks.Contains(tileDown.NormalBlock) &&
                    floodBlocks.Contains(tileRight.NormalBlock) &&
                    floodBlocks.Contains(tileDown_Right.NormalBlock))
                    returnValue = true;
        }

        return returnValue;
    }

    public void AddCreateItem(ItemInfo info)
    {
        foreach (var item in itemList)
            if (item.matrix == info.matrix)
                return;
        itemList.Add(info);
    }

    public bool IsMovingEnd()
    {
        var returnValue = true;
        foreach (var block in blockList)
            if (block.IsMoving)
                returnValue = false;
        return returnValue;
    }

    public bool IsCombineBlocks()
    {
        if (firstBlock == null || secondBlock == null) return false;

        var returnValue = false;

        var firstValue = 0;
        var secondValue = 0;

        switch (firstBlock.ID)
        {
            case EID.NORMAL:
                firstValue = (int)EItem.NONE;
                break;

            case EID.HORIZONTAL:
                firstValue = (int)EItem.HORIZONTAL_BOMB;
                break;

            case EID.VERTICAL:
                firstValue = (int)EItem.VERTICAL_BOMB;
                break;

            case EID.RHOMBUS:
                firstValue = (int)EItem.RHOMBUS_BOMB;
                break;

            case EID.X:
                firstValue = (int)EItem.X_BOMB;
                break;

            case EID.COLOR_BOMB:
                firstValue = (int)EItem.COLOR_BOMB;
                break;

            case EID.FISH:
                firstValue = (int)EItem.FISH;
                break;

            default:
                return false;
        }

        switch (secondBlock.ID)
        {
            case EID.NORMAL:
                secondValue = (int)EItem.NONE;
                break;

            case EID.HORIZONTAL:
                secondValue = (int)EItem.HORIZONTAL_BOMB;
                break;

            case EID.VERTICAL:
                secondValue = (int)EItem.VERTICAL_BOMB;
                break;

            case EID.RHOMBUS:
                secondValue = (int)EItem.RHOMBUS_BOMB;
                break;

            case EID.X:
                secondValue = (int)EItem.X_BOMB;
                break;

            case EID.COLOR_BOMB:
                secondValue = (int)EItem.COLOR_BOMB;
                break;

            case EID.FISH:
                secondValue = (int)EItem.FISH;
                break;

            default:
                return false;
        }

        if (combines.ContainsKey(firstValue + secondValue))
        {
            combine = combines[firstValue + secondValue];
            returnValue = true;
        }
        else
        {
            returnValue = false;
        }

        return returnValue;
    }

    public void MoneyItemJamApply(GameTile tile)
    {
        if (tile == null) return;
        if (tile.IsFreeJam()) HasJamInCombine = true;
    }

    public void ActiveCombine()
    {
        if (firstBlock != null && secondBlock != null)
        {
            firstBlock.IsCombine = true;
            secondBlock.IsCombine = true;

            var tile = firstBlock.Tile;
            var tile2 = secondBlock.Tile;

            if (tile.IsFreeJam() || tile2.IsFreeJam())
            {
                HasJamInCombine = true;
                tile.RegisterJamPop();
                tile2.RegisterJamPop();
            }

            if (combine == ECombine.RAINBOW_NORMAL || combine == ECombine.RAINBOW_X
                                                   || combine == ECombine.RAINBOW_RHOMBUS ||
                                                   combine == ECombine.RAINBOW_DIRECTION ||
                                                   combine == ECombine.RAINBOW_FISH)
            {
                GameTile rainbowTile = null;
                var rainbowColor = EColor.NONE;
                if (firstBlock.ID == EID.COLOR_BOMB)
                {
                    rainbowTile = tile;
                    rainbowColor = secondBlock.Color;
                    if (secondBlock.MadeColor != EColor.NONE)
                    {
                        rainbowColor = secondBlock.MadeColor;
                    }
                }
                else
                {
                    rainbowTile = tile2;
                    rainbowColor = firstBlock.Color;
                    if (firstBlock.MadeColor != EColor.NONE)
                    {
                        rainbowColor = firstBlock.MadeColor;
                    }
                }

                if (combine == ECombine.RAINBOW_NORMAL)
                {
                    rainbowTile.RegisterCombinePop(combine, rainbowColor);
                }
                else
                {
                    if (BaseSystem.GetInstance != null)
                    {
                        if (BaseSystem.GetInstance.GetSystemList("Fantasy") || BaseSystem.GetInstance.GetSystemList("Castle"))
                        {
                            rainbowTile.RegisterCombinePop(combine, rainbowColor);
                        }
                        else
                        {
                            rainbowTile.RegisterCombinePop(combine, EColor.NONE);
                        }
                    }
                    else
                    {
                        rainbowTile.RegisterCombinePop(combine, EColor.NONE);
                    }
                    //rainbowTile.RegisterCombinePop(combine, rainbowColor);
                    //rainbowTile.RegisterCombinePop(combine, EColor.NONE);
                }

                return;
            }

            if (combine == ECombine.RAINBOW_RAINBOW)
            {
                SwapPosition.x = (firstBlock.Tile.GetPosition().x + secondBlock.Tile.GetPosition().x) * 0.5f;
                SwapPosition.y = (firstBlock.Tile.GetPosition().y + secondBlock.Tile.GetPosition().y) * 0.5f;
                if (firstBlock.Tile.Matrix.x - secondBlock.Tile.Matrix.x == 0)
                    IsSwapHorizontal = false;
                else
                    IsSwapHorizontal = true;
            }

            var color = tile.NormalBlock.Color;
            tile.RemoveBlock(firstBlock);
            tile2.RemoveBlock(secondBlock);

            if (tile != null) tile.RegisterCombinePop(combine, color);

            firstBlock.Clear();
            secondBlock.Clear();
            HasJamInCombine = false;
        }
    }

    public void ActiveObstacle()
    {
        IsEndObstacleActive = false;
        StartCoroutine(ActiveObstacleCoroutine());
    }

    public void ActiveRainbowBox()
    {
        IsEndObstacleActive = false;
        StartCoroutine(ActiveRainbowBoxCoroutine());
    }

    private IEnumerator ActiveObstacleCoroutine()
    {
        var obstacleDictionary = new Dictionary<EID, List<GameBlock>>();

        var LavaList = GetBlocksToUseID(EID.LAVA);
        var BandageList = GetBlocksToUseID(EID.BANDAGE);
        var ChameleonList = GetBlocksToUseID(EID.CHAMELEON);
        var TurnBoxList = GetBlocksToUseID(EID.TURN_BOX);
        var DoubleList = GetBlocksToUseID(EID.DOUBLE);
        var FactionList = GetBlocksToUseID(EID.FACTION);

        var TimeBombIceList = GetBlocksToUseID(EID.TIMEBOMB_ICE);
        var TimeBombLavaList = GetBlocksToUseID(EID.TIMEBOMB_LAVA);
        var LavaCreatorList = GetBlocksToUseID(EID.CREATOR_LAVA);
        var BombCreatorList = GetBlocksToUseID(EID.CREATOR_BOMB);

        var lavaCount = 0;
        var bandageCount = 0;

        if (LavaList.Count != 0)
        {
            foreach (var lava in LavaList) lavaCount += lava.HP;
        }
        else if (LavaCreatorList.Count != 0)
        {
            obstacleDictionary.Add(EID.CREATOR_LAVA, LavaCreatorList);

            var isDamaged = false;
            foreach (var creatorBlock in LavaCreatorList)
            {
                var creator = (ObLavaCreator)creatorBlock;
                if (creator.IsDamaged)
                {
                    isDamaged = true;
                    creator.IsDamaged = false;
                }
            }

            if (!isDamaged)
            {
                GameTile targetTile = null;
                var directions = new List<EDirection>();

                foreach (var lava in obstacleDictionary[EID.CREATOR_LAVA])
                {
                    var lavaTile = lava.Tile;

                    directions.Add(EDirection.LEFT);
                    directions.Add(EDirection.RIGHT);
                    directions.Add(EDirection.UP);
                    directions.Add(EDirection.DOWN);

                    while (directions.Count > 0)
                    {
                        var ranNum = Random.Range(0, directions.Count);
                        var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                        if (checkNullBlockTile != null)
                            if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                checkNullBlockTile.TileKind != ETileKind.VOID)
                                if (checkNullBlockTile.NormalBlock == null &&
                                    checkNullBlockTile.GetTopBlockOrNull() == null)
                                    if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix, directions[ranNum]))
                                    {
                                        targetTile = checkNullBlockTile;
                                        break;
                                    }

                        directions.RemoveAt(ranNum);
                    }

                    directions.Clear();
                }

                if (targetTile == null)
                    foreach (var lava in obstacleDictionary[EID.CREATOR_LAVA])
                    {
                        var lavaTile = lava.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                    checkNullBlockTile.TileKind != ETileKind.VOID)
                                    if (checkNullBlockTile.NormalBlock != null)
                                        if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                            == EBlockKind.NORMAL)
                                            if (checkNullBlockTile.IsPopAble() &&
                                                checkNullBlockTile.GetTopBlockOrNull() == null)
                                                if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix,
                                                        directions[ranNum]))
                                                {
                                                    targetTile = checkNullBlockTile;
                                                    break;
                                                }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile == null)
                    foreach (var lava in obstacleDictionary[EID.CREATOR_LAVA])
                    {
                        var lavaTile = lava.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                    checkNullBlockTile.TileKind != ETileKind.VOID)
                                    if (checkNullBlockTile.NormalBlock != null)
                                        if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                            == EBlockKind.BOMB)
                                            if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix,
                                                    directions[ranNum]))
                                            {
                                                targetTile = checkNullBlockTile;
                                                break;
                                            }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile != null)
                {
                    var block = GetGameBlockOrNull(EID.LAVA);
                    if (block != null)
                    {
                        if (targetTile.NormalBlock != null)
                        {
                            var destroyBlock = targetTile.NormalBlock;
                            targetTile.RemoveBlock(destroyBlock);
                            destroyBlock.Clear();
                        }

                        block.Init();
                        block.Setting(EColor.NONE, EID.LAVA);
                        block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.LAVA));
                        block.ApplySprite();
                        block.SetTile(targetTile);
                        block.SetPosition();
                        block.SetAnimTrigger("Scale");
                        ObstacleLogic.LavaCount++;
                    }
                }
            }
        }

        if (BandageList.Count != 0)
            foreach (var bandage in BandageList)
                bandageCount += bandage.HP;

        if (lavaCount < ObstacleLogic.LavaCount)
            ObstacleLogic.LavaCount = lavaCount;
        else
            obstacleDictionary.Add(EID.LAVA, LavaList);

        if (bandageCount < ObstacleLogic.BandageCount)
            ObstacleLogic.BandageCount = bandageCount;
        else
            obstacleDictionary.Add(EID.BANDAGE, BandageList);

        if (TimeBombIceList.Count != 0) obstacleDictionary.Add(EID.TIMEBOMB_ICE, TimeBombIceList);
        if (TimeBombLavaList.Count != 0) obstacleDictionary.Add(EID.TIMEBOMB_LAVA, TimeBombLavaList);
        if (ChameleonList.Count != 0) obstacleDictionary.Add(EID.CHAMELEON, ChameleonList);
        if (TurnBoxList.Count != 0)
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("TurnSwitch");
            obstacleDictionary.Add(EID.TURN_BOX, TurnBoxList);
        }

        if (DoubleList.Count != 0) obstacleDictionary.Add(EID.DOUBLE, DoubleList);
        if (BombCreatorList.Count != 0) obstacleDictionary.Add(EID.CREATOR_BOMB, BombCreatorList);

        if (obstacleDictionary.ContainsKey(EID.LAVA))
        {
            var isCreate = false;
            if (ObstacleLogic.LavaCount > 20)
            {
                var tempLava = new List<GameBlock>();
                foreach (var lava in obstacleDictionary[EID.LAVA])
                    if (lava.HP == 1)
                        tempLava.Add(lava);
                if (tempLava.Count > 0)
                {
                    var ranNum = Random.Range(0, tempLava.Count);
                    tempLava[ranNum].HP = 2;
                    tempLava[ranNum].ApplySprite();
                    ObstacleLogic.LavaCount++;
                }
                else
                {
                    isCreate = true;
                }
            }
            else
            {
                isCreate = true;
            }

            if (isCreate)
            {
                GameTile targetTile = null;
                var directions = new List<EDirection>();

                foreach (var lava in obstacleDictionary[EID.LAVA])
                {
                    var lavaTile = lava.Tile;

                    directions.Add(EDirection.LEFT);
                    directions.Add(EDirection.RIGHT);
                    directions.Add(EDirection.UP);
                    directions.Add(EDirection.DOWN);

                    while (directions.Count > 0)
                    {
                        var ranNum = Random.Range(0, directions.Count);
                        var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                        if (checkNullBlockTile != null)
                            if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                checkNullBlockTile.TileKind != ETileKind.VOID)
                                if (checkNullBlockTile.NormalBlock == null &&
                                    checkNullBlockTile.GetTopBlockOrNull() == null)
                                    if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix, directions[ranNum]))
                                    {
                                        targetTile = checkNullBlockTile;
                                        break;
                                    }

                        directions.RemoveAt(ranNum);
                    }

                    directions.Clear();
                }

                if (targetTile == null)
                    foreach (var lava in obstacleDictionary[EID.LAVA])
                    {
                        var lavaTile = lava.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                    checkNullBlockTile.TileKind != ETileKind.VOID)
                                    if (checkNullBlockTile.NormalBlock != null)
                                        if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                            == EBlockKind.NORMAL)
                                            if (checkNullBlockTile.IsPopAble() &&
                                                checkNullBlockTile.GetTopBlockOrNull() == null)
                                                if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix,
                                                        directions[ranNum]))
                                                {
                                                    targetTile = checkNullBlockTile;
                                                    break;
                                                }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile == null)
                    foreach (var lava in obstacleDictionary[EID.LAVA])
                    {
                        var lavaTile = lava.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = lavaTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.TileKind != ETileKind.LADDER &&
                                    checkNullBlockTile.TileKind != ETileKind.VOID)
                                    if (checkNullBlockTile.NormalBlock != null)
                                        if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                            == EBlockKind.BOMB)
                                            if (!TileManager.GetInstance.CheckBarricade(lavaTile.Matrix,
                                                    directions[ranNum]))
                                            {
                                                targetTile = checkNullBlockTile;
                                                break;
                                            }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile != null)
                {
                    var block = GetGameBlockOrNull(EID.LAVA);
                    if (block != null)
                    {
                        if (targetTile.NormalBlock != null)
                        {
                            var destroyBlock = targetTile.NormalBlock;
                            targetTile.RemoveBlock(destroyBlock);
                            destroyBlock.Clear();
                        }

                        block.Init();
                        block.Setting(EColor.NONE, EID.LAVA);
                        block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.LAVA));
                        block.ApplySprite();
                        block.SetTile(targetTile);
                        block.SetPosition();
                        block.SetAnimTrigger("Scale");
                        ObstacleLogic.LavaCount++;
                    }
                }

                if (targetTile == null)
                {
                    var tempLava = new List<GameBlock>();
                    foreach (var lava in obstacleDictionary[EID.LAVA])
                        if (lava.HP == 1)
                            tempLava.Add(lava);
                    if (tempLava.Count > 0)
                    {
                        var ranNum = Random.Range(0, tempLava.Count);
                        tempLava[ranNum].HP = 2;
                        tempLava[ranNum].ApplySprite();
                        ObstacleLogic.LavaCount++;
                    }
                }
            }
        }

        if (obstacleDictionary.ContainsKey(EID.BANDAGE))
        {
            var isCreate = false;
            if (ObstacleLogic.BandageCount > 30)
            {
                var tempBandage = new List<GameBlock>();
                foreach (var bandage in obstacleDictionary[EID.BANDAGE])
                    if (bandage.HP < 4)
                        tempBandage.Add(bandage);
                if (tempBandage.Count > 0)
                {
                    var ranNum = Random.Range(0, tempBandage.Count);
                    tempBandage[ranNum].HP += 1;
                    tempBandage[ranNum].ApplySprite();
                    ObstacleLogic.BandageCount++;
                }
                else
                {
                    isCreate = true;
                }
            }
            else
            {
                isCreate = true;
            }

            if (isCreate)
            {
                GameTile targetTile = null;
                var directions = new List<EDirection>();

                if (targetTile == null)
                    foreach (var bandage in obstacleDictionary[EID.BANDAGE])
                    {
                        var bandageTile = bandage.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = bandageTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.NormalBlock != null)
                                    if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                        == EBlockKind.NORMAL)
                                        if (checkNullBlockTile.IsPopAble())
                                            if (checkNullBlockTile.GetTopBlockOrNull() == null)
                                            {
                                                targetTile = checkNullBlockTile;
                                                break;
                                            }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile == null)
                    foreach (var bandage in obstacleDictionary[EID.BANDAGE])
                    {
                        var bandageTile = bandage.Tile;

                        directions.Add(EDirection.LEFT);
                        directions.Add(EDirection.RIGHT);
                        directions.Add(EDirection.UP);
                        directions.Add(EDirection.DOWN);

                        while (directions.Count > 0)
                        {
                            var ranNum = Random.Range(0, directions.Count);
                            var checkNullBlockTile = bandageTile.GetConnectTileOrNull(directions[ranNum]);
                            if (checkNullBlockTile != null)
                                if (checkNullBlockTile.NormalBlock != null)
                                    if (GetBlockKind(checkNullBlockTile.NormalBlock.ID)
                                        == EBlockKind.BOMB)
                                        if (checkNullBlockTile.GetTopBlockOrNull() == null)
                                        {
                                            targetTile = checkNullBlockTile;
                                            break;
                                        }

                            directions.RemoveAt(ranNum);
                        }
                    }

                if (targetTile != null)
                {
                    var block = GetGameBlockOrNull(EID.BANDAGE);
                    if (block != null)
                    {
                        block.Init();
                        block.Setting(EColor.NONE, EID.BANDAGE);
                        block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.BANDAGE));
                        block.ApplySprite();
                        block.SetTile(targetTile);
                        block.SetPosition();
                        block.SetAnimTrigger("Scale");

                        ObstacleLogic.BandageCount++;
                    }
                }

                if (targetTile == null)
                {
                    var tempBandage = new List<GameBlock>();
                    foreach (var bandage in obstacleDictionary[EID.BANDAGE])
                        if (bandage.HP < 4)
                            tempBandage.Add(bandage);
                    if (tempBandage.Count > 0)
                    {
                        var ranNum = Random.Range(0, tempBandage.Count);
                        tempBandage[ranNum].HP += 1;
                        tempBandage[ranNum].ApplySprite();
                        ObstacleLogic.BandageCount++;
                    }
                    else
                    {
                        isCreate = true;
                    }
                }
            }
        }

        var isChameleonBeing = false;

        foreach (var item in obstacleDictionary)
            foreach (var block in item.Value)
            {
                if (item.Key == EID.TIMEBOMB_ICE)
                {
                    if (block.ETC > 1)
                    {
                        if (block.Tile != null)
                        {
                            var TopBlock = block.Tile.GetTopBlockOrNull();
                            if (TopBlock != null)
                                if (TopBlock.ID == EID.JAIL)
                                    continue;
                        }

                        block.ETC--;
                        if (block.ETC < 6)
                        {
                            block.SetAnimTrigger("TimeBomb");
                            if (SoundManager.GetInstance != null)
                                SoundManager.GetInstance.Play("TimeBombNoTime");
                        }

                        var timer = block.gameObject.GetComponentInChildren<BombTimer>();
                        if (timer != null) timer.SetTurn(block.ETC);
                    }
                    else
                    {
                        if (SoundManager.GetInstance != null)
                            SoundManager.GetInstance.Play("TimeBomb");
                        var tile = block.Tile;
                        ParticleManager.GetInstance.CreateActiveTimeIcePartice(tile.GetPosition());

                        for (var d = 0; d < (int)EDirection.NONE; d++)
                        {
                            var targetTile = tile.GetConnectTileOrNull((EDirection)d);
                            if (targetTile == null) continue;
                            if (targetTile.TileKind == ETileKind.LADDER
                                || targetTile.TileKind == ETileKind.VOID) continue;

                            if (targetTile.GetTopBlockOrNull() != null) continue;
                            if (targetTile.NormalBlock != null)
                                if (GetBlockKind(targetTile.NormalBlock.ID) == EBlockKind.OBSTACLE)
                                    continue;

                            var iceBlock = GetGameBlockOrNull(EID.ICE);
                            if (iceBlock != null)
                            {
                                iceBlock.Init();
                                iceBlock.Setting(EColor.NONE, EID.ICE);
                                iceBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.ICE));
                                iceBlock.ApplySprite();
                                iceBlock.SetTile(targetTile);
                                iceBlock.SetPosition();
                            }
                        }

                        var middleIceBlock = GetGameBlockOrNull(EID.ICE);
                        if (middleIceBlock != null)
                        {
                            middleIceBlock.Init();
                            middleIceBlock.Setting(EColor.NONE, EID.ICE);
                            middleIceBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.ICE));
                            middleIceBlock.ApplySprite();
                            middleIceBlock.SetTile(tile);
                            middleIceBlock.SetPosition();
                        }

                        var color = block.Color;
                        tile.RemoveBlock(block);
                        block.Clear();

                        var middleNormalBlock = GetGameBlockOrNull(EID.NORMAL);
                        if (middleNormalBlock != null)
                        {
                            middleNormalBlock.Init();
                            middleNormalBlock.Setting(color, EID.NORMAL);
                            middleNormalBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.NORMAL));
                            middleNormalBlock.ApplySprite();
                            middleNormalBlock.SetTile(tile);
                            middleNormalBlock.SetPosition();
                        }
                    }
                }

                if (item.Key == EID.TIMEBOMB_LAVA)
                {
                    if (block.ETC > 1)
                    {
                        if (block.Tile != null)
                        {
                            var TopBlock = block.Tile.GetTopBlockOrNull();
                            if (TopBlock != null)
                                if (TopBlock.ID == EID.JAIL)
                                    continue;
                        }

                        block.ETC--;
                        if (block.ETC < 6)
                        {
                            block.SetAnimTrigger("TimeBomb");
                            if (SoundManager.GetInstance != null)
                                SoundManager.GetInstance.Play("TimeBombNoTime");
                        }

                        var timer = block.gameObject.GetComponentInChildren<BombTimer>();
                        if (timer != null) timer.SetTurn(block.ETC);
                    }
                    else
                    {
                        if (SoundManager.GetInstance != null)
                            SoundManager.GetInstance.Play("TimeBomb");
                        var tile = block.Tile;
                        ParticleManager.GetInstance.CreateActiveTimeLavaPartice(tile.GetPosition());

                        for (var d = 0; d < (int)EDirection.UP_LEFT; d++)
                        {
                            var targetTile = tile.GetConnectTileOrNull((EDirection)d);
                            if (targetTile == null) continue;
                            if (targetTile.TileKind == ETileKind.LADDER
                                || targetTile.TileKind == ETileKind.VOID) continue;

                            if (targetTile.GetTopBlockOrNull() != null) continue;
                            if (targetTile.NormalBlock != null)
                                if (GetBlockKind(targetTile.NormalBlock.ID) == EBlockKind.OBSTACLE)
                                    continue;

                            if (targetTile.NormalBlock != null)
                            {
                                targetTile.NormalBlock.Clear();
                                targetTile.RemoveBlock(targetTile.NormalBlock);
                            }

                            var lavaBlock = GetGameBlockOrNull(EID.LAVA);
                            if (lavaBlock != null)
                            {
                                lavaBlock.Init();
                                lavaBlock.Setting(EColor.NONE, EID.LAVA);
                                lavaBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.LAVA));
                                lavaBlock.ApplySprite();
                                lavaBlock.SetTile(targetTile);
                                lavaBlock.SetPosition();
                                ObstacleLogic.LavaCount++;
                            }
                        }

                        tile.RemoveBlock(block);
                        block.Clear();

                        var middleLavaBlock = GetGameBlockOrNull(EID.LAVA);
                        if (middleLavaBlock != null)
                        {
                            middleLavaBlock.Init();
                            middleLavaBlock.Setting(EColor.NONE, EID.LAVA, 2);
                            middleLavaBlock.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(EID.LAVA));
                            middleLavaBlock.ApplySprite();
                            middleLavaBlock.SetTile(tile);
                            middleLavaBlock.SetPosition();

                            ObstacleLogic.LavaCount += 2;
                        }
                    }
                }

                if (item.Key == EID.CHAMELEON)
                {
                    isChameleonBeing = true;
                    block.ActiveAbility();
                    //yield return new WaitForSeconds(0.03f);
                }

                if (item.Key == EID.TURN_BOX) block.ActiveAbility();
                if (item.Key == EID.DOUBLE) block.ActiveAbility();
                if (item.Key == EID.CREATOR_BOMB)
                {
                    //block.ActiveAbility();
                }
            }

        if (FactionList.Count != 0)
        {
            var factions = new bool[3] { false, false, false };

            foreach (var block in FactionList)
                if (block.HP != 0)
                    factions[block.ETC] = true;
            foreach (var block in FactionList)
                if (!factions[block.ETC])
                {
                    block.ActiveAbility();
                    AnimationManager.FactionCount++;
                }
        }

        if (isChameleonBeing)
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("ChangeChameleon");

        if (TileManager.GetInstance.MovingRail()) yield return new WaitForSeconds(0.05f);

        yield return new WaitUntil(() =>
            IsMovingEnd() && TileManager.GetInstance.IsRailEnd && AnimationManager.FactionCount == 0);

        TileManager.GetInstance.MovingGear();

        yield return new WaitUntil(() =>
            IsMovingEnd() && TileManager.GetInstance.IsRailEnd && AnimationManager.FactionCount == 0);

        TileManager.GetInstance.MovingClimber();

        yield return new WaitUntil(() =>
            IsMovingEnd() && TileManager.GetInstance.IsClimberEnd && AnimationManager.FactionCount == 0);

        IsEndObstacleActive = true;
    }

    private IEnumerator ActiveRainbowBoxCoroutine()
    {
        var bigObj = rainbowBoxList[0];
        rainbowBoxList.RemoveAt(0);

        var leftTopBlock = TileManager.GetInstance.GetLeftTopBlockInBigObjOrNull(bigObj);
        if (leftTopBlock != null) leftTopBlock.Tile.RegisterCombinePop(ECombine.RAINBOW_RAINBOW, EColor.NONE);
        TileManager.GetInstance.BigClear(bigObj);

        //GameBlock block = TileManager.GetInstance.GetTileToUseMatrix(bigMiddleObj.Matrix).GetBlock(EDepth.MIDDLE);
        //block.IsEndPopAnim = false;
        //IsPopLogicClear--;
        //TileManager.GetInstance.GetTileToUseMatrix(bigMiddleObj.Matrix).RegisterCombine(block, ECombineKind.RAINBOW_RAINBOW, false);
        //StageManager.GetInstance.Pop(bigMiddleObj.ID, bigMiddleObj.GetComponent<RectTransform>().anchoredPosition);

        //RemoveBigObj(bigMiddleObj);
        //DynamicObjectPool.GetInstance.PoolObject(bigMiddleObj.gameObject, true);
        //bigMiddleObj.Setting();

        yield return new WaitUntil(() => AnimationManager.AnimCount == 0);

        IsEndObstacleActive = true;
    }

    public List<GameBlock> GetBlocksToUseID(EID id)
    {
        var blocks = new List<GameBlock>();

        foreach (var block in blockList)
            if (block.ID == id)
                blocks.Add(block);

        return blocks;
    }

    public int GetCountToUseIDandPreview(EID id)
    {
        var result = 0;

        foreach (var block in blockList)
            if (block.ID == id)
                if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                    result++;

        return result;
    }

    public void RegisterBombBlock()
    {
        StartCoroutine(RegisterBombBlockCoroutine());
    }

    private IEnumerator RegisterBombBlockCoroutine()
    {
        //TODO

        var blocks = new List<GameBlock>();

        foreach (var block in blockList)
            if (block.ID == EID.COLOR_BOMB)
                blocks.Add(block);

        firstBlock = null;
        secondBlock = null;

        combine = ECombine.RAINBOW_NORMAL;

        foreach (var rainbow in blocks)
        {
            var color = GetMostColor();
            rainbow.Tile.RegisterCombinePop(ECombine.RAINBOW_NORMAL, color);
        }

        blocks.Clear();

        yield return new WaitUntil(() => AnimationManager.RainbowCount == 0);

        foreach (var block in blockList)
            if (block.ID == EID.VERTICAL || block.ID == EID.HORIZONTAL
                                         || block.ID == EID.RHOMBUS || block.ID == EID.X)
                blocks.Add(block);

        foreach (var block in blocks)
        {
            if (block.Tile == null) continue;
            if (block.ID != EID.COLOR_BOMB) block.Pop();
        }
    }

    public bool IsJamHater(EID id)
    {
        if (id == EID.BOX || id == EID.OAK || id == EID.SHIELD || id == EID.JAIL
            || id == EID.ICE || id == EID.LAVA || id == EID.CLAM || id == EID.GLASS_COLOR ||
            id == EID.BOX_COLOR || id == EID.CHAMELEON || id == EID.GIFTBOX || id == EID.BANDAGE
            || id == EID.CONNECTION || id == EID.METAL_OAK) return true;
        return false;
    }

    public bool IsBigBlock(EID id)
    {
        if (bigObjList.Contains(id)) return true;
        return false;
    }

    public void AddRainbowBox(BigObject bigObject)
    {
        rainbowBoxList.Add(bigObject);
    }

    public bool IsRainbowBoxBroken()
    {
        if (rainbowBoxList.Count == 0)
            return false;
        return true;
    }

    public void AddOpenBox(GameTile tile, BlockInfo info)
    {
        if (!giftList.ContainsKey(tile)) giftList.Add(tile, info);
    }

    public void RelicCountCheck()
    {
        var missions = StageManager.GetInstance.GetMissions();
        var missionCount = 0;

        if (RelicCoolTime1 > 0)
        {
            var list = GetBlocksToUseID(EID.DROP_RELIC1);
            missionCount = 0;
            foreach (var mission in missions)
                if (mission.ID == EID.DROP_RELIC1)
                    missionCount = mission.count;

            missionCount /= 2;
            missionCount = Mathf.Max(missionCount, 1);

            if (list != null)
                if (list.Count < missionCount)
                    RelicCoolTime1--;
        }

        if (RelicCoolTime2 > 0)
        {
            var list = GetBlocksToUseID(EID.DROP_RELIC2);
            missionCount = 0;
            foreach (var mission in missions)
                if (mission.ID == EID.DROP_RELIC2)
                    missionCount = mission.count;

            missionCount /= 2;
            missionCount = Mathf.Max(missionCount, 1);

            if (list != null)
                if (list.Count == 0)
                    RelicCoolTime2--;
        }
    }

    public void HighlightOff()
    {
        foreach (var item in blockList) item.SetHighlight(false);
    }

    public void CreateGiftBlock()
    {
        foreach (var item in giftList)
        {
            var block = GetGameBlockOrNull(item.Value.ID);
            if (block != null)
            {
                block.Init();
                block.Setting(item.Value.color, item.Value.ID);
                block.SetProperty(DataContainer.GetInstance.GetObstacleDataOrNULL(item.Value.ID));
                block.ApplySprite();
                block.SetTile(item.Key);
                block.SetPosition();

                if (block.ID == EID.COLOR_BOMB)
                {
                    var DummyObj = DynamicObjectPool.GetInstance.GetObjectForType("RainbowDummy", false);
                    DummyObj.transform.SetParent(block.transform);
                    DummyObj.transform.position = block.transform.position;
                    block.GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }

        giftList.Clear();
    }

    public void DestroyInvisibleBox()
    {
        var tempList = new List<GameBlock>();

        foreach (var block in blockList)
            if (block.ID == EID.INVISIBLE_BOX)
                if (TileManager.GetInstance.IsPreViewTile(block.Tile))
                    tempList.Add(block);

        foreach (var block in tempList)
        {
            //block.Remove();
            block.Attacked(10);
            block.Pop();
            block.Tile.RemoveBlock(block);
            block.Clear();
        }
    }

    public bool IsDropMoving()
    {
        foreach (var block in blockList)
            if (block.isDropMoving)
                return true;
        return false;
    }

    public void EndDrop()
    {
        foreach (var block in blockList)
        {
            if (block == null) continue;
            block.IsDropped = false;
            if (block.Tile == null) continue;

            var nextTile = block.Tile.GetConnectTileOrNull(block.Tile.DropDirection);
            if (nextTile != null)
                if (nextTile.TileKind != ETileKind.START)
                    if (nextTile.IsDropAble(nextTile.DropDirection))
                        block.IsDropped = true;
        }
    }

    public bool SwapOkay()
    {
        if (!IsSwapAble) return false;
        if (!IsMovingEnd()) return false;
        if (TileManager.GetInstance.IsBombing()) return false;
        if (AnimationManager.AnimCount != 0) return false;

        return true;
    }

    public void SetBlockPosition()
    {
        foreach (var item in blockList) item.SetTilePosition();

        ResetFishPosition();
    }

    public void ResetFishPosition()
    {
        autoTargetGetter.EndTurn();
    }

    public void AddBoosterItem(EID eID)
    {
        var targetBlock = GetRandomNormalBlockOrNull();
        if (targetBlock == null) return;
        targetBlock.Setting(targetBlock.Color, eID);
        var color = EColor.NONE;
        var sprite = GetBlockSprite(eID, color, 1);
        if (sprite == null) return;
        var obj = Instantiate(BoosterChangeItem, transform);
    }

    public void AddBlock(GameBlock block)
    {
        if (!blockList.Contains(block)) blockList.Add(block);
    }

    public void Clear_Combine()
    {
        foreach (var block in blockList)
        {
            block.IsCombine = false;
        }
    }

    public void Clear()
    {
        StopAllCoroutines();
        ClearSwapBlock();
        ClearPopCondition();

        combines.Clear();
        itemList.Clear();
        popList.Clear();
        blockKindList.Clear();
        giftList.Clear();

        var tempList = new List<GameBlock>();
        foreach (var item in blockList) tempList.Add(item);
        foreach (var block in tempList) block.Clear();

        combine = ECombine.NONE;
        HasJamInCombine = false;

        FireworkCount = 0;
        SwapCount = 0;

        rainbowBoxList.Clear();

        tempList.Clear();
        blockList.Clear();

        bigObjList.Clear();
        bigSizeDictionary.Clear();

        autoTargetGetter.Clear();

        DOTween.Clear();
    }

    public void SetBlockCollider(bool value)
    {
        foreach (var item in blockList) item.SetColliderEnable(value);
    }
}