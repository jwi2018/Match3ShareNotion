using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HpColorMatrix
{
    public EColor color;
    public List<Sprite> hp;
}

[Serializable]
public class BlockSpriteInfo
{
    public EID id;
    public List<HpColorMatrix> colors;
}

[Serializable]
public class ExceptionSpriteInfo
{
    public ExceptionList Examples;
    public List<HpColorMatrix> colors;
}

[Serializable]
public class MissionImage
{
    public EID eID;
    public Sprite MissionSprite;
}

public class SpriteContainer : MonoBehaviour
{
    public List<BlockSpriteInfo> list;
    public List<ExceptionSpriteInfo> exceptions;
    public List<MissionImage> MissionImages;

    public Sprite GetSpriteOrNull(EID _id, EColor _color, int _hp)
    {
        //if (_id != EID.NORMAL)
        //{
        //    Debug.Log($"EID : {(EID)_id}");
        //}
        
        foreach (var info in list)
            if (info.id == _id)
            {
                //[강]여기가 블럭 스프라이트 추가해주는곳
                foreach (var matrix in info.colors)
                    if (matrix.color == _color)
                        if (matrix.hp.Count >= _hp && _hp > 0)
                            return matrix.hp[_hp - 1];
                
                return info.colors[0].hp[0];
            }

        return null;
    }

    public Sprite GetSpriteOrNull(ExceptionList exception, EColor _color, int _hp)
    {
        Debug.Log($"ExceptionList : {exception}");
        
        foreach (var info in exceptions)
            if (info.Examples == exception)
            {
                foreach (var matrix in info.colors)
                    if (matrix.color == _color)
                        if (matrix.hp.Count >= _hp && _hp > 0)
                            return matrix.hp[_hp - 1];

                return info.colors[0].hp[0];
            }


        return null;
    }

    public List<HpColorMatrix> GetCountOfContainer(EID _id)
    {
        if (_id != EID.NORMAL)
        {
            Debug.Log($"EID : {(EID)_id}");
        }

        var returnValue = new List<HpColorMatrix>();
        foreach (var info in list)
            if (info.id == _id)
                foreach (var matrix in info.colors)
                    returnValue.Add(matrix);
        return returnValue;
    }

    public List<HpColorMatrix> GetCountOfContainer(ExceptionList exception)
    {
        var returnValue = new List<HpColorMatrix>();
        foreach (var info in exceptions)
            if (info.Examples == exception)
                foreach (var matrix in info.colors)
                    returnValue.Add(matrix);
        return returnValue;
    }

    public Sprite GetMissionImage(EID iD)
    {
        Sprite returnValue = null;

        foreach (var item in MissionImages)
            if (item.eID == iD)
                returnValue = item.MissionSprite;

        if (returnValue == null) returnValue = GetSpriteOrNull(iD, EColor.RED, 1);
        return returnValue;
    }
}