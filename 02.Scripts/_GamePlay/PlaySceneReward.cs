using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneReward : MonoBehaviour
{
    private int ranNum;
    [SerializeField] private List<GameObject> rewardItemObj = new List<GameObject>();

    public void ItemChange()
    {
        ranNum = Random.Range(0, 4);

        for (int i = 0; i < rewardItemObj.Count; i++)
        {
            if (i == ranNum) rewardItemObj[i].SetActive(true);
            else rewardItemObj[i].SetActive(false);
        }
    }

    public void ShowAd()
    {
        PopupManager popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        popupManager.ExceptionPopup();
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item_reward");
            BlockManager.GetInstance.AddStartItem(ranNum);
            this.gameObject.SetActive(false);
        });
        /*AD.ShowAd(ERewardedKind.GETITEM, () =>
        {
            BlockManager.GetInstance.AddStartItem(ranNum);
            this.gameObject.SetActive(false);
        });*/
    }
}