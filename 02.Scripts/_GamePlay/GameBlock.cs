using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public struct ItemBlockList
{
    public EItem eItem;
    public GameBlock masterBlock;
    public List<GameBlock> removeBlocks;
}

public struct BlockChangeProperty
{
    public EColor color;
    public int hp;
    public int etc;
}

[Serializable]
public class BlockProperty
{
    public EDepth depth;
    public BlockMoveProperty moveProperty;
    public BlockPopProperty popProperty;
}

[Serializable]
public struct BlockMoveProperty
{
    public bool Swap;
    public bool Drop;
    public bool AnotherDrop;
    public bool AnotherSwap;
}

[Serializable]
public struct BlockPopProperty
{
    public bool SidePop;
    public bool SideBombPop;
    public bool ColorPop;
    public bool BombPop;
    public bool Break;
    public bool AnotherPop;
    public bool SideColorPop;
    public bool AntiBomb;
}

public class GameBlock : Block
{
    [SerializeField] protected BoxCollider2D collider;
    [SerializeField] protected Animator animator;

    public float speed;
    public bool isDropMoving;
    protected float acc = 11f;

    protected SpriteRenderer blockRenderer;
    protected EColor color;
    protected EColor madeColor = EColor.NONE;
    protected List<Vector3> destination = new List<Vector3>();

    protected Dictionary<EDirection, int> directionMatchCounts = new Dictionary<EDirection, int>();
    protected Vector2 dragEndPosition;
    protected Vector2 dragStartPosition;
    protected int etc;
    protected int hp = 1;
    protected EID id;
    protected bool isMasterBlock;
    protected bool isMouseDown;
    protected bool isMoving;
    protected bool isPopConditionClear;
    protected bool isRainbowActive;
    protected bool isTunnel;
    protected EID masterID;
    protected bool isDrag = false;
    public GameBlock isUseSp;

    protected GameBlock myMasterBlock;

    protected BlockProperty property = new BlockProperty();
    protected List<GameBlock> serventBlocks = new List<GameBlock>();
    protected GameTile tile;

    protected bool isAcorn = false;

    public bool IsPopConditionClear
    {
        get => isPopConditionClear;
        set => isPopConditionClear = value;
    }

    public void SetMadeColor(EColor color)
    {
        madeColor = color;
    }

    public SpriteRenderer BlockRenderer => blockRenderer;
    public EColor Color => color;
    public EColor MadeColor => madeColor;
    public GameTile Tile => tile;
    public bool IsMatchMark { get; set; }
    public bool IsDropped { get; set; }
    public bool IsMoving => isMoving;
    public bool IsCombine { get; set; }

    public bool IsMasterBlock
    {
        get => isMasterBlock;
        set => isMasterBlock = value;
    }

    public EID MasterItemKind => masterID;
    public EID ID => id;
    public BlockProperty Property => property;
    public int RainbowLightningCount { get; set; }

    public int HP
    {
        get => hp;
        set => hp = value;
    }

    public int ETC
    {
        get => etc;
        set => etc = value;
    }

    public bool IsBombed { get; set; }
    public SpriteRenderer Renderer => blockRenderer;

    protected Sequence dropSequence = null;
    protected Vector3 bounceVector = new Vector3(0, 0.2f);

    public void KillDropSequence()
    {
        transform.DOPause();
        if (dropSequence != null)
        {
            dropSequence.Kill(false);
            dropSequence = null;
        }
    }

    public virtual void FixedUpdate()
    {
        if (isDropMoving && destination.Count > 0)
            if (!isMoving)
            {
                speed += AnimationManager.GetInstance.dropAcc * Time.deltaTime;
                speed = Mathf.Min(AnimationManager.GetInstance.dropMaxSpeed, speed);
                var target = Vector2.MoveTowards(transform.position, destination[0], Time.deltaTime * speed);
                transform.position = target;
                if ((transform.position - destination[0]).sqrMagnitude < AnimationManager.GetInstance.movingRange)
                {
                    transform.position = destination[0];
                    //isMoving = false;
                    destination.RemoveAt(0);
                    /*
                    if (destination.Count == 0)
                    {
                        if (!IsDropAbleMaybe())
                        {
                            isDropMoving = false;
                            //destination.Clear();
                            speed = AnimationManager.GetInstance.dropStartSpeed;
                        }
                    }
                    */
                }
            }

        if (isDropMoving && destination.Count == 0)
        {
            if (!IsDropAbleMaybe())
            {
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("BlockDrop");
                isDropMoving = false;

                //if (StaticGameSettings.BounceValue > 0)
                //{
                //    animator.SetTrigger("DropEnd");

                //    if (dropSequence == null)
                //    {
                //        dropSequence = DOTween.Sequence();
                //    }

                //    if (tile != null)
                //    {
                //        switch (tile.DropDirection)
                //        {
                //            case EDirection.DOWN:
                //                bounceVector.x = 0;
                //                bounceVector.y = StaticGameSettings.BounceValue;
                //                break;

                //            case EDirection.UP:
                //                bounceVector.x = 0;
                //                bounceVector.y = -StaticGameSettings.BounceValue;
                //                break;

                //            case EDirection.RIGHT:
                //                bounceVector.x = -StaticGameSettings.BounceValue;
                //                bounceVector.y = 0;
                //                break;

                //            case EDirection.LEFT:
                //                bounceVector.x = StaticGameSettings.BounceValue;
                //                bounceVector.y = 0;
                //                break;
                //        }
                //    }
                //    Vector3 targetPosition = transform.position;

                //    dropSequence.Append(transform.DOMove(targetPosition + bounceVector, 0.3f)).SetEase(Ease.OutQuart);
                //    dropSequence.Append(transform.DOMove(targetPosition, 0.2f)).SetEase(Ease.OutQuart);
                //}

                speed = 0;
            }
        }
        if (!isDropMoving && destination.Count > 0)
        {
            destination.Clear();
        }
    }

    protected virtual void OnMouseDown()
    {
        if (StageManager.GetInstance.SelectItem != EUseItem.NONE) return;

        dragStartPosition.x = Input.mousePosition.x;
        dragStartPosition.y = Input.mousePosition.y;

        //GameObject gobTarget = GameObject.Find("Image - Move");
        //Vector3 vecTargetPosition = gobTarget.transform.position;
        //GameObject gobuICam = GameObject.Find("Camera");
        //Camera uiCam = gobuICam.GetComponent<Camera>();

        //Vector3 tilePos = uiCam.ScreenToWorldPoint(tile.transform.position);

        //SpriteMover.StartMoveTarget(tile.NormalBlock.blockRenderer.sprite, 0.9f, tile.transform.position, vecTargetPosition, TestAction);

        isMouseDown = true;
    }

    private void OnMouseDrag()
    {
        if (StageManager.GetInstance.IsTutorialActive())
            StageManager.GetInstance.TutorialConditionClear(ETutorialCondition.TOUCH);

        if (!BlockManager.GetInstance.SwapOkay()) return;
        if (StageManager.GetInstance.SelectItem != EUseItem.NONE) return;
        if (BlockManager.GetInstance.SwapCount > 0) return;
        if (!isMouseDown) return;
        if (tile == null) return;
        if (!tile.IsSwapAble()) return;

        dragEndPosition.x = Input.mousePosition.x;
        dragEndPosition.y = Input.mousePosition.y;

        var x = dragEndPosition.x - dragStartPosition.x;
        var y = dragEndPosition.y - dragStartPosition.y;

        var direction = EDirection.NONE;

        if (x > BlockManager.GetInstance.DragRange)
            direction = EDirection.RIGHT;
        else if (x < BlockManager.GetInstance.DragRange * -1)
            direction = EDirection.LEFT;
        else if (y > BlockManager.GetInstance.DragRange)
            direction = EDirection.UP;
        else if (y < BlockManager.GetInstance.DragRange * -1) direction = EDirection.DOWN;

        if (direction != EDirection.NONE)
        {
            if (!Tile.IsSwapAble(direction)) return;
            isMouseDown = false;
        }
        else
        {
            return;
        }

        Debug.Log("[버그 추적] LogicManager :: OnMouseDrag 7");
        CameraWork.IsMoving = false;
        AnimationManager.AnimCount = 0;
        AnimationManager.RainbowCount = 0;
        AnimationManager.AutoTargetBombCount = 0;
        if (Tile != null)
        {
            isDrag = true;
            Tile.Swap(direction);
        }
    }

    protected void OnMouseUp()
    {
        if (property.depth == EDepth.NORMAL || property.depth == EDepth.TOP)
            if (tile != null)
                if (TileManager.GetInstance.IsPreViewTile(tile))
                    TileManager.GetInstance.ClickBlock(this);
        if (StageManager.GetInstance.SelectItemAgo == EUseItem.NONE)
        {
            if (!isDrag)
            {
                if (DoubleClickSystem.GetInstance != null)
                {
                    foreach (var eid in DoubleClickSystem.GetInstance.GetBlockList())
                    {
                        if (id == eid)
                        {
                            BlockManager.GetInstance.clickedBlock = this;
                            BlockManager.GetInstance.IsDoubleClick = true;
                            //this.BombPop();
                        }
                    }
                }
            }
        }

        StageManager.GetInstance.SelectItemAgo = EUseItem.NONE;
        isDrag = false;

        /*if (Match3DebugSystem.instance != null)
        {
            Match3DebugSystem.instance?.SetBlock(this);
        }*/
    }

    public virtual void Init()
    {
        if (blockRenderer == null) blockRenderer = GetComponent<SpriteRenderer>();
        property.moveProperty = new BlockMoveProperty();
        property.popProperty = new BlockPopProperty();

        acc = AnimationManager.GetInstance.dropAcc;
    }

    public void SetTilePosition()
    {
        if (tile != null) SetPosition(tile);
    }

    public virtual void Setting(EColor _color, EID _id, int _hp = 1, int _etc = 0)
    {
        color = _color;
        id = _id;
        etc = _etc;
        if (id == EID.COLOR_BOMB) color = EColor.NONE;
        var timer = GetComponentInChildren<BombTimer>();
        if (timer != null) timer.SetTurn(etc);
        IsMatchMark = false;
        IsDropped = false;
        //isMasterBlock = false;
        //TODO
        hp = _hp;
        isRainbowActive = false;
        IsCombine = false;
        blockRenderer.color = new Color(1, 1, 1, 1);
    }

    public virtual void SetProperty(BlockProperty blockProperty)
    {
        if (blockProperty != null)
        {
            property.depth = blockProperty.depth;

            property.moveProperty.AnotherDrop = blockProperty.moveProperty.AnotherDrop;
            property.moveProperty.AnotherSwap = blockProperty.moveProperty.AnotherSwap;
            property.moveProperty.Drop = blockProperty.moveProperty.Drop;
            property.moveProperty.Swap = blockProperty.moveProperty.Swap;

            property.popProperty.AnotherPop = blockProperty.popProperty.AnotherPop;
            property.popProperty.AntiBomb = blockProperty.popProperty.AntiBomb;
            property.popProperty.BombPop = blockProperty.popProperty.BombPop;
            property.popProperty.Break = blockProperty.popProperty.Break;
            property.popProperty.ColorPop = blockProperty.popProperty.ColorPop;
            property.popProperty.SideBombPop = blockProperty.popProperty.SideBombPop;
            property.popProperty.SideColorPop = blockProperty.popProperty.SideColorPop;
            property.popProperty.SidePop = blockProperty.popProperty.SidePop;
        }
        else
        {
            property.depth = EDepth.NORMAL;

            property.moveProperty.AnotherDrop = true;
            property.moveProperty.AnotherSwap = true;
            property.moveProperty.Drop = true;
            property.moveProperty.Swap = true;

            property.popProperty.AnotherPop = true;
            property.popProperty.AntiBomb = false;
            property.popProperty.BombPop = true;
            property.popProperty.Break = true;
            property.popProperty.ColorPop = true;
            property.popProperty.SideBombPop = false;
            property.popProperty.SideColorPop = false;
            property.popProperty.SidePop = false;
        }

        if (property.depth == EDepth.NORMAL || property.depth == EDepth.TOP)
            collider.enabled = true;
        else
            collider.enabled = false;
    }

    public virtual void SetTile(GameTile _tile)
    {
        if (_tile != null)
            tile = _tile;
        //transform.position = tile.GetPosition();
    }

    public void SetPosition(GameTile _tile = null)
    {
        if (_tile == null)
        {
            if (tile != null)
                //transform.localPosition = tile.GetPosition();
                transform.localPosition = tile.GetPosition();
        }
        else
        {
            transform.localPosition = _tile.GetPosition();
        }
    }

    public void SetStartTile(GameTile _tile)
    {
        //TODO
        var x = 0f;
        var y = 0f;

        switch (_tile.DropDirection)
        {
            case EDirection.LEFT:
                x = 1f;
                break;

            case EDirection.RIGHT:
                x = -1f;
                break;

            case EDirection.UP:
                y = -1f;
                break;

            case EDirection.DOWN:
                y = 1f;
                break;
        }

        SetTile(_tile);
        isMoving = false;

        transform.position = _tile.GetPosition() + new Vector3(x * TileManager.GetInstance.TileSize.x * 0.8f,
            y * TileManager.GetInstance.TileSize.y * 0.8f);

        //Color startColor = blockRenderer.color;
        //startColor.a = 0.5f;
        //blockRenderer.color = startColor;
        //startColor.a = 1f;

        var targetPosition = _tile.GetPosition();
        var sequence = DOTween.Sequence();
        //sequence.Append(transform.DOMove(targetPosition, 0.1f));
        //sequence.AppendCallback(() => isMoving = false);
        //sequence.Join(blockRenderer.DOColor(startColor, 0.04f));
        //sequence.Append(blockRenderer.DOColor(startColor, 0.5f));

        destination.Add(_tile.GetPosition());
        if (!isDropMoving)
        {
            isDropMoving = true;
            speed = AnimationManager.GetInstance.dropStartSpeed;
        }
    }

    public virtual void ApplySprite()
    {
        if (BlockManager.GetInstance.IsBigBlock(ID) || ID == EID.TABLET || ID == EID.DOUBLE)
            blockRenderer.sprite = null;
        else
            blockRenderer.sprite = BlockManager.GetInstance.GetBlockSprite(id, color, hp);

        if (ID == EID.CLIMBER_NEST)
        {
            blockRenderer.sortingOrder = (int)EDepth.TOP;
            switch (Tile.ReverseDirection)
            {
                case EDirection.RIGHT:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;

                case EDirection.LEFT:
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;

                case EDirection.UP:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;

                case EDirection.DOWN:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }
        else
        {
            blockRenderer.sortingOrder = (int)property.depth;
        }
    }

    public void MarkMatch(EDirection direction, int matchCount, EColor color)
    {
        var block = this;

        for (var i = 0; i < matchCount; i++)
            if (block != null)
            {
                block.IsPopConditionClear = true;
                block.MarkMatchCount(direction, matchCount - i, color);
                block = BlockManager.GetInstance.GetConnectBlockOrNull(block, direction);
            }

        var reverseD = EDirection.NONE;
        block = this;

        switch (direction)
        {
            case EDirection.UP:
                reverseD = EDirection.DOWN;
                break;

            case EDirection.DOWN:
                reverseD = EDirection.UP;
                break;

            case EDirection.LEFT:
                reverseD = EDirection.RIGHT;
                break;

            case EDirection.RIGHT:
                reverseD = EDirection.LEFT;
                break;

            default:
                reverseD = EDirection.NONE;
                break;
        }

        for (var i = matchCount; i > 0; i--)
            if (block != null)
            {
                block.MarkMatchCount(reverseD, i, color);
                block = BlockManager.GetInstance.GetConnectBlockOrNull(block, reverseD);
            }
    }

    public void MarkMatchCount(EDirection direction, int matchCount, EColor color)
    {
        if (matchCount == 1) return;
        if (directionMatchCounts.ContainsKey(direction))
            return;
        directionMatchCounts.Add(direction, matchCount);
    }

    public void MakePopList(ref List<GameBlock> list, EColor color)
    {
        if (!IsPopConditionClear || Color != color) return;

        list.Add(this);
        IsPopConditionClear = false;

        if (directionMatchCounts.ContainsKey(EDirection.UP))
            if (BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.UP) != null)
                BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.UP).MakePopList(ref list, color);
        if (directionMatchCounts.ContainsKey(EDirection.LEFT))
            if (BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.LEFT) != null)
                BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.LEFT).MakePopList(ref list, color);
        if (directionMatchCounts.ContainsKey(EDirection.RIGHT))
            if (BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.RIGHT) != null)
                BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.RIGHT).MakePopList(ref list, color);
        if (directionMatchCounts.ContainsKey(EDirection.DOWN))
            if (BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.DOWN) != null)
                BlockManager.GetInstance.GetConnectBlockOrNull(this, EDirection.DOWN).MakePopList(ref list, color);
    }

    public virtual void Pop(bool isShowParticle = true)
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn)
                return;
        StageManager.GetInstance.AddScore(100);
        if (tile != null) StageManager.GetInstance.ShowScoreText(100, tile.Matrix, color);
        if (tile != null)
            if (property.depth == EDepth.NORMAL)
                tile.RegisterSidePop(color);
        if (isAcorn)
        {
            if (AcornSystem.GetInstance != null)
            {
                var acorn = GetComponentInChildren<DummyObject>();
                if (acorn != null)
                {
                    var collectObj = DynamicObjectPool.GetInstance.GetObjectForType("MissionItemRootingOb", false);
                    if (collectObj == null) return;

                    var collectScript = collectObj.GetComponent<MissionItemRooting>();
                    if (collectScript == null) return;

                    var acornSprite = acorn.GetComponentInChildren<SpriteRenderer>();
                    if (acornSprite != null)
                    {
                        collectScript.AcornAnim(acornSprite.sprite, transform.position);
                    }

                    Destroy(acorn.gameObject);
                }

                AcornSystem.GetInstance.GetAcornItem();
                isAcorn = false;
            }
        }

        if (isUseSp != null)
        {
            isUseSp.BombPop();
        }
    }

    public virtual void Attacked(int damage = 1, bool isShowParticle = true)
    {
        if (tile != null)
            if (id != EID.GOLD && id != EID.TABLET_FLOOR)
                tile.IsDamagedThisTurn = true;
        hp -= damage;
        hp = Mathf.Max(0, hp);
    }

    public virtual void BombPop()
    {
        if (isMasterBlock) return;
        if (tile != null)
            if (tile.IsDamagedThisTurn && property.depth != EDepth.FLOOR)
                return;
        if (IsBombed) return;
        IsBombed = true;
        Attacked();
        if (DoubleClickSystem.GetInstance != null)
        {
            if (DoubleClickSystem.GetInstance.GetBlockList().Contains(id) == true)
            {
                if (VibrationConnector.GetInstance != null)
                {
                    VibrationConnector.StartVibration();
                }
            }
        }
        if (isAcorn)
        {
            if (AcornSystem.GetInstance != null)
            {
                var acorn = GetComponentInChildren<DummyObject>();
                if (acorn != null)
                {
                    var collectObj = DynamicObjectPool.GetInstance.GetObjectForType("MissionItemRootingOb", false);
                    if (collectObj == null) return;

                    var collectScript = collectObj.GetComponent<MissionItemRooting>();
                    if (collectScript == null) return;

                    var acornSprite = acorn.GetComponentInChildren<SpriteRenderer>();
                    if (acornSprite != null)
                    {
                        collectScript.AcornAnim(acornSprite.sprite, transform.position);
                    }

                    Destroy(acorn.gameObject);
                }

                AcornSystem.GetInstance.GetAcornItem();
                isAcorn = false;
            }
        }
    }

    public virtual void JamPop()
    {
        if (tile != null) tile.RegisterJamPop();
    }

    public virtual void SidePop(EColor _color, EDirection _direction)
    {
    }

    public virtual void BombSidePop()
    {
    }

    public void MoveShuffle(GameTile tile)
    {
        var targetPosition = tile.GetPosition();
        isMoving = true;
        var sequence = DOTween.Sequence();
        sequence.Append(
            transform.DOMove(targetPosition, 0.22f).SetEase(Ease.Linear)
        );
        sequence.AppendCallback(() => isMoving = false);
    }

    public void SetMasterBlock(EItem item)
    {
        switch (item)
        {
            case EItem.HORIZONTAL_BOMB:
                masterID = EID.HORIZONTAL;
                break;

            case EItem.VERTICAL_BOMB:
                masterID = EID.VERTICAL;
                break;

            case EItem.RHOMBUS_BOMB:
                masterID = EID.RHOMBUS;
                break;

            case EItem.X_BOMB:
                masterID = EID.X;
                break;

            case EItem.COLOR_BOMB:
                masterID = EID.COLOR_BOMB;
                break;

            case EItem.FISH:
                masterID = EID.FISH;
                break;
        }

        isMasterBlock = true;
        if (tile != null)
        {
            var info = new ItemInfo(this);
            BlockManager.GetInstance.AddCreateItem(info);
        }
    }

    public virtual void ItemCreatePop(GameBlock masterBlock)
    {
        if (!isMasterBlock)
        {
            if (id == EID.NORMAL)
            {
                if (tile != null)
                {
                    var topBlock = tile.GetTopBlockOrNull();
                    if (topBlock != null)
                        if (topBlock.id == EID.JAIL)
                        {
                            topBlock.Pop();
                            return;
                        }
                }

                var serventParticleObj = DynamicObjectPool.GetInstance.GetObjectForType("Particle_JewelMove", false);
                serventParticleObj.transform.SetParent(transform);
                serventParticleObj.transform.localPosition = Vector3.zero;

                myMasterBlock = masterBlock;
                masterBlock.AddServentBlock(this);
                isMoving = true;
                var targetPosition = masterBlock.Tile.GetPosition();
                var sequence = DOTween.Sequence();
                sequence.Append(
                    transform.DOMove(targetPosition, 0.22f).SetEase(Ease.Linear)
                //transform.DOMove(targetPosition, 1f).SetEase(Ease.Linear)
                ).OnComplete(() => isMoving = false);
                sequence.AppendCallback(() =>
                    OnEndServentMoving()
                );
                sequence.AppendCallback(() =>
                    DynamicObjectPool.GetInstance.PoolObject(serventParticleObj, false)
                );
            }
            else
            {
                Pop();
                /*
                if (id == EID.HORIZONTAL || id == EID.VERTICAL
                    || id == EID.X || id == EID.RHOMBUS)
                {
                    tile.RegisterBombPop(this);
                }
                */
            }
        }
        else
        {
            var timer = GetComponentInChildren<BombTimer>();
            if (timer != null)
            {
                timer.Clear();
                DynamicObjectPool.GetInstance.PoolObject(timer.gameObject, false);
            }

            if (EID.NORMAL != ID)
                if (tile != null)
                    tile.RegisterBombPop(this);
            StageManager.GetInstance.CollectMission(ID, color, transform);
            tile.RegisterFloorPop();
            tile.RegisterSidePop(color);
        }
    }

    public void OnEndServentMoving()
    {
        if (myMasterBlock != null) myMasterBlock.EndServentMoving(this);
    }

    public void EndServentMoving(GameBlock serventBlock)
    {
        if (serventBlocks.Contains(serventBlock))
        {
            serventBlock.Pop(false);
            serventBlocks.Remove(serventBlock);
            if (serventBlocks.Count == 0)
            {
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play("BlockCreateItem");
                //isMasterBlock = false;
                if (animator != null)
                    animator.SetTrigger("Item");
            }
        }
    }

    public void SetAnimTrigger(string animTrigger)
    {
        animator.SetTrigger(animTrigger);
    }

    public virtual bool Drop(GameTile targetTile)
    {
        IsDropped = false;
        return true;
    }

    public virtual bool Move(GameTile targetTile)
    {
        IsDropped = false;
        return true;
    }

    public bool IsDropAble()
    {
        if (!property.moveProperty.Drop) return false;
        if (tile == null) return false;

        return true;
    }

    public bool IsDropAbleMaybe()
    {
        if (!IsDropAble()) return false;

        var baseTile = tile;
        var nextTile = TileManager.GetInstance.GetNextTileOrNull(tile);

        while (nextTile != null)
        {
            if (baseTile.IsDropAble(baseTile.DropDirection) && nextTile.IsDroppedAble())
                return true;
            if (!baseTile.IsDropAble(baseTile.DropDirection)) return false;
            baseTile = TileManager.GetInstance.GetNextTileOrNull(baseTile);
            nextTile = TileManager.GetInstance.GetNextTileOrNull(nextTile);
        }

        return false;
    }

    public bool EnterTunnel(GameTile targetTile, EDirection startDirection = EDirection.NONE
        , EDirection endDirection = EDirection.NONE)
    {
        IsDropped = false;
        if (targetTile.NormalBlock != null) return false;
        if (destination.Count > 0 && startDirection == EDirection.NONE) return false;
        KillDropSequence();
        //isMoving = true;
        isMoving = false;
        isTunnel = true;

        StartCoroutine(EnterTunnelCoroutine(targetTile, startDirection, endDirection));

        return true;
    }

    private IEnumerator EnterTunnelCoroutine(GameTile targetTile, EDirection startDirection = EDirection.NONE
        , EDirection endDirection = EDirection.NONE)
    {
        var targetPosition = gameObject.transform.position;
        var nextPosition = targetTile.gameObject.transform.position;

        var sDirection = EDirection.NONE;
        if (startDirection == EDirection.NONE)
            sDirection = Tile.DropDirection;
        else
            sDirection = startDirection;
        switch (sDirection)
        {
            case EDirection.UP:
                targetPosition.y += TileManager.GetInstance.TileSize.y * 0.5f;
                break;

            case EDirection.DOWN:
                targetPosition.y -= TileManager.GetInstance.TileSize.y * 0.5f;
                break;

            case EDirection.RIGHT:
                targetPosition.x += TileManager.GetInstance.TileSize.x * 0.5f;
                break;

            case EDirection.LEFT:
                targetPosition.x -= TileManager.GetInstance.TileSize.x * 0.5f;
                break;
        }

        var removeTile = tile;
        tile = null;
        SetTile(targetTile);
        var eDirection = EDirection.NONE;
        if (endDirection == EDirection.NONE)
            eDirection = targetTile.DropDirection;
        else
            eDirection = endDirection;
        switch (eDirection)
        {
            case EDirection.UP:
                nextPosition.y -= TileManager.GetInstance.TileSize.y * 0.5f;
                break;

            case EDirection.DOWN:
                nextPosition.y += TileManager.GetInstance.TileSize.y * 0.5f;
                break;

            case EDirection.RIGHT:
                nextPosition.x -= TileManager.GetInstance.TileSize.x * 0.5f;
                break;

            case EDirection.LEFT:
                nextPosition.x += TileManager.GetInstance.TileSize.x * 0.5f;
                break;
        }

        if (!isDropMoving)
        {
            //destination.Clear();
            isDropMoving = true;
            speed = AnimationManager.GetInstance.dropStartSpeed;
        }

        destination.Add(targetTile.GetPosition());

        var alphaColor = blockRenderer.color;
        alphaColor.a = 0.1f;

        var sequence = DOTween.Sequence();
        /*
        sequence.Append(
            transform.DOMove(targetPosition, 0.05f)//.SetEase(Ease.Linear)
                       );
        */
        sequence.Join(blockRenderer.DOColor(alphaColor, 0.01f));
        sequence.AppendCallback(() => isTunnel = false);

        yield return new WaitWhile(() => isTunnel);

        gameObject.transform.position = nextPosition;
        alphaColor.a = 1f;
        yield return new WaitForSeconds(0.02f);

        var sequence2 = DOTween.Sequence();
        /*
        sequence2.Append(
            transform.DOMove(tile.gameObject.transform.position, 0.05f)//.SetEase(Ease.Linear)
                       );
                       */
        sequence2.Join(blockRenderer.DOColor(alphaColor, 0.01f));
        sequence2.AppendCallback(() => isMoving = false);
        removeTile.RemoveBlock(this);
    }

    public virtual void Remove()
    {
        //Debug.LogWarningFormat($"{ID}");
        if (ID == EID.CLIMBER)
        {
            TileManager.GetInstance.IsClimberDamaged = true;
        }
        StageManager.GetInstance.CollectMission(id, color, transform);
    }

    public virtual void Clear()
    {
        KillDropSequence();
        IsBombed = false;
        isMoving = false;
        StopAllCoroutines();
        //if (isMasterBlock) return;
        directionMatchCounts.Clear();
        IsMatchMark = false;
        id = EID.NONE;
        serventBlocks.Clear();
        tile = null;
        myMasterBlock = null;
        isMasterBlock = false;
        isRainbowActive = false;
        isDropMoving = false;
        destination.Clear();
        IsCombine = false;
        var dummy = GetComponentInChildren<DummyObject>();
        if (dummy != null)
        {
            dummy.Clear();
            DynamicObjectPool.GetInstance.PoolObject(dummy.gameObject, false);
        }

        var timer = GetComponentInChildren<BombTimer>();
        if (timer != null)
        {
            timer.Clear();
            DynamicObjectPool.GetInstance.PoolObject(timer.gameObject, false);
        }

        BlockManager.GetInstance.RemoveBlockFromList(this);
        DynamicObjectPool.GetInstance.PoolObject(gameObject, true);
    }

    public virtual void Swap(GameTile _tile)
    {
        BlockManager.GetInstance.StartBlockSwap();
        var targetPosition = new Vector3();
        if (_tile != null) targetPosition = _tile.GetPosition();
        var swapParticleObj = DynamicObjectPool.GetInstance.GetObjectForType("Particle_JewelMove", true);
        if (swapParticleObj != null)
        {
            swapParticleObj.transform.SetParent(transform);
            swapParticleObj.transform.localPosition = Vector3.zero;
        }

        var sequence = DOTween.Sequence();
        if (sequence != null)
        {
            sequence.Append(transform.DOMove(targetPosition, AnimationManager.GetInstance.swapSpeed));
            sequence.AppendCallback(() => SetTile(_tile));
            sequence.AppendCallback(() => BlockManager.GetInstance.EndBlockSwap());
            sequence.AppendCallback(() =>
                DynamicObjectPool.GetInstance.PoolObject(swapParticleObj, true));
        }
    }

    public List<ItemBlockList> ItemCreateCheck(List<GameBlock> PopList)
    {
        GameBlock CompleteBlock = null;
        var direction = EDirection.NONE;

        var decomulationBlocks = new List<ItemBlockList>();
        var RemoveBlocks = new List<ItemBlockList>();

        if (PopList.Count == 3)
        {
            if (VibrationConnector.GetInstance != null)
            {
                VibrationConnector.StartVibration();
            }

            var lists = new ItemBlockList();
            lists.eItem = EItem.NONE;
            lists.removeBlocks = PopList;
            decomulationBlocks.Add(lists);
        }
        else if (PopList.Count == 4) //계산량 감소를 위해 별도 추가 함
        {
            if (VibrationConnector.GetInstance != null)
            {
                VibrationConnector.StartVibration();
            }
            var MaxmatchCount = 0;
            var item = EItem.NONE;
            foreach (var block in PopList)
            {
                for (var d = 0; d < 4; d++)
                {
                    var blockTile = block.Tile;
                    if (blockTile == null) continue;
                    if (!TileManager.GetInstance.IsPreViewTile(blockTile)) continue;
                    if (!blockTile.IsPopAble()) continue;
                    if (block.IsDropAble() && blockTile.IsDropAble(blockTile.DropDirection)) continue;

                    var nextTile = blockTile.GetConnectTileOrNull((EDirection)d);
                    if (nextTile == null) continue;

                    if (!nextTile.IsPopAble()) continue;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                    var nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) continue;
                    if (nextBlock.IsDropAble() && blockTile.IsDropAble(blockTile.DropDirection)) continue;
                    if (nextBlock.Color != block.Color) continue;
                    if (!PopList.Contains(nextBlock)) continue;

                    var isColorMatch = true;
                    var matchCount = 1;
                    while (isColorMatch)
                    {
                        matchCount++;
                        nextTile = nextTile.GetConnectTileOrNull((EDirection)d);
                        if (nextTile == null)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!TileManager.GetInstance.IsPreViewTile(nextTile))
                        {
                            isColorMatch = false;
                            break;
                        }

                        nextBlock = nextTile.NormalBlock;
                        if (nextBlock == null)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!PopList.Contains(nextBlock))
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (nextBlock.Color != block.Color)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                        {
                            isColorMatch = false;
                            break;
                        }
                    }

                    if (MaxmatchCount < matchCount && matchCount > 1)
                    {
                        MaxmatchCount = matchCount;
                        CompleteBlock = block;
                        direction = (EDirection)d;
                    }
                }

                if (MaxmatchCount == 4)
                {
                    if (direction == EDirection.UP || direction == EDirection.DOWN)
                    {
                        item = EItem.HORIZONTAL_BOMB;
                        var lists = new ItemBlockList();
                        lists.eItem = item;
                        lists.masterBlock = CompleteBlock.tile.GetConnectTileOrNull(direction).NormalBlock;
                        if (lists.masterBlock.Tile.GetTopBlockOrNull() != null &&
                            lists.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                            foreach (var _cBlock in PopList)
                                if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                {
                                    if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                    {
                                        if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                        {
                                            lists.masterBlock = _cBlock;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lists.masterBlock = _cBlock;
                                        break;
                                    }
                                }

                        lists.removeBlocks = PopList;
                        decomulationBlocks.Add(lists);
                    }
                    else
                    {
                        item = EItem.VERTICAL_BOMB;
                        var lists = new ItemBlockList();
                        lists.eItem = item;
                        lists.masterBlock = CompleteBlock.tile.GetConnectTileOrNull(direction).NormalBlock;
                        if (lists.masterBlock.Tile.GetTopBlockOrNull() != null &&
                            lists.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                            foreach (var _cBlock in PopList)
                                if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                {
                                    if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                    {
                                        if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                        {
                                            lists.masterBlock = _cBlock;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        lists.masterBlock = _cBlock;
                                        break;
                                    }
                                }

                        lists.removeBlocks = PopList;
                        decomulationBlocks.Add(lists);
                    }

                    break;
                }
            }

            if (MaxmatchCount < 3) //FishCheck
            {
                if (CompleteBlock != null || direction != EDirection.NONE)
                {
                    item = EItem.FISH;
                    var lists = new ItemBlockList();
                    lists.eItem = item;
                    lists.masterBlock = CompleteBlock;
                    if (lists.masterBlock.Tile.GetTopBlockOrNull() != null &&
                        lists.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                        foreach (var _cBlock in PopList)
                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                            {
                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                {
                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                    {
                                        lists.masterBlock = _cBlock;
                                        break;
                                    }
                                }
                                else
                                {
                                    lists.masterBlock = _cBlock;
                                    break;
                                }
                            }

                    lists.removeBlocks = PopList;
                    decomulationBlocks.Add(lists);
                }
            }
            else if (MaxmatchCount == 3)
            {
                item = EItem.NONE;
                var checkBlocks = new List<GameBlock>();
                checkBlocks.Add(CompleteBlock);

                var nextTile = CompleteBlock.tile.GetConnectTileOrNull(direction);
                if (nextTile == null) return null;
                if (!nextTile.IsPopAble()) return null;
                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) return null;

                var nextBlock = nextTile.NormalBlock;
                if (nextBlock == null) return null;
                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) return null;
                if (nextBlock.Color != CompleteBlock.Color) return null;
                if (!PopList.Contains(nextBlock)) return null;

                for (var i = 0; i < 2; i++)
                {
                    checkBlocks.Add(nextBlock);
                    if (i == 1) continue;
                    nextTile = nextBlock.tile.GetConnectTileOrNull(direction);
                    if (nextTile == null) break;
                    if (!nextTile.IsPopAble()) break;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                    nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) break;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                    if (nextBlock.Color != CompleteBlock.Color) break;
                    if (!PopList.Contains(nextBlock)) break;
                }

                if (checkBlocks.Count == 3)
                {
                    var list = new ItemBlockList();
                    list.eItem = item;
                    list.masterBlock = CompleteBlock;
                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                        foreach (var _cBlock in PopList)
                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                            {
                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                {
                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                    {
                                        list.masterBlock = _cBlock;
                                        break;
                                    }
                                }
                                else
                                {
                                    list.masterBlock = _cBlock;
                                    break;
                                }
                            }

                    list.removeBlocks = checkBlocks;
                    decomulationBlocks.Add(list);
                }
            }
        }
        else
        {
            if (VibrationConnector.GetInstance != null)
            {
                VibrationConnector.StartVibration();
            }
            var MaxmatchCount = 0;
            foreach (var block in PopList)
            {
                var item = EItem.NONE;
                MaxmatchCount = 0;
                block.directionMatchCounts.Clear();

                for (var d = 0; d < 4; d++)
                {
                    var blockTile = block.Tile;
                    if (blockTile == null) continue;
                    if (!TileManager.GetInstance.IsPreViewTile(blockTile)) continue;
                    if (!blockTile.IsPopAble()) continue;
                    if (block.IsDropAble() && blockTile.IsDropAble(blockTile.DropDirection)) continue;

                    var nextTile = blockTile.GetConnectTileOrNull((EDirection)d);
                    if (nextTile == null) continue;
                    if (!nextTile.IsPopAble()) continue;
                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                    var nextBlock = nextTile.NormalBlock;
                    if (nextBlock == null) continue;
                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                    if (nextBlock.Color != block.Color) continue;
                    if (!PopList.Contains(nextBlock)) continue;

                    var isColorMatch = true;
                    var matchCount = 1;
                    while (isColorMatch)
                    {
                        matchCount++;
                        nextTile = nextTile.GetConnectTileOrNull((EDirection)d);
                        if (nextTile == null)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!TileManager.GetInstance.IsPreViewTile(nextTile))
                        {
                            isColorMatch = false;
                            break;
                        }

                        nextBlock = nextTile.NormalBlock;
                        if (nextBlock == null)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (!PopList.Contains(nextBlock))
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (nextBlock.Color != block.Color)
                        {
                            isColorMatch = false;
                            break;
                        }

                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                        {
                            isColorMatch = false;
                            break;
                        }
                    }

                    if (!directionMatchCounts.ContainsKey((EDirection)d))
                    {
                        directionMatchCounts.Add((EDirection)d, matchCount);
                    }
                    else
                    {
                        if (directionMatchCounts[(EDirection)d] < matchCount)
                            directionMatchCounts[(EDirection)d] = matchCount;
                    }

                    if (MaxmatchCount < matchCount)
                    {
                        MaxmatchCount = Mathf.Max(MaxmatchCount, matchCount);
                        CompleteBlock = block;
                        direction = (EDirection)d;
                    }
                }

                if (MaxmatchCount > 2)
                {
                    if (MaxmatchCount > 4)
                    {
                        var checkBlocks = new List<GameBlock>();
                        checkBlocks.Add(block);

                        var nextTile = block.tile.GetConnectTileOrNull(direction);
                        if (nextTile == null) continue;
                        if (!nextTile.IsPopAble()) continue;
                        if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                        var nextBlock = nextTile.NormalBlock;
                        if (nextBlock == null) continue;
                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                        if (nextBlock.Color != block.Color) continue;
                        if (!PopList.Contains(nextBlock)) continue;

                        var masterBlock = block;

                        for (var i = 0; i < 4; i++)
                        {
                            checkBlocks.Add(nextBlock);
                            if (i == 1) masterBlock = nextBlock;
                            else if (i == 3) continue;
                            nextTile = nextTile.GetConnectTileOrNull(direction);
                            if (nextTile == null) break;
                            if (!nextTile.IsPopAble()) break;
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                            nextBlock = nextTile.NormalBlock;
                            if (nextBlock == null) break;
                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                            if (nextBlock.Color != block.Color) break;
                            if (!PopList.Contains(nextBlock)) break;
                        }

                        if (checkBlocks.Count == 5)
                        {
                            item = EItem.COLOR_BOMB;
                            var list = new ItemBlockList();
                            list.eItem = item;
                            list.masterBlock = checkBlocks[2];
                            //list.masterBlock = CompleteBlock;

                            if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                foreach (var _cBlock in PopList)
                                    if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                    {
                                        if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                        {
                                            if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                            {
                                                list.masterBlock = _cBlock;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            list.masterBlock = _cBlock;
                                            break;
                                        }
                                    }

                            list.removeBlocks = checkBlocks;
                            decomulationBlocks.Add(list);
                        }
                    }

                    if (item != EItem.COLOR_BOMB)
                    {
                        if (direction != EDirection.NONE)
                            for (var d = 0; d < 4; d++)
                            {
                                if ((int)direction == d) continue;

                                var checkDirection = (EDirection)d;

                                var checkBlocks = new List<GameBlock>();
                                checkBlocks.Add(block);
                                var nextTile = block.tile.GetConnectTileOrNull(checkDirection);
                                if (nextTile == null) continue;
                                if (!nextTile.IsPopAble()) continue;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                                var nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) continue;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                if (nextBlock.Color != block.Color) continue;
                                if (!PopList.Contains(nextBlock)) continue;

                                for (var i = 0; i < 2; i++)
                                {
                                    checkBlocks.Add(nextBlock);
                                    if (i == 1) continue;
                                    nextTile = nextTile.GetConnectTileOrNull(checkDirection);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;

                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;
                                }

                                nextTile = block.tile.GetConnectTileOrNull(direction);
                                if (nextTile == null) continue;
                                if (!nextTile.IsPopAble()) continue;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) continue;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                if (nextBlock.Color != block.Color) continue;
                                if (!PopList.Contains(nextBlock)) continue;

                                for (var i = 0; i < 2; i++)
                                {
                                    checkBlocks.Add(nextBlock);
                                    if (i == 1) continue;
                                    nextTile = nextTile.GetConnectTileOrNull(direction);
                                    if (nextTile == null) break;

                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;

                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;
                                }

                                if (checkBlocks.Count == 5)
                                {
                                    item = EItem.RHOMBUS_BOMB;
                                    var list = new ItemBlockList();
                                    list.eItem = item;
                                    list.masterBlock = CompleteBlock;
                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                        foreach (var _cBlock in PopList)
                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                {
                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                    {
                                                        list.masterBlock = _cBlock;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }

                                    list.removeBlocks = checkBlocks;
                                    decomulationBlocks.Add(list);
                                }
                            }

                        if (item != EItem.COLOR_BOMB && item != EItem.RHOMBUS_BOMB)
                        {
                            var checkBlocks = new List<GameBlock>();
                            checkBlocks.Add(block);
                            for (var d = 0; d < 4; d++)
                            {
                                var nextTile = block.tile.GetConnectTileOrNull((EDirection)d);
                                if (nextTile == null) break;
                                if (!nextTile.IsPopAble()) break;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                                var nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) break;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                if (nextBlock.Color != block.Color) break;
                                if (!PopList.Contains(nextBlock)) break;

                                checkBlocks.Add(nextBlock);
                            }

                            if (checkBlocks.Count == 5)
                            {
                                item = EItem.RHOMBUS_BOMB;
                                var list = new ItemBlockList();
                                list.eItem = item;
                                list.masterBlock = CompleteBlock;
                                if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                    list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                    foreach (var _cBlock in PopList)
                                        if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                        {
                                            if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                list.masterBlock = _cBlock;
                                                break;
                                            }
                                        }

                                list.removeBlocks = checkBlocks;
                                decomulationBlocks.Add(list);
                            }
                        }

                        if (item != EItem.RHOMBUS_BOMB && item != EItem.COLOR_BOMB)
                            if (direction != EDirection.NONE)
                            {
                                var checkDirection_1 = EDirection.NONE;
                                var checkDirection_2 = EDirection.NONE;
                                if (direction == EDirection.DOWN || direction == EDirection.UP)
                                {
                                    checkDirection_1 = EDirection.LEFT;
                                    checkDirection_2 = EDirection.RIGHT;
                                }
                                else
                                {
                                    checkDirection_1 = EDirection.UP;
                                    checkDirection_2 = EDirection.DOWN;
                                }

                                var checkBlocks = new List<GameBlock>();
                                checkBlocks.Add(block);
                                var nextTile = block.tile.GetConnectTileOrNull(direction);
                                if (nextTile == null) continue;
                                if (!nextTile.IsPopAble()) continue;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                                var nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) continue;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                if (nextBlock.Color != block.Color) continue;
                                if (!PopList.Contains(nextBlock)) continue;

                                for (var i = 0; i < 2; i++)
                                {
                                    checkBlocks.Add(nextBlock);
                                    if (i == 1) continue;
                                    nextTile = nextTile.GetConnectTileOrNull(direction);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;
                                }

                                var blockCheck = false;

                                while (!blockCheck)
                                {
                                    nextTile = block.tile.GetConnectTileOrNull(checkDirection_1);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;

                                    blockCheck = true;
                                }

                                if (blockCheck) checkBlocks.Add(nextBlock);

                                blockCheck = false;

                                while (!blockCheck)
                                {
                                    nextTile = block.tile.GetConnectTileOrNull(checkDirection_2);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;

                                    blockCheck = true;
                                }

                                if (blockCheck) checkBlocks.Add(nextBlock);

                                if (checkBlocks.Count == 5)
                                {
                                    // remove XBomb
                                    item = EItem.X_BOMB;
                                    // item = EItem.RHOMBUS_BOMB;

                                    var list = new ItemBlockList();
                                    list.eItem = item;
                                    list.masterBlock = CompleteBlock;
                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                        foreach (var _cBlock in PopList)
                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                {
                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                    {
                                                        list.masterBlock = _cBlock;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }

                                    list.removeBlocks = checkBlocks;
                                    decomulationBlocks.Add(list);
                                }
                            }
                    }

                    if (item != EItem.RHOMBUS_BOMB && item != EItem.X_BOMB && item != EItem.COLOR_BOMB)
                    {
                        if (directionMatchCounts.ContainsKey(EDirection.UP) &&
                            directionMatchCounts[EDirection.UP] > 3 ||
                            directionMatchCounts.ContainsKey(EDirection.DOWN) &&
                            directionMatchCounts[EDirection.DOWN] > 3 ||
                            directionMatchCounts.ContainsKey(EDirection.LEFT) &&
                            directionMatchCounts[EDirection.LEFT] > 3 ||
                            directionMatchCounts.ContainsKey(EDirection.RIGHT) &&
                            directionMatchCounts[EDirection.RIGHT] > 3)
                        {
                            var checkDirection = EDirection.NONE;

                            for (var d = 0; d < 4; d++)
                            {
                                if (directionMatchCounts.ContainsKey((EDirection)d) &&
                                    directionMatchCounts[(EDirection)d] > 3) checkDirection = (EDirection)d;
                                else continue;
                                var checkBlocks = new List<GameBlock>();
                                checkBlocks.Add(block);

                                var nextTile = block.tile.GetConnectTileOrNull(checkDirection);
                                if (nextTile == null) continue;
                                if (!nextTile.IsPopAble()) continue;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                                var nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) continue;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                if (nextBlock.Color != block.Color) continue;
                                if (!PopList.Contains(nextBlock)) continue;

                                var masterBlock = nextBlock;

                                for (var i = 0; i < 3; i++)
                                {
                                    checkBlocks.Add(nextBlock);
                                    if (i == 2) continue;
                                    nextTile = nextTile.GetConnectTileOrNull(checkDirection);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;
                                }

                                if (checkBlocks.Count == 4)
                                {
                                    if (d == (int)EDirection.DOWN || d == (int)EDirection.UP)
                                        item = EItem.HORIZONTAL_BOMB;
                                    else item = EItem.VERTICAL_BOMB;
                                    var list = new ItemBlockList();
                                    list.eItem = item;
                                    list.masterBlock = CompleteBlock.Tile.GetConnectTileOrNull(direction).NormalBlock;
                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                        foreach (var _cBlock in PopList)
                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                {
                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                    {
                                                        list.masterBlock = _cBlock;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }

                                    list.removeBlocks = checkBlocks;
                                    decomulationBlocks.Add(list);
                                }
                            }
                        }

                        if (item != EItem.VERTICAL_BOMB && item != EItem.HORIZONTAL_BOMB)
                        {
                            if (StaticGameSettings.IsAbleFish) // if (true) //FishCheck
                            {
                                var leftUp = false;
                                var leftDown = false;
                                var rightUp = false;
                                var rightDown = false;
                                var checkBlocks = new List<GameBlock>();
                                GameTile tile_1 = null;
                                GameTile tile_2 = null;
                                GameTile tile_3 = null;
                                GameBlock block_1 = null;
                                GameBlock block_2 = null;
                                GameBlock block_3 = null;

                                while (!leftUp)
                                {
                                    tile_1 = block.tile.GetConnectTileOrNull(EDirection.DOWN);
                                    if (tile_1 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                    tile_2 = block.tile.GetConnectTileOrNull(EDirection.DOWN_RIGHT);
                                    if (tile_2 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                    tile_3 = block.tile.GetConnectTileOrNull(EDirection.RIGHT);
                                    if (tile_3 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                    block_1 = tile_1.NormalBlock;
                                    if (block_1 == null) break;
                                    if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                    if (!PopList.Contains(block_1)) break;
                                    block_2 = tile_2.NormalBlock;
                                    if (block_2 == null) break;
                                    if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                    if (!PopList.Contains(block_2)) break;
                                    block_3 = tile_3.NormalBlock;
                                    if (block_3 == null) break;
                                    if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                    if (!PopList.Contains(block_3)) break;

                                    leftUp = true;
                                }

                                if (leftUp)
                                {
                                    checkBlocks.Add(block_1);
                                    checkBlocks.Add(block_2);
                                    checkBlocks.Add(block_3);
                                    checkBlocks.Add(block);
                                    item = EItem.FISH;

                                    for (var i = 0; i < checkBlocks.Count; i++)
                                        for (var dic = 0; dic < 4; dic++)
                                        {
                                            var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                            if (nextTile == null) continue;
                                            if (!nextTile.IsPopAble()) continue;
                                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                            var nextBlock = nextTile.NormalBlock;
                                            if (nextBlock == null) continue;
                                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                                continue;
                                            if (nextBlock.Color != block.Color) continue;
                                            if (!PopList.Contains(nextBlock)) continue;

                                            if (block.color == nextBlock.color)
                                                if (!checkBlocks.Contains(nextBlock))
                                                {
                                                    var FishList = new List<GameBlock>();
                                                    FishList.AddRange(checkBlocks);
                                                    FishList.Add(nextBlock);
                                                    var list = new ItemBlockList();
                                                    list.eItem = item;
                                                    list.masterBlock = CompleteBlock;
                                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                        foreach (var _cBlock in PopList)
                                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                            {
                                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                {
                                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                    {
                                                                        list.masterBlock = _cBlock;
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list.masterBlock = _cBlock;
                                                                    break;
                                                                }
                                                            }

                                                    list.removeBlocks = FishList;
                                                    decomulationBlocks.Add(list);
                                                }
                                        }
                                }
                                else
                                {
                                    while (!leftDown)
                                    {
                                        tile_1 = block.tile.GetConnectTileOrNull(EDirection.UP);
                                        if (tile_1 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                        tile_2 = block.tile.GetConnectTileOrNull(EDirection.UP_RIGHT);
                                        if (tile_2 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                        tile_3 = block.tile.GetConnectTileOrNull(EDirection.RIGHT);
                                        if (tile_3 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                        block_1 = tile_1.NormalBlock;
                                        if (block_1 == null) break;
                                        if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                        if (!PopList.Contains(block_1)) break;
                                        block_2 = tile_2.NormalBlock;
                                        if (block_2 == null) break;
                                        if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                        if (!PopList.Contains(block_2)) break;
                                        block_3 = tile_3.NormalBlock;
                                        if (block_3 == null) break;
                                        if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                        if (!PopList.Contains(block_3)) break;
                                        leftDown = true;
                                    }

                                    if (leftDown)
                                    {
                                        checkBlocks.Add(block_1);
                                        checkBlocks.Add(block_2);
                                        checkBlocks.Add(block_3);
                                        checkBlocks.Add(block);
                                        item = EItem.FISH;

                                        for (var i = 0; i < checkBlocks.Count; i++)
                                            for (var dic = 0; dic < 4; dic++)
                                            {
                                                var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                                if (nextTile == null) continue;
                                                if (!nextTile.IsPopAble()) continue;
                                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                                var nextBlock = nextTile.NormalBlock;
                                                if (nextBlock == null) continue;
                                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                                    continue;
                                                if (nextBlock.Color != block.Color) continue;
                                                if (!PopList.Contains(nextBlock)) continue;

                                                if (block.color == nextBlock.color)
                                                    if (!checkBlocks.Contains(nextBlock))
                                                    {
                                                        var FishList = new List<GameBlock>();
                                                        FishList.AddRange(checkBlocks);
                                                        FishList.Add(nextBlock);
                                                        var list = new ItemBlockList();
                                                        list.eItem = item;
                                                        list.masterBlock = CompleteBlock;
                                                        if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                            list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                            foreach (var _cBlock in PopList)
                                                                if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                                {
                                                                    if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                    {
                                                                        if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                        {
                                                                            list.masterBlock = _cBlock;
                                                                            break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        list.masterBlock = _cBlock;
                                                                        break;
                                                                    }
                                                                }

                                                        list.removeBlocks = FishList;
                                                        decomulationBlocks.Add(list);
                                                    }
                                            }
                                    }
                                    else
                                    {
                                        while (!rightUp)
                                        {
                                            tile_1 = block.tile.GetConnectTileOrNull(EDirection.DOWN);
                                            if (tile_1 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                            tile_2 = block.tile.GetConnectTileOrNull(EDirection.DOWN_LEFT);
                                            if (tile_2 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                            tile_3 = block.tile.GetConnectTileOrNull(EDirection.LEFT);
                                            if (tile_3 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                            block_1 = tile_1.NormalBlock;
                                            if (block_1 == null) break;
                                            if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                            if (!PopList.Contains(block_1)) break;
                                            block_2 = tile_2.NormalBlock;
                                            if (block_2 == null) break;
                                            if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                            if (!PopList.Contains(block_2)) break;
                                            block_3 = tile_3.NormalBlock;
                                            if (block_3 == null) break;
                                            if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                            if (!PopList.Contains(block_3)) break;
                                            rightUp = true;
                                        }

                                        if (rightUp)
                                        {
                                            checkBlocks.Add(block_1);
                                            checkBlocks.Add(block_2);
                                            checkBlocks.Add(block_3);
                                            checkBlocks.Add(block);
                                            item = EItem.FISH;

                                            for (var i = 0; i < checkBlocks.Count; i++)
                                                for (var dic = 0; dic < 4; dic++)
                                                {
                                                    var nextTile = checkBlocks[i].tile
                                                        .GetConnectTileOrNull((EDirection)dic);
                                                    if (nextTile == null) continue;
                                                    if (!nextTile.IsPopAble()) continue;
                                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                                    var nextBlock = nextTile.NormalBlock;
                                                    if (nextBlock == null) continue;
                                                    if (nextBlock.IsDropAble() &&
                                                        nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                                    if (nextBlock.Color != block.Color) continue;
                                                    if (!PopList.Contains(nextBlock)) continue;

                                                    if (block.color == nextBlock.color)
                                                        if (!checkBlocks.Contains(nextBlock))
                                                        {
                                                            var FishList = new List<GameBlock>();
                                                            FishList.AddRange(checkBlocks);
                                                            FishList.Add(nextBlock);
                                                            var list = new ItemBlockList();
                                                            list.eItem = item;
                                                            list.masterBlock = CompleteBlock;
                                                            if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                                list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                                foreach (var _cBlock in PopList)
                                                                    if (_cBlock.ID == EID.NORMAL ||
                                                                        _cBlock.ID == EID.CHAMELEON)
                                                                    {
                                                                        if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                        {
                                                                            if (_cBlock.Tile.GetTopBlockOrNull().ID !=
                                                                                EID.JAIL)
                                                                            {
                                                                                list.masterBlock = _cBlock;
                                                                                break;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            list.masterBlock = _cBlock;
                                                                            break;
                                                                        }
                                                                    }

                                                            list.removeBlocks = FishList;
                                                            decomulationBlocks.Add(list);
                                                        }
                                                }
                                        }
                                        else
                                        {
                                            while (!rightDown)
                                            {
                                                tile_1 = block.tile.GetConnectTileOrNull(EDirection.LEFT);
                                                if (tile_1 == null) break;
                                                if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                                tile_2 = block.tile.GetConnectTileOrNull(EDirection.UP_LEFT);
                                                if (tile_2 == null) break;
                                                if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                                tile_3 = block.tile.GetConnectTileOrNull(EDirection.UP);
                                                if (tile_3 == null) break;
                                                if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                                block_1 = tile_1.NormalBlock;
                                                if (block_1 == null) break;
                                                if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection))
                                                    break;
                                                if (!PopList.Contains(block_1)) break;
                                                block_2 = tile_2.NormalBlock;
                                                if (block_2 == null) break;
                                                if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection))
                                                    break;
                                                if (!PopList.Contains(block_2)) break;
                                                block_3 = tile_3.NormalBlock;
                                                if (block_3 == null) break;
                                                if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection))
                                                    break;
                                                if (!PopList.Contains(block_3)) break;
                                                rightDown = true;
                                            }

                                            if (rightDown)
                                            {
                                                checkBlocks.Add(block_1);
                                                checkBlocks.Add(block_2);
                                                checkBlocks.Add(block_3);
                                                checkBlocks.Add(block);
                                                item = EItem.FISH;

                                                for (var i = 0; i < checkBlocks.Count; i++)
                                                    for (var dic = 0; dic < 4; dic++)
                                                    {
                                                        var nextTile = checkBlocks[i].tile
                                                            .GetConnectTileOrNull((EDirection)dic);
                                                        if (nextTile == null) continue;
                                                        if (!nextTile.IsPopAble()) continue;
                                                        if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                                        var nextBlock = nextTile.NormalBlock;
                                                        if (nextBlock == null) continue;
                                                        if (nextBlock.IsDropAble() &&
                                                            nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                                        if (nextBlock.Color != block.Color) continue;
                                                        if (!PopList.Contains(nextBlock)) continue;

                                                        if (block.color == nextBlock.color)
                                                            if (!checkBlocks.Contains(nextBlock))
                                                            {
                                                                var FishList = new List<GameBlock>();
                                                                FishList.AddRange(checkBlocks);
                                                                FishList.Add(nextBlock);
                                                                var list = new ItemBlockList();
                                                                list.eItem = item;
                                                                list.masterBlock = CompleteBlock;
                                                                if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                                    list.masterBlock.Tile.GetTopBlockOrNull().ID ==
                                                                    EID.JAIL)
                                                                    foreach (var _cBlock in PopList)
                                                                        if (_cBlock.ID == EID.NORMAL ||
                                                                            _cBlock.ID == EID.CHAMELEON)
                                                                        {
                                                                            if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                            {
                                                                                if (_cBlock.Tile.GetTopBlockOrNull().ID !=
                                                                                    EID.JAIL)
                                                                                {
                                                                                    list.masterBlock = _cBlock;
                                                                                    break;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                list.masterBlock = _cBlock;
                                                                                break;
                                                                            }
                                                                        }

                                                                list.removeBlocks = FishList;
                                                                decomulationBlocks.Add(list);
                                                            }
                                                    }
                                            }
                                        }
                                    }
                                }

                                if (item != EItem.FISH)
                                {
                                    checkBlocks = new List<GameBlock>();
                                    checkBlocks.Add(block);

                                    var nextTile = block.tile.GetConnectTileOrNull(direction);
                                    if (nextTile == null) return null;
                                    if (!nextTile.IsPopAble()) return null;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) return null;

                                    var nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) return null;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                        return null;
                                    if (nextBlock.Color != block.Color) return null;
                                    if (!PopList.Contains(nextBlock)) return null;

                                    for (var i = 0; i < 2; i++)
                                    {
                                        checkBlocks.Add(nextBlock);
                                        if (i == 1) continue;
                                        nextTile = nextBlock.tile.GetConnectTileOrNull(direction);
                                        if (nextTile == null) break;
                                        if (!nextTile.IsPopAble()) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                        nextBlock = nextTile.NormalBlock;
                                        if (nextBlock == null) break;
                                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                            break;
                                        if (nextBlock.Color != block.Color) break;
                                        if (!PopList.Contains(nextBlock)) break;
                                    }

                                    if (checkBlocks.Count == 3)
                                    {
                                        var list = new ItemBlockList();
                                        list.eItem = item;
                                        list.masterBlock = CompleteBlock;
                                        if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                            list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                            foreach (var _cBlock in PopList)
                                                if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                {
                                                    if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                    {
                                                        if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                        {
                                                            list.masterBlock = _cBlock;
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        list.masterBlock = _cBlock;
                                                        break;
                                                    }
                                                }

                                        list.removeBlocks = checkBlocks;
                                        decomulationBlocks.Add(list);
                                    }
                                }
                            }
                            else
                            {
                                var checkBlocks = new List<GameBlock>();

                                checkBlocks = new List<GameBlock>();
                                checkBlocks.Add(block);

                                var nextTile = block.tile.GetConnectTileOrNull(direction);
                                if (nextTile == null) return null;
                                if (!nextTile.IsPopAble()) return null;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) return null;

                                var nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) return null;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) return null;
                                if (nextBlock.Color != block.Color) return null;
                                if (!PopList.Contains(nextBlock)) return null;

                                for (var i = 0; i < 2; i++)
                                {
                                    checkBlocks.Add(nextBlock);
                                    if (i == 1) continue;
                                    nextTile = nextBlock.tile.GetConnectTileOrNull(direction);
                                    if (nextTile == null) break;
                                    if (!nextTile.IsPopAble()) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                    nextBlock = nextTile.NormalBlock;
                                    if (nextBlock == null) break;
                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                    if (nextBlock.Color != block.Color) break;
                                    if (!PopList.Contains(nextBlock)) break;
                                }

                                if (checkBlocks.Count == 3)
                                {
                                    var list = new ItemBlockList();
                                    list.eItem = item;
                                    list.masterBlock = CompleteBlock;
                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                        foreach (var _cBlock in PopList)
                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                {
                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                    {
                                                        list.masterBlock = _cBlock;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }

                                    list.removeBlocks = checkBlocks;
                                    decomulationBlocks.Add(list);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (item != EItem.COLOR_BOMB)
                    {
                        var checkBlocks = new List<GameBlock>();
                        checkBlocks.Add(block);
                        for (var d = 0; d < 4; d++)
                        {
                            var nextTile = block.tile.GetConnectTileOrNull((EDirection)d);
                            if (nextTile == null) break;
                            if (!nextTile.IsPopAble()) break;
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                            var nextBlock = nextTile.NormalBlock;
                            if (nextBlock == null) break;
                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                            if (nextBlock.Color != block.Color) break;
                            if (!PopList.Contains(nextBlock)) break;

                            checkBlocks.Add(nextBlock);
                        }

                        if (checkBlocks.Count == 5)
                        {
                            item = EItem.RHOMBUS_BOMB;
                            var list = new ItemBlockList();
                            list.eItem = item;
                            list.masterBlock = CompleteBlock;
                            if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                foreach (var _cBlock in PopList)
                                    if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                    {
                                        if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                        {
                                            if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                            {
                                                list.masterBlock = _cBlock;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            list.masterBlock = _cBlock;
                                            break;
                                        }
                                    }

                            list.removeBlocks = checkBlocks;
                            decomulationBlocks.Add(list);
                        }
                    }

                    if (item == EItem.NONE || item == EItem.FISH)
                    {
                        var leftUp = false;
                        var leftDown = false;
                        var rightUp = false;
                        var rightDown = false;
                        var checkBlocks = new List<GameBlock>();
                        GameTile tile_1 = null;
                        GameTile tile_2 = null;
                        GameTile tile_3 = null;
                        GameBlock block_1 = null;
                        GameBlock block_2 = null;
                        GameBlock block_3 = null;
                        if (StaticGameSettings.IsAbleFish)//if (false)           //FishCheck
                        {
                            while (!leftUp)
                            {
                                tile_1 = block.tile.GetConnectTileOrNull(EDirection.DOWN);
                                if (tile_1 == null) break;
                                if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                tile_2 = block.tile.GetConnectTileOrNull(EDirection.DOWN_RIGHT);
                                if (tile_2 == null) break;
                                if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                tile_3 = block.tile.GetConnectTileOrNull(EDirection.RIGHT);
                                if (tile_3 == null) break;
                                if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                block_1 = tile_1.NormalBlock;
                                if (block_1 == null) break;
                                if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                if (!PopList.Contains(block_1)) break;
                                block_2 = tile_2.NormalBlock;
                                if (block_2 == null) break;
                                if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                if (!PopList.Contains(block_2)) break;
                                block_3 = tile_3.NormalBlock;
                                if (block_3 == null) break;
                                if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                if (!PopList.Contains(block_3)) break;

                                leftUp = true;
                            }

                            if (leftUp)
                            {
                                checkBlocks.Add(block_1);
                                checkBlocks.Add(block_2);
                                checkBlocks.Add(block_3);
                                checkBlocks.Add(block);
                                item = EItem.FISH;

                                for (var i = 0; i < checkBlocks.Count; i++)
                                    for (var dic = 0; dic < 4; dic++)
                                    {
                                        var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                        if (nextTile == null) continue;
                                        if (!nextTile.IsPopAble()) continue;
                                        if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                        var nextBlock = nextTile.NormalBlock;
                                        if (nextBlock == null) continue;
                                        if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                        if (nextBlock.Color != block.Color) continue;
                                        if (!PopList.Contains(nextBlock)) continue;

                                        if (block.color == nextBlock.color)
                                            if (!checkBlocks.Contains(nextBlock))
                                            {
                                                var FishList = new List<GameBlock>();
                                                FishList.AddRange(checkBlocks);
                                                FishList.Add(nextBlock);
                                                var list = new ItemBlockList();
                                                list.eItem = item;
                                                list.masterBlock = CompleteBlock;
                                                if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                    list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                    foreach (var _cBlock in PopList)
                                                        if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                        {
                                                            if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                            {
                                                                if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                {
                                                                    list.masterBlock = _cBlock;
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                list.masterBlock = _cBlock;
                                                                break;
                                                            }
                                                        }

                                                list.removeBlocks = FishList;
                                                decomulationBlocks.Add(list);
                                            }
                                    }
                            }
                            else
                            {
                                while (!leftDown)
                                {
                                    tile_1 = block.tile.GetConnectTileOrNull(EDirection.UP);
                                    if (tile_1 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                    tile_2 = block.tile.GetConnectTileOrNull(EDirection.UP_RIGHT);
                                    if (tile_2 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                    tile_3 = block.tile.GetConnectTileOrNull(EDirection.RIGHT);
                                    if (tile_3 == null) break;
                                    if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                    block_1 = tile_1.NormalBlock;
                                    if (block_1 == null) break;
                                    if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                    if (!PopList.Contains(block_1)) break;
                                    block_2 = tile_2.NormalBlock;
                                    if (block_2 == null) break;
                                    if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                    if (!PopList.Contains(block_2)) break;
                                    block_3 = tile_3.NormalBlock;
                                    if (block_3 == null) break;
                                    if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                    if (!PopList.Contains(block_3)) break;

                                    leftDown = true;
                                }

                                if (leftDown)
                                {
                                    checkBlocks.Add(block_1);
                                    checkBlocks.Add(block_2);
                                    checkBlocks.Add(block_3);
                                    checkBlocks.Add(block);
                                    item = EItem.FISH;

                                    for (var i = 0; i < checkBlocks.Count; i++)
                                        for (var dic = 0; dic < 4; dic++)
                                        {
                                            var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                            if (nextTile == null) continue;
                                            if (!nextTile.IsPopAble()) continue;
                                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                            var nextBlock = nextTile.NormalBlock;
                                            if (nextBlock == null) continue;
                                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                                            if (nextBlock.Color != block.Color) continue;
                                            if (!PopList.Contains(nextBlock)) continue;

                                            if (block.color == nextBlock.color)
                                                if (!checkBlocks.Contains(nextBlock))
                                                {
                                                    var FishList = new List<GameBlock>();
                                                    FishList.AddRange(checkBlocks);
                                                    FishList.Add(nextBlock);
                                                    var list = new ItemBlockList();
                                                    list.eItem = item;
                                                    list.masterBlock = CompleteBlock;
                                                    if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                        list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                        foreach (var _cBlock in PopList)
                                                            if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                            {
                                                                if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                {
                                                                    if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                    {
                                                                        list.masterBlock = _cBlock;
                                                                        break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list.masterBlock = _cBlock;
                                                                    break;
                                                                }
                                                            }

                                                    list.removeBlocks = FishList;
                                                    decomulationBlocks.Add(list);
                                                }
                                        }
                                }
                                else
                                {
                                    while (!rightUp)
                                    {
                                        tile_1 = block.tile.GetConnectTileOrNull(EDirection.DOWN);
                                        if (tile_1 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                        tile_2 = block.tile.GetConnectTileOrNull(EDirection.DOWN_LEFT);
                                        if (tile_2 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                        tile_3 = block.tile.GetConnectTileOrNull(EDirection.LEFT);
                                        if (tile_3 == null) break;
                                        if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                        block_1 = tile_1.NormalBlock;
                                        if (block_1 == null) break;
                                        if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                        if (!PopList.Contains(block_1)) break;
                                        block_2 = tile_2.NormalBlock;
                                        if (block_2 == null) break;
                                        if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                        if (!PopList.Contains(block_2)) break;
                                        block_3 = tile_3.NormalBlock;
                                        if (block_3 == null) break;
                                        if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                        if (!PopList.Contains(block_3)) break;

                                        rightUp = true;
                                    }

                                    if (rightUp)
                                    {
                                        checkBlocks.Add(block_1);
                                        checkBlocks.Add(block_2);
                                        checkBlocks.Add(block_3);
                                        checkBlocks.Add(block);
                                        item = EItem.FISH;

                                        for (var i = 0; i < checkBlocks.Count; i++)
                                            for (var dic = 0; dic < 4; dic++)
                                            {
                                                var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                                if (nextTile == null) continue;
                                                if (!nextTile.IsPopAble()) continue;
                                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                                var nextBlock = nextTile.NormalBlock;
                                                if (nextBlock == null) continue;
                                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                                    continue;
                                                if (nextBlock.Color != block.Color) continue;
                                                if (!PopList.Contains(nextBlock)) continue;

                                                if (block.color == nextBlock.color)
                                                    if (!checkBlocks.Contains(nextBlock))
                                                    {
                                                        var FishList = new List<GameBlock>();
                                                        FishList.AddRange(checkBlocks);
                                                        FishList.Add(nextBlock);
                                                        var list = new ItemBlockList();
                                                        list.eItem = item;
                                                        list.masterBlock = CompleteBlock;
                                                        if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                            list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                            foreach (var _cBlock in PopList)
                                                                if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                                {
                                                                    if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                    {
                                                                        if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                        {
                                                                            list.masterBlock = _cBlock;
                                                                            break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        list.masterBlock = _cBlock;
                                                                        break;
                                                                    }
                                                                }

                                                        list.removeBlocks = FishList;
                                                        decomulationBlocks.Add(list);
                                                    }
                                            }
                                    }
                                    else
                                    {
                                        while (!rightDown)
                                        {
                                            tile_1 = block.tile.GetConnectTileOrNull(EDirection.LEFT);
                                            if (tile_1 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_1)) break;
                                            tile_2 = block.tile.GetConnectTileOrNull(EDirection.UP_LEFT);
                                            if (tile_2 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_2)) break;
                                            tile_3 = block.tile.GetConnectTileOrNull(EDirection.UP);
                                            if (tile_3 == null) break;
                                            if (!TileManager.GetInstance.IsPreViewTile(tile_3)) break;
                                            block_1 = tile_1.NormalBlock;
                                            if (block_1 == null) break;
                                            if (block_1.IsDropAble() && tile_1.IsDropAble(tile_1.DropDirection)) break;
                                            if (!PopList.Contains(block_1)) break;
                                            block_2 = tile_2.NormalBlock;
                                            if (block_2 == null) break;
                                            if (block_2.IsDropAble() && tile_2.IsDropAble(tile_2.DropDirection)) break;
                                            if (!PopList.Contains(block_2)) break;
                                            block_3 = tile_3.NormalBlock;
                                            if (block_3 == null) break;
                                            if (block_3.IsDropAble() && tile_3.IsDropAble(tile_3.DropDirection)) break;
                                            if (!PopList.Contains(block_3)) break;
                                            rightDown = true;
                                        }

                                        if (rightDown)
                                        {
                                            checkBlocks.Add(block_1);
                                            checkBlocks.Add(block_2);
                                            checkBlocks.Add(block_3);
                                            checkBlocks.Add(block);
                                            item = EItem.FISH;

                                            for (var i = 0; i < checkBlocks.Count; i++)
                                                for (var dic = 0; dic < 4; dic++)
                                                {
                                                    var nextTile = checkBlocks[i].tile.GetConnectTileOrNull((EDirection)dic);
                                                    if (nextTile == null) continue;
                                                    if (!nextTile.IsPopAble()) continue;
                                                    if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;
                                                    var nextBlock = nextTile.NormalBlock;
                                                    if (nextBlock == null) continue;
                                                    if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection))
                                                        continue;
                                                    if (nextBlock.Color != block.Color) continue;
                                                    if (!PopList.Contains(nextBlock)) continue;

                                                    if (block.color == nextBlock.color)
                                                        if (!checkBlocks.Contains(nextBlock))
                                                        {
                                                            var FishList = new List<GameBlock>();
                                                            FishList.AddRange(checkBlocks);
                                                            FishList.Add(nextBlock);
                                                            var list = new ItemBlockList();
                                                            list.eItem = item;
                                                            list.masterBlock = CompleteBlock;
                                                            if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                                                list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                                                foreach (var _cBlock in PopList)
                                                                    if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                                                    {
                                                                        if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                                                        {
                                                                            if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                                            {
                                                                                list.masterBlock = _cBlock;
                                                                                break;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            list.masterBlock = _cBlock;
                                                                            break;
                                                                        }
                                                                    }

                                                            list.removeBlocks = FishList;
                                                            decomulationBlocks.Add(list);
                                                        }
                                                }
                                        }
                                    }
                                }
                            }
                        }

                        if (item != EItem.FISH)
                        {
                            checkBlocks = new List<GameBlock>();
                            checkBlocks.Add(block);

                            var nextTile = block.tile.GetConnectTileOrNull(direction);
                            if (nextTile == null) continue;
                            if (!nextTile.IsPopAble()) continue;
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                            var nextBlock = nextTile.NormalBlock;
                            if (nextBlock == null) continue;
                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                            if (nextBlock.Color != block.Color) continue;
                            if (!PopList.Contains(nextBlock)) continue;

                            for (var i = 0; i < 2; i++)
                            {
                                checkBlocks.Add(nextBlock);
                                if (i == 1) continue;
                                nextTile = nextBlock.tile.GetConnectTileOrNull(direction);
                                if (nextTile == null) break;
                                if (!nextTile.IsPopAble()) break;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) break;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                if (nextBlock.Color != block.Color) break;
                                if (!PopList.Contains(nextBlock)) break;
                            }

                            if (checkBlocks.Count == 3)
                            {
                                var list = new ItemBlockList();
                                list.eItem = item;
                                list.masterBlock = CompleteBlock;
                                if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                    list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                    foreach (var _cBlock in PopList)
                                        if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                        {
                                            if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                list.masterBlock = _cBlock;
                                                break;
                                            }
                                        }

                                list.removeBlocks = checkBlocks;
                                decomulationBlocks.Add(list);
                            }
                        }
                        else
                        {
                            var checkmyBlocks = new List<GameBlock>();

                            checkmyBlocks = new List<GameBlock>();
                            checkmyBlocks.Add(block);

                            var nextTile = block.tile.GetConnectTileOrNull(direction);
                            if (nextTile == null) continue;
                            if (!nextTile.IsPopAble()) continue;
                            if (!TileManager.GetInstance.IsPreViewTile(nextTile)) continue;

                            var nextBlock = nextTile.NormalBlock;
                            if (nextBlock == null) continue;
                            if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) continue;
                            if (nextBlock.Color != block.Color) continue;
                            if (!PopList.Contains(nextBlock)) continue;

                            for (var i = 0; i < 2; i++)
                            {
                                checkmyBlocks.Add(nextBlock);
                                if (i == 1) continue;
                                nextTile = nextBlock.tile.GetConnectTileOrNull(direction);
                                if (nextTile == null) break;
                                if (!nextTile.IsPopAble()) break;
                                if (!TileManager.GetInstance.IsPreViewTile(nextTile)) break;
                                nextBlock = nextTile.NormalBlock;
                                if (nextBlock == null) break;
                                if (nextBlock.IsDropAble() && nextTile.IsDropAble(nextTile.DropDirection)) break;
                                if (nextBlock.Color != block.Color) break;
                                if (!PopList.Contains(nextBlock)) break;
                            }

                            if (checkmyBlocks.Count == 3)
                            {
                                var list = new ItemBlockList();
                                list.eItem = item;
                                list.masterBlock = CompleteBlock;
                                if (list.masterBlock.Tile.GetTopBlockOrNull() != null &&
                                    list.masterBlock.Tile.GetTopBlockOrNull().ID == EID.JAIL)
                                    foreach (var _cBlock in PopList)
                                        if (_cBlock.ID == EID.NORMAL || _cBlock.ID == EID.CHAMELEON)
                                        {
                                            if (_cBlock.Tile.GetTopBlockOrNull() != null)
                                            {
                                                if (_cBlock.Tile.GetTopBlockOrNull().ID != EID.JAIL)
                                                {
                                                    list.masterBlock = _cBlock;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                list.masterBlock = _cBlock;
                                                break;
                                            }
                                        }

                                list.removeBlocks = checkBlocks;
                                decomulationBlocks.Add(list);
                            }
                        }
                    }
                }
            }
        }

        var itemCheck = true;

        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.COLOR_BOMB)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.RHOMBUS_BOMB)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.X_BOMB)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.VERTICAL_BOMB)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.HORIZONTAL_BOMB)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.FISH)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        itemCheck = true;
        while (itemCheck)
        {
            itemCheck = false;
            for (var i = 0; i < decomulationBlocks.Count; i++)
                if (decomulationBlocks[i].eItem == EItem.NONE)
                {
                    var Nothing = true;
                    foreach (var block in decomulationBlocks[i].removeBlocks)
                    {
                        for (var j = 0; j < RemoveBlocks.Count; j++)
                            if (RemoveBlocks[j].removeBlocks.Contains(block))
                            {
                                Nothing = false;
                                break;
                            }

                        if (Nothing == false) break;
                    }

                    if (Nothing)
                    {
                        RemoveBlocks.Add(decomulationBlocks[i]);
                        decomulationBlocks.RemoveAt(i);
                        itemCheck = true;
                        break;
                    }
                }
        }

        return RemoveBlocks;
    }

    public void AddServentBlock(GameBlock block)
    {
        if (!serventBlocks.Contains(block)) serventBlocks.Add(block);
    }

    public void StartRainbow(List<GameBlock> blocks, EID targetID, bool isJam)
    {
        if (isRainbowActive) return;
        isRainbowActive = true;
        property.popProperty.Break = false;
        AnimationManager.RainbowCount++;
        StartCoroutine(RainbowCoroutine(blocks, targetID, isJam));
    }

    public virtual void ActiveAbility()
    {
    }

    private IEnumerator RainbowCoroutine(List<GameBlock> blocks, EID targetID, bool isJam)
    {
        var rainbowAnim = DynamicObjectPool.GetInstance.GetObjectForType("RainbowAnim", false);
        var rainbowLines = new List<GameObject>();

        if (rainbowAnim != null)
        {
            rainbowAnim.transform.SetParent(transform);
            rainbowAnim.transform.localPosition = Vector3.zero;
            rainbowAnim.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        foreach (var block in blocks)
        {
            if (block == null) continue;
            if (block.Tile == null) continue;
            //Show Raiser ShowRaiser(block, targetID)
            var rainbowLine = DynamicObjectPool.GetInstance.GetObjectForType("RainbowLineRenderers", false);
            if (rainbowLine != null)
            {
                rainbowLines.Add(rainbowLine);

                rainbowLine.transform.SetParent(transform);
                rainbowLine.transform.localPosition = Vector3.zero;
                rainbowLine.transform.rotation = Quaternion.Euler(0, 0, 0);
                RainbowLightningCount++;

                var line = rainbowLine.GetComponent<RainbowLineController>();
                line.Init(this, targetID);
                line.SetTarget(block.gameObject, block);
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitUntil(() => RainbowLightningCount == 0);
        GameObject gob = PrefabRegister.GetInstance.GetPrefab("Particle_JewelRainbow", transform);

        yield return new WaitForSeconds(0.2f);
        ParticleManager.GetInstance.ShowParticle(EID.COLOR_BOMB, EColor.NONE, 0, transform.position);

        foreach (var line in rainbowLines) DynamicObjectPool.GetInstance.PoolObject(line, false);

        if (tile != null)
        {
            tile.RegisterFloorPop();
            tile.RemoveBlock(this);
        }
        else
        {
            DebugX.LogError("Tile Null");
        }

        var countString = new StringBuilder("RainCount: ");
        countString.Append(AnimationManager.RainbowCount);
        DebugX.Log(countString);
        Remove();
        Clear();

        // foreach (var block in blocks)
        // {
        //     if (block.Tile != null && Tile != null)
        //         if (block.Tile.Matrix == Tile.Matrix)
        //             continue;
        //     if (block.ID == EID.FISH)
        //     {
        //         yield return new WaitForSeconds(0.08f);
        //         if (isJam) block.JamPop();
        //         block.Pop();
        //     }
        // }
        foreach (var block in blocks)
        {
            if (isJam) block.JamPop();
            block.Pop();
        }

        DynamicObjectPool.GetInstance.PoolObject(rainbowAnim, false);

        //yield return new WaitForSeconds(0.2f);

        AnimationManager.RainbowCount--;
    }

    public void SetEndLightning()
    {
        RainbowLightningCount--;
    }

    public void SetIDWhenActiveRainbow(EID objectID)
    {
        if (id == EID.NORMAL)
        {
            var resultID = objectID;
            if (objectID == EID.HORIZONTAL || objectID == EID.VERTICAL)
            {
                if (Random.Range(0, 2) == 0)
                    resultID = EID.HORIZONTAL;
                else
                    resultID = EID.VERTICAL;
            }

            if (DoubleClickSystem.GetInstance != null)
            {
                foreach (var spBlock in DoubleClickSystem.GetInstance.GetBlockList())
                {
                    if (objectID == spBlock)
                    {
                        color = EColor.NONE;
                    }
                }
            }

            id = resultID;
            ApplySprite();
        }
    }

    public virtual void SetHighlight(bool value)
    {
        if (value)
        {
            Renderer.sortingOrder = (int)property.depth + 1000;
            Renderer.maskInteraction = SpriteMaskInteraction.None;
            Renderer.gameObject.layer = 5;
            if (ID == EID.COLOR_BOMB)
            {
                var dummyObject = transform.GetComponentInChildren<DummyObject>();
                if (dummyObject != null) dummyObject.SetHighlight(value);
            }
            if (ID == EID.CHAMELEON)
            {
                Transform[] allChildren = GetComponentsInChildren<Transform>();
                foreach (var child in allChildren)
                {
                    var childRenderer = child.GetComponent<SpriteRenderer>();
                    childRenderer.sortingOrder = (int)property.depth + 1000;
                    childRenderer.maskInteraction = SpriteMaskInteraction.None;
                    childRenderer.gameObject.layer = 5;
                }
            }
            if (ID == EID.DOUBLE)
            {
                Transform[] allChildren = GetComponentsInChildren<Transform>();
                foreach (var child in allChildren)
                {
                    var childRenderer = child.GetComponent<SpriteRenderer>();
                    //childRenderer.sortingOrder = (int)property.depth + 1000;
                    childRenderer.maskInteraction = SpriteMaskInteraction.None;
                    childRenderer.gameObject.layer = 5;
                }
            }
        }
        else
        {
            Renderer.sortingOrder = (int)property.depth;
            Renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            Renderer.gameObject.layer = 0;
        }
    }

    public void SetColliderEnable(bool value = false)
    {
        collider.enabled = value;
    }

    public bool IsACorn()
    {
        return isAcorn;
    }

    public void SetACorn(bool value)
    {
        isAcorn = value;
    }
}