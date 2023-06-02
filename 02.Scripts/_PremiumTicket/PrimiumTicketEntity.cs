using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PrimiumTicketEntity : MonoBehaviour
{
    [SerializeField] Text Leveltxt;
    [SerializeField] List<Sprite> Item_IMG = new List<Sprite>();

    [SerializeField] GameObject Lockobj_lv1;
    [SerializeField] GameObject Lockobj;
    [SerializeField] GameObject[] receiveItemCheck;

    [Header("프리미엄 존")]
    [SerializeField] GameObject primium_obj;
    [SerializeField] List<Image> primium_img = new List<Image>();
    [SerializeField] List<RectTransform> primium_img_scale = new List<RectTransform>();
    [SerializeField] List<Text> primium_txt = new List<Text>();
    [SerializeField] GameObject primium_LockObj;
    [SerializeField] GameObject primiumGetBtn;
    [SerializeField] GameObject primium_effect;

    [Header("무료 존")]
    [SerializeField] GameObject Free_obj;
    [SerializeField] List<Image> Free_img = new List<Image>();
    [SerializeField] List<RectTransform> Free_img_scale = new List<RectTransform>();
    [SerializeField] List<Text> Free_txt = new List<Text>();
    [SerializeField] GameObject freeGetBtn;


    public void SetTicketEntity(int _entitynum, bool _Isthislevelforlock, bool _isReceiveLevel_free, bool _isReceiveLevel_primium)
    {
        //좌물쇠 밑
        // 1레벨이면 0
        // 2레벨이면 1
        // 3레벨이면 2
        if ((_entitynum + 1) == PrimiumTicketSystem.GetInstance.LEVEL) 
            primium_effect.SetActive(true);
            
        //프리미엄쪽 자물쇠
        if (PrimiumTicketSystem.GetInstance.ISBUYPRIMIUMTICKET) primium_LockObj.SetActive(false);

        //레벨 자물쇠
        if (_Isthislevelforlock)
        {
            Lockobj.SetActive(true);
        }
        //첫뻔재 엔티티만 해당됨
        if (_entitynum.Equals(0))
        {
            if (PrimiumTicketSystem.GetInstance.LEVEL == 1)
            {
                Lockobj_lv1.SetActive(true);
            }
            else if (PrimiumTicketSystem.GetInstance.LEVEL == 2)
            {
                Lockobj.SetActive(true);
            }
        }

        //받은 상품들 체크표시
        if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM > _entitynum) receiveItemCheck[0].SetActive(true);
        if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE > _entitynum) receiveItemCheck[1].SetActive(true);
        

        //프리쪽 받기 버튼
        if (_isReceiveLevel_free)
        {
            if (PrimiumTicketSystem.GetInstance.LEVEL == 1) freeGetBtn.SetActive(false);

            if (PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE == 1)
            {
                freeGetBtn.SetActive(false);
                if(PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE == 29) freeGetBtn.SetActive(true);
            }
            else
            {
                freeGetBtn.SetActive(true);
                    
            }
        }

        //프리미엄쪽 받기 버튼
        if (_isReceiveLevel_primium)
        {
            if(PrimiumTicketSystem.GetInstance.ISBUYPRIMIUMTICKET)
            {
                if (PrimiumTicketSystem.GetInstance.LEVEL == 1)
                {
                    if (PrimiumTicketSystem.GetInstance.ISBUYPRIMIUMTICKET) primiumGetBtn.SetActive(false);
                }

                if (PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM == 1)
                {
                    primiumGetBtn.SetActive(false);
                    if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM == 29) primiumGetBtn.SetActive(true);
                }
                else
                {
                    primiumGetBtn.SetActive(true);
                }
            }
        }


        Leveltxt.text = (_entitynum + 1).ToString();
        if(PrimiumTicketSystem.GetInstance != null)
        {
            //프리미엄쪽
            {
                int level = PrimiumTicketSystem.MAXLEVEL + _entitynum;
                
                if(PrimiumTicketSystem.GetInstance.ItemValue[level].itemCount > 1)
                {
                    primium_obj.SetActive(true);
                    for (int i=0; i< PrimiumTicketSystem.GetInstance.ItemValue[level].itemCount; i++)
                    {
                        var item_name = PrimiumTicketSystem.GetInstance.ItemValue[level].Item;
                        switch (i)
                        {
                            case 0:
                                item_name = PrimiumTicketSystem.GetInstance.ItemValue[level].Item;
                                primium_txt[i].text = PrimiumTicketSystem.GetInstance.ItemValue[level].Count.ToString();
                                break;

                            case 1:
                                item_name = PrimiumTicketSystem.GetInstance.ItemValue[level].Item_1;
                                primium_txt[i].text = PrimiumTicketSystem.GetInstance.ItemValue[level].Count_1.ToString();
                                break;
                        }

                        switch (item_name)
                        {
                            case "None":
                                primium_img[i].sprite = Item_IMG[0];
                                break;

                            case "Hammer":
                                primium_img[i].sprite = Item_IMG[1];
                                break;

                            case "Bomb":
                                primium_img[i].sprite = Item_IMG[2];
                                break;

                            case "Color":
                                primium_img[i].sprite = Item_IMG[3];
                                break;

                            case "Hammer+Bomb":
                                primium_img[i].sprite = Item_IMG[4];
                                primium_img_scale[i].localScale *= 1.3f;
                                break;

                            case "Hammer+Color":
                                primium_img[i].sprite = Item_IMG[5];
                                primium_img_scale[i].localScale *= 1.3f;
                                break;

                            case "Bomb+Color":
                                primium_img[i].sprite = Item_IMG[6];
                                primium_img_scale[i].localScale *= 1.3f;
                                break;
                        }
                    }
                        
                }
                else
                {
                    var item_name = PrimiumTicketSystem.GetInstance.ItemValue[level].Item;
                    switch (item_name)
                    {
                        case "None":
                            primium_img[0].sprite = Item_IMG[0];
                            break;

                        case "Hammer":
                            primium_img[0].sprite = Item_IMG[1];
                            break;

                        case "Bomb":
                            primium_img[0].sprite = Item_IMG[2];
                            break;

                        case "Color":
                            primium_img[0].sprite = Item_IMG[3];
                            break;

                        case "Hammer+Bomb":
                            primium_img[0].sprite = Item_IMG[4];
                            primium_img_scale[0].localScale *= 1.3f;
                            break;

                        case "Hammer+Color":
                            primium_img[0].sprite = Item_IMG[5];
                            primium_img_scale[0].localScale *= 1.3f;
                            break;

                        case "Bomb+Color":
                            primium_img[0].sprite = Item_IMG[6];
                            primium_img_scale[0].localScale *= 1.3f;
                            break;



                    }

                    primium_txt[0].text = PrimiumTicketSystem.GetInstance.ItemValue[level].Count.ToString();
                }
                
            }


            //프리쪽
            {
                if (PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].itemCount > 1)
                {
                    Free_obj.SetActive(true);
                    for (int i = 0; i < PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].itemCount; i++)
                    {
                        var item_name = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Item;
                        switch (i)
                        {
                            case 0:
                                item_name = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Item;
                                Free_txt[i].text = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Count.ToString();
                                break;

                            case 1:
                                item_name = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Item_1;
                                Free_txt[i].text = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Count_1.ToString();
                                break;
                        }

                        switch (item_name)
                        {
                            case "None":
                                Free_img[i].sprite = Item_IMG[0];
                                break;

                            case "Hammer":
                                Free_img[i].sprite = Item_IMG[1];
                                break;

                            case "Bomb":
                                Free_img[i].sprite = Item_IMG[2];
                                break;

                            case "Color":
                                Free_img[i].sprite = Item_IMG[3];
                                break;

                            case "Hammer+Bomb":
                                Free_img[i].sprite = Item_IMG[4];
                                Free_img_scale[i].localScale *= 1.3f;
                                break;

                            case "Hammer+Color":
                                Free_img[i].sprite = Item_IMG[5];
                                Free_img_scale[i].localScale *= 1.3f;
                                break;

                            case "Bomb+Color":
                                Free_img[i].sprite = Item_IMG[6];
                                Free_img_scale[i].localScale *= 1.3f;
                                break;
                        }
                    }

                }
                else
                {
                    var item_name = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Item;
                    switch (item_name)
                    {
                        case "None":
                            Free_img[0].sprite = Item_IMG[0];
                            break;

                        case "Hammer":
                            Free_img[0].sprite = Item_IMG[1];
                            break;

                        case "Bomb":
                            Free_img[0].sprite = Item_IMG[2];
                            break;

                        case "Color":
                            Free_img[0].sprite = Item_IMG[3];
                            break;

                        case "Hammer+Bomb":
                            Free_img[0].sprite = Item_IMG[4];
                            Free_img_scale[0].localScale *= 1.3f;
                            break;

                        case "Hammer+Color":
                            Free_img[0].sprite = Item_IMG[5];
                            Free_img_scale[0].localScale *= 1.3f;
                            break;

                        case "Bomb+Color":
                            Free_img[0].sprite = Item_IMG[6];
                            Free_img_scale[0].localScale *= 1.3f;
                            break;
                    }

                    Free_txt[0].text = PrimiumTicketSystem.GetInstance.ItemValue[_entitynum].Count.ToString();
                }
            }
        }
    }

    //확인버튼 누른 후
    public void ChangeEntity(bool _isActive, bool _isPrimium = false)
    {
        if (_isPrimium)
        {
            if (!_isActive)
            {
                primiumGetBtn.SetActive(_isActive);
                receiveItemCheck[0].SetActive(!_isActive);
            }
            else
            {
                if (PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM == 1)
                {
                    primiumGetBtn.SetActive(!_isActive);
                    if(PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM.Equals(29)) primiumGetBtn.SetActive(true);
                }
                else
                {
                    primiumGetBtn.SetActive(_isActive);
                }

            }
        }
        else
        {
            if(!_isActive)
            {
                freeGetBtn.SetActive(_isActive);
                receiveItemCheck[1].SetActive(!_isActive);
            }
            else
            {
                if(PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE == 1)
                {
                    freeGetBtn.SetActive(!_isActive);
                    if (PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE.Equals(29)) freeGetBtn.SetActive(_isActive);
                }
                else
                {
                    freeGetBtn.SetActive(_isActive);
                }
                    
            }
        }
        
        
    }

    //아이템 구입 후
    public void BuyTicket(int _level)
    {
        if (_level == 0) StartCoroutine(GetButtonCoru());
        primium_LockObj.SetActive(false);
    }

    IEnumerator GetButtonCoru()
    {
        yield return new WaitForSeconds(0.3f);

        if(PrimiumTicketSystem.GetInstance.LEVEL == 1) primiumGetBtn.SetActive(false);
        else primiumGetBtn.SetActive(true);
    }
}
