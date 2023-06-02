using UnityEngine;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private RectTransform[] tabletTransforms;
    [SerializeField] private EditorToggleManager _toggleMaster;

    [SerializeField] private int kind;

    private int direction = 3;

    public void RotateObj(int _direction)
    {
        switch ((EDirection) _direction)
        {
            case EDirection.DOWN:
                foreach (var item in tabletTransforms) item.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case EDirection.UP:
                foreach (var item in tabletTransforms) item.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case EDirection.RIGHT:
                foreach (var item in tabletTransforms) item.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                break;
            case EDirection.LEFT:
                foreach (var item in tabletTransforms) item.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
        }

        direction = _direction;
        MapEditor.GetInstance.SetObjectMode(EID.TABLET, EColor.NONE, kind, direction);
    }

    public void SelectObj(int _kind)
    {
        kind = _kind;
        MapEditor.GetInstance.SetObjectMode(EID.TABLET, EColor.NONE, kind, direction);
    }

    public void ActiveUI()
    {
        MapEditor.GetInstance.SetTabletTop(true);
        gameObject.SetActive(true);
        MapEditor.GetInstance.SetObjectMode(EID.TABLET, EColor.NONE, 0, direction);
    }

    public void CloseUI()
    {
        MapEditor.GetInstance.SetTabletTop(false);
        gameObject.SetActive(false);
        _toggleMaster.SelectIDClear();
    }
}