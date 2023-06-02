using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pop_GetRewardEntity : MonoBehaviour
{
    [SerializeField] private AnimationCurve Curve_X;

    [SerializeField] private AnimationCurve Curve_Y;

    [SerializeField] private float MoveTime = 0.8f;

    [SerializeField] private Image _mImage;

    [SerializeField] private Text _mText;

    [SerializeField] private GameObject _coinBase;

    [SerializeField] private GameObject getItemParticle;

    Vector3 vecTargetPosition = Vector3.zero;

    [SerializeField] string strEndEffectName = "";
    string strEndSound = "";

    public void StartReward(Vector3 TargetPosition, string _strEndEffectName, string _endSound, float ExceptionSize = 1.0f)
    {
        strEndEffectName = _strEndEffectName;
        strEndSound = _endSound;
        _mImage.transform.DOShakeRotation(1, new Vector3(0, 0, 40)).OnComplete(() =>
        {
            StartMove(TargetPosition, ExceptionSize);
        });
    }

    public void SetData(Sprite sprite)
    {
        _mImage.sprite = sprite;
    }

    public void StartMove(Vector3 TargetPosition, float ExceptionSize = 1.0f)
    {
        vecTargetPosition = TargetPosition;

        //_mImage.SetNativeSize();
        var Min = Mathf.Min(200.0f * ExceptionSize / _mImage.GetComponent<RectTransform>().sizeDelta.x,
            200.0f * ExceptionSize / _mImage.GetComponent<RectTransform>().sizeDelta.y);
        var Sizex = _mImage.GetComponent<RectTransform>().sizeDelta.x * Min;
        var Sizey = _mImage.GetComponent<RectTransform>().sizeDelta.y * Min;
        _mImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Sizex, Sizey);
        _mImage.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        _mImage.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 0);
        

        StartCoroutine(MoveAnimation(transform.position));
    }

    public void CoinCreate()
    {
        for (var i = 0; i < 5; i++)
        {
            var obj = Instantiate(_coinBase, _mImage.transform);
            obj.SetActive(true);
            obj.GetComponent<CoinMoveController>().StartCurved();
        }
    }

    public void EndAnim()
    {
        GameObject gobEnd = PrefabRegister.GetInstance.GetPrefab(strEndEffectName, transform.parent);
        gobEnd.transform.position = transform.position;
        SoundManager.GetInstance.Play(strEndSound);


        Destroy(gameObject);

        //gameObject.SetActive(false);
    }

    public void ItemClose(float Time = 1.0f)
    {
        StartCoroutine(DelayClose(Time));
    }

    private IEnumerator MoveAnimation(Vector3 StartPosition)
    {
        var XPositionCurved = new AnimationCurve();
        var YPositionCurved = new AnimationCurve();

        var EndPosition = vecTargetPosition;

        var Value0_X = new Keyframe();
        Value0_X.time = 0.0f;
        Value0_X.value = StartPosition.x;
        var Value0_Y = new Keyframe();
        Value0_Y.time = 0.0f;
        Value0_Y.value = StartPosition.y;

        if (StartPosition.x > EndPosition.x) Value0_X.outTangent = -6f;
        else Value0_X.outTangent = 6f;

        var Value2_X = new Keyframe();
        Value2_X.time = MoveTime;
        Value2_X.value = EndPosition.x;
        var Value2_Y = new Keyframe();
        Value2_Y.time = MoveTime;
        Value2_Y.value = EndPosition.y;

        if (StartPosition.x > EndPosition.x) Value2_X.inTangent = 2f;
        else Value2_X.inTangent = -2f;

        Value0_Y.outTangent = 5.0f;

        XPositionCurved.AddKey(Value0_X);
        YPositionCurved.AddKey(Value0_Y);
        XPositionCurved.AddKey(Value2_X);
        YPositionCurved.AddKey(Value2_Y);

        Curve_X.keys = XPositionCurved.keys;
        Curve_Y.keys = YPositionCurved.keys;

        var TotalTime = 0.0f;
        transform.position = new Vector3(Curve_X.Evaluate(TotalTime), Curve_Y.Evaluate(TotalTime), 0);
        while (TotalTime < MoveTime + 0.1f)
        {
            TotalTime += Time.deltaTime;
            if (TotalTime > 0.1f)
                transform.position = new Vector3(Curve_X.Evaluate(TotalTime - 0.1f),
                    Curve_Y.Evaluate(TotalTime - 0.1f), 0);
            yield return new WaitForEndOfFrame();
        }

        //GameObject gob = PrefabRegister.GetInstance.GetPrefab("Particle - Butterfly bomb_blue");
        transform.position = new Vector3(EndPosition.x, EndPosition.y, 0);
        EndAnim();
        yield return new WaitForSeconds(0.75f);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DelayClose(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().SetTrigger("Off");
        if (getItemParticle != null) getItemParticle.SetActive(true);
    }
}