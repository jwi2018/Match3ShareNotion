using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class EncryptedPlayerPrefs
{
    // Encrypted PlayerPrefs
    // Written by Sven Magnus
    // MD5 code by Matthew Wegner (from [url]http://www.unifycommunity.com/wiki/index.php?title=MD5[/url])


    // Modify this key in this file :
    private static readonly string privateKey = "9ETrEsWaFRach3gexaDr";

    // Add some values to this array before using EncryptedPlayerPrefs
    public static string[] keys;


    public static string Md5(string strToEncrypt)
    {
        var ue = new UTF8Encoding();
        var bytes = ue.GetBytes(strToEncrypt);

        var md5 = new MD5CryptoServiceProvider();
        var hashBytes = md5.ComputeHash(bytes);

        var hashString = "";

        for (var i = 0; i < hashBytes.Length; i++) hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');

        return hashString.PadLeft(32, '0');
    }

    public static void SaveEncryption(string key, string type, string value)
    {
        var keyIndex = (int) Mathf.Floor(Random.value * keys.Length);
        var secretKey = keys[keyIndex];
        var check = Md5(type + "_" + privateKey + "_" + secretKey + "_" + value);
        PlayerPrefs.SetString(key + "_encryption_check", check);
        PlayerPrefs.SetInt(key + "_used_key", keyIndex);
        PlayerPrefs.Save();
    }

    public static bool CheckEncryption(string key, string type, string value)
    {
        var keyIndex = PlayerPrefs.GetInt(key + "_used_key");
        var secretKey = keys[keyIndex];
        var check = Md5(type + "_" + privateKey + "_" + secretKey + "_" + value);
        if (!PlayerPrefs.HasKey(key + "_encryption_check")) return false;
        var storedCheck = PlayerPrefs.GetString(key + "_encryption_check");
        return storedCheck == check;
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        SaveEncryption(key, "int", value.ToString());
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        SaveEncryption(key, "float", Mathf.Floor(value * 1000).ToString());
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        SaveEncryption(key, "string", value);
    }

    public static int GetInt(string key)
    {
        return GetInt(key, 0);
    }

    public static float GetFloat(string key)
    {
        return GetFloat(key, 0f);
    }

    public static string GetString(string key)
    {
        return GetString(key, "");
    }

    public static int GetInt(string key, int defaultValue)
    {
        var value = PlayerPrefs.GetInt(key);
        if (!CheckEncryption(key, "int", value.ToString())) return defaultValue;
        return value;
    }

    public static float GetFloat(string key, float defaultValue)
    {
        var value = PlayerPrefs.GetFloat(key);
        if (!CheckEncryption(key, "float", Mathf.Floor(value * 1000).ToString())) return defaultValue;
        return value;
    }

    public static string GetString(string key, string defaultValue)
    {
        var value = PlayerPrefs.GetString(key);
        if (!CheckEncryption(key, "string", value)) return defaultValue;
        return value;
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(key + "_encryption_check");
        PlayerPrefs.DeleteKey(key + "_used_key");
    }
}