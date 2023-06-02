using UnityEngine;

public class TurnBox : ObstacleBlock
{
    // Color: NONE(disable), RED(able)
    [SerializeField] private Highlight highlight;

    public override void SetProperty(BlockProperty blockProperty)
    {
        base.SetProperty(blockProperty);
        if (color == EColor.RED)
        {
            property.popProperty.BombPop = true;
            property.popProperty.SidePop = true;
            animator.SetBool("Turn", true);
        }
        else if (color == EColor.NONE)
        {
            property.popProperty.BombPop = false;
            property.popProperty.SidePop = false;
            animator.SetBool("Turn", false);
        }

        if (highlight != null) highlight.Init();
    }

    public override void ActiveAbility()
    {
        var pro = new BlockProperty();
        pro = property;

        if (color == EColor.NONE)
        {
            Setting(EColor.RED, id, hp, etc);
            SetProperty(pro);
        }
        else if (color == EColor.RED)
        {
            Setting(EColor.NONE, id, hp, etc);
            SetProperty(pro);
        }
        //ApplySprite();
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        if (highlight != null) highlight.SetHighlight(value);
    }
}