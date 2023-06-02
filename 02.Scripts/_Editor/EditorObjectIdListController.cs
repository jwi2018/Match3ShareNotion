using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorObjectIdListController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _activeControlList;

    [SerializeField] private List<Image> _ButtonColorControlList;

    [SerializeField] private EditorToggleManager _toggleMaster;

    public void OnClickButton(GameObject _activeObj)
    {
        if (_activeObj == null)
        {
            var color = new Color(1, 1, 1, 1);
            foreach (var item in _ButtonColorControlList) item.color = color;
            foreach (var item in _activeControlList)
                if (item.activeSelf)
                {
                    item.transform.localPosition = new Vector3(0, 100000, 0);
                    return;
                }
        }

        foreach (var item in _activeControlList)
            if (item == _activeObj)
            {
                _activeObj.SetActive(true);
                if (_activeObj.transform.GetChild(0).GetComponent<Toggle>() != null)
                {
                    var selectid = (EID) _activeObj.transform.GetChild(0).GetComponent<TogglesStatus>().GetStatus.Value;
                    _toggleMaster.SetObjectID = selectid;
                }

                _activeObj.transform.localPosition = new Vector3(0, 100, 0);
            }
            else
            {
                item.SetActive(false);
            }
    }

    public void OnClickButton_Color(Image obj)
    {
        var Yello = new Color(1, 1, 0, 1);
        var White = new Color(1, 1, 1, 1);

        foreach (var item in _ButtonColorControlList)
            if (item == obj) item.color = Yello;
            else item.color = White;
    }
}