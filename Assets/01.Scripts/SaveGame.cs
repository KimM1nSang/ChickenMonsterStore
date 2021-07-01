using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;
using Newtonsoft.Json.Bson;

public class SaveGame : MonoBehaviour
{
    //싱글톤====================
    static GameObject _container;
    static GameObject Container
    {
        get
        {
            return _container;
        }
    }
    static SaveGame _instance;
    public static SaveGame Instance
    {
        get
        {
            if (!_instance)
            {
                _container = new GameObject();
                _container.name = "SaveGame";
                _instance = _container.AddComponent(typeof(SaveGame)) as SaveGame;
                DontDestroyOnLoad(_container);
            }
            return _instance;
        }
    }
    // =================================================
    public string GameDataFileName = ".json";

    private string filePath = "";


    private SaveData _data;

    public SaveData data
    {
        get
        {
            if (_data == null)
            {
                LoadGameData();
                SaveGameData();
            }
            return _data;
        }
    }
    string key = "awesomekey";

    private void Awake()
    {
        filePath = string.Concat(Application.persistentDataPath, GameDataFileName);
        Debug.Log(filePath);
    }

    public void NewGameData()
    {
        Debug.Log("새로운 파일 생성");
        _data = new SaveData();
        data.money = 200;
    }

    public static string Decrypt(string textToDecrypt, string key)

    {

        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;

        rijndaelCipher.Padding = PaddingMode.PKCS7;



        rijndaelCipher.KeySize = 128;

        rijndaelCipher.BlockSize = 128;

        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)

        {

            len = keyBytes.Length;

        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;

        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(plainText);

    }


    public static string Encrypt(string textToEncrypt, string key)

    {

        RijndaelManaged rijndaelCipher = new RijndaelManaged();

        rijndaelCipher.Mode = CipherMode.CBC;

        rijndaelCipher.Padding = PaddingMode.PKCS7;



        rijndaelCipher.KeySize = 128;

        rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);

        byte[] keyBytes = new byte[16];

        int len = pwdBytes.Length;

        if (len > keyBytes.Length)

        {

            len = keyBytes.Length;

        }

        Array.Copy(pwdBytes, keyBytes, len);

        rijndaelCipher.Key = keyBytes;

        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);

        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));

    }

    public void LoadGameData()
    {
        if (File.Exists(filePath))
        {
            Debug.Log("불러오기");
            string FromJsonData = File.ReadAllText(filePath);

            string load = Decrypt(FromJsonData, key);

            _data = JsonUtility.FromJson<SaveData>(load);
        }
        else
        {
            NewGameData();
        }

    }

    public void SaveGameData()
    {
        string ToJsonData = JsonUtility.ToJson(data, true);
        Debug.Log("저장완료");
        string save = Encrypt(ToJsonData, key);

        File.WriteAllText(filePath, save);
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause()
    {
        SaveGameData();
    }
}
