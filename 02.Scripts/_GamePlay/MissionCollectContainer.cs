using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MissionSprite
{
    public string name;
    public EID ID;
    public List<Sprite> values;
}

public class MissionCollectContainer : MonoBehaviour
{
    public List<MissionSprite> missionSprites;

    public Sprite GetSpriteOrNull(EID _id, int value)
    {
        foreach (var info in missionSprites)
            if (info.ID == _id)
            {
                var i = 0;
                foreach (var sprite in info.values)
                {
                    if (i == value) return sprite;
                    i++;
                }
            }

        return null;
    }
}