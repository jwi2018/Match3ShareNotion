using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarChestScript : MonoBehaviour
{
    public GameObject ChestParticle;
    public GameObject OpenButton;
    public GameObject CloseButton;
    public GameObject lItem;
    public GameObject rItem;
    public List<Sprite> ItemList;

    [SerializeField] private Sprite GoldImage;

    [SerializeField] private List<int> Gold;

    [SerializeField] private Text _goldText;

    public GameObject ChestLight;


    public void ChestOpen()
    {
        ChestParticle.SetActive(true);
    }

    public void StartAnim()
    {
        GetComponent<Animator>().SetTrigger("Open");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("StarBoxGetItem");
        OpenButton.SetActive(false);
    }

    public void EndAnim()
    {
        CloseButton.SetActive(false);
        transform.parent.parent.parent.parent.gameObject.SetActive(false);
        ChestLight.SetActive(false);
        OpenButton.SetActive(true);
    }

    private IEnumerator WhatItem()
    {
        var GoldBox = new List<int>();
        GoldBox.Add(Gold[0]);
        GoldBox.Add(Gold[0]);
        GoldBox.Add(Gold[0]);
        GoldBox.Add(Gold[1]);
        GoldBox.Add(Gold[1]);
        GoldBox.Add(Gold[2]);

        var myGold = Random.Range(0, GoldBox.Count);

        lItem.GetComponent<Image>().sprite = GoldImage;
        if (PlayerData.GetInstance != null) PlayerData.GetInstance.Gold += GoldBox[myGold];
        _goldText.text = GoldBox[myGold].ToString();

        var itemBox = new List<int>();
        itemBox.Add(0);
        itemBox.Add(0);
        itemBox.Add(0);
        itemBox.Add(1);
        itemBox.Add(1);
        itemBox.Add(2);
        itemBox.Add(2);
        itemBox.Add(3);

        var myrItem = Random.Range(0, itemBox.Count);

        rItem.GetComponent<Image>().sprite = ItemList[itemBox[myrItem]];
        if (PlayerData.GetInstance != null)
            switch (itemBox[myrItem])
            {
                case 0:
                    PlayerData.GetInstance.ItemHammer++;
                    break;
                case 1:
                    PlayerData.GetInstance.ItemCross++;
                    break;
                case 2:
                    PlayerData.GetInstance.ItemBomb++;
                    break;
                case 3:
                    PlayerData.GetInstance.ItemColor++;
                    break;
            }

        if (PlayerData.GetInstance != null)
        {
            //PlayerData.GetInstance.OpenChestCount = PlayerData.GetInstance.OpenChestCount + 1;
            var manager = transform.GetComponentInParent<PopupManager>();
            manager.ChestOpen();
        }

        //Don't Touch WaitTime
        yield return new WaitForSeconds(0.98f);
        ChestLight.SetActive(true);

        lItem.transform.parent.gameObject.SetActive(true);
        lItem.transform.parent.GetComponent<Animator>().SetTrigger("Left");

        yield return new WaitForSeconds(0.48f);

        rItem.transform.parent.gameObject.SetActive(true);
        rItem.transform.parent.GetComponent<Animator>().SetTrigger("Right");
        //yield return new WaitForSeconds(0.1f);
        //ChestLight.SetActive(false);

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("StarBoxGetItem");
        yield return new WaitForSeconds(0.4f);
        CloseButton.SetActive(true);
        CloseButton.GetComponent<Animator>().SetTrigger("Start");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("StarBoxClose");
    }
}