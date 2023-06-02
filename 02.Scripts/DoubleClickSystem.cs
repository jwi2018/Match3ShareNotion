using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClickSystem : Singleton<DoubleClickSystem>
{
    private List<EID> DoubleClickBlockList = new List<EID>()
        {EID.X, EID.FISH, EID.VERTICAL, EID.HORIZONTAL, EID.RHOMBUS};
    public List<EID> GetBlockList()
    {
        return DoubleClickBlockList;
    }

    public void addRainbow(bool isCircus)
    {
        if(isCircus) DoubleClickBlockList.Add(EID.COLOR_BOMB);
    }
}
