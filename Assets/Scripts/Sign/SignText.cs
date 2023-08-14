using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignText : MonoBehaviour
{
    private const string signInTime = "signInTim";
    private const string signInCount = "signInCount";

    private DateTime _toDay;
    private DateTime _lastDay;
    private TimeSpan _interval;

    private int _signInCount;
    private int _maxSignInCount = 7;

    public Button signInBtn;
    public Text signInBtnText;
    public Toggle[] toggles;

    private bool showNextSignInTime;//��ʾ��һ��ǩ��ʱ��
    private bool isMaxSignCount;

    private void OnEnable()
    {
        signInBtn.onClick.AddListener(OnSignInBtnOnClicked);
        UpdateUI();
        _toDay = DateTime.Now;
        _lastDay = DateTime.Parse(PlayerPrefs.GetString(signInTime, DateTime.MinValue.ToString()));
        _signInCount = PlayerPrefs.GetInt(signInCount, 0);

        if (isCanSignIn())
        {
            signInBtn.interactable = true;
            signInBtnText.text = "����ǩ��";
        }
        else
        {
            showNextSignInTime = true;
            signInBtn.interactable = false;
        }
    }

    private void Update()
    {
        if (isMaxSignCount)
        {
            return;
        }

        if (showNextSignInTime)
        {
            _interval = _toDay.AddDays(1).Date - DateTime.Now;// _lastDay.AddDays(1).Date�ǻ�ȡǩ��ʱ���ĵڶ����0��0��0�룬Ҳ���ǻ��ж�ò��ܹ����еڶ���ǩ��(��Ҫ��õ���ڶ���)
            if (_interval >= TimeSpan.Zero)
            {
                signInBtnText.text = $"{_interval.Hours}:{_interval.Minutes}:{_interval.Seconds}";
            }
            else
            {
                signInBtn.interactable = true;
                signInBtnText.text = "����ǩ��";
                showNextSignInTime = false;
            }
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _signInCount; i++)
        {
            toggles[i].isOn = true;
        }
    }

    public void OnSignInBtnOnClicked()
    {
        //���Ž���
        _signInCount++;
        PlayerPrefs.GetString(signInTime, DateTime.Now.ToString());
        PlayerPrefs.GetInt(signInCount, _signInCount);
        showNextSignInTime = true;
        signInBtn.interactable = false;
        UpdateUI();
    }

    private bool isCanSignIn()
    {
        if (_signInCount >= _maxSignInCount)
        {
            signInBtn.interactable = false;
            signInBtnText.text = "�ﵽ���ǩ����";
            isMaxSignCount = true;
            return false;
        }

        if (_lastDay.Year <= _toDay.Year && _lastDay.Month <= _toDay.Month && _lastDay.Day < _toDay.Day)
        {
            return true;
        }
        return false;
    }
}
