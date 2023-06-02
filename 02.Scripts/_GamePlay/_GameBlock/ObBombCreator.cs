using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObBombCreator : ObstacleBlock
{
    [SerializeField] private SpriteRenderer mainRenderer;
    [SerializeField] private Highlight highlight;
    [SerializeField] private List<Material> LightningMat = new List<Material>();
    [SerializeField] private float DelayTime = 0.07f;


    private bool isActive;
    private float lineTimer;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isActive)
        {
            lineTimer += Time.deltaTime;
            if (lineTimer > DelayTime)
            {
                lineTimer = 0f;
                foreach (var item in LightningMat)
                {
                    var offset = item.GetTextureOffset("_MainTex");
                    offset.y += 0.167f;
                    if (offset.y >= 1) offset.y = 0;
                    item.SetTextureOffset("_MainTex", offset);
                }
            }
        }
    }

    public override void Init()
    {
        base.Init();
        blockRenderer = mainRenderer;
    }

    public override void ActiveAbility()
    {
        if (!isActive)
        {
            AnimationManager.RainbowCount++;
            StartCoroutine(AbilityCoroutine());
        }
    }

    private IEnumerator AbilityCoroutine()
    {
        isActive = true;
        var creatorLines = new List<GameObject>();
        var blocks = new List<GameBlock>();

        blocks.Add(BlockManager.GetInstance.GetRandomNormalBlockOrNull());
        blocks.Add(BlockManager.GetInstance.GetRandomNormalBlockOrNull());
        blocks.Add(BlockManager.GetInstance.GetRandomNormalBlockOrNull());
        animator.SetTrigger("Active");
        yield return new WaitForSeconds(0.2f);

        foreach (var block in blocks)
        {
            if (block == null) continue;
            if (block.Tile == null) continue;

            var rainbowLine = DynamicObjectPool.GetInstance.GetObjectForType("Rainbow_LineRenderer", false);
            if (rainbowLine != null)
            {
                creatorLines.Add(rainbowLine);

                rainbowLine.transform.SetParent(transform);
                rainbowLine.transform.localPosition = Vector3.zero;
                rainbowLine.transform.rotation = Quaternion.Euler(0, 0, 0);
                RainbowLightningCount++;

                var line = rainbowLine.GetComponent<LightningLine>();
                line.SetBossBlock(this, block, EID.VERTICAL);
                line.RendererSetting(transform, block.gameObject.transform, 0.42f);
            }

            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitUntil(() => RainbowLightningCount == 0);
        yield return new WaitForSeconds(0.2f);

        foreach (var line in creatorLines) DynamicObjectPool.GetInstance.PoolObject(line, false);
        if (tile != null)
        {
            //tile.RegisterFloorPop();
            //tile.RemoveBlock(this);
        }

        hp = 4;
        ApplySprite();
        isActive = false;
        //Remove();
        //Clear();
        AnimationManager.RainbowCount--;
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        if (hp == 1)
        {
            if (tile != null) ActiveAbility();
        }
        else if (hp > 1)
        {
            base.SidePop(_color, _direction);
        }
    }

    public override void BombPop()
    {
        if (hp == 1)
        {
            if (tile != null) ActiveAbility();
        }
        else if (hp > 1)
        {
            base.BombPop();
        }
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        if (highlight != null) highlight.SetHighlight(value);
    }

    public override void Clear()
    {
        isActive = false;
        base.Clear();
    }
}