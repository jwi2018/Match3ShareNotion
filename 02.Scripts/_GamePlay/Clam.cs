using UnityEngine;

public class Clam : ObstacleBlock
{
    [SerializeField] private SpriteRenderer clamRender;

    public override void Init()
    {
        base.Init();
        blockRenderer = clamRender;
        clamRender.sortingOrder = (int) EDepth.NORMAL + 1;
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        base.SidePop(_color, _direction);

        if (hp == 1)
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClamOpen");
            animator.SetTrigger("Damage");
        }
    }

    public override void BombPop()
    {
        base.BombPop();
        if (hp == 1)
        {
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClamOpen");
            animator.SetTrigger("Damage");
        }
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);
        if (value)
        {
            clamRender.sortingOrder = (int) EDepth.NORMAL + 1000 + 1;
            clamRender.maskInteraction = SpriteMaskInteraction.None;
            clamRender.gameObject.layer = 5;
        }
        else
        {
            clamRender.sortingOrder = (int) EDepth.NORMAL + 1;
            clamRender.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            clamRender.gameObject.layer = 0;
        }
    }
}