using UnityEngine;

public class GetCoinPopup : MonoBehaviour
{
    [SerializeField] private UserGold Gold;

    private void Start()
    {
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldAdd(Gold);
        }
    }

    public void CallCoinPlusNormal()
    {
        //PlayerData.GetInstance.Gold += 200;
    }

    public void CallCoinPlusTwice()
    {
        //PlayerData.GetInstance.Gold += 400;
    }

    public void GoldRefrash()
    {
        var obj = transform.parent.gameObject;
        if(obj.GetComponentInChildren<PrimiumTicketPopup>() != null)
        {
            obj.GetComponent<PopupManager>().GoldRefresh(true);
        }

        obj.GetComponent<PopupManager>().GoldRefresh();
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");
    }

    public void ClearRewardSetting(int num)
    {
        Gold.GetRewardGold(num);
    }

    public void End()
    {
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldRemove(Gold);
        }
    }

    public void AnimationStart(int value)
    {
        // 버블팡 참조
        //if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("GetCoin");
        //RewardCoinValue = value;
        //if (goldText != null)
        //{
        //    goldText.text = value.ToString();
        //}
        //if (gold != null)
        //{
        //    PopupManager popupManager = this.transform.parent.GetComponent<PopupManager>();
        //    popupManager.GoldAdd(gold);


        //    this.GetComponent<Animator>().SetTrigger("Next_Me");

        //    //this.GetComponent<Animator>().SetTrigger("Next");

        //    //Transform parentObj = null;
        //    //this.GetComponent<Animator>().SetTrigger("Next");
        //    //if (temp.transform.parent.name == "UI Top")
        //    //{
        //    //    parentObj = temp.transform.parent;
        //    //    //RectTransform value = new RectTransform();
        //    //    //value = temp.GetComponent<RectTransform>();
        //    //    temp.transform.SetParent(this.transform, false);
        //    //    EndPosition = temp.transform.localPosition;
        //    //    EndPosition.x += 15.0f;
        //    //    EndPosition.y -= 30.0f;
        //    //    temp.transform.SetParent(parentObj, false);
        //    //}
        //    //else
        //    //{
        //    //    parentObj = temp.transform.parent;
        //    //    temp.transform.SetParent(this.transform, false);
        //    //    EndPosition = temp.transform.localPosition;
        //    //    EndPosition.x -= 95.0f;
        //    //    temp.transform.SetParent(parentObj, false);
        //    //}

        //    GameObject obj = Instantiate(GetCoin_BeforePopup, this.transform.parent);
        //    obj.GetComponent<Animator>().SetTrigger("Normal");
        //    moveObject.SetActive(false);
        //  }
    }
}