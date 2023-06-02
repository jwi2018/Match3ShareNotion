using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingAnim : MonoBehaviour
{
    [SerializeField] private MissionClearPopup missionClearPopup = null;
    [SerializeField] private Transform firstStar = null;
    [SerializeField] private Transform secondStar = null;
    [SerializeField] private Transform thirdStar = null;
    [SerializeField] private Transform savingBoxTransform = null;
    [SerializeField] private GameObject savingCoin = null;

    [SerializeField] private SavingBoxStatus boxStatus = null;

    private List<GameObject> coins = new List<GameObject>();

    private int beforeStar = 0;
    private int getStar = 0;

    public void StartAnim(int BeforeStar, int GetStar)
    {
        GameObject obj = Instantiate(savingCoin, this.transform);
        beforeStar = BeforeStar;
        getStar = GetStar;

        StartCoroutine(AnimationCoroutine());
    }

    IEnumerator AnimationCoroutine()
    {
        if (this.getStar > this.beforeStar)
        {
            for (int i = this.beforeStar + 1; i <= this.getStar; i++)
            {
                int roop = 0;
                Transform StartPosition = null;

                int temp = 0;

                if (i == 1)
                {
                    temp = 30;
                    if (PlayerData.GetInstance.SavingCoin + 30 > SavingInfomation.isTotalSavingCoin)
                    {
                        temp = 30 - (PlayerData.GetInstance.SavingCoin + 30 - SavingInfomation.isTotalSavingCoin);
                    }
                    StartPosition = firstStar;
                }

                else if (i == 2)
                {
                    temp = 60;
                    if (PlayerData.GetInstance.SavingCoin + 60 > SavingInfomation.isTotalSavingCoin)
                    {
                        temp = 60 - (PlayerData.GetInstance.SavingCoin + 60 - SavingInfomation.isTotalSavingCoin);
                    }
                    StartPosition = secondStar;
                }
                else if (i == 3)
                {
                    temp = 90;
                    if (PlayerData.GetInstance.SavingCoin + 90 > SavingInfomation.isTotalSavingCoin)
                    {
                        temp = 90 - (PlayerData.GetInstance.SavingCoin + 90 - SavingInfomation.isTotalSavingCoin);
                    }
                    StartPosition = thirdStar;
                }


                if (temp > 0)
                {
                    PlayerData.GetInstance.SavingCoin += temp;
                    if (boxStatus != null)
                    {
                        boxStatus.AddSavingCoin(temp);
                    }
                }
                else continue;

                while (roop < 5)
                {
                    GameObject obj = GetCoinObject();
                    obj.transform.position = StartPosition.position;
                    obj.SetActive(true);
                    obj.GetComponent<SavingCoinAnim>().StartAnim(StartPosition.position, savingBoxTransform.position);

                    roop++;
                    if (roop > 5) break;
                    yield return new WaitForSeconds(0.1f);
                }
                if (i == 1)
                {
                    boxStatus.SavingAnimation(temp);
                }
                else if (i == 2)
                {
                    boxStatus.SavingAnimation(temp);
                }
                else if (i == 3)
                {
                    boxStatus.SavingAnimation(temp);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        missionClearPopup.RewardAnimationEnd();
    }

    private GameObject GetCoinObject()
    {
        GameObject returnValue = null;

        foreach(var item in coins)
        {
            if(!item.activeSelf)
            {
                returnValue = item;
            }
        }
        if (returnValue == null)
        {
            returnValue = Instantiate(savingCoin, this.transform);
            return returnValue;
        }
        else
            return returnValue;
    }
}
