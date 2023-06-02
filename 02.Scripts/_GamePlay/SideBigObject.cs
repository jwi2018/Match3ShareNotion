using System.Collections.Generic;
using UnityEngine;

public class SideBigObject : BigObject
{
    [SerializeField] private SpriteRenderer[] renderers;
    private readonly List<int> correctionValue = new List<int>();

    private bool isRemove;

    private void Update()
    {
        var isEnd = true;
        foreach (var item in subParts)
            if (item.gameObject.activeSelf)
                if (item.Damaged)
                    isEnd = false;
        if (isEnd && !IsEndAnim) IsEndAnim = true;


        if (isRemove) return;
        var isClear = true;
        foreach (var item in subParts)
            if (item.gameObject.activeSelf)
                isClear = false;
        if (isClear)
        {
            ParticleManager.GetInstance.ShowParticle(EID.BIG_SIDE, EColor.NONE, 0, transform.position);
            isRemove = true;
            Clear();
        }
    }

    public override void Setting(EColor color, Vector2Int matrix, Vector2Int _size)
    {
        base.Setting(color, matrix, _size);
        ID = EID.BIG_SIDE;
        size.x = 2;
        size.y = 2;

        foreach (var item in subParts) item.gameObject.SetActive(true);

        foreach (var item in renderers) correctionValue.Add(item.sortingOrder - (int) EDepth.NORMAL);
    }

    public override void SidePop(EColor color, EDirection direction, Vector2Int matrix)
    {
        base.SidePop(color, direction, matrix);
        if (isDamagedOnce) return;
        isDamagedOnce = true;

        Damaged();
    }

    public override void BombPop(Vector2Int matrix)
    {
        if (isDamagedOnce) return;
        isDamagedOnce = true;

        base.BombPop(matrix);
        Damaged();
    }

    public override void Damaged()
    {
        base.Damaged();


        GetComponent<Animator>().SetTrigger("Damage");

        var num = 0;
        var preHp = 0;
        foreach (var item in subParts)
            if (item.gameObject.activeSelf && !item.Damaged)
                preHp++;

        foreach (var item in subParts)
        {
            num++;
            if (item.gameObject.activeSelf && !item.Damaged)
            {
                item.animator.SetTrigger("Damage");
                item.Damaged = true;
                IsEndAnim = false;
                if (preHp > 4)
                    ParticleManager.GetInstance.ShowParticle(EID.BIG_SIDE, EColor.NONE, 1, transform.position);
                else
                    ParticleManager.GetInstance.ShowParticle(EID.BIG_SIDE, EColor.NONE, 2, transform.position);
                break;
            }
        }
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
        base.Clear();
        TileManager.GetInstance.BigClear(this);
        StageManager.GetInstance.CollectMission(EID.BIG_SIDE, EColor.NONE, transform);
        ParticleManager.GetInstance.ShowParticle(EID.BIG_SIDE, EColor.NONE, 0, transform.position);
        correctionValue.Clear();
    }
}