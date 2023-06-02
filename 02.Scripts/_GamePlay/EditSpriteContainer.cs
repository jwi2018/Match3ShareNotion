using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EEditImage
{
    DIRECTION,
    TILE_START,
    TILE_END,
    TILE_NORMAL,
    TILE_LADDER,
    TILE_EMPTY
}

[Serializable]
public class EditSprites
{
    public EEditImage editImage;
    public Sprite sprite;
}

public class EditSpriteContainer : MonoBehaviour
{
    public List<EditSprites> list;

    public Sprite GetSpriteOrNull(EEditImage image)
    {
        foreach (var info in list)
            if (info.editImage == image)
                return info.sprite;
        return null;
    }
}