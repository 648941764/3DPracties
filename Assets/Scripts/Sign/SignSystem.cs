using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignSystem : MonoBehaviour
{
    private const string previousSignInTime = "PreviousSinInTime";//ǩ����ʱ��
    private const string signInCount = "signInCount"; //ǩ���Ĵ���

    private DateTime _toDay;
    private DateTime _lastDay; //�ϴ�ǩ����ʱ��
    private TimeSpan _interval;//ʱ����

    private int _signInCount;//ǩ������
    private int _maxSignInCount = 7;//���ǩ������
    private bool _showNextSignInTime; //�Ƿ���ʾ��һ��ǩ��ʱ��
    private bool _isMaxSignInCount; //�Ƿ�ﵽ��ǩ������

    public Toggle[] _isSignInToggleTips;//�Ƿ��Ѿ�ǩ����; 
    public Button _signInBtn;
    public Text _signInBtnCount; // ��ʾʱ��

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
            //����ǩ��
            _signInBtn.interactable = true;
            _signInBtnCount.text = "����ǩ��";
        }
        else
        {
            //������ǩ��
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
                _signInBtnCount.text = "����ǩ��";
                _showNextSignInTime = false;
            }
        }
    }

    public void OnSignInBtnClick()
    {
        _signInCount++;
        //������
        int currentNum = _signInCount;
        Award currenAward = awards[currentNum -1];
        BackpackManager.Instance.AddItem(currenAward.id, currenAward.amount);//�ѽ�����Ʒ��ӵ�����

        PlayerPrefs.SetString(previousSignInTime, DateTime.Now.ToString());
        PlayerPrefs.SetInt(signInCount, _signInCount);
        Debug.Log("ǩ���ɹ�");
        _showNextSignInTime = true;
        _signInBtn.interactable = false;

        UpdateUI();
    }

    //�Ƿ����ǩ��
    private bool IsCanSignIn()
    {
        if (_signInCount >= _maxSignInCount)
        {
            _signInBtn.interactable = false;
            _signInBtnCount.text = "�ﵽ���ǩ����";

            _isMaxSignInCount = true;
            return false;
        }

        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return true;
        }
        return false;
    }
    #region ����ʱ��תʱ���
    //����ʱ��תʱ���
    //private long LocalTimeToTimeStamp(DateTime localTime)
    //{
    //    return ((DateTimeOffset)localTime).ToUnixTimeMilliseconds();
    //}

    ////ʱ���ת����ʱ��

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
