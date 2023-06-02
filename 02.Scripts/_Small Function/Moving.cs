using System;
using System.Collections;
using UnityEngine;

public class Moving : MonoBehaviour
{
    [SerializeField] private AnimationCurve _MoveningPosition_X;

    [SerializeField] private AnimationCurve _MoveningPosition_Y;

    [SerializeField] private Transform _StartTransform;

    [SerializeField] private Transform _EndTransform;

    [SerializeField] private float MoveTime = 0.4f;

    private Action endAction;

    public bool IsMoving { get; private set; }

    public void StartAnim(Transform StartTransform, Transform EndTransform, Action action, float _MovingTime = 0.4f)
    {
        if (SoundManager.GetInstance != null && !StageManager.GetInstance.IsSkipOn)
            SoundManager.GetInstance.Play("RemainMoveFireworkShoot");
        _StartTransform = StartTransform;
        _EndTransform = EndTransform;
        MoveTime = _MovingTime;
        endAction = action;
        StartCoroutine(MoveningAnim());
    }

    private IEnumerator MoveningAnim()
    {
        var XPositionCurved = new AnimationCurve();
        var YPositionCurved = new AnimationCurve();

        var Value0_X = new Keyframe();
        Value0_X.time = 0.0f;
        Value0_X.value = _StartTransform.position.x;
        var Value0_Y = new Keyframe();
        Value0_Y.time = 0.0f;
        Value0_Y.value = _StartTransform.position.y;
        var Value1_X = new Keyframe();
        Value1_X.time = MoveTime;
        Value1_X.value = _EndTransform.position.x;
        var Value1_Y = new Keyframe();
        Value1_Y.time = MoveTime;
        Value1_Y.value = _EndTransform.position.y;

        XPositionCurved.AddKey(Value0_X);
        YPositionCurved.AddKey(Value0_Y);
        XPositionCurved.AddKey(Value1_X);
        YPositionCurved.AddKey(Value1_Y);

        _MoveningPosition_X.keys = XPositionCurved.keys;
        _MoveningPosition_Y.keys = YPositionCurved.keys;

        var TotalTime = 0.0f;
        while (TotalTime < MoveTime)
        {
            TotalTime += Time.deltaTime;
            transform.position = new Vector3(_MoveningPosition_X.Evaluate(TotalTime),
                _MoveningPosition_Y.Evaluate(TotalTime), 0);
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(_EndTransform.position.x, _EndTransform.position.y, 0);
        yield return new WaitForEndOfFrame();

        endAction();
    }
}