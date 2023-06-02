using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private Button goldButton;
    [SerializeField] private Button packageButton;
    [SerializeField] private Button itemButton;

    [SerializeField] private GameObject goldMenu;
    [SerializeField] private GameObject packageMenu;
    [SerializeField] private GameObject itemMenu;

    public void Start()
    {
        goldButton.onClick.AddListener(()=>ChangeMenu(EShopKind.COIN));
        packageButton.onClick.AddListener(()=>ChangeMenu(EShopKind.PACKAGE));
        itemButton.onClick.AddListener(()=>ChangeMenu(EShopKind.GOLD));
    }

    public void OnDestroy()
    {
        goldButton.onClick.RemoveAllListeners();
        packageButton.onClick.RemoveAllListeners();
        itemButton.onClick.RemoveAllListeners();
    }

    public void ChangeMenu(EShopKind kind)
    {
        goldMenu.SetActive(false);
        packageMenu.SetActive(false);
        itemMenu.SetActive(false);
        
        switch (kind)
        {
            case EShopKind.PACKAGE:
                packageMenu.SetActive(false);
                break;
            case EShopKind.COIN:
                goldMenu.SetActive(false);
                break;
            case EShopKind.GOLD:
                itemMenu.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }
}
