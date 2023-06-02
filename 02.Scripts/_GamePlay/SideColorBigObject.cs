using System.Collections.Generic;
using UnityEngine;

public class SideColorBigObject : BigObject
{
    [SerializeField] private SpriteRenderer[] renderers;
    private readonly List<int> correctionValue = new List<int>();

    public override void Setting(EColor color, Vector2Int matrix, Vector2Int _size)
    {
        base.Setting(color, matrix, _size);
        ID = EID.BIG_SIDE_COLOR;
        size.x = 2;
        size.y = 2;
        if (color != EColor.NONE)
        {
            foreach (var item in subParts)
            {
                item.gameObject.SetActive(true);
                item.Color = color;
                item.SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(color));
            }
        }
        else
        {
            foreach (var item in subParts) item.gameObject.SetActive(true);
            subParts[0].Color = EColor.RED;
            subParts[0].SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(EColor.RED));

            subParts[1].Color = EColor.YELLOW;
            subParts[1].SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(EColor.YELLOW));

            subParts[2].Color = EColor.GREEN;
            subParts[2].SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(EColor.GREEN));

            subParts[3].Color = EColor.BLUE;
            subParts[3].SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(EColor.BLUE));

            subParts[4].Color = EColor.PURPLE;
            subParts[4].SetSprite(BlockManager.GetInstance.GetBigSideColorSprite(EColor.PURPLE));
        }

        foreach (var item in renderers) correctionValue.Add(item.sortingOrder - (int) EDepth.NORMAL);
    }

    public override void SidePop(EColor color, EDirection direction, Vector2Int matrix)
    {
        base.SidePop(color, direction, matrix);
        if (isDamagedOnce) return;

        foreach (var item in subParts)
            if (item.Color == color && item.gameObject.activeSelf)
            {
                StageManager.GetInstance.CollectMission(EID.BIG_SIDE_COLOR, item.Color, transform);
                item.gameObject.SetActive(false);
                isDamagedOnce = true;
                break;
            }

        if (isDamagedOnce)
            Damaged();
    }

    public override void BombPop(Vector2Int matrix)
    {
        base.BombPop(matrix);
        if (isDamagedOnce) return;

        foreach (var item in subParts)
            if (item.gameObject.activeSelf)
            {
                StageManager.GetInstance.CollectMission(EID.BIG_SIDE_COLOR, item.Color, transform);
                item.gameObject.SetActive(false);
                isDamagedOnce = true;
                break;
            }

        Damaged();
    }

    public override void Damaged()
    {
        base.Damaged();
        var isClear = true;
        GetComponent<Animator>().SetTrigger("Damage");
        foreach (var item in subParts)
            if (item.gameObject.activeSelf)
                isClear = false;

        if (isClear) Clear();
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        for (var i = 0; i < renderers.Length; i++)
            if (value)
            {
                renderers[i].sortingOrder = (int) EDepth.NORMAL + correctionValue[i] + 1000;
                renderers[i].maskInteraction = SpriteMaskInteraction.None;
                renderers[i].gameObject.layer = 5;
            }
            else
            {
                renderers[i].sortingOrder = (int) EDepth.NORMAL + correctionValue[i];
                renderers[i].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                renderers[i].gameObject.layer = 0;
            }
    }

    public override void Clear()
    {
        ParticleManager.GetInstance.ShowParticle(EID.BIG_SIDE_COLOR, EColor.NONE, 0, transform.position);
        base.Clear();
        TileManager.GetInstance.BigClear(this);
    }
}