using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<IncreaseTime> increaseList = new List<IncreaseTime>();
    public List<IncreaseTime> increaToAddList = new List<IncreaseTime>();
    public static Test instance;

    public static Test Instance=> instance;
    //{
    //    get { return instance; }
    //}

    private void Update()
    {
        int i = -1;
        if (increaToAddList.Count > 0)
        {
            increaseList.AddRange(increaToAddList);
            increaToAddList.Clear();
        }

        while (++i < increaseList.Count)
        {
            increaseList[i].Tick();
        }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public IncreaseTime StartTime(float maxTime,float speed,Action<float> onTick,Action complete, bool autoBegin = false)
    {
        IncreaseTime time = new IncreaseTime()
        {
            _maxTime = maxTime,
            _currentTime = 0f,
            _speed = speed,
            _onTick = onTick,
            _onComplete = complete,
        };
        increaToAddList.Add(time);
        if (autoBegin)
        {
            time.Begin();
        }
        return time;
    }

    public class IncreaseTime
    {
        public float _maxTime;
        public float _currentTime;
        public float _speed;
        public Action<float> _onTick;
        public Action _onComplete;
        public float _elapsed;
        public bool IsRunning;
        public bool IsComplete => _currentTime >= _maxTime;

        public void Begin()
        {
            IsRunning = true;
        }

        public void Pause()
        {
            IsRunning = !IsRunning;
        }

        public void Reset()
        {
            IsRunning = false;
            _currentTime = 0f;
        }

        public void Tick()
        {
            if (!IsRunning)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            while(_elapsed >= _speed)
            {
                _elapsed -= _speed;
                _currentTime += _speed;
                _onTick?.Invoke(_currentTime);
            }

            if (_currentTime >= _maxTime)
            {
                IsRunning = false;
                _onComplete?.Invoke();
            }
        }
    }
}
