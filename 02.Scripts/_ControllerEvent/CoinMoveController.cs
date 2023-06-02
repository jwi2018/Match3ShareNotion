using System.Collections;
using UnityEngine;

public class CoinMoveController : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve_rotation;

    public void StartCurved()
    {
        StartCoroutine(GetCoinRotate());
    }

    private IEnumerator GetCoinRotate()
    {
        var Value0_R = new Keyframe();
        Value0_R.time = 0.0f;
        Value0_R.value = 0.0f;
        var Value1_R = new Keyframe();
        Value1_R.time = 0.15f;
        Value1_R.value = Random.Range(-25, 25);
        var Value2_R = new Keyframe();
        Value2_R.time = 0.3f;
        Value2_R.value = Random.Range(-25, 25);
        var Value3_R = new Keyframe();
        Value3_R.time = 0.45f;
        Value3_R.value = Random.Range(-25, 25);
        var Value4_R = new Keyframe();
        Value4_R.time = 0.6f;
        Value4_R.value = Random.Range(-25, 25);

        _curve_rotation.AddKey(Value0_R);
        _curve_rotation.AddKey(Value1_R);
        _curve_rotation.AddKey(Value2_R);
        _curve_rotation.AddKey(Value3_R);
        _curve_rotation.AddKey(Value4_R);

        var TotalTime = 0.0f;
        while (TotalTime < Value4_R.time)
        {
            TotalTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }
}