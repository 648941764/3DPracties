using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignSystem : MonoBehaviour
{
    private const string previousSignInTime = "PreviousSinInTime";//签到的时间
    private const string signInCount = "signInCount"; //签到的次数

    private DateTime _toDay;
    private DateTime _lastDay; //上次签到的时间
    private TimeSpan _interval;//时间间隔

    private int _signInCount;//签到次数
    private int _maxSignInCount = 7;//最大签到次数
    private bool _showNextSignInTime; //是否显示下一次签到时间
    private bool _isMaxSignInCount; //是否达到最签到次数

    public Toggle[] _isSignInToggleTips;//是否已经签到了; 
    public Button _signInBtn;
    public Text _signInBtnCount; // 显示时间

    public Award[] awards;
    public Image[] images;
    private void Awake()
    {
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
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(previousSignInTime, DateTime.MinValue.ToString()));
        _signInCount = PlayerPrefs.GetInt(signInCount, 0);
        UpdateUI();

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
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _signInCount; i++)
        {
            _isSignInToggleTips[i].isOn = true;
            images[i].color = new Color(0.4811321f, 0.4811321f, 0.4811321f);
        }
    }

   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.DeleteKey(previousSignInTime);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.DeleteKey(signInCount);
        }

        if (_isMaxSignInCount)
        {
            return;
        }

        if (_showNextSignInTime)
        {
            _interval = _toDay.AddDays(1).Date - DateTime.Now;
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
        //给奖励
        int currentNum = _signInCount;
        Award currenAward = awards[currentNum -1];
        BackpackManager.Instance.AddItem(currenAward.id, currenAward.amount);//把奖励物品添加到背包

        PlayerPrefs.SetString(previousSignInTime, DateTime.Now.ToString());
        PlayerPrefs.SetInt(signInCount, _signInCount);
        Debug.Log("签到成功");
        _showNextSignInTime = true;
        _signInBtn.interactable = false;

        UpdateUI();
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
