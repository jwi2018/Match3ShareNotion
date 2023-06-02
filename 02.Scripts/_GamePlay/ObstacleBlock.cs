using DG.Tweening;
using UnityEngine;

public class ObstacleBlock : GameBlock
{
    public override void Init()
    {
        base.Init();
    }

    public override void Setting(EColor _color, EID _id, int _hp = 1, int _etc = 0)
    {
        base.Setting(_color, _id, _hp, _etc);
    }


    public override void SetTile(GameTile _tile)
    {
        base.SetTile(_tile);
        tile.AddBlock(this);
    }

    public override void Pop(bool isShowParticle = true)
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn && property.depth != EDepth.FLOOR)
                return;
        if (BlockManager.GetInstance.IsBigBlock(ID))
        {
        }
        else
        {
            if (!property.popProperty.ColorPop) return;
            var isJailHave = false;
            if (tile != null)
            {
                var TopBlock = tile.GetTopBlockOrNull();
                if (TopBlock != null)
                    if (ID != EID.JAIL && TopBlock.ID == EID.JAIL)
                    {
                        isJailHave = true;
                        TopBlock.Pop();
                        return;
                    }
            }

            if (isMasterBlock) return;

            base.Pop(isShowParticle);
            Attacked();
            if (hp <= 0)
            {
                if (id == EID.HORIZONTAL || id == EID.VERTICAL || id == EID.RHOMBUS
                    || id == EID.X)
                    tile.RegisterBombPop(this);
                if (tile != null)
                    if (property.popProperty.Break && !isJailHave)
                    {
                        if (id == EID.TIMEBOMB_ICE || id == EID.TIMEBOMB_LAVA) tile.RegisterFloorPop();
                        tile.RemoveBlock(this);
                        Remove();
                        Clear();
                        tile = null;
                    }
            }
            else
            {
                ApplySprite();
            }
        }
    }

    public override void ItemCreatePop(GameBlock masterBlock)
    {
        base.ItemCreatePop(masterBlock);
    }

    public override void Attacked(int damage = 1, bool isShowParticle = true)
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn && property.depth != EDepth.FLOOR)
                return;

        if (isShowParticle)
        {
            ParticleManager.GetInstance.ShowParticle(ID, Color, HP - 1, transform.position);
            if (id == EID.RELIC_IN_INVISIBLE_BOX && HP == 1)
                ParticleManager.GetInstance.ShowParticle(ID, EColor.RED, 0, transform.position);
        }

        base.Attacked(damage);

        if (SoundManager.GetInstance != null)
            switch (ID)
            {
                case EID.OAK:
                    SoundManager.GetInstance.Play("Oak");
                    break;
                case EID.GOLD:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.GoldMine);
                    break;
                case EID.ICE:
                    SoundManager.GetInstance.Play("Ice");
                    break;
                case EID.GIFTBOX:
                    SoundManager.GetInstance.Play("GiftBox");
                    break;
                case EID.TABLET_FLOOR:
                    SoundManager.GetInstance.Play("Sand");
                    break;
                case EID.BOX:
                    SoundManager.GetInstance.Play("Box");
                    break;
                case EID.BOX_COLOR:
                    SoundManager.GetInstance.Play("ColorBox");
                    break;
                case EID.TIMEBOMB_ICE:
                    //SoundManager.GetInstance.Play(SoundManager.GetInstance.TimeBomb);
                    break;
                case EID.TIMEBOMB_LAVA:
                    //SoundManager.GetInstance.Play(SoundManager.GetInstance.TimeBomb);
                    break;
                case EID.ACTINIARIA:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Lamp);
                    break;
                case EID.LAVA:
                    SoundManager.GetInstance.Play("Lava");
                    break;
                case EID.BANDAGE:
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.Band);
                    break;
                case EID.RELIC_IN_INVISIBLE_BOX:
                    SoundManager.GetInstance.Play("PoolHit");
                    break;
                case EID.INVISIBLE_BOX:
                    SoundManager.GetInstance.Play("PoolHit");
                    break;
                case EID.SHIELD:
                    SoundManager.GetInstance.Play("Box");
                    break;
                case EID.METAL_BOX:
                    SoundManager.GetInstance.Play("MetalDestroy");
                    break;
                case EID.METAL_OAK:
                    SoundManager.GetInstance.Play("MetalDestroy");
                    break;
                case EID.TURN_BOX:
                    SoundManager.GetInstance.Play("TurnDestroy");
                    break;
            }
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn)
                return;
        if (!TileManager.GetInstance.IsPreViewTile(tile)) return;

        if (BlockManager.GetInstance.IsBigBlock(ID))
        {
            TileManager.GetInstance.BigSidePop(this, _color, _direction);
        }
        else
        {
            var isPopOk = false;
            base.SidePop(color, _direction);
            if (property.popProperty.SidePop)
                isPopOk = true;
            else if (property.popProperty.SideColorPop && color == _color) isPopOk = true;

            if (isPopOk)
            {
                Attacked();
                if (hp <= 0)
                {
                    StageManager.GetInstance.AddScore(50);
                    if (tile != null)
                        if (property.popProperty.Break)
                        {
                            tile.RemoveBlock(this);

                            Remove();
                            Clear();
                            tile = null;
                        }
                }
                else
                {
                    ApplySprite();
                }
            }
        }
    }

    public override void BombPop()
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn && property.depth != EDepth.FLOOR)
                return;
        if (RainbowLightningCount != 0) return;
        if (BlockManager.GetInstance.IsBigBlock(ID))
        {
            TileManager.GetInstance.BigBombPop(this);
        }
        else
        {
            if (isMasterBlock) return;
            if (!property.popProperty.BombPop) return;
            base.BombPop();

            if (id == EID.COLOR_BOMB)
            {
                BlockManager.GetInstance.RegisterRainbow(EColor.NONE, EID.NORMAL, this, tile.IsFreeJam());
                return;
            }

            if (id == EID.HORIZONTAL || id == EID.VERTICAL || id == EID.RHOMBUS
                || id == EID.X)
            {
                tile.RegisterBombPop(this);
            }

            if (hp <= 0)
            {
                StageManager.GetInstance.AddScore(50);
                if (tile != null) StageManager.GetInstance.ShowScoreText(50, tile.Matrix, color);
                if (tile != null)
                {
                    tile.RemoveBlock(this);

                    Remove();
                    Clear();
                    tile = null;
                }
            }
            else
            {
                ApplySprite();
            }
        }
    }

    public override void BombSidePop()
    {
        if (!property.popProperty.SideBombPop) return;

        //hp--;
        Attacked();
        if (hp <= 0)
        {
            StageManager.GetInstance.AddScore(50);
            if (tile != null) StageManager.GetInstance.ShowScoreText(50, tile.Matrix, color);
            if (tile != null)
                if (property.popProperty.Break)
                {
                    tile.RemoveBlock(this);

                    Remove();
                    Clear();
                    tile = null;
                }
        }
        else
        {
            ApplySprite();
        }
    }

    public override bool Drop(GameTile targetTile)
    {
        if (!base.Drop(targetTile)) return false;
        if (targetTile.NormalBlock != null) return false;
        if (isMoving || isTunnel) return false;

        KillDropSequence();

        tile = null;
        //isMoving = true;
        SetTile(targetTile);

        /*
        Vector3 targetPosition = targetTile.GetPosition();

        Sequence sequence = DOTween.Sequence();
        sequence.SetSpeedBased(true);
        sequence.Append(
            transform.DOMove(targetPosition, 0.09f).SetEase(Ease.Linear)
        ).OnComplete(() => isMoving = false);
        */

        destination.Add(targetTile.GetPosition());
        if (!isDropMoving)
        {
            isDropMoving = true;
            speed = AnimationManager.GetInstance.dropStartSpeed;
        }

        return true;
    }

    public override bool Move(GameTile targetTile)
    {
        if (!base.Move(targetTile)) return false;
        if (isMoving) return false;

        tile = null;
        isMoving = true;
        SetTile(targetTile);

        var targetPosition = targetTile.GetPosition();

        var startColor = blockRenderer.color;
        startColor.a = 0.5f;
        blockRenderer.color = startColor;
        startColor.a = 1f;

        var sequence = DOTween.Sequence();
        sequence.SetSpeedBased(true);
        sequence.Append(
            transform.DOMove(targetPosition, 0.09f).SetEase(Ease.Linear)
        ).OnComplete(() => isMoving = false);
        sequence.Join(blockRenderer.DOColor(startColor, 0.02f));

        return true;
    }

    public override void Remove()
    {
        base.Remove();
        var timer = GetComponentInChildren<BombTimer>();
        if (timer != null)
        {
            timer.Clear();
            DynamicObjectPool.GetInstance.PoolObject(timer.gameObject, false);
        }

        if (ID == EID.GIFTBOX)
        {
            int ranColorInt;
            if (DoubleClickSystem.GetInstance != null)
            {
                ranColorInt = (int)EColor.NONE;
            }
            else
            {
                ranColorInt = Random.Range(0, (int) EColor.ORANGE);
            }

            var ranIDInt = Random.Range(0, 100);
            var ranID = EID.NONE;
            if (ranIDInt > 95) ranID = EID.COLOR_BOMB;
            else if (ranIDInt > 80) ranID = EID.RHOMBUS;
            else if (ranIDInt > 40) ranID = EID.HORIZONTAL;
            else ranID = EID.VERTICAL;

            var info = new BlockInfo(ranID, (EColor) ranColorInt, 1, 0);
            BlockManager.GetInstance.AddOpenBox(tile, info);
        }

        if (ID == EID.CLAM)
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("ClamBroken");
    }
}