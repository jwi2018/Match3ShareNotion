using System;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public class GpgsManager : Singleton<GpgsManager>
{
    public bool bLogin { get; set; }

    public void OnApplicationQuit()
    {
        //LogoutGPGS();
    }

    public void InitGPGS()
    {
        bLogin = Social.localUser.authenticated;

#if UNITY_ANDROID
        /*PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;*/
        //PlayGamesPlatform.Activate();
        if (PlayerData.GetInstance != null)
            if (PlayerData.GetInstance.IsGpgsLogin)
                LoginGPGS(null);
#endif
#if UNITY_IOS
#endif
        /*
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                bLogin = true;
            }
            else
            {
            }
        });
        */
    }

    public void LoginGPGS(Action<bool> callBackFunction)
    {
#if UNITY_ANDROID
        if (!Social.localUser.authenticated)
        {
            var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = false;
            PlayGamesPlatform.Activate();
            Debug.Log(config + " config");
            //if (callBackFunction != null)
            {
                Social.localUser.Authenticate(LoginCallbackGPGS);
            }
        }
#endif
#if UNITY_IOS
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                bLogin = true;
            }
            else
            {
            }
        });
#endif
    }

    public void LoginCallbackGPGS(bool result)
    {
#if UNITY_ANDROID
        bLogin = result;
        if (bLogin)
            if (PlayerData.GetInstance != null)
                PlayerData.GetInstance.IsGpgsLogin = true;
        if (!Social.localUser.authenticated) PlayerData.GetInstance.IsGpgsLogin = false;
#endif
    }

    public void LogoutGPGS()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
            bLogin = false;
            if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsGpgsLogin = false;
        }
#endif
    }

    public string GetNameGPGS()
    {
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
            return Social.localUser.userName;
        return null;
#endif
        return null;
    }

    public void ReportHighStage(int stage)
    {
#if UNITY_ANDROID
        if (bLogin)
        {
            // PlayGamesPlatform.Instance.ReportScore(stage, GPGSIds.leaderboard_the_best_score, (bool success) =>
            // {
            //     if (success)
            //     {
            //
            //     }
            //     else
            //     {
            //     }
            // });
        }
#endif
#if UNITY_IOS
        if (bLogin)
        {
            Social.ReportScore(stage, "com..ios.jewelsfantasypirate.leaderboard01", (bool success) =>
            {
                if (success)
                {
                }
                else
                {
                    Social.localUser.Authenticate((bool succ) =>
                    {
                        if (succ)
                        {
                            bLogin = true;
                            Social.ReportScore(stage, "com..ios.jewelsfantasypirate.leaderboard01",
                                               (bool successful) =>
                            {
                            });
                        }
                    });
                }
            });
        }
#endif
    }

    public void ShowLeaderboardUI()
    {
        UpdateRecord();
#if UNITY_ANDROID
        if (bLogin) Social.ShowLeaderboardUI();
        else
            LoginGPGS(LeaderboardCallback);
        //Social.ShowLeaderboardUI();
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
#endif
#if UNITY_IOS
        if (bLogin) Social.ShowLeaderboardUI();
        else
        {
            LoginGPGS(LeaderboardCallback);
        }
        //Social.ShowLeaderboardUI();
#endif
    }

    public void LeaderboardCallback(bool result)
    {
        bLogin = result;
        if (bLogin) Social.ShowLeaderboardUI();
    }

    public void AchievementCallback(bool result)
    {
        bLogin = result;
        if (bLogin) Social.ShowAchievementsUI();
    }

    public void UpdateRecord()
    {
        ReportHighStage(StageManager.StageNumber);
        SetAchievementString(StageManager.StageNumber);

        var starValue = 0;
        for (var i = 1; i <= StageManager.StageNumber; i++) starValue += PlayerData.GetInstance.GetLevelStartCount(i);
        SetAchievementStarString(starValue);
    }

    public void ShowAchievementUI()
    {
        UpdateRecord();
#if UNITY_ANDROID
        if (bLogin) Social.ShowAchievementsUI();
        else
            LoginGPGS(AchievementCallback);
        //Social.ShowLeaderboardUI();
        PlayGamesPlatform.Instance.ShowAchievementsUI();
#endif
#if UNITY_IOS
        if (bLogin) Social.ShowAchievementsUI();
        else
        {
            LoginGPGS(AchievementCallback);
            //Social.ShowLeaderboardUI();
        }
#endif
    }

    public void ReportProgress(string name, double progress)
    {
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ReportProgress(name, 100f, null);
#elif UNITY_IOS
            Social.ReportProgress(name, 100, null);
#endif
    }

    public void SetAchievementString(int num)
    {
#if UNITY_ANDROID
        switch (num)
        {
            case 10:
                ReportProgress(GPGSIds.achievement_clear_10_level, 100);
                break;
            case 50:
                ReportProgress(GPGSIds.achievement_clear_50_level, 100);
                break;
            case 100:
                ReportProgress(GPGSIds.achievement_clear_100_level, 100);
                break;
            case 300:
                ReportProgress(GPGSIds.achievement_clear_300_level, 100);
                break;
            case 500:
                ReportProgress(GPGSIds.achievement_clear_500_level, 100);
                break;
            case 700:
                ReportProgress(GPGSIds.achievement_clear_700_level, 100);
                break;
            case 1000:
                ReportProgress(GPGSIds.achievement_clear_1000_level, 100);
                break;
            case 1500:
                ReportProgress(GPGSIds.achievement_clear_1500_level, 100);
                break;
            case 2000:
                ReportProgress(GPGSIds.achievement_clear_2000_level, 100);
                break;
            case 2500:
                ReportProgress(GPGSIds.achievement_clear_2500_level, 100);
                break;
            case 3000:
                ReportProgress(GPGSIds.achievement_clear_3000_level, 100);
                break;
        }
#endif
#if UNITY_IOS
        switch (num)
        {
           case 10:
               ReportProgress("achievement_clear_10_level_IOS", 100);
                break;
            case 50:
                ReportProgress("achievement_clear_50_level_IOS", 100);
                break;
            case 100:
                ReportProgress("achievement_clear_100_level_IOS", 100);
                break;
            case 300:
                ReportProgress("achievement_clear_300_level_IOS", 100);
                break;
            case 500:
                ReportProgress("achievement_clear_500_level_IOS", 100);
                break;
            case 700:
                ReportProgress("achievement_clear_700_level_IOS", 100);
                break;
            case 1000:
                ReportProgress("achievement_clear_1000_level_IOS", 100);
                break;
            case 1500:
                ReportProgress("achievement_clear_1500_level_IOS", 100);
                break;
            case 2000:
                ReportProgress("achievement_clear_2000_level_IOS", 100);
                break;
            case 2500:
                ReportProgress("achievement_clear_2500_level_IOS", 100);
                break;
            case 3000:
                ReportProgress("achievement_clear_3000_level_IOS", 100);
                break;
        }
#endif
    }

    public void SetAchievementStarString(int num)
    {
#if UNITY_ANDROID
        switch (num)
        {
            case 100:
                ReportProgress(GPGSIds.achievement_collect_100_stars, 100);
                break;
            case 200:
                ReportProgress(GPGSIds.achievement_collect_200_stars, 100);
                break;
            case 300:
                ReportProgress(GPGSIds.achievement_collect_300_stars, 100);
                break;
            case 500:
                ReportProgress(GPGSIds.achievement_collect_500_stars, 100);
                break;
            case 700:
                ReportProgress(GPGSIds.achievement_collect_700_stars, 100);
                break;
            case 900:
                ReportProgress(GPGSIds.achievement_collect_900_stars, 100);
                break;
            case 1000:
                ReportProgress(GPGSIds.achievement_collect_1000_stars, 100);
                break;
            case 1500:
                ReportProgress(GPGSIds.achievement_collect_1500_stars, 100);
                break;
            case 2000:
                ReportProgress(GPGSIds.achievement_collect_2000_stars, 100);
                break;
            case 3000:
                ReportProgress(GPGSIds.achievement_collect_3000_stars, 100);
                break;
        }
#endif
#if UNITY_IOS
        switch (num)
        {
             case 100:
                ReportProgress("achievement_collect_100_stars_IOS", 100);
                break;
            case 200:
                ReportProgress("achievement_collect_200_stars_IOS", 100);
                break;
            case 300:
                ReportProgress("achievement_collect_300_stars_IOS", 100);
                break;
            case 500:
                ReportProgress("achievement_collect_500_stars_IOS", 100);
                break;
            case 700:
                ReportProgress("achievement_collect_700_stars_IOS", 100);
                break;
            case 900:
                ReportProgress("achievement_collect_900_stars_IOS", 100);
                break;
            case 1000:
                ReportProgress("achievement_collect_1000_stars_IOS", 100);
                break;
            case 1500:
                ReportProgress("achievement_collect_1500_stars_IOS", 100);
                break;
            case 2000:
                ReportProgress("achievement_collect_2000_stars_IOS", 100);
                break;
            case 3000:
                ReportProgress("achievement_collect_3000_stars_IOS", 100);
                break;
        }
#endif
    }
}