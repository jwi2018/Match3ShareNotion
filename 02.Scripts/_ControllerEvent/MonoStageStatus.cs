using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonoStageStatus : MonoBehaviour
{
    [SerializeField] private GameObject stagePopup;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject gobStagePopup = null;
    [SerializeField] private Text currentStage = null;
    private bool isPopupShown = false;
    [SerializeField] private GameObject environment;

    private UnityEngine.Coroutine corAnimatorOff = null;

    private void Start()
    {
        gobStagePopup.SetActiveSelf(false);

        if (currentStage != null)
        {
            int icurStage = PlayerData.GetInstance.PresentLevel + 1;

            currentStage.text = string.Format("Lv. {0}", icurStage.ToString("#,##0"));
        }
    }

    public void ShowNHidePopup()
    {
        //스테이지 시작시 실행.
        if (EventLevelSystem.GetInstance != null)
        {
            EventLevelSystem.GetInstance.IsEventLevel = false;
        }

        isPopupShown = !isPopupShown;
        //gobStagePopup.SetActiveSelf(isPopupShown);
        if (isPopupShown)
        {
            if (null != corAnimatorOff)
            {
                StopCoroutine(corAnimatorOff);
            }
            FirebaseManager.GetInstance.FirebaseLogEvent("stage_enter");
            gobStagePopup.SetActiveSelf(true);
            gobStagePopup.GetComponent<Animator>().SetTrigger("On");
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
            environment.SetActive(false);
        }
        else
        {
            if (null != corAnimatorOff)
            {
                StopCoroutine(corAnimatorOff);
            }
            if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
            gobStagePopup.GetComponent<Animator>().SetTrigger("Off");
            environment.SetActive(true);
        }
    }

    private IEnumerator CoAnimatorOff(float offTime)
    {
        yield return new WaitForSeconds(offTime);

        gobStagePopup.SetActiveSelf(false);
    }

    /*public void StageButtonOnOff()
    {
        if (animator != null)
        {
            if (stagePopup != null)
            {
                if (stagePopup.activeSelf)
                {
                    animator.SetTrigger("Close");
                }
                else
                {
                    animator.SetTrigger("Open");
                }
            }
        }
    }*/
}