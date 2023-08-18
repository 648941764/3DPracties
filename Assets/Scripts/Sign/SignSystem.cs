using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DataManager;

public class SignSystem : MonoBehaviour
{
    public Backpack backpack;
    private const string previousSignInTime = "PreviousSinInTime";//签到的时间
    private const string signInCount = "signInCount"; //签到的次数

    private DateTime _toDay;
    private DateTime _lastDay; //上次签到的时间
    private TimeSpan _interval;//时间间隔

    private int _signInCount;//签到次数
    private int _maxSignInCount = 7;//最大签到次数
    private bool _showNextSignInTime; //是否显示下一次签到时间
    private bool _isMaxSignInCount; //是否达到最签到次数
    private int _signInNum;
    private int num = -1;

    public Toggle[] _isSignInToggleTips;//是否已经签到了; 
    public Button _signInBtn;
    public Text _signInBtnCount; // 显示时间
    public Text signDayText;

    public Award[] awards;
    public Image[] images;
    
    List<bool> signInToggleStatus = new List<bool> { false, false, false, false, false, false, false };

    public DateTime GetLastDay()
    {
        SignDate cfg = DataManager.Instance.LoadData();
        if (cfg == null)
        {
            return DateTime.MinValue.Date;
        }
        else
        {
            DateTime currentDataTime = DateTime.Parse(cfg.previousSignInTime);
            return currentDataTime;
        }
    }

    public int GetSignCount()
    {
        SignDate cfg = DataManager.Instance.LoadData();
        if (cfg == null)
        {
            return 0;
        }
        else
        {
            int currentSignCount = cfg.signInCount;
            return currentSignCount;
        }
    }

    private void Awake()
    {
        DataManager.Instance.LoadData();
        //DataManager.Instance.SaveData(_lastDay.ToString(), _signInCount);
        SignDate loadedData = DataManager.Instance.LoadData();
        awards = new Award[]
        {
            new Award{id = 1003, amount = 1},
            new Award{id = 1001, amount = 5},
            new Award{id = 1002, amount = 3},
            new Award{id = 1005, amount = 1},
            new Award{id = 1004, amount = 1},
            new Award{id = 1001, amount = 5},
            new Award{id = 1002, amount = 7},
        };

        _signInBtn.onClick.AddListener(OnSignInBtnClick);
        _toDay = DateTime.Now;
        _lastDay = GetLastDay();//DateTime.Parse(loadedData.previousSignInTime);
        _signInCount = GetSignCount();//loadedData.signInCount;
         
        if (IsCanSignIn())
        {
            //可以签到
            _signInBtn.interactable = true;
            _signInBtnCount.text = "可以签到";
        }
        else
        {
            //不可以签到
            _showNextSignInTime = true;
            _signInBtn.interactable = false;
        }
        //遍历列表里的toggle哪些是已经签到了的
        signInToggleStatus = DataManager.Instance.LoadSignInToggleStatus();

        for (int i = 0; i < _isSignInToggleTips.Length && i < signInToggleStatus.Count; i++)
        {
            _isSignInToggleTips[i].isOn = signInToggleStatus[i];
        }


        for (int i = 0; i < _isSignInToggleTips.Length; ++i)
        {
            
            if (_isSignInToggleTips[i].isOn == true)
            {
                _signInNum++;
            }
        }
        signDayText.text =  _signInNum.ToString();
        //遍历toggle数组，找到最后一个toggle.isOn为true的
        int lastToggle = GetLastSignIn();
        for (int i = 0; i < lastToggle; i++)
        {
            images[i].color = new Color(0.490566f, 0.490566f, 0.490566f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            DataManager.Instance.ResetData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DataManager.Instance.ResetAllSignInToggle();
        }

        if (_isMaxSignInCount)
        {
            return;
        }

        if (_showNextSignInTime)
        {
            _interval = _toDay.AddDays(1).Date - DateTime.Now;//显示今天和明天还有多少时间差距

            if (_toDay.DayOfWeek == DayOfWeek.Monday)//每个周一的时候直接刷新数据
            {
                DataManager.Instance.ResetData();
                DataManager.Instance.ResetAllSignInToggle();
                return;
            }

            if (_interval >= TimeSpan.Zero)
            {
                _signInBtnCount.text = $"{_interval.Hours}:{_interval.Minutes}:{_interval.Seconds}";
            }
            else
            {
                _signInBtn.interactable = true;
                _signInBtnCount.text = "可以签到";
                _showNextSignInTime = false;
            }
        }
    }

    public void OnSignInBtnClick()
    {
        _signInCount++;
        num++;

        SignDate loadedData = DataManager.Instance.LoadData();
        DateTime currentDate = DateTime.Now.Date;
        DateTime lastSignInDate = GetLastDay();//DateTime.Parse(loadedData.lastSignInDate);
        int daysSinceLastSignIn = (int)currentDate.Subtract(lastSignInDate).TotalDays;

        if (daysSinceLastSignIn >= 0 && daysSinceLastSignIn < awards.Length)
        {
            int num = (int)currentDate.DayOfWeek - 1;
            Award currentAward = awards[num];
            BackpackManager.Instance.AddItem(currentAward.id, currentAward.amount);
            DataManager.Instance.SaveData(DateTime.Now.ToString(), _signInCount, currentDate.ToString());//保存签到数据
            _isSignInToggleTips[num].isOn = true;
            signInToggleStatus[num] = _isSignInToggleTips[num].isOn;
            DataManager.Instance.SaveSignInToggleStatus(signInToggleStatus);
        }
        else
        {
            int current = num; //(int)currentDate.DayOfWeek - 1;
            //current = current == 0 ? 6 : current;
            Award currentAward = awards[current];
            BackpackManager.Instance.AddItem(currentAward.id, currentAward.amount);
            backpack.ShowAll();
            DataManager.Instance.SaveData(DateTime.Now.ToString(), _signInCount, currentDate.ToString());//保存签到数据
            _isSignInToggleTips[current].isOn = true;
            //signInToggleStatus.Add(_isSignInToggleTips[current].isOn);
            signInToggleStatus[current] = _isSignInToggleTips[current].isOn;
            DataManager.Instance.SaveSignInToggleStatus(signInToggleStatus);

        }

        for (int i = 0; i < _isSignInToggleTips.Length; ++i)
        {

            if (_isSignInToggleTips[i].isOn == true)
            {
                _signInNum++;
            }
        }
        _lastDay = DateTime.Now;
        signDayText.text = _signInNum.ToString();
        Debug.Log("签到成功");
        _showNextSignInTime = true;
        _signInBtn.interactable = false;

        int lastToggle = GetLastSignIn();
        for (int i = 0; i < lastToggle; i++)
        {
            images[i].color = new Color(0.490566f, 0.490566f, 0.490566f);
        }
    }

    //是否可以签到
    private bool IsCanSignIn()
    {
        if (_signInCount >= _maxSignInCount)
        {
            _signInBtn.interactable = false;
            _signInBtnCount.text = "达到最大签到数";

            _isMaxSignInCount = true;
            return false;
        }

        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return true;
        }
        return false;
    }

    private int GetLastSignIn()
    {
        for (int i = _isSignInToggleTips.Length - 1; i >= 0; i--)
        {
            if (_isSignInToggleTips[i].isOn == true)
            {
                return i;
            }
        }
        return 0;
        
    }
    #region 本地时间转时间戳
    //本地时间转时间戳
    //private long LocalTimeToTimeStamp(DateTime localTime)
    //{
    //    return ((DateTimeOffset)localTime).ToUnixTimeMilliseconds();
    //}

    ////时间戳转本地时间

    //private DateTime TimeStampToLocalTime(long timeStamp)
    //{
    //    return DateTime.UtcNow.AddSeconds(timeStamp).ToLocalTime();
    //}
    #endregion  
}
public class Award
{
    public int id;
    public int amount;
}