using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TimerController;

public class TimeCall : MonoBehaviour
{
    public int count = 100;

    Button prefab;
    int i;

    Dictionary<Button, Timer> timerDic = new Dictionary<Button, Timer>();
    Dictionary<Button, Text> textDic = new Dictionary<Button, Text>();

    private void Awake()
    {
        prefab = Resources.Load<Button>("Button");
    }

    private void Start()
    {
        i = -1;
        while (++i < count)
        {
            Button btn = Instantiate(prefab, transform);
            Text text = btn.GetComponentInChildren<Text>();
            float duration = Random.Range(1f, 15f);
            float speed = 0.001f; Random.Range(0.001f, 2f);
            text.text = duration.ToString("f2") + "-" + speed;
            Timer timer = TimerController.Instance.StartTimer(
                duration,
                speed,
                currentTime => text.text = currentTime.ToString("f2"),
                () => text.text = "ÕÍ≥…"
            );
            timerDic.Add(btn, timer);
            textDic.Add(btn, text);
            btn.onClick.AddListener(() =>
            {
                Timer current = timerDic[btn];
                if (current.IsComplete)
                {
                    current.Reset();
                    textDic[btn].text = "÷ÿ÷√";
                }
                else
                {
                    current.Pause();
                    if (!current._isRunning)
                    {
                        textDic[btn].text = "‘›Õ£";
                    }
                }
            });
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("ALL", GUILayout.Width(200), GUILayout.Height(100)))
        {
            foreach (var item in timerDic.Keys)
            {
                item.onClick.Invoke();
            }
        }
        if (GUILayout.Button("Reset", GUILayout.Width(200), GUILayout.Height(100)))
        {
            foreach (var item in timerDic.Keys)
            {
                timerDic[item].Reset();
                textDic[item].text = "Reset";
            }
        }
    }
}
