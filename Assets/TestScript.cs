using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public BackPackSystem BackPackSystem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int item_id = Random.Range(1001, 1006);
            if (BackPackSystem.IsEmptyBackPack())
            {
                BackPackSystem.GetItem(item_id,Random.Range(0,5));
            }
        }
    }

}
