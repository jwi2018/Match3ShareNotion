using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UserGold : MonoBehaviour
{
    [SerializeField] private Text UserGoldText;

    private int MyGold;

    private void Start()
    {
        if (PlayerData.GetInstance != null)
        {
            MyGold = PlayerData.GetInstance.Gold;
            UserGoldText.text = MyGold.ToString("#,##0");
        }
    }

    public void Goldsynchronization()
    {
        if (PlayerData.GetInstance != null)
        {
            StopAllCoroutines();
            StartCoroutine(ProductionEvent());
        }
    }

    public void GoldFixed()
    {
        if (PlayerData.GetInstance != null)
        {
            MyGold = PlayerData.GetInstance.Gold;
            UserGoldText.text = MyGold.ToString("#,##0");
        }
    }

    public void GetRewardGold(int num)
    {
        MyGold -= num;
        UserGoldText.text = MyGold.ToString("#,##0");
    }

    private IEnumerator ProductionEvent(float time = 0.8f)
    {
        var _isGoldAdd = true;
        var differenceGold = PlayerData.GetInstance.Gold - MyGold;
        if (differenceGold >= 0) _isGoldAdd = true;
        else _isGoldAdd = false;
        var tempGold = differenceGold;
        while (true)
        {
            var FreamGold = (int)(tempGold * Time.deltaTime / time);
            MyGold += FreamGold;
            differenceGold -= FreamGold;

            if (_isGoldAdd)
            {
                if (differenceGold < 0)
                {
                    MyGold = PlayerData.GetInstance.Gold;
                    UserGoldText.text = MyGold.ToString("#,##0");
                    break;
                }
            }
            else
            {
                if (differenceGold > 0)
                {
                    MyGold = PlayerData.GetInstance.Gold;
                    UserGoldText.text = MyGold.ToString("#,##0");
                    break;
                }
            }

            UserGoldText.text = MyGold.ToString("#,##0");
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }
}