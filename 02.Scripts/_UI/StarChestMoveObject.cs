using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChestMoveObject : MonoBehaviour
{
    [SerializeField]
    private StarChestPopup starchestPopup = null;

    [SerializeField]
    private Image image = null;

    [SerializeField]
    private Transform startTransform = null;

    [SerializeField]
    private Transform endTransform = null;

    [SerializeField]
    private AnimationCurve scaleCurve_0 = null;

    [SerializeField]
    private AnimationCurve xPositionCurved_0 = new AnimationCurve();

    [SerializeField]
    private AnimationCurve yPositionCurved_0 = new AnimationCurve();

    [SerializeField]
    private float moveTime = 0.5f;

    public void StartMove(Sprite sprite, int type = 0)
    {
        image.sprite = sprite;
        if (type == 1)
        {
            this.transform.Rotate(new Vector3(0, 0, -45));
        }
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        Vector3 StartPosition = startTransform.position;
        Vector3 EndPosition = endTransform.position;

        Keyframe Value0_X = new Keyframe();
        Value0_X.time = 0.0f;
        Value0_X.value = StartPosition.x;
        Keyframe Value0_Y = new Keyframe();
        Value0_Y.time = 0.0f;
        Value0_Y.value = StartPosition.y;

        Keyframe Value1_X = new Keyframe();
        Value1_X.time = moveTime;
        Value1_X.value = EndPosition.x;
        Keyframe Value1_Y = new Keyframe();
        Value1_Y.time = moveTime;
        Value1_Y.value = EndPosition.y;

        xPositionCurved_0.AddKey(Value0_X);
        yPositionCurved_0.AddKey(Value0_Y);
        xPositionCurved_0.AddKey(Value1_X);
        yPositionCurved_0.AddKey(Value1_Y);
        float TotalTime = 0.0f;
        SoundManager.GetInstance.Play("GetCoin");
        starchestPopup.EndMoveAnimation();
        while (TotalTime < moveTime)
        {
            TotalTime += Time.deltaTime;
            this.transform.position = new Vector3(xPositionCurved_0.Evaluate(TotalTime), yPositionCurved_0.Evaluate(TotalTime), 0);
            this.transform.localScale = new Vector3(scaleCurve_0.Evaluate(TotalTime), scaleCurve_0.Evaluate(TotalTime), 1);
            yield return new WaitForEndOfFrame();
        }
        this.transform.position = new Vector3(xPositionCurved_0.Evaluate(moveTime), yPositionCurved_0.Evaluate(moveTime));
        this.transform.localScale = new Vector3(scaleCurve_0.Evaluate(moveTime), scaleCurve_0.Evaluate(moveTime), 1);

        yield return new WaitForEndOfFrame();
    }
}