using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Pop_GetReward : PopupSetting
{
    [SerializeField] private Text _mText;

    [SerializeField] private Sprite testSprite;

    [SerializeField] private GameObject gobTarget = null;

    [SerializeField] private string strEndEffectName = "";

    private Vector3 vecTargetPosition = Vector3.zero;

    [SerializeField] private GameObject itemBase = null;
    [SerializeField] private Transform itemBaseParent = null;
    public float randomRadius = 2.8f;

    [SerializeField] private GameObject trailParticle;

    [SerializeField] private List<DailyQuestRewardSprite> rewardSprite = new List<DailyQuestRewardSprite>();
    private Dictionary<EDailyQuestRewardType, DailyQuestRewardSprite> dicRewardSprite = new Dictionary<EDailyQuestRewardType, DailyQuestRewardSprite>();

    private void Awake()
    {
        itemBase.SetActive(false);
        dicRewardSprite = rewardSprite.ToDictionary(t => t.eRewarditem, t => t);
    }

    private void Start()
    {
        StartCoroutine(CoDestroy(7f));
    }

    private IEnumerator CoDestroy(float delayTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(delayTime);
            Destroy(gameObject);
        }
    }

    [ContextMenu("ShowRewardTest")]
    public void ShowRewardTest()
    {
        gobTarget = GameObject.Find("Text_Gold");
        vecTargetPosition = gobTarget.transform.position;
        //ShowReward(testSprite, 5, 10, vecTargetPosition);
    }

    public void ShowReward(EDailyQuestRewardType rewardType, int spawnAmount, int iValue, string _strEndSound, Vector3 targetPosition)
    {
        if (dicRewardSprite.ContainsKey(rewardType))
        {
            var _sprite = dicRewardSprite[rewardType].spriteReward;

            ShowReward(_sprite, spawnAmount, iValue, _strEndSound, targetPosition);
        }
    }

    public void ShowReward(Sprite spItem, int spawnAmount, int iValue, string _strEndSound, Vector3 targetPosition)
    {
        DOTweenEx.DOTextInt(_mText, 1, iValue, 1);
        _mText.DOFade(0, 1).SetDelay(2.0f);
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject loadItem = Instantiate(itemBase, itemBaseParent);
            loadItem.SetActive(true);
            if (null != trailParticle)
            {
                Instantiate(trailParticle, loadItem.transform);
            }
            Pop_GetRewardEntity rewardEntity = loadItem.GetComponent<Pop_GetRewardEntity>();
            rewardEntity.SetData(spItem);

            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * (spawnAmount * 0.4f);
            var targetPos = new Vector3(transform.position.x + randomPos.x, transform.position.y + randomPos.y, transform.position.z);
            loadItem.transform.DOMove(targetPos, .8f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                SoundManager.GetInstance.Play("ChangeChameleon");
                Pop_GetRewardEntity rewardEntity = loadItem.GetComponent<Pop_GetRewardEntity>();
                rewardEntity.StartReward(targetPosition, strEndEffectName, _strEndSound);
            });
        }
    }

    public string GiveReward(EDailyQuestRewardType rewardType, int amount)
    {
        string r_endSound = "";
        switch (rewardType)
        {
            case EDailyQuestRewardType.COIN:
                PlayerData.GetInstance.Gold += amount;
                r_endSound = "GetCoin";
                break;

            case EDailyQuestRewardType.HAMMER:
                PlayerData.GetInstance.ItemHammer += amount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.BOMB:
                PlayerData.GetInstance.ItemBomb += amount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.COLOR:
                PlayerData.GetInstance.ItemColor += amount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.HAMMERBOMB:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemBomb += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.HAMMERCOLOR:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.BOMBCOLOR:
                PlayerData.GetInstance.ItemBomb += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.ALLITEM:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemBomb += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "ClearStar";
                break;

            case EDailyQuestRewardType.ACORN:
                PlayerData.GetInstance.Acorn += amount;
                r_endSound = "StarBoxGetItem";
                break;
        }

        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            popupManager.GoldRefresh();
        }

        return r_endSound;
    }
}