using UnityEngine;

public class Actiniaria : ObstacleBlock
{
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private AudioClip damageSound;

    public override void Init()
    {
        base.Init();
        blockRenderer = spriteRender;
    }

    public override void SidePop(EColor _color, EDirection _direction)
    {
        ParticleManager.GetInstance.ShowParticle(ID, Color, 0, transform.position);
        animator.SetTrigger("Damage");
        StageManager.GetInstance.CollectMission(EID.ACTINIARIA, EColor.NONE, transform);
        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.Play("ActiniaDamage");
    }

    public override void BombPop()
    {
        if (IsBombed) return;
        IsBombed = true;
        ParticleManager.GetInstance.ShowParticle(ID, Color, 0, transform.position);
        animator.SetTrigger("Damage");
        StageManager.GetInstance.CollectMission(EID.ACTINIARIA, EColor.NONE, transform);
        if (SoundManager.GetInstance != null)
            SoundManager.GetInstance.Play("ActiniaDamage");
    }
}