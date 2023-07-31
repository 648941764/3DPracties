using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Test;

public class TestCall : MonoBehaviour
{
    public int count = 100;
    Button prefab;
    int i = -1;
    Dictionary<Button, IncreaseTime> timeDic = new Dictionary<Button, IncreaseTime>();
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
            float maxTime = Random.Range(10f, 13f);
            float speed = 0.001f;
            text.text = maxTime.ToString("f2") + "-" + speed;
            IncreaseTime timer = Test.instance.StartTime
                (
                maxTime,
                speed,
                currentTime => text.text = currentTime.ToString("F2"),
                () => text.text = "ÕÍ≥…"
                );
            timeDic.Add(btn, timer);
            textDic.Add(btn, text);

            btn.onClick.AddListener(() =>
            {
                IncreaseTime current = timeDic[btn];
                if (current.IsComplete)
                {
                    current.Reset();
                    textDic[btn].text = "÷ÿ÷√";
                }
                else
                {
                    current.Pause();
                    if (!current.IsRunning)
                    {
                        textDic[btn].text = "‘›Õ£";
                    }
                }

            });
        }
      
    }
}
