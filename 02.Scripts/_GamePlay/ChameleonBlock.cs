using UnityEngine;

public class ChameleonBlock : ObstacleBlock
{
    [SerializeField] private Sprite chameleonBaseSprite;
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer ChangeColorSprite;
    [SerializeField] private SpriteRenderer nextColorSpirte;
    [SerializeField] private Animator chameleonAnimator;

    public override void Setting(EColor _color, EID _id, int _hp = 1, int _etc = 0)
    {
        base.Setting(_color, _id, _hp, _etc);
        ChangeColorSprite.color = new Color(1, 1, 1, 1);
        nextColorSpirte.color = new Color(1, 1, 1, 0);
    }

    public override void ApplySprite()
    {
        if (id == EID.CHAMELEON)
        {
            //base.ApplySprite();
            baseRenderer.sprite = chameleonBaseSprite;
            ChangeColorSprite.gameObject.SetActive(true);
            ChangeColorSprite.sprite = BlockManager.GetInstance.GetBlockSprite(EID.CHAMELEON, color, 1);
        }
        
        else
        {
            base.ApplySprite();
            ChangeColorSprite.gameObject.SetActive(false);
            nextColorSpirte.gameObject.SetActive(false);
        }
    }

    public override void Pop(bool isShowParticle = true)
    {
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

        if (id == EID.HORIZONTAL || id == EID.VERTICAL || id == EID.X || id == EID.RHOMBUS || id == EID.COLOR_BOMB)
        {
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
        else
        {
            if (tile != null) tile.RegisterFloorPop();
            base.Pop(isShowParticle);
        }
    }

    public override void BombPop()
    {
        if (id == EID.HORIZONTAL || id == EID.VERTICAL || id == EID.X || id == EID.RHOMBUS || id == EID.COLOR_BOMB)
        {
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
        else
        {
            if (tile != null) tile.RegisterFloorPop();
            base.BombPop();
        }
    }

    public override void ActiveAbility()
    {
        if (id != EID.CHAMELEON) return;

        var ranColor = 0;
        var iter = true;

        while (iter)
        {
            ranColor = Random.Range(0, 5);
            if ((EColor) ranColor != color) iter = false;
        }

        chameleonAnimator.SetTrigger("Chameleon");
        color = (EColor) ranColor;
        //ChangeColorSprite.sprite = BlockManager.GetInstance.GetBlockSprite(EID.CHAMELEON, color, 1);
        nextColorSpirte.sprite = BlockManager.GetInstance.GetBlockSprite(EID.CHAMELEON, color, 1);
        ParticleManager.GetInstance.ShowParticle(EID.CHAMELEON, EColor.NONE, 0, transform.position);
    }

    public void ChameleonAnimEnd()
    {
        ChangeColorSprite.sprite = nextColorSpirte.sprite;
    }

    public void ChameleonAnimInit()
    {
        ChangeColorSprite.color = new Color(1, 1, 1, 1);
        nextColorSpirte.color = new Color(1, 1, 1, 0);
    }

    public override void Clear()
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
        BlockManager.GetInstance.RemoveBlockFromList(this);
        DynamicObjectPool.GetInstance.PoolObject(gameObject, true);
    }
    
}