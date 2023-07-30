using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerController : MonoBehaviour
{
    private static TimerController _instance;
    public static TimerController Instance => _instance;

    List<Timer> _timerToAddList = new List<Timer>();
    //List<Timer> _timerToRemoveList = new List<Timer>();
    List<Timer> _timerList = new List<Timer>();

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        _instance = this;
    }

    private void Update()
    {
        int i = -1;
        if (_timerToAddList.Count > 0)
        {
            _timerList.AddRange(_timerToAddList);
            _timerToAddList.Clear();
        }

        while (++i < _timerList.Count)
        {
            _timerList[i].Tick();
            //if (timer.IsComplete) { _timerToRemoveList.Add(timer); }
        }
        //if (_timerToRemoveList.Count > 0)
        //{
        //    i = -1;
        //    while (++i < _timerToRemoveList.Count)
        //    {
        //        _timerList.Remove(_timerToRemoveList[i]);
        //    }
        //    _timerToRemoveList.Clear();
        //}
    }

    public Timer StartTimer(float duration, float speed, Action<float> onTick, Action onComplete, bool autoBegin = false)
    {
        Timer timer = new Timer()
        {
            _duration = duration,
            _speed = speed,
            _currentTime = duration,
            _onTick = onTick,
            _onComplete = onComplete,
        };
        _timerToAddList.Add(timer);
        if (autoBegin)
        {
            timer.Begin();
        }
        return timer;
    }

    public class Timer
    {
        public float _duration;
        public float _speed;
        public float _currentTime;
        public Action<float> _onTick;
        public Action _onComplete;
        public bool _isRunning;

        private float _elapsed;

        public bool IsComplete => _currentTime <= 0f;

        public void Begin()
        {
            //if (_currentTime <= 0)
            //{
            //    _currentTime = _duration;
            //}
            _isRunning = true;
        }

        public void Pause()
        {
            _isRunning = !_isRunning;
        }

        public void Reset()
        {
            _isRunning = false;
            _currentTime = _duration;
        }

        public void Tick()
        {
            if (!_isRunning) { return; }

            _elapsed += Time.deltaTime;
            while (_elapsed >= _speed)
            {
                _elapsed -= _speed;
                _currentTime -= _speed;
                _onTick?.Invoke(_currentTime);
            }

            if (_currentTime <= 0f)
            {
                _isRunning = false;
                _onComplete?.Invoke();
            }
        }
    }
}
