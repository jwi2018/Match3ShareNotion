using System;
using System.Collections.Generic;

[Serializable]
public class SpecialItemStatus
{
    public BlockInfo blockInfo;
    public int persent;
}

[Serializable]
public class SpecialItemList
{
    public List<SpecialItemStatus> specialItemStatuses;
}

public class SpecialTileItemList : Singleton<SpecialTileItemList>
{
    public List<SpecialItemList> specialItemLists;

    public List<SpecialItemStatus> GetSpecialItems(int EtcValue)
    {
        if (specialItemLists.Count > EtcValue)
        {
            var returnValue = new List<SpecialItemStatus>();
            returnValue.AddRange(specialItemLists[EtcValue].specialItemStatuses);
            return returnValue;
        }

        return null;
    }
}