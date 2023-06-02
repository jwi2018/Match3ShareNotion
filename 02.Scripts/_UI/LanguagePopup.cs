using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguagePopup : PopupSetting
{
    [SerializeField] private GameObject[] _language;

    [SerializeField] private GameObject languageFrame;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
#if UNITY_IOS
        /*
         * IOS는 태국어 관련 이슈가 있으므로 태국어가 사용되지 않으며
         * 태국어 버튼 위치에 아랍어 버튼을 두기 위해 직접적으로 값을 변경합니다.
         * 차후 언어관련 추가가 있을 경우 이 부분에서 수정이 이루어지면 됩니다.
         */
        foreach (var item in _language)
        {
            if (item.name == "Thai")
            {
                item.SetActive(false);
            }

            //if (item.name == "Arabic")
            //{
            //    item.GetComponent<RectTransform>().anchorMin = new Vector2(0.0f, 1.0f);
            //    item.GetComponent<RectTransform>().anchorMax = new Vector2(0.0f, 1.0f);
            //    item.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1.0f);
            //}
            //if(languageFrame != null)
            //{
            //    languageFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(680f, 710f);
            //}
        }
#endif
        //foreach (var langElement in _language)
        //{
        //    TogglesActivator activator = langElement.GetComponent<TogglesActivator>();
        //    if (activator != null)
        //    {
        //        activator.Initialize();
        //    }
        //}

/*
        foreach (var item in _language)
        {
            if (item.name == "Thai")
            {
                item.SetActive(false);
            }
        }
*/
        StartCoroutine(DelayInitialize(0.01f));
    }

    private IEnumerator DelayInitialize(float fDelay)
    {
        yield return new WaitForSeconds(fDelay);

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.NumLanguage != 0)
            {
                var SiblingIndex = PlayerData.GetInstance.NumLanguage;
                _language[SiblingIndex - 1].GetComponent<Toggle>().isOn = true;
            }
            else
            {
                var Language = Application.systemLanguage.ToString();
                var SiblingIndex = 0;
                foreach (var nString in _language)
                    if (nString.name == Language)
                    {
                        SiblingIndex = nString.transform.GetSiblingIndex();
                        break;
                    }

                _language[SiblingIndex].GetComponent<Toggle>().isOn = true;
                PlayerData.GetInstance.NumLanguage = SiblingIndex + 1;
            }
        }
        else
        {
            var Language = Application.systemLanguage.ToString();
            var SiblingIndex = 0;
            foreach (var nString in _language)
                if (nString.name == Language)
                {
                    SiblingIndex = nString.transform.GetSiblingIndex();
                    break;
                }

            _language[SiblingIndex].GetComponent<Toggle>().isOn = true;

            PlayerData.GetInstance.NumLanguage = SiblingIndex + 1;
        }

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void LanguageSave(Toggle toggle)
    {
        if (toggle.isOn)
            if (LanguageManager.GetInstance != null)
                LanguageManager.GetInstance.SetLanguage(toggle.transform.GetSiblingIndex() + 1);
    }
}