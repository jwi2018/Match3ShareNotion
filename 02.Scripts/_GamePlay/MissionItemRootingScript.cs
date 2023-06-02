using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionItemRootingScript : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Transform EndPosition;

    [SerializeField] private Animator _animator;

    [SerializeField] private float MoveTime;

    private Vector2 Point_End;
    private Vector2 Point_First;
    private Vector2 Point_Second;

    private Vector2 Point_Start;

    public Transform SetEndPosition
    {
        set => EndPosition = value;
    }

    public float SetMoveTime
    {
        set => MoveTime = value;
    }


    /*private void Start()
    {
        StartCoroutine(RandomPosition(this.transform.position, EndPosition.position));
    }*/

    public void Setting(Vector2 startPosition, float moveTime = 1.0f)
    {
        _rectTransform.anchoredPosition = startPosition;
        MoveTime = moveTime;
    }

    public void StartNormalDroping()
    {
        _animator.SetTrigger("Normal");
    }

    public void OscillateItemDroping()
    {
        _animator.SetTrigger("Oscillate");
    }

    public void GoldRooting(Vector2 StartPosition, Vector2 GoldAndPosition)
    {
        StartCoroutine(RandomPosition(StartPosition, GoldAndPosition));
    }

    private IEnumerator NormalItemDropingBezier()
    {
        Point_Start = transform.position;
        Point_End = EndPosition.position;
        //Vector2 Point_Middle = (this.transform.position - EndPosition.position)*0.5f;
        //Point_Middle.y *= -1;
        if (transform.position.x < EndPosition.position.x)
        {
            Point_First = new Vector2(-1, -3);
            Point_Second = new Vector2(1, -3);
        }
        else
        {
            Point_First = new Vector2(1, -3);
            Point_Second = new Vector2(-1, -3);
        }

        #region -----CurvePointSet-----

        var CurvePoint = new List<Vector2>();

        for (var t = 0.0f; t <= 1.0f; t += 0.016f)
        {
            var P1 = Lerp(Point_Start, Point_First, t);
            var P2 = Lerp(Point_First, Point_Second, t);
            var P3 = Lerp(Point_Second, Point_End, t);
            var CurvePoint1 = Lerp(P1, P2, t);
            var CurvePoint2 = Lerp(P2, P3, t);
            var Curve = Lerp(CurvePoint1, CurvePoint2, t);
            CurvePoint.Add(Curve);
        }

        #endregion

        var WaitTime = MoveTime * 0.016f;
        for (var i = 0; i < CurvePoint.Count; i++)
        {
            transform.position = CurvePoint[i];
            yield return new WaitForSeconds(WaitTime);
        }

        transform.position = EndPosition.position;
        yield return new WaitForEndOfFrame();
        _animator.SetTrigger("End");
    }

    private IEnumerator OscillateItemDropingBezier()
    {
        Point_Start = transform.position;
        Point_End = EndPosition.position;
        //Vector2 Point_Middle = (this.transform.position - EndPosition.position)*0.5f;
        //Point_Middle.y *= -1;
        if (transform.position.x < EndPosition.position.x)
        {
            Point_First = new Vector2(-1, -3);
            Point_Second = new Vector2(1, -3);
        }
        else
        {
            Point_First = new Vector2(1, -3);
            Point_Second = new Vector2(-1, -3);
        }

        #region -----CurvePointSet-----

        var CurvePoint = new List<Vector2>();

        for (var t = 0.0f; t <= 1.0f; t += 0.016f)
        {
            var P1 = Lerp(Point_Start, Point_First, t);
            var P2 = Lerp(Point_First, Point_Second, t);
            var P3 = Lerp(Point_Second, Point_End, t);
            var CurvePoint1 = Lerp(P1, P2, t);
            var CurvePoint2 = Lerp(P2, P3, t);
            var Curve = Lerp(CurvePoint1, CurvePoint2, t);
            CurvePoint.Add(Curve);
        }

        #endregion

        var WaitTime = MoveTime * 0.016f;
        for (var i = 0; i < CurvePoint.Count; i++)
        {
            transform.position = CurvePoint[i];
            yield return new WaitForSeconds(WaitTime / 3);
        }

        transform.position = EndPosition.position;
        _animator.SetTrigger("End");
        yield return new WaitForEndOfFrame();
    }

    private Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return (1f - t) * a + t * b;
    }

    private IEnumerator RandomPosition(Vector2 StartPosition, Vector2 GoldAndPosition)
    {
        Point_Start = transform.position;
        var Xpos = Random.Range(-1f, 1f);
        var Ypos = Random.Range(-1f, 1f);
        Point_End = Point_Start - new Vector2(Xpos, Ypos);

        var CurvePoint = new List<Vector2>();

        for (var t = 0.0f; t <= 1.0f; t += 0.016f)
        {
            var P1 = Lerp(Point_Start, Point_End, t);
            CurvePoint.Add(P1);
        }

        var WaitTime = MoveTime * 0.016f * 0.2f;
        for (var i = 0; i < CurvePoint.Count; i++)
        {
            transform.position = CurvePoint[i];
            yield return new WaitForSeconds(WaitTime);
        }

        transform.position = CurvePoint[CurvePoint.Count - 1];

        StartCoroutine(RewardAnimation(StartPosition, GoldAndPosition));
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator RewardAnimation(Vector2 StartPosition, Vector2 GoldAndPosition)
    {
        Point_Start = transform.position;
        Point_End = GoldAndPosition;
        var Point_Middle = (Vector2) (transform.position - EndPosition.position) * 0.5f;
        if (Point_Start.y - StartPosition.y > 0) Point_Middle.y *= -1;

        Point_Middle.y *= -1;

        #region -----CurvePointSet-----

        var CurvePoint = new List<Vector2>();

        for (var t = 0.0f; t <= 1.0f; t += 0.016f)
        {
            var P1 = Lerp(Point_Start, Point_Middle, t);
            var P2 = Lerp(Point_Middle, Point_End, t);
            var Curve = Lerp(P1, P2, t);
            CurvePoint.Add(Curve);
        }

        #endregion

        var WaitTime = MoveTime * 0.016f * 0.5f;
        for (var i = 0; i < CurvePoint.Count; i++)
        {
            transform.position = CurvePoint[i];
            yield return new WaitForSeconds(WaitTime);
        }

        transform.position = EndPosition.position;
        yield return new WaitForEndOfFrame();
        //_animator.SetTrigger("End");
    }

    public void MovingEnd()
    {
        //StageManager.GetInstance.ApplyCollectItem(gameObject.GetComponent<CollectItem>()._missionKind);
        //DynamicObjectPool.GetInstance.PoolObject(gameObject, false);
    }
}