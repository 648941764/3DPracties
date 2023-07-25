using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{

    public static TimerController instance;

    public static TimerController Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }
    //public Text uiText;
    //public Text uiText2;
    //public Button startBtn, pauseBtn, resetBtn;
    //private TimeControll t1,t2;

    private void Start()
    {
        //t2 = new TimeControll(12, 2, uiText2,"计时器二号");

        //t1 = new TimeControll(10, 1.00f,uiText,"计时器一号");
        //uiText.text = string.Format(t1._currentTime.ToString("F2"));
        //uiText2.text = string.Format(t1._currentTime.ToString("F2"));
        //startBtn.onClick.AddListener(OnstartBtnClick);
        //pauseBtn.onClick.AddListener(OnPauseBtnClick);
        //resetBtn.onClick.AddListener(OnResetBtnClick);
    }

    private void Update()
    {

    }
    //public void OnstartBtnClick()
    //{
    //    if ( t1.isTimerRunning == false)
    //    {
    //        //StartCoroutine(t1.Timer());
    //        //StartCoroutine(t2.Timer());
    //        t1.Begin();
    //        t2.Begin();
    //        t1.isTimerRunning = true;
    //        t2.isTimerRunning = true;
    //    }
    //}

    //public void OnPauseBtnClick()
    //{
    //    //StopCoroutine(t1.Timer());
    //    //StopCoroutine(t2.Timer());
    //    t1.Pause();
    //    t2.Pause();
    //}

    //public void OnResetBtnClick()
    //{
    //    t1.Reset();
    //    t2.Reset();
    //    uiText.text = string.Format(t1._currentTime.ToString("F2"));
    //    uiText2.text = string.Format(t2._currentTime.ToString("F2"));
    //    t1.isTimerRunning = false;
    //    t2.isTimerRunning = false;
    //}

    private Text _timerText;
    private float _duration;
    private float _currentTime;
    private bool _isRunning;
    private float _speed;
    private string _method;
    private bool isTimerRunning = false;
    public TimerController(float duration, float speed, Text timerText, string method)
    {
        _speed = speed;
        _duration = duration;
        _currentTime = _duration;
        _isRunning = false;
        _timerText = timerText;
        _method = method;
        _timerText.text = string.Format(duration.ToString("F2"));
    }

    public void Begin()
    {
        if (_currentTime <= 0)
        {
            _currentTime = _duration;
        }
        _isRunning = true;
    }

    public void Pause()
    {
        _isRunning = !_isRunning;
        if (_currentTime <= 0)
        {
            _timerText.text = string.Format("倒计时结束");
        }
        //else
        //{
        //    isRunning = !isRunning;
        //}
    }

    public void Reset()
    {
        _isRunning = false;
        _currentTime = _duration;
    }

    public void Timer()
    {
        _currentTime = _duration;
        _isRunning = true;
        if (_currentTime >= 0 && _isRunning == true)
        {
            _currentTime -= _speed * Time.deltaTime;
        }
        _timerText.text = string.Format("倒计时结束");
        _isRunning = false;
    }

    public string Method()
    {
        return _method;
    }

}
