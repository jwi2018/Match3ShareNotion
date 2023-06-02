using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif
//for encoding
using System.Text;
//for extra save ui
using UnityEngine.SocialPlatforms;
//for text, remove
using UnityEngine.UI;

public class GpgsSaveManager : Singleton<GpgsSaveManager>
{
    //keep track of saving or loading during callbacks.
    private bool m_saving;
    //save name. This name will work, change it if you like.
    private string m_saveName = "game_save_name";
    //This is the saved file. Put this in seperate class with other variables for more advanced setup. Remember to change merging, toBytes and fromBytes for more advanced setup.
    private string saveString = "";

    //check with GPG (or other*) if user is authenticated. *e.g. GameCenter
    private bool Authenticated
    {
        get
        {
            return Social.Active.localUser.authenticated;
        }
    }
    //merges loaded bytearray with old save
    private void ProcessCloudData(byte[] cloudData)
    {
#if UNITY_ANDROID
        if (cloudData == null)
        {
            Debug.Log("No data saved to the cloud yet...");
            //SaveData.GetInstance.SetDebugMassage("No data saved to the cloud yet...");
            return;
        }
        Debug.Log("Decoding cloud data from bytes.");
        //SaveData.GetInstance.SetDebugMassage("Decoding cloud data from bytes.");
        string progress = FromBytes(cloudData);
        Debug.Log("Merging with existing game progress.");
        //SaveData.GetInstance.SetDebugMassage("Merging with existing game progress.");
        MergeWith(progress);
#endif
    }

    //load save from cloud
    public void LoadFromCloud(string filename)
    {
        m_saveName = filename;
        Debug.Log("Loading game progress from the cloud.");
        //SaveData.GetInstance.SetDebugMassage("Loading game progress from the cloud.");
        m_saving = false;
#if UNITY_ANDROID
        if (((PlayGamesPlatform)Social.Active).SavedGame != null)
        {
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            m_saveName, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            SavedGameOpened);
        }
        else
        {
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(3); //Text = Errer
            Debug.Log("loadfromcloud");
        }
#endif
    }
#if UNITY_ANDROID
    //overwrites old file or saves a new one
    public void SaveToCloud(string filename)
    {

        m_saveName = filename;
        if (Authenticated)
        {
            Debug.Log("Saving progress to the cloud... filename: " + m_saveName);
            //SaveData.GetInstance.SetDebugMassage("Saving progress to the cloud... filename: " + m_saveName);
            m_saving = true;
            //save to named file
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                m_saveName, //name of file. If save doesn't exist it will be created with this name
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SavedGameOpened);
        }
        else
        {
            Debug.Log("Not authenticated!");
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(2); //Text = Errer
            //SaveData.GetInstance.SetDebugMassage("Not authenticated!");
            Debug.Log("savefromcloud");
        }
    }
#endif

#if UNITY_ANDROID
    //save is opened, either save or load it.
    private void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {

        //check success
        if (status == SavedGameRequestStatus.Success)
        {
            //saving
            if (m_saving)
            {
                //read bytes from save
                byte[] data = ToBytes();
                //create builder. here you can add play time, time created etc for UI.
                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                //saving to cloud
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, data, SavedGameWritten);
                //loading
            }
            else
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
            }
            //error
        }
        else
        {
            //SaveData.GetInstance.LoadFail();
            Debug.LogWarning("Error opening game: " + status);
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(3); //Text = Errer
            //SaveData.GetInstance.SetDebugMassage("Error opening game: " + status);
            Debug.Log("savegameopened");
        }

    }

    //callback from SavedGameOpened. Check if loading result was successful or not.
    private void SavedGameLoaded(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("SaveGameLoaded, success=" + status);
            //SaveData.GetInstance.SetDebugMassage("SaveGameLoaded, success=" + status);
            ProcessCloudData(data);
        }
        else
        {
            Debug.LogWarning("Error reading game: " + status);
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(3); //Text = Errer
            //SaveData.GetInstance.SetDebugMassage("Error reading game: " + status);
            Debug.Log("savedgameloaded");
        }
    }

    //callback from SavedGameOpened. Check if saving result was successful or not.
    private void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(0);
            Debug.Log("Game " + game.Description + " written");
            //SaveData.GetInstance.SetDebugMassage("Game " + game.Description + " written");
            Debug.Log("savedgamewritten");
        }
        else
        {
            Debug.LogWarning("Error saving game: " + status);
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(2); //Text = Error
            //SaveData.GetInstance.SetDebugMassage("Error saving game: " + status);
            Debug.Log("savedgamewritten");
        }
    }
#endif
    //merge local save with cloud save. Here is where you change the merging betweeen cloud and local save for your setup.
    private void MergeWith(string other)
    {
#if UNITY_ANDROID
        if (other != "")
        {
            PlayerData.GetInstance.SetCloudData(JsonUtility.FromJson<SaveDataClass>(other));
            GameObject obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().CallTextLog(1);
            Debug.Log("mergewith");
            //LoadMng.GetInstance.WorkComplite();
        }
        else
        {
            Debug.Log("Loaded save string doesn't have any content");
            GameObject obj = GameObject.Find("PopupManager");
            obj.transform.GetChild(obj.transform.childCount - 1).GetComponent<PopupSetting>().OffPopupSetting();
            obj.GetComponent<PopupManager>().CallTextLog(3); //Text = Errer
            //SaveData.GetInstance.SetDebugMassage("Loaded save string doesn't have any content");
            //SaveData.GetInstance.LoadFail();
            //LoadMng.GetInstance.WorkComplite();
            Debug.Log("mergewith");
        }
#endif
    }

    //return saveString as bytes
    private byte[] ToBytes()
    {
#if UNITY_ANDROID
        saveString = PlayerData.GetInstance.ToJsonData();
        byte[] bytes = Encoding.UTF8.GetBytes(saveString);
        return bytes;
#endif
        return null;
    }

    //take bytes as arg and return string
    private string FromBytes(byte[] bytes)
    {
        string decodedString = Encoding.UTF8.GetString(bytes);
        return decodedString;
    }
}