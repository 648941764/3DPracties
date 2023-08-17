using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public class SignDate
    {
        public string previousSignInTime;
        public int signInCount;
        public string lastSignInDate;
    }

    private List<bool> _signInToggleStatus = new List<bool> { false, false, false, false, false, false, false };
    private const string SavePath = "SaveData.json";
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("DataManager");
                _instance = go.AddComponent<DataManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveData(string previousSignInTime, int signInCount, string lastSignInDate)
    {
        SignDate data = new SignDate
        {
            previousSignInTime = previousSignInTime,
            signInCount = signInCount,
            lastSignInDate = lastSignInDate
        };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SavePath), json);
    }

    public SignDate LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, SavePath);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<SignDate>(json);
        }
        return null;
    }

    public void ResetData()
    {
        string path = Path.Combine(Application.persistentDataPath, SavePath);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void SaveSignInToggleStatus(List<bool> status)
    {
        _signInToggleStatus = status;
        string json = JsonConvert.SerializeObject(_signInToggleStatus);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "SignInToggleStatus.json"), json);
    }

    public List<bool> LoadSignInToggleStatus()
    {
        string path = Path.Combine(Application.persistentDataPath, "SignInToggleStatus.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _signInToggleStatus = JsonConvert.DeserializeObject<List<bool>>(json);
            return _signInToggleStatus;
        }
        return new List<bool>();
    }
    public void ResetAllSignInToggle()
    {
        _signInToggleStatus.Clear();
        List<bool> currentList = new List<bool> { false, false, false, false, false, false, false };
        SaveSignInToggleStatus(currentList);
    }
}




