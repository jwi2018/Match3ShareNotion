using UnityEngine;

public class DoubleBlock : ObstacleBlock
{
    [SerializeField] private SpriteRenderer doubleRenderer;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroyObjSound;
    [SerializeField] private AudioClip recoverySound;

    [SerializeField] private Highlight highlight;

    private bool isDamage;

    public override void Init()
    {
        base.Init();
        blockRenderer = doubleRenderer;

        if (highlight != null) highlight.Init();
    }

    public override void Attacked(int damage = 1, bool isShowParticle = true)
    {
        if (SoundManager.GetInstance != null)
        {
            if (hp == 2)
                SoundManager.GetInstance.Play("DoubleBlockHit");
            else if (hp == 1) SoundManager.GetInstance.Play("DoubleBlockDestroy");
        }

        base.Attacked(damage, isShowParticle);
        animator.SetTrigger("Damage");
        isDamage = true;
    }

    public override void ActiveAbility()
    {
        base.ActiveAbility();
        if (isDamage)
        {
            isDamage = false;
        }
        else
        {
            if (hp == 1)
            {
                hp++;
                animator.SetTrigger("Recovery");
                if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DoubleBlockRecovery");
            }
        }
    }

    public override void SetHighlight(bool value)
    {
        base.SetHighlight(value);

        if (highlight != null) highlight.SetHighlight(value);
    }
}