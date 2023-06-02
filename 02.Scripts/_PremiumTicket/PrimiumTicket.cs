using System.Collections;
using CompleteProject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimiumTicket : MonoBehaviour
{
    [SerializeField] Text starCounttxt;
    [SerializeField] Text Leveltxt;
    [SerializeField] Text notice_txt;
    [SerializeField] Image GuageIMG;
    [SerializeField] Animator GetStarAnim;
    [SerializeField] GameObject alram;
    [SerializeField] GameObject primium_effect;

    private int StarCount;
    private int MaxStarCount;

    bool PlayingAnim = false;
    void Start()
    {
        CheckStarCount();
    }

    public void click()
    {
        //테스트
        PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT = 1;

        for (int i = 0; i <2; i++)
        {
            CalculateStar();
        }
        //StartCoroutine(StarAnim());
        //Purchaser.GetInstance.BuyProductID("com.actionfit.ios.jewelpiratematch.2000coin");
    }

    public void Settext()
    {
        if(PrimiumTicketSystem.GetInstance.ISBUYPRIMIUMTICKET)
        {
            if (PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE > 1
            || PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_PRIMIUM > 1)
            {
                notice_txt.text = I2.Loc.LocalizationManager.GetTermTranslation("PrimiumTicket_GetItem");
                primium_effect.SetActive(true);
            }
            else
            {
                notice_txt.text = I2.Loc.LocalizationManager.GetTermTranslation("Obtain_Stars");
                primium_effect.SetActive(false);
            }
        }
        else
        {
            if (PrimiumTicketSystem.GetInstance.LEVEL - PrimiumTicketSystem.GetInstance.RECEIVELEVEL_FREE > 1)
            {
                notice_txt.text = I2.Loc.LocalizationManager.GetTermTranslation("PrimiumTicket_GetItem");
                primium_effect.SetActive(true);
            }
            else
            {
                notice_txt.text = I2.Loc.LocalizationManager.GetTermTranslation("Obtain_Stars");
                primium_effect.SetActive(false);
            }
        }
        



        if (PrimiumTicketSystem.GetInstance.LEVEL > PrimiumTicketSystem.MAXLEVEL)
        {
            Leveltxt.text = PrimiumTicketSystem.MAXLEVEL.ToString();
            MaxStarCount = PrimiumTicketSystem.MAXLEVEL;
        }
        else
        {
            Leveltxt.text = PrimiumTicketSystem.GetInstance.LEVEL.ToString();
            MaxStarCount = PrimiumTicketSystem.GetInstance.LEVEL;
        }

        StarCount = PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR;
        starCounttxt.text = $"{StarCount} / {MaxStarCount}";
        float amount = (float)StarCount / MaxStarCount;
        GuageIMG.fillAmount = amount;
    }

    void CheckStarCount()
    {
        Settext();
        if (PrimiumTicketSystem.GetInstance != null)
        {
            //애니메이션 실행
            switch (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT)
            {
                case 0:
                    GetStarAnim.SetTrigger("GetStartAnim_idle");
                    break;

                case 1:
                    GetStarAnim.SetTrigger("GetStarAnim_1");
                    break;

                case 2:
                    GetStarAnim.SetTrigger("GetStarAnim_2");
                    break;

                default:
                    GetStarAnim.SetTrigger("GetStarAnim_3");
                    break;

            }
        }
    }



    #region 애니메이션 함수
    public void FirstStar()
    {
        if(PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
        {
            //30레벨이 만렙
            CalculateStar();
        }
    }

    public void SecondStar()
    {
        if (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
        {
            CalculateStar();
        }
    }

    public void AnimEnd()
    {
        if (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
        {
            CalculateStar();



            if (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
            {
                StartCoroutine(StarAnim());
            }
        }
    }

    public void MoreStarAnim()
    {
        if (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
        {
            CalculateStar();

            PlayingAnim = true;
        }
    }

    public void OnePlayAnim()
    {
        GetStarAnim.SetBool("GetStarAnim_4", PlayingAnim);
    }

    #endregion

    public void CalculateStar()
    {
        if (!PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR.Equals(PrimiumTicketSystem.MAXLEVEL)) PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR++;

        if (PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR.Equals(PrimiumTicketSystem.GetInstance.LEVEL))
        {
            if (PrimiumTicketSystem.GetInstance.LEVEL < PrimiumTicketSystem.MAXLEVEL)
            {
                PrimiumTicketSystem.GetInstance.LEVEL++;
                PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR = 0;
                PrimiumTicketSystem.GetInstance.IsLevelup = true;
            }
            else  //레벨 30일때
            {
                PrimiumTicketSystem.GetInstance.LEVEL = 31;
                if (PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR >= PrimiumTicketSystem.MAXLEVEL) PrimiumTicketSystem.GetInstance.PRIMIUMTICKETSTAR = 30;
            }
        }


        Settext();

        PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT--;
    }


 

    IEnumerator StarAnim()
    {
        PlayingAnim = true;

        while (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT > 0)
        {
            yield return null;

            if (PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT == 0) break;
            if (PlayingAnim) GetStarAnim.SetBool("GetStarAnim_4", PlayingAnim);
            PlayingAnim = false;
        }
    }
    
}
