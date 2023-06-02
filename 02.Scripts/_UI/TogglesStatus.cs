using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ToggleStatus
{
    public enum StatusKinds
    {
        EID = 0,
        EColor,
        EHP,
        EDirection,
        ETileKind
    }

    public StatusKinds _status;
    public int Value;
    public Image image;
    public Text text;
    public EDirection direction;
    public ETileKind tileKind;
    public int StartLevel;
}

public class TogglesStatus : MonoBehaviour
{
    [SerializeField] private ToggleStatus _myStatus;

    public ToggleStatus GetStatus => _myStatus;

    public int SetColor
    {
        set => _myStatus.Value = value;
    }

    public int SetHP
    {
        set => _myStatus.Value = value;
    }

    public Sprite Setsprite
    {
        set
        {
            _myStatus.image.sprite = value;
            _myStatus.image.SetNativeSize();

            var Min = Mathf.Min(70.0f / _myStatus.image.GetComponent<RectTransform>().sizeDelta.x,
                70.0f / _myStatus.image.GetComponent<RectTransform>().sizeDelta.y);
            var Sizex = _myStatus.image.GetComponent<RectTransform>().sizeDelta.x * Min;
            var Sizey = _myStatus.image.GetComponent<RectTransform>().sizeDelta.y * Min;
            _myStatus.image.GetComponent<RectTransform>().sizeDelta = new Vector2(Sizex, Sizey);
            _myStatus.image.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            _myStatus.image.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public string Settext
    {
        set => _myStatus.text.text = value;
    }

    public int SetDirection
    {
        set => _myStatus.direction = (EDirection) value;
    }

    public int SetTileKind
    {
        set => _myStatus.tileKind = (ETileKind) value;
    }

    public void Start()
    {
        if (_myStatus.StartLevel != 0)
            if (StageManager.StageNumber < _myStatus.StartLevel)
                gameObject.SetActive(false);
    }
}