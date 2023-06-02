using System;
using DG.Tweening;
using LogicStates;
using UnityEngine;

public class NormalBlock : GameBlock
{
    private float clickTime = 0;
    
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
            if (tile.IsDamagedThisTurn)
                return;
        if (isMasterBlock)
        {
            if (id != EID.NORMAL)
                if (tile != null)
                    tile.RegisterBombPop(this);
            return;
        }

        base.Pop();

        if (tile != null)
        {
            var topBlock = tile.GetTopBlockOrNull();
            if (topBlock != null)
                if (topBlock.ID == EID.JAIL)
                {
                    topBlock.Pop();
                    return;
                }
        }

        Attacked(isShowParticle: isShowParticle);
        if (hp <= 0)
        {
            if (id != EID.NORMAL)
                if (tile != null)
                    tile.RegisterBombPop(this);
            if (tile != null)
            {
                tile.RegisterFloorPop();
                tile.RemoveBlock(this);

                Remove();
                Clear();
                tile = null;
            }
        }
    }

    public override void Attacked(int damage = 1, bool isShowParticle = true)
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn)
                return;
        base.Attacked(damage);
        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.Play("BlockDestroy");
        if (isShowParticle) ParticleManager.GetInstance.ShowParticle(ID, Color, HP, transform.position);
    }

    public override void BombPop()
    {
        if (tile != null)
            if (tile.IsDamagedThisTurn)
                return;
        if (RainbowLightningCount != 0) return;
        if (isMasterBlock) return;
        base.BombPop();
        if (hp <= 0)
        {
            StageManager.GetInstance.AddScore(50);
            if (tile != null) StageManager.GetInstance.ShowScoreText(50, tile.Matrix, color);
            if (id != EID.NORMAL)
            {
                if (id == EID.COLOR_BOMB)
                {
                    BlockManager.GetInstance.RegisterRainbow(EColor.NONE, EID.NORMAL, this, tile.IsFreeJam());
                    return;
                }

                if (tile != null) tile.RegisterBombPop(this);
            }

            if (tile != null)
            {
                tile.RegisterFloorPop();
                tile.RegisterBombSidePop();
                tile.RemoveBlock(this);
                tile = null;
                Remove();
                Clear();
            }
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

    // void onMouseDoblueClick_3D()
    // {
    //     if (StaticGameSettings.IsUseDoubleClick)
    //     {
    //
    //         if (DoubleClickSystem.GetInstance.GetBlockList().Contains(ID))
    //         {
    //             BlockManager.GetInstance.IsDoubleClick = true;
    //             BombPop();
    //         }
    //         
    //     }
    // }
    
    void OnMouseUp()
    {
        base.OnMouseUp();
        
        if (Match3DebugSystem.instance != null)
        {
            Match3DebugSystem.instance?.SetBlock(this);
        }
    }


}