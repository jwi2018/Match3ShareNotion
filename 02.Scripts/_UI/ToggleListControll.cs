using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleListControll : MonoBehaviour
{
    [SerializeField] private EditorToggleManager ToggleMaster;

    [SerializeField] private List<GameObject> OpenAndCloseObject;

    [SerializeField] private GameObject MasterObject;

    [SerializeField] private Toggle _OnOffButton;

    [SerializeField] private GameObject ToggleCheckImage;

    private ToggleGroup toggles;

    public GameObject GetMasterObj => MasterObject;

    private void Awake()
    {
        toggles = GetComponent<ToggleGroup>();
        foreach (var item in toggles.ActiveToggles())
            if (item.isOn)
            {
                ToggleCheckImage.transform.SetParent(item.transform);
                ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
            }

        if (_OnOffButton != null) OnClickToggleButton();
    }


    public void OnClickToggleButton()
    {
        if (_OnOffButton.isOn)
            foreach (var item in OpenAndCloseObject)
                if (item.GetComponent<TogglesStatus>().GetStatus.Value >= 0)
                    item.SetActive(true);
                else item.SetActive(false);
        else
            foreach (var item in OpenAndCloseObject)
                item.SetActive(false);
    }

    public void ChangeToggleValue()
    {
        for (var i = 0; i < OpenAndCloseObject.Count; i++)
        {
            var item = OpenAndCloseObject[i].GetComponent<Toggle>();
            if (item.isOn)
            {
                var SetStatus = item.GetComponent<TogglesStatus>();
                switch (SetStatus.GetStatus._status)
                {
                    case ToggleStatus.StatusKinds.EColor:
                        ToggleMaster.SetColor = (EColor) SetStatus.GetStatus.Value;
                        ToggleCheckImage.transform.SetParent(item.transform);
                        ToggleCheckImage.transform.Translate(new Vector3(0, 0, 0));
                        break;
                    case ToggleStatus.StatusKinds.EHP:
                        ToggleMaster.SetHP = SetStatus.GetStatus.Value;
                        ToggleCheckImage.transform.SetParent(item.transform);
                        ToggleCheckImage.transform.Translate(new Vector3(0, 0, 0));
                        break;
                    case ToggleStatus.StatusKinds.EDirection:
                        ToggleMaster.SetintValue = SetStatus.GetStatus.Value;
                        ToggleCheckImage.transform.SetParent(item.transform);
                        ToggleCheckImage.transform.Translate(new Vector3(0, 0, 0));
                        break;
                    case ToggleStatus.StatusKinds.ETileKind:
                        ToggleMaster.SetintValue = SetStatus.GetStatus.Value;
                        ToggleCheckImage.transform.SetParent(item.transform);
                        ToggleCheckImage.transform.Translate(new Vector3(0, 0, 0));
                        break;
                }
            }
        }
    }

    public void ChangeToggleObjectValue()
    {
        if (toggles.ActiveToggles() != null)
            foreach (var item in toggles.ActiveToggles())
                if (item.isOn)
                {
                    if (item.GetComponent<TogglesStatus>() != null)
                    {
                        ToggleCheckImage.transform.SetParent(item.transform);
                        ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                        ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                        if (item.GetComponent<TogglesStatus>().GetStatus._status == ToggleStatus.StatusKinds.EID)
                        {
                            ToggleMaster.SetObjectID = (EID) item.GetComponent<TogglesStatus>().GetStatus.Value;
                        }
                        else
                        {
                            if (item.GetComponent<TogglesStatus>().GetStatus._status ==
                                ToggleStatus.StatusKinds.EDirection)
                            {
                                ToggleMaster.SetintValue = item.GetComponent<TogglesStatus>().GetStatus.Value;
                                ToggleMaster.SetException = ExceptionList.IDGRAVITY;
                                ToggleMaster.SetColor = EColor.NONE;
                            }
                            else if (item.GetComponent<TogglesStatus>().GetStatus._status ==
                                     ToggleStatus.StatusKinds.ETileKind)
                            {
                                ToggleMaster.SetintValue = item.GetComponent<TogglesStatus>().GetStatus.Value;
                                ToggleMaster.SetException = ExceptionList.IDTILE;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Plase Add To SpriteContainer this ObjectID");
                    }
                }
    }

    public void ChangeObjectValueColor(List<ColorAndSprite> lists)
    {
        for (var i = 0; i < OpenAndCloseObject.Count; i++)
        {
            OpenAndCloseObject[i].transform.rotation = new Quaternion(0, 0, 0, 0);
            if (i < lists.Count)
            {
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = (int) lists[i].color;
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.color = new Color(1, 1, 1, 1);
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().Setsprite = lists[i].sprite;
                OpenAndCloseObject[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                if (ToggleMaster.SetObjectID == EID.BARRICADE)
                {
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = i;
                    if ((EDirection) i == EDirection.UP)
                    {
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.GetComponent<Transform>()
                            .Rotate(new Vector3(0, 0, 90));
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image
                            .GetComponent<RectTransform>().localPosition = new Vector3(0, 40, 0);
                    }
                    else if ((EDirection) i == EDirection.DOWN)
                    {
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.GetComponent<Transform>()
                            .Rotate(new Vector3(0, 0, 90));
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image
                            .GetComponent<RectTransform>().localPosition = new Vector3(0, -40, 0);
                    }
                    else if ((EDirection) i == EDirection.LEFT)
                    {
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image
                            .GetComponent<RectTransform>().localPosition = new Vector3(-40, 0, 0);
                    }
                    else if ((EDirection) i == EDirection.RIGHT)
                    {
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image
                            .GetComponent<RectTransform>().localPosition = new Vector3(40, 0, 0);
                    }
                }
            }
            else
            {
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = -1;
            }
        }

        var sprite = lists[0].sprite;
        var color = EColor.NONE;

        var _item = OpenAndCloseObject[0].GetComponent<Toggle>();
        _item.isOn = true;
        color = (EColor) _item.GetComponent<TogglesStatus>().GetStatus.Value;
        ToggleCheckImage.transform.SetParent(_item.transform);
        ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
        ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
        sprite = _item.GetComponent<TogglesStatus>().GetStatus.image.sprite;

        MasterObject.GetComponent<Image>().sprite = sprite;
        MasterObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        ToggleCheckImage.transform.localRotation = new Quaternion(0, 0, 0, 0);
        OnClickToggleButton();
        ToggleMaster.SetColor = color;
        ToggleMaster.LogMessage();
    }

    public void ChangeObjectValueColor(ColorAndSprite list, int TotalNum, int Value, ExceptionList exception)
    {
        var Markingtrue = 0;
        for (var i = 0; i < OpenAndCloseObject.Count; i++)
            if (i < TotalNum)
            {
                if (i == Value)
                {
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = (int) list.color;
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.color = new Color(1, 1, 1, 1);
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().Setsprite = list.sprite;
                    OpenAndCloseObject[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                    if (exception == ExceptionList.IDTILE)
                    {
                        OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetTileKind = i;
                    }
                    else if (exception == ExceptionList.IDGRAVITY)
                    {
                        if ((EDirection) Value == EDirection.UP)
                        {
                            OpenAndCloseObject[i].transform.Rotate(new Vector3(0, 0, 0));
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetDirection = (int) EDirection.DOWN;
                        }
                        else if ((EDirection) Value == EDirection.DOWN)
                        {
                            OpenAndCloseObject[i].transform.Rotate(new Vector3(0, 0, 180));
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.color =
                                new Color(1, 1, 0, 1);
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetDirection = (int) EDirection.UP;
                        }
                        else if ((EDirection) Value == EDirection.RIGHT)
                        {
                            OpenAndCloseObject[i].transform.Rotate(new Vector3(0, 0, 90));
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.color =
                                new Color(0, 1, 0, 1);
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetDirection = (int) EDirection.RIGHT;
                        }
                        else if ((EDirection) Value == EDirection.LEFT)
                        {
                            OpenAndCloseObject[i].transform.Rotate(new Vector3(0, 0, -90));
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.image.color =
                                new Color(1, 0, 0, 1);
                            OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetDirection = (int) EDirection.LEFT;
                        }
                    }
                }
            }
            else
            {
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = -1;
            }

        var sprite = list.sprite;
        var color = EColor.NONE;
        if (exception == ExceptionList.IDTILE)
        {
            for (var i = 0; i < OpenAndCloseObject.Count; i++)
                if (i != ToggleMaster.SetintValue)
                {
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetColor = -1;
                }
                else
                {
                    var item = OpenAndCloseObject[i].GetComponent<Toggle>();
                    sprite = item.GetComponent<TogglesStatus>().GetStatus.image.sprite;
                    color = (EColor) item.GetComponent<TogglesStatus>().GetStatus.Value;
                    ToggleCheckImage.transform.SetParent(item.transform);
                    ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                    ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                    Markingtrue = i;
                }
        }
        else
        {
            var _item = OpenAndCloseObject[0].GetComponent<Toggle>();
            _item.isOn = true;
            color = (EColor) _item.GetComponent<TogglesStatus>().GetStatus.direction;
            ToggleCheckImage.transform.SetParent(_item.transform);
            ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
            ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
            sprite = _item.GetComponent<TogglesStatus>().GetStatus.image.sprite;
            Markingtrue = 0;
        }

        MasterObject.GetComponent<Image>().sprite = sprite;
        MasterObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
            MasterObject.transform.Rotate(new Vector3(0, 0,
                OpenAndCloseObject[Markingtrue].transform.localEulerAngles.z));
        ToggleCheckImage.transform.localRotation = new Quaternion(0, 0, 0, 0);

        if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
            ToggleCheckImage.transform.Rotate(new Vector3(0, 0, -ToggleCheckImage.transform.parent.localEulerAngles.z));
        else
            ToggleCheckImage.transform.localRotation = new Quaternion(0, 0, 0, 0);
        OnClickToggleButton();
        ToggleMaster.SetColor = color;
        if (ToggleMaster.SetException == ExceptionList.IDGRAVITY) ToggleMaster.SetintValue = (int) color;
        ToggleMaster.LogMessage();
    }

    public void ChangeObjectValueColor()
    {
        Sprite sprite = null;
        var color = EColor.NONE;

        for (var i = 0; i < OpenAndCloseObject.Count; i++)
        {
            if (ToggleMaster.SetException != ExceptionList.IDGRAVITY)
                OpenAndCloseObject[i].transform.rotation = new Quaternion(0, 0, 0, 0);
            var item = OpenAndCloseObject[i].GetComponent<Toggle>();
            if (item.isOn)
            {
                if (OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.Value == -1)
                {
                    var _item = OpenAndCloseObject[0].GetComponent<Toggle>();
                    _item.isOn = true;
                    ToggleCheckImage.transform.SetParent(_item.transform);
                    ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                    ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                    sprite = _item.GetComponent<TogglesStatus>().GetStatus.image.sprite;

                    if (ToggleMaster.SetException == ExceptionList.IDTILE)
                        ToggleMaster.SetintValue =
                            (int) OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.tileKind;
                    else if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
                        ToggleMaster.SetintValue =
                            (int) OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.direction;
                    break;
                }

                sprite = item.GetComponent<TogglesStatus>().GetStatus.image.sprite;
                color = (EColor) item.GetComponent<TogglesStatus>().GetStatus.Value;
                ToggleCheckImage.transform.SetParent(item.transform);
                ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
                    ToggleMaster.SetintValue = (int) item.GetComponent<TogglesStatus>().GetStatus.direction;
                else if (ToggleMaster.SetException == ExceptionList.IDTILE)
                    ToggleMaster.SetintValue =
                        (int) OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.tileKind;
                break;
            }
        }

        MasterObject.GetComponent<Image>().sprite = sprite;
        MasterObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        ToggleCheckImage.transform.localRotation = new Quaternion(0, 0, 0, 0);
        if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
        {
            if ((EDirection) ToggleMaster.SetintValue == EDirection.UP)
                MasterObject.transform.Rotate(new Vector3(0, 0, 180));
            else if ((EDirection) ToggleMaster.SetintValue == EDirection.DOWN)
                MasterObject.transform.Rotate(new Vector3(0, 0, 0));
            else if ((EDirection) ToggleMaster.SetintValue == EDirection.RIGHT)
                MasterObject.transform.Rotate(new Vector3(0, 0, 90));
            else if ((EDirection) ToggleMaster.SetintValue == EDirection.LEFT)
                MasterObject.transform.Rotate(new Vector3(0, 0, -90));
        }

        if (ToggleMaster.SetException == ExceptionList.IDGRAVITY)
            ToggleCheckImage.transform.Rotate(new Vector3(0, 0, -ToggleCheckImage.transform.parent.localEulerAngles.z));

        OnClickToggleButton();
        ToggleMaster.SetColor = color;
        ToggleMaster.LogMessage();
    }

    public void ChangeObjectValueHP(int HpValue)
    {
        var HP = 1;
        for (var i = 0; i < OpenAndCloseObject.Count; i++)
        {
            if (i == 0)
                if (ToggleMaster.SetObjectID == EID.CLAM || ToggleMaster.SetObjectID == EID.DOUBLE)
                {
                    OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetHP = -1;
                    HP++;
                    continue;
                }

            if (i < HpValue)
            {
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetHP = HP;
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().Settext = HP.ToString();
                HP++;
            }
            else
            {
                OpenAndCloseObject[i].GetComponent<TogglesStatus>().SetHP = -1;
            }
        }

        var textInfo = -1;
        var _item = OpenAndCloseObject[0].GetComponent<Toggle>();
        if (_item.GetComponent<TogglesStatus>().GetStatus.Value == -1)
            _item = OpenAndCloseObject[1].GetComponent<Toggle>();
        _item.isOn = true;
        ToggleCheckImage.transform.SetParent(_item.transform);
        ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
        ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
        textInfo = _item.GetComponent<TogglesStatus>().GetStatus.Value;
        MasterObject.GetComponent<Text>().text = textInfo.ToString();
        OnClickToggleButton();
        ToggleMaster.SetHP = textInfo;
    }

    public void ChangeObjectValueHP()
    {
        var textInfo = -1;

        for (var i = 0; i < OpenAndCloseObject.Count; i++)
        {
            var item = OpenAndCloseObject[i].GetComponent<Toggle>();

            if (item.isOn)
            {
                if (OpenAndCloseObject[i].GetComponent<TogglesStatus>().GetStatus.Value == -1)
                {
                    var _item = OpenAndCloseObject[0].GetComponent<Toggle>();
                    _item.isOn = true;
                    ToggleCheckImage.transform.SetParent(_item.transform);
                    ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                    ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                    textInfo = _item.GetComponent<TogglesStatus>().GetStatus.Value;
                    break;
                }

                textInfo = item.GetComponent<TogglesStatus>().GetStatus.Value;
                ToggleCheckImage.transform.SetParent(item.transform);
                ToggleCheckImage.transform.localPosition = new Vector3(0, 0, 0);
                ToggleCheckImage.transform.localScale = new Vector3(1, 1, 1);
                break;
            }
        }

        MasterObject.GetComponent<Text>().text = textInfo.ToString();
        OnClickToggleButton();
        ToggleMaster.SetHP = textInfo;
    }
}