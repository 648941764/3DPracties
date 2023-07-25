using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCall : MonoBehaviour
{
    public Text textUi;
    public Button StartBtn;
    public Button PauseBtn;
    public Button ResetBtn;
    public TimerController t1;
    private float currenTime;
    private void Start()
    {
        t1 = new TimerController(10f, 1f, textUi, "Ò»ºÅ");
        //currenTime = TimerController.instance._currentTime;
        textUi.text = currenTime.ToString("F2");

    }
}
