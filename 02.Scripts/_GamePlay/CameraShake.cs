using System.Collections;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    [SerializeField] private GameObject ShakingObj;

    private Vector2 _defalutPosition;
    private float _rangeX;
    private float _rangeY;
    private float _strong;

    private float _time;

    public float SetTime
    {
        set
        {
            if (_time < value) _time = value;
        }
    }

    public float SetStrong
    {
        set
        {
            if (_strong < value) _strong = value;
        }
    }

    public GameObject SetObject
    {
        set
        {
            ShakingObj = value;
            _defalutPosition = ShakingObj.transform.position;
        }
    }

    public void Shaking(float time, float strong)
    {
        if (!gameObject.activeSelf) return;

        StopAllCoroutines();
        StartCoroutine(ShakingObject(time, strong));
    }

    private IEnumerator ShakingObject(float time, float strong)
    {
        yield return new WaitForEndOfFrame();
        if (ShakingObj == null) yield return null;

        var StartTime = 0.0f;

        _strong = strong;

        while (StartTime < time)
        {
            StartTime += Time.deltaTime;
            ShakingObj.transform.position = _defalutPosition;

            _rangeX = Random.Range(-_strong, _strong);
            _rangeY = Random.Range(-_strong, _strong);

            ShakingObj.transform.Translate(new Vector2(_rangeX,
                _rangeY));
            yield return new WaitForEndOfFrame();
            _strong = _strong * 0.9f;
        }

        ShakingObj.transform.position = _defalutPosition;
    }
}