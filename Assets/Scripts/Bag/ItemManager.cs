using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static ItemData;

public class ItemManager : MonoBehaviour
{
    List<ItemData> dataList;
    public static ItemManager instance;
    public static ItemManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            LoadItmeData();
            DontDestroyOnLoad(gameObject);
        }
        

    }

    private void LoadItmeData()
    {
        string path = "Data/Item/Data";
        TextAsset database = Resources.Load<TextAsset>(path);
        string datas = database.text;
        string[] total_datas = datas.Split('\n');
        dataList = new List<ItemData>(total_datas.Length);
        foreach (var item in total_datas)
        {
            string[] data = item.Split('#');
            ItemData tpm = new ItemData();
            tpm.id = int.Parse(data[0]);
            tpm.name = data[1];
            tpm.icon = data[2];
            tpm.type = (ItemData.ItemType)int.Parse(data[3]);
            tpm.eType = (ItemData.EquimentType)int.Parse(data[4]);
            tpm.attaack = int.Parse(data[5]);
            tpm.defense = int.Parse(data[6]);
            tpm.speed = float.Parse(data[7]);
            tpm.hp = int.Parse(data[8]);
            tpm.mp = int.Parse(data[9]);
            tpm.description = data[10];
            dataList.Add(tpm);
        }
    }

    public ItemData GetRandomitem()
    {
        return dataList[UnityEngine.Random.Range(0, dataList.Count)];
    }

    public ItemData GetRadomNormalItem()
    {
        while (true)
        {
            var tmp = dataList[UnityEngine.Random.Range(0, dataList.Count)];
            if (tmp.type == ItemType.Normal)
            {
                return tmp;
            }
        }
    }

    public ItemData GetRadomEquimentItem()
    {
        while (true)
        {
            var tmp = dataList[UnityEngine.Random.Range(0, dataList.Count)];
            if (tmp.type == ItemType.Equipment)
            {
                return tmp;
            }
        }
    }

    public ItemData GetItemByID(int id)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].id == id)
            {
                return dataList[i];
            }
        }
        return null;
    }
}
