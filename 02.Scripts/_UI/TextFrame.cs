using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFrame : MonoBehaviour
{
    [SerializeField] private Text _frameText;

    private int _lowFrame = 0;
    private int _nowFrame;
    private float _timedelta;

    private void Start()
    {
        StartCoroutine(RefrashFrame());
    }

    private IEnumerator RefrashFrame()
    {
        while (true)
        {
            _timedelta = Time.deltaTime;
            _timedelta *= 60;
            _timedelta = 1 / _timedelta;
            _nowFrame = (int) (60 * _timedelta);
            _frameText.text = _nowFrame.ToString();

            if (_nowFrame < 30)
                _frameText.color = new Color(1, 0, 0, 1);
            else if (_nowFrame < 40)
                _frameText.color = new Color(1, 1, 0, 1);
            else if (_nowFrame < 50)
                _frameText.color = new Color(0, 0, 1, 1);
            else
                _frameText.color = new Color(0, 1, 0, 1);
            yield return new WaitForSeconds(0.1f);
        }
    }
}