using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingCoinAnim : MonoBehaviour
{
    [SerializeField] private AnimationCurve _ItemRootingPosition_X;
    [SerializeField] private AnimationCurve _ItemRootingPosition_Y;
    [SerializeField] private AnimationCurve _ItemRootingScale;

    [SerializeField] private SavingBoxStatus savingBox = null;

    private Vector2 startPosition = new Vector2(0, 0);
    private Vector2 endPosition = new Vector2(0, 0);
    private float moveTime = 0.4f;

    public void StartAnim(Vector2 StartPosition, Vector2 EndPosition)
    {
        startPosition = StartPosition;
        endPosition = EndPosition;
        this.transform.position = StartPosition;
        StartCoroutine(CoinMove());
    }

    IEnumerator CoinMove()
    {
        
        AnimationCurve XPositionCurved = new AnimationCurve();
        AnimationCurve YPositionCurved = new AnimationCurve();

        Keyframe Value0_X = new Keyframe();
        Value0_X.time = 0.0f;
        Value0_X.value = startPosition.x;
        Keyframe Value0_Y = new Keyframe();
        Value0_Y.time = 0.0f;
        Value0_Y.value = startPosition.y;

        if (startPosition.x > endPosition.x)
        {
            Value0_X.outTangent = -6f;
        }
        else
        {
            Value0_X.outTangent = 6f;
        }
        Value0_Y.outTangent = 6f;

        //Keyframe Value1_Y = new Keyframe();
        //Value1_Y.time = _moveTime * 0.25f;
        //Value1_Y.value = _mImage.transform.position.y + ((_StartPosition.y - _EndPosition.y) * 0.15f);

        Keyframe Value2_X = new Keyframe();
        Value2_X.time = moveTime;
        Value2_X.value = endPosition.x;
        Keyframe Value2_Y = new Keyframe();
        Value2_Y.time = moveTime;
        Value2_Y.value = endPosition.y;

        if (startPosition.x > endPosition.x)
        {
            Value2_X.inTangent = 2f;
        }
        else
        {
            Value2_X.inTangent = -2f;
        }


        XPositionCurved.AddKey(Value0_X);
        YPositionCurved.AddKey(Value0_Y);
        //YPositionCurved.AddKey(Value1_Y);
        XPositionCurved.AddKey(Value2_X);
        YPositionCurved.AddKey(Value2_Y);

        _ItemRootingPosition_X.keys = XPositionCurved.keys;
        _ItemRootingPosition_Y.keys = YPositionCurved.keys;

        float TotalTime = 0.0f;
        while (TotalTime < moveTime)
        {
            TotalTime += Time.deltaTime;
            this.transform.position = new Vector3(_ItemRootingPosition_X.Evaluate(TotalTime), _ItemRootingPosition_Y.Evaluate(TotalTime), 0);
            this.transform.localScale = new Vector3(_ItemRootingScale.Evaluate(TotalTime), _ItemRootingScale.Evaluate(TotalTime), 1);
            yield return new WaitForEndOfFrame();
        }
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play(SoundManager.GetInstance.savingCoin);
        }
        savingBox.ShowParticle();
        this.transform.position = new Vector3(endPosition.x, endPosition.y, 0);
        this.gameObject.SetActive(false);
        //this.GetComponent<Animator>().SetTrigger("End");
    }
}
