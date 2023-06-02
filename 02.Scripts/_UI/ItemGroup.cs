using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDictionary : SerializableDictionary<int, UIItemButton> { }

public class ItemGroup : MonoBehaviour
{
   [SerializeField] private ItemDictionary itemList;
   private void Start()
   {
       foreach (var item in itemList)
       {
           item.Value.Init(item.Key);
       }
   }

   public void Refresh()
   {
       foreach (var item in itemList)
       {
           item.Value.refresh(item.Key);
       }
   }
}
