using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditImageTile : MonoBehaviour
{
    [SerializeField] private Image tileImage;

    //[SerializeField] private Image gravityImage = null;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private SpriteRenderer gravityRenderer;
    [SerializeField] private Transform gravityTransform;

    private readonly EditTile editTile = new EditTile();
    private RectTransform gravityRectTransform = null;
    private bool isMouseActive;

    private readonly Dictionary<BlockInfo, EditFieldObject> objects = new Dictionary<BlockInfo, EditFieldObject>();
    public Vector2Int Matrix => editTile.Matrix;
    public ETileKind TileKind => editTile.TileKind;
    public EDirection DropDirection => editTile.DropDirection;

    private void OnMouseDown()
    {
        MapEditor.GetInstance.IsMouseDown = true;
        MapEditor.GetInstance.TileMouseDown(this);
    }

    private void OnMouseEnter()
    {
        if (MapEditor.GetInstance.IsBigMode) MapEditor.GetInstance.TileMouseEnter(this);
    }

    private void OnMouseExit()
    {
        isMouseActive = false;
    }

    private void OnMouseOver()
    {
        if (!MapEditor.GetInstance.IsMouseDown) return;
        if (isMouseActive) return;

        switch (MapEditor.GetInstance.Mode)
        {
            case EEditorMode.TILE:
                editTile.SetTileKind(MapEditor.GetInstance.TileKind);
                ApplyImage();
                if (MapEditor.GetInstance.TileKind == ETileKind.VOID) RemoveObject();
                break;
            case EEditorMode.GRAVITY:
                editTile.SetDropDirection(MapEditor.GetInstance.DropDirection);
                ApplyImage();
                break;
            case EEditorMode.OBJECT:
                if (TileKind == ETileKind.VOID || TileKind == ETileKind.LADDER ||
                    MapEditor.GetInstance.BlockINFO.ID == EID.NONE
                    || MapEditor.GetInstance.BlockINFO.HP == -1) return;
                if (MapEditor.GetInstance.PreRailProperty == null)
                {
                    var info = new BlockInfo(MapEditor.GetInstance.BlockINFO.ID,
                        MapEditor.GetInstance.BlockINFO.color,
                        MapEditor.GetInstance.BlockINFO.HP,
                        MapEditor.GetInstance.BlockINFO.ETC);
                    if (!MapEditor.GetInstance.IsBigMode)
                    {
                        if (info.ID != EID.BARRICADE && info.ID != EID.TUNNEL_ENTRANCE
                                                     && info.ID != EID.TUNNEL_EXIT)
                        {
                            InputObject(MapEditor.GetInstance.Depth, info);
                        }
                        else if (info.ID == EID.BARRICADE)
                        {
                            MapEditor.GetInstance.BarricadeActiveImage(this);
                        }
                        else if (info.ID == EID.TUNNEL_ENTRANCE)
                        {
                            if (info.color == EColor.RED)
                                MapEditor.GetInstance.TouchTunnel(this, true);
                            else
                                MapEditor.GetInstance.TouchTunnel(this, false);
                        }
                        else if (info.ID == EID.TUNNEL_EXIT)
                        {
                            MapEditor.GetInstance.TouchTunnel(this, false);
                        }
                    }
                    else
                    {
                        MapEditor.GetInstance.TileMouseEnter(this);
                    }
                }
                else
                {
                    MapEditor.GetInstance.TouchRail(this);
                }

                break;
        }

        MapEditor.GetInstance.SetTileInfo(Matrix, editTile);

        isMouseActive = true;
        MapEditor.GetInstance.ModifySpecialMission();
    }

    private void OnMouseUp()
    {
        isMouseActive = false;
        MapEditor.GetInstance.IsMouseDown = false;
    }

    public void Init()
    {
        editTile.Init();
        if (gravityRenderer != null) gravityTransform = gravityRenderer.GetComponent<Transform>();
    }

    public void Setting(Vector2Int _matrix, ETileKind _tileKind, EDirection _dropDirection)
    {
        editTile.Setting(_matrix, _tileKind, _dropDirection);

        ApplyImage();
    }

    public void InputObject(EDepth depth, BlockInfo obj)
    {
        if (depth == EDepth.FLOOR && MapEditor.GetInstance.CheckRailExist(Matrix)) return;
        if (MapEditor.GetInstance.IsBigObject(Matrix, depth) && depth != EDepth.FLOOR) return;

        if (editTile.IsExistBlock(depth))
        {
            if (editTile.GetObjectOrNull(depth).ID
                == obj.ID && editTile.GetObjectOrNull(depth).HP == obj.HP)
            {
                var isExist = false;
                foreach (var o in objects)
                    if (o.Key.ID == obj.ID)
                        isExist = true;

                if (!isExist)
                {
                    var imageObj = MapEditor.GetInstance.GetEditFieldObjectOrNull(obj);
                    imageObj.Setting(rectTransform, depth);
                    if (obj.ID == EID.TARGET_RELIC) imageObj.SetPosition(new Vector2(0, -30));
                    objects.Add(obj, imageObj);
                }
                else
                {
                    var popInfo = editTile.PopBlockOrNull(depth);
                    if (popInfo != null)
                        foreach (var o in objects)
                            if (popInfo.ID == o.Key.ID)
                            {
                                Destroy(objects[o.Key].gameObject);
                                objects.Remove(o.Key);
                                break;
                            }
                }
            }
            else
            {
                var popInfo = editTile.PopBlockOrNull(depth);
                if (popInfo != null)
                {
                    Destroy(objects[popInfo].gameObject);
                    objects.Remove(popInfo);
                }

                if (editTile.InputBlock(depth, obj) && !MapEditor.GetInstance.IsBigMode)
                {
                    var imageObj = MapEditor.GetInstance.GetEditFieldObjectOrNull(obj);
                    imageObj.Setting(rectTransform, depth);
                    if (obj.ID == EID.TARGET_RELIC) imageObj.SetPosition(new Vector2(0, -30));
                    objects.Add(obj, imageObj);
                }
            }
        }
        else
        {
            if (editTile.InputBlock(depth, obj))
            {
                var imageObj = MapEditor.GetInstance.GetEditFieldObjectOrNull(obj);
                imageObj.Setting(rectTransform, depth);
                if (obj.ID == EID.TARGET_RELIC) imageObj.SetPosition(new Vector2(0, -30));
                objects.Add(obj, imageObj);
            }
        }
    }

    public bool IsBlockInputAble(EDepth depth)
    {
        if (editTile.IsExistBlock(depth)) return false;

        return true;
    }

    public void ApplyImage()
    {
        if (tileImage != null)
            switch (editTile.TileKind)
            {
                case ETileKind.NORMAL:
                    tileImage.sprite = MapEditor.GetInstance.GetSpriteOrNull(EEditImage.TILE_NORMAL);
                    rectTransform.sizeDelta = new Vector2(80, 80);
                    break;
                case ETileKind.START:
                    tileImage.sprite = MapEditor.GetInstance.GetSpriteOrNull(EEditImage.TILE_START);
                    rectTransform.sizeDelta = new Vector2(80, 100);
                    //rectTransform.localPosition = rectTransform.localPosition + new Vector3(0, 10, 0);
                    break;
                case ETileKind.END:
                    tileImage.sprite = MapEditor.GetInstance.GetSpriteOrNull(EEditImage.TILE_END);
                    rectTransform.sizeDelta = new Vector2(80, 100);
                    //rectTransform.localPosition = rectTransform.localPosition + new Vector3(0, -10, 0);
                    break;
                case ETileKind.LADDER:
                    tileImage.sprite = MapEditor.GetInstance.GetSpriteOrNull(EEditImage.TILE_LADDER);
                    rectTransform.sizeDelta = new Vector2(80, 80);
                    break;
                case ETileKind.VOID:
                    tileImage.sprite = MapEditor.GetInstance.GetSpriteOrNull(EEditImage.TILE_EMPTY);
                    rectTransform.sizeDelta = new Vector2(80, 80);
                    break;
            }

        if (gravityRenderer != null)
            switch (editTile.DropDirection)
            {
                case EDirection.RIGHT:
                    gravityTransform.rotation = Quaternion.Euler(0, 0, 90);
                    gravityRenderer.color = Color.green;
                    break;
                case EDirection.LEFT:
                    gravityTransform.rotation = Quaternion.Euler(0, 0, 270);
                    gravityRenderer.color = Color.red;
                    break;
                case EDirection.UP:
                    gravityTransform.rotation = Quaternion.Euler(0, 0, 180);
                    gravityRenderer.color = Color.yellow;
                    break;
                case EDirection.DOWN:
                    gravityTransform.rotation = Quaternion.Euler(0, 0, 0);
                    gravityRenderer.color = Color.white;
                    break;
            }
    }

    private void RemoveObject()
    {
        editTile.Clear();
        Clear();
        MapEditor.GetInstance.RemoveAtMatrix(Matrix);
    }

    public void Clear()
    {
        editTile.Clear();
        foreach (var item in objects) Destroy(item.Value.gameObject);
        objects.Clear();
    }

    public void SetGravityTopMode(bool value)
    {
        if (value)
            gravityRenderer.sortingOrder = 9;
        else
            gravityRenderer.sortingOrder = 1;
    }
}