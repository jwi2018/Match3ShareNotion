using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemRooting : MonoBehaviour
{
    [SerializeField] private Image _mImage;

    [SerializeField] private AnimationCurve _ItemRootingPosition_X;

    [SerializeField] private AnimationCurve _ItemRootingPosition_Y;

    [SerializeField] private AnimationCurve _ItemRootingScale;

    [SerializeField] private Vector2 _StartPosition;

    [SerializeField] private Vector2 _EndPosition;

    [SerializeField] private AnimationListener _listener;

    [SerializeField] private float _moveTime = 0.66f;

    [SerializeField] private ParticleSystem particle_Ob;

    private int _index;
    private bool _playSound;

    public void StartAnim(Sprite sprite, Vector2 StartPosition, Vector2 EndPosition, int index,
        int Type = 0, bool Sound = true, float MoveTime = 0.66f)
    {
        _mImage.sprite = sprite;
        _mImage.SetNativeSize();
        _StartPosition = StartPosition;
        _EndPosition = EndPosition;
        _moveTime = MoveTime;
        _playSound = Sound;
        transform.position = StartPosition;
        if (Type == 0 && MoveTime < 1.0f) _moveTime = 1.0f;
        if (Type == 0) GetComponent<Animator>().SetTrigger("Normal");
        else GetComponent<Animator>().SetTrigger("Oscillate");
        _index = index;
    }

    public void AcornAnim(Sprite sprite, Vector2 position)
    {
        particle_Ob.gameObject.SetActive(false);
        _mImage.sprite = sprite;
        _mImage.SetNativeSize();
        _StartPosition = position;
        transform.position = position;
        GetComponent<Animator>().SetTrigger("Acorn");
    }

    public void End()
    {
       Destroy(gameObject);
    }

    public void MovingStart()
    {
        StartCoroutine(ItemRooting());
    }

    public void MovingEnd()
    {
        StageManager.CollectAnimCount--;
        StageManager.GetInstance.CollectMovingMissionItem(_index);
        StageManager.GetInstance.SetUI();
    }

    private IEnumerator ItemRooting()
    {
        var XPositionCurved = new AnimationCurve();
        var YPositionCurved = new AnimationCurve();

        var Value0_X = new Keyframe();
        Value0_X.time = 0.0f;
        Value0_X.value = _StartPosition.x;
        var Value0_Y = new Keyframe();
        Value0_Y.time = 0.0f;
        Value0_Y.value = _StartPosition.y;

        if (_StartPosition.x > _EndPosition.x)
            Value0_X.outTangent = -6f;
        else
            Value0_X.outTangent = 6f;

        var Value1_Y = new Keyframe();
        Value1_Y.time = _moveTime * 0.25f;
        Value1_Y.value = _StartPosition.y + (_StartPosition.y - _EndPosition.y) * 0.15f;

        var Value2_X = new Keyframe();
        Value2_X.time = _moveTime;
        Value2_X.value = _EndPosition.x;
        var Value2_Y = new Keyframe();
        Value2_Y.time = _moveTime;
        Value2_Y.value = _EndPosition.y;

        if (_StartPosition.x > _EndPosition.x)
            Value2_X.inTangent = 2f;
        else
            Value2_X.inTangent = -2f;


        XPositionCurved.AddKey(Value0_X);
        YPositionCurved.AddKey(Value0_Y);
        YPositionCurved.AddKey(Value1_Y);
        XPositionCurved.AddKey(Value2_X);
        YPositionCurved.AddKey(Value2_Y);

        _ItemRootingPosition_X.keys = XPositionCurved.keys;
        _ItemRootingPosition_Y.keys = YPositionCurved.keys;


        var TotalTime = 0.0f;
        while (TotalTime < _moveTime)
        {
            TotalTime += Time.deltaTime;
            transform.position = new Vector3(_ItemRootingPosition_X.Evaluate(TotalTime),
                _ItemRootingPosition_Y.Evaluate(TotalTime), 0);
            transform.localScale = new Vector3(_ItemRootingScale.Evaluate(TotalTime),
                _ItemRootingScale.Evaluate(TotalTime), 1);
            _mImage.transform.localScale = new Vector3(_ItemRootingScale.Evaluate(TotalTime),
                _ItemRootingScale.Evaluate(TotalTime), 1);
            yield return new WaitForEndOfFrame();
        }

        transform.position = new Vector3(_EndPosition.x, _EndPosition.y, 0);
        GetComponent<Animator>().SetTrigger("End");
        if (_playSound) _listener.Play(63);
    }
}