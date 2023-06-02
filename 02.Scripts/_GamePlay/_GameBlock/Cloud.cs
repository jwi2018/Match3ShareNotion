using UnityEngine;

public class Cloud : ObstacleBlock
{
    [SerializeField] private SpriteRenderer cloudRender;

    public override void Init()
    {
        base.Init();
        blockRenderer = cloudRender;
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);
        if (value)
        {
            cloudRender.sortingOrder = (int) EDepth.TOP + 1000;
            cloudRender.maskInteraction = SpriteMaskInteraction.None;
            //cloudRender.gameObject.layer = 5;
        }
        else
        {
            cloudRender.sortingOrder = (int) EDepth.TOP;
            cloudRender.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            //cloudRender.gameObject.layer = 0;
        }
    }
}