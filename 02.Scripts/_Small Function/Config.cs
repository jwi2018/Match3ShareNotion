using UnityEngine;

public class Config : MonoBehaviour
{
#if UNITY_ANDROID
    public static string Admob_ID = "ca-app-pub-3636561524673964~7831156620";
    public static string Admob_Reward_Shop = "ca-app-pub-3636561524673964/5013421590";
    public static string Admob_Reward_Result = "ca-app-pub-3636561524673964/5344594080";
    public static string Admob_Reward_Main = "ca-app-pub-3636561524673964/7448013245";
    public static string Admob_Reward_Roulette = "ca-app-pub-3636561524673964/9283839094";
    public static string Admob_Reward_InGamePlayItem = "ca-app-pub-3636561524673964/2195686568";
    public static string Admob_Reward_InGamePlayMove = "ca-app-pub-3636561524673964/6134931575";
    public static string Admob_Reward_ContinueStage = "ca-app-pub-3636561524673964/3046448780";
    public static string Admob_FrontBanner_Play = "ca-app-pub-3636561524673964/4630278215";
    public static string Admob_FrontBanner_Stage = "ca-app-pub-3636561524673964/4630278215";
    public static string Admob_FrontBanner_PlayToStage = "ca-app-pub-3636561524673964/9092267404";
    public static string Admob_FrontBanner_ContinueStage = "ca-app-pub-3636561524673964/2718430745";
    public static string Admob_Banner_Play = "ca-app-pub-3636561524673964/2004114875";
    public static string Admob_Banner_End = "ca-app-pub-3636561524673964/9691033204";
#elif UNITY_IOS
    public static string Admob_ID = "ca-app-pub-3636561524673964~2335287366";
    public static string Admob_Reward_Shop = "ca-app-pub-3636561524673964/8517552332";
    public static string Admob_Reward_Result = "ca-app-pub-3636561524673964/3457082753";
    public static string Admob_Reward_Main = "ca-app-pub-3636561524673964/8325980649";
    public static string Admob_Reward_Roulette = "ca-app-pub-3636561524673964/3265225653";
    public static string Admob_Reward_InGamePlayItem = "ca-app-pub-3636561524673964/7012898978";
    public static string Admob_Reward_InGamePlayMove = "ca-app-pub-3636561524673964/5891674405";
    public static string Admob_Reward_ContinueStage = "ca-app-pub-3636561524673964/3238020470";
    public static string Admob_FrontBanner_Play = "ca-app-pub-3636561524673964/9639347720";
    public static string Admob_FrontBanner_Stage = "ca-app-pub-3636561524673964/9639347720";
    public static string Admob_FrontBanner_PlayToStage = "ca-app-pub-3636561524673964/1760572291";
    public static string Admob_FrontBanner_ContinueStage = "ca-app-pub-3636561524673964/7820861410";
    public static string Admob_Banner_Play = "ca-app-pub-3636561524673964/8326266054";
    public static string Admob_Banner_End = "ca-app-pub-3636561524673964/8134408954";
#else
    public static string Admob_ID = "ca-app-pub-3636561524673964~7831156620";
    public static string Admob_Reward_Shop = "ca-app-pub-3636561524673964/5013421590";
    public static string Admob_Reward_Result = "ca-app-pub-3636561524673964/5344594080";
    public static string Admob_Reward_Main = "ca-app-pub-3636561524673964/7448013245";
    public static string Admob_Reward_Roulette = "ca-app-pub-3636561524673964/9283839094";
    public static string Admob_Reward_InGamePlayItem = "ca-app-pub-3636561524673964/2195686568";
    public static string Admob_Reward_InGamePlayMove = "ca-app-pub-3636561524673964/6134931575";
    public static string Admob_Reward_ContinueStage = "ca-app-pub-3636561524673964/3046448780";
    public static string Admob_FrontBanner_Play = "ca-app-pub-3636561524673964/4630278215";
    public static string Admob_FrontBanner_Stage = "ca-app-pub-3636561524673964/4630278215";
    public static string Admob_FrontBanner_PlayToStage = "ca-app-pub-3636561524673964/9092267404";
    public static string Admob_FrontBanner_ContinueStage = "ca-app-pub-3636561524673964/2718430745";
    public static string Admob_Banner_Play = "ca-app-pub-3636561524673964/2004114875";
    public static string Admob_Banner_End = "ca-app-pub-3636561524673964/9691033204";
#endif
}