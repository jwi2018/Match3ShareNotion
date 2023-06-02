using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardAdsGetItem : MonoBehaviour
{
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private float StartScale = 1.7f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve scaleCurve_End;
    [SerializeField] private float startSpeed = 0.01f;
    [SerializeField] private float addSpeed_1 = 0.004f;
    [SerializeField] private float addSpeed_2 = 0.0015f;
    [SerializeField] private AudioClip endSound;
    private EColor color = EColor.NONE;
    private Vector2 endPosition;


    private GameBlock targetBlock;
    private EID targetID = EID.NONE;

    public void StartAnim(Sprite sprite, Vector2 StartPosition, Vector2 EndPosition, float MoveTime = 0.5f)
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("CashBlockSelect");
        gameObject.SetActive(true);
        spriteRenderer.sprite = sprite;
        endPosition = EndPosition;
        moveTime = MoveTime;
        transform.position = StartPosition;
        MoveStart();
    }

    public void SetProperty(GameBlock _targetBlock, EID _targetID, EColor _color)
    {
        targetBlock = _targetBlock;
        targetID = _targetID;
        color = _color;
    }

    public void MoveStart()
    {
        StartCoroutine(NormalItemDropingBezier());
    }


    private IEnumerator NormalItemDropingBezier()
    {
        yield return new WaitForSeconds(0.1f);
        var TotalTime = 0.0f;
        var ScaleTime = 0.0f;
        ScaleTime = scaleCurve.keys[scaleCurve.length - 1].time;
        while (TotalTime < ScaleTime)
        {
            TotalTime += Time.deltaTime;
            transform.localScale = new Vector3(scaleCurve.Evaluate(TotalTime), scaleCurve.Evaluate(TotalTime), 1);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.4f);

        var Point_Start = new Vector2(0, 0);
        var Point_End = new Vector2(0, 0);

        Point_Start = transform.position;
        Point_End = endPosition;

        var Point_Middle = new Vector2(0, 0);

        var Vtemp = new Vector2(0, 0);

        Vtemp = Point_Start - (Point_Start - Point_End) * 0.5f;
        Point_Middle.x = Vtemp.x + (Point_Start.y - Point_End.y) * 0.5f * 1.712f;
        Point_Middle.y = Vtemp.y - (Point_Start.x - Point_End.x) * 0.5f * 1.712f;

        var CurvePoint = new List<Vector2>();
        var SizePoint = new List<float>();
        addSpeed_1 = 0.004f;
        for (var t = 0.0f; t <= 1.0f;)
        {
            var P1 = Lerp(Point_Start, Point_Middle, t);
            var P2 = Lerp(Point_Middle, Point_End, t);
            var Curve = Lerp(P1, P2, t);
            var sCurve = Lerp(StartScale, 1.2f, t);
            CurvePoint.Add(Curve);
            SizePoint.Add(sCurve);
            t += addSpeed_1;
            addSpeed_1 += addSpeed_2;
        }

        var WaitTime = moveTime * 0.016f;
        for (var i = 0; i < CurvePoint.Count; i++)
        {
            transform.position = CurvePoint[i];
            transform.localScale = new Vector2(SizePoint[i], SizePoint[i]);
            yield return new WaitForEndOfFrame();
        }

        transform.position = endPosition;
        transform.localScale = new Vector3(1.2f, 1.2f, 1);

        //yield return new WaitForSeconds(0.1f);
        TotalTime = 0.0f;
        ScaleTime = scaleCurve_End[scaleCurve_End.length - 1].time;
        var isEffect = false;
        while (TotalTime < ScaleTime)
        {
            yield return new WaitForEndOfFrame();
            TotalTime += Time.deltaTime;
            transform.localScale =
                new Vector3(scaleCurve_End.Evaluate(TotalTime), scaleCurve_End.Evaluate(TotalTime), 1);
        }

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("BlockCreateItem");
        
        
        //targetBlock.Setting(EColor.NONE, targetID);
        //targetBlock.Setting(targetBlock.Color, targetID);
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy") || BaseSystem.GetInstance.GetSystemList("Castle"))
            {
                targetBlock.Setting(targetBlock.Color, targetID);
            }
            else
            {
                targetBlock.Setting(EColor.NONE, targetID);
            }
        }
        else
        {
            targetBlock.Setting(EColor.NONE, targetID);
        }
        
        targetBlock.ApplySprite();
        if (targetBlock.Tile != null)
        {
            var particle = DynamicObjectPool.GetInstance.GetObjectForType("Particle_ChangeItem", false);
            particle.transform.position = targetBlock.Tile.GetPosition();
        }

        gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

    public Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return (1f - t) * a + t * b;
    }

    public float Lerp(float a, float b, float t)
    {
        return (1f - t) * a + t * b;
    }
}