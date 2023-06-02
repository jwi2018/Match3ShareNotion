using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAnimController : MonoBehaviour
{
    public PrimiumTicketPopup primiumTicketPopup;


    public void UnlockLevelupAnim()
    {
        if (primiumTicketPopup != null)
        {
            primiumTicketPopup.UnlockLevelup();
        }
        else
        {
            primiumTicketPopup = FindObjectOfType<PrimiumTicketPopup>();
            primiumTicketPopup.UnlockLevelup();
        }
    }

    public void UnlockBuyTicketAnim()
    {
        if (primiumTicketPopup != null)
        {
            primiumTicketPopup.UnlockBuyTicket();
        }
        else
        {
            primiumTicketPopup = FindObjectOfType<PrimiumTicketPopup>();
            primiumTicketPopup.UnlockBuyTicket();
        }
    }


    public void UnlockSound()
    {
        SoundManager.GetInstance.Play("SeasonPass_UnlockOpen");
    }
}
