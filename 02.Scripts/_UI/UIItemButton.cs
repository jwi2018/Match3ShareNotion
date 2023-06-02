using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemButton : MonoBehaviour
{
    [SerializeField] private GameObject lockBack;
    [SerializeField] private GameObject itemLock;

    [SerializeField] private GameObject itemLockOff;
    [SerializeField] private GameObject itemCount;

    [SerializeField] private Animator animator;

    public void Init(int number)
    {
        if (PlayerData.GetInstance.IsItemUnlock.ContainsKey(number))
        {
            refresh(number);
            if (animator != null)
            {
                animator.SetBool("Click",true);
            }
        }
    }

    public void refresh(int number)
    {
        if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
        {
            lockBack.SetActive(false);
            itemLock.SetActive(false);

            itemLockOff.SetActive(false);
            itemCount.SetActive(true);
        }
        else
        {
            if (!PlayerData.GetInstance.IsItemUnlock[number])
            {
                lockBack.SetActive(true);
                itemLock.SetActive(true);

                itemLockOff.SetActive(false);
                itemCount.SetActive(false);
            }
            else
            {
                lockBack.SetActive(false);
                itemLock.SetActive(false);

                itemCount.SetActive(true); // true

                if (!PlayerData.GetInstance.IsItemClicked[number])
                {
                    itemLockOff.SetActive(false); //true
                }
                else
                {
                    itemLockOff.SetActive(false);
                }
            }
        }
         
    }
}
