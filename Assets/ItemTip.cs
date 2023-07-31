using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTip : MonoBehaviour
{
    public Text contentText;
   public void SetCountent(string content)
    {
        contentText.text = content;
    }
}
