using I2.Loc;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    [SerializeField] private SetLanguage Language;

    public void Init()
    {
        switch (PlayerData.GetInstance.NumLanguage)
        {
            case 1:
                Language._Language = "English";
                break;
            case 2:
                Language._Language = "French";
                break;
            case 3:
                Language._Language = "German";
                break;
            case 4:
                Language._Language = "Spanish";
                break;
            case 5:
                Language._Language = "Italian";
                break;
            case 6:
                Language._Language = "Russian";
                break;
            case 7:
                Language._Language = "Portuguese";
                break;
            case 8:
                Language._Language = "Turkish";
                break;
            case 9:
                Language._Language = "Korean";
                break;
            case 10:
                Language._Language = "Japanese";
                break;
            case 11:
                Language._Language = "Chinese (SW)";
                break;
            case 12:
                Language._Language = "Chinese (TW)";
                break;
            case 13:
                Language._Language = "Indonesian";
                break;
            case 14:
                Language._Language = "Malay";
                break;
            case 16:
                Language._Language = "Arabic";
                break;

#if !UNITY_IOS
            case 15:
                Language._Language = "Thai";
                break;
#endif
            default:
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        PlayerData.GetInstance.NumLanguage = 1;
                        Language._Language = "English";
                        break;
                    case SystemLanguage.French:
                        PlayerData.GetInstance.NumLanguage = 2;
                        Language._Language = "French";
                        break;
                    case SystemLanguage.German:
                        PlayerData.GetInstance.NumLanguage = 3;
                        Language._Language = "German";
                        break;
                    case SystemLanguage.Spanish:
                        PlayerData.GetInstance.NumLanguage = 4;
                        Language._Language = "Spanish";
                        break;
                    case SystemLanguage.Italian:
                        PlayerData.GetInstance.NumLanguage = 5;
                        Language._Language = "Italian";
                        break;
                    case SystemLanguage.Russian:
                        PlayerData.GetInstance.NumLanguage = 6;
                        Language._Language = "Russian";
                        break;
                    case SystemLanguage.Portuguese:
                        PlayerData.GetInstance.NumLanguage = 7;
                        Language._Language = "Portuguese";
                        break;
                    case SystemLanguage.Turkish:
                        PlayerData.GetInstance.NumLanguage = 8;
                        Language._Language = "Turkish";
                        break;
                    case SystemLanguage.Korean:
                        PlayerData.GetInstance.NumLanguage = 9;
                        Language._Language = "Korean";
                        break;
                    case SystemLanguage.Japanese:
                        PlayerData.GetInstance.NumLanguage = 10;
                        Language._Language = "Japanese";
                        break;
                    case SystemLanguage.ChineseSimplified:
                        PlayerData.GetInstance.NumLanguage = 11;
                        Language._Language = "Chinese (SW)";
                        break;
                    case SystemLanguage.ChineseTraditional:
                        PlayerData.GetInstance.NumLanguage = 12;
                        Language._Language = "Chinese (TW)";
                        break;
                    case SystemLanguage.Indonesian:
                        PlayerData.GetInstance.NumLanguage = 13;
                        Language._Language = "Indonesian";
                        break;
                    case SystemLanguage.Thai:
#if !UNITY_IOS
                        PlayerData.GetInstance.NumLanguage = 15;
                        Language._Language = "Thai";
#else
                        PlayerData.GetInstance.NumLanguage = 1;
                        Language._Language = "English";
#endif
                        break;
                    case SystemLanguage.Arabic:
                        PlayerData.GetInstance.NumLanguage = 16;
                        Language._Language = "Arabic";
                        break;
                    default:
                        PlayerData.GetInstance.NumLanguage = 1;
                        Language._Language = "English";
                        break;
                }

                break;
        }

        Language.ApplyLanguage();
    }

    public string GetLanguages()
    {
        string returnValue = null;

        switch (PlayerData.GetInstance.NumLanguage)
        {
            case 1:
                returnValue = "English";
                break;
            case 2:
                returnValue = "French";
                break;
            case 3:
                returnValue = "German";
                break;
            case 4:
                returnValue = "Spanish";
                break;
            case 5:
                returnValue = "Italian";
                break;
            case 6:
                returnValue = "Russian";
                break;
            case 7:
                returnValue = "Portuguese";
                break;
            case 8:
                returnValue = "Turkish";
                break;
            case 9:
                returnValue = "Korean";
                break;
            case 10:
                returnValue = "Japanese";
                break;
            case 11:
                returnValue = "Chinese (SW)";
                break;
            case 12:
                returnValue = "Chinese (TW)";
                break;
            case 13:
                returnValue = "Indonesian";
                break;
            case 14:
                returnValue = "Malay";
                break;
            case 15:
                returnValue = "Thai";
                break;
            case 16:
                returnValue = "Arabic";
                break;
        }

        return returnValue;
    }

    public void SetLanguage(int LanguageNumber)
    {
        switch (LanguageNumber)
        {
            case 1:
                Language._Language = "English";
                break;
            case 2:
                Language._Language = "French";
                break;
            case 3:
                Language._Language = "German";
                break;
            case 4:
                Language._Language = "Spanish";
                break;
            case 5:
                Language._Language = "Italian";
                break;
            case 6:
                Language._Language = "Russian";
                break;
            case 7:
                Language._Language = "Portuguese";
                break;
            case 8:
                Language._Language = "Turkish";
                break;
            case 9:
                Language._Language = "Korean";
                break;
            case 10:
                Language._Language = "Japanese";
                break;
            case 11:
                Language._Language = "Chinese (SW)";
                break;
            case 12:
                Language._Language = "Chinese (TW)";
                break;
            case 13:
                Language._Language = "Indonesian";
                break;
            case 14:
                Language._Language = "Malay";
                break;
            case 15:
                Language._Language = "Thai";
                break;
            case 16:
                Language._Language = "Arabic";
                break;
            default:
                PlayerData.GetInstance.NumLanguage = 1;
                Language._Language = "English";
                Language.ApplyLanguage();
                return;
        }

        PlayerData.GetInstance.NumLanguage = LanguageNumber;
        Language.ApplyLanguage();
    }

    public int GetLanguagesForNotification()
    {
        var returnValue = 0;

        switch (PlayerData.GetInstance.NumLanguage)
        {
            case 1:
                returnValue = 0;
                break;
            case 2:
                returnValue = 7;
                break;
            case 3:
                returnValue = 5;
                break;
            case 4:
                returnValue = 6;
                break;
            case 5:
                returnValue = 14;
                break;
            case 6:
                returnValue = 8;
                break;
            case 7:
                returnValue = 12;
                break;
            case 8:
                returnValue = 13;
                break;
            case 9:
                returnValue = 1;
                break;
            case 10:
                returnValue = 4;
                break;
            case 11:
                returnValue = 2;
                break;
            case 12:
                returnValue = 3;
                break;
            case 13:
                returnValue = 9;
                break;
            case 14:
                returnValue = 10;
                break;
            case 15:
                returnValue = 11;
                break;
            case 16:
                returnValue = 15;
                break;
        }

        return returnValue;
    }
}