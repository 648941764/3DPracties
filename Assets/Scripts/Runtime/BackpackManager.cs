using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Item
{
    public int
        id, amount;
}

public sealed class BackpackManager
{
    public const int ITEM_SLOT_CTN = 50;
    // private static readonly Lazy<BackpackManager> sr_Lazy = new Lazy<BackpackManager>(() => _instance = new BackpackManager());
    private static BackpackManager _instance;
    public static BackpackManager Instance => _instance;

    static BackpackManager()
    {
        _instance = new BackpackManager();
    }

    private Item[] items;
    Dictionary<int, ItemData> itemCfg;

    private BackpackManager() 
    {
        items = new Item[ITEM_SLOT_CTN];
        itemCfg = new Dictionary<int, ItemData>();
        string path = "Data/Item/Data";
        TextAsset database = Resources.Load<TextAsset>(path);
        string datas = database.text;
        string[] total_datas = datas.Split('\n');
        foreach (var item in total_datas)
        {
            string[] data = item.Split('#');
            ItemData tmp = new ItemData();
            tmp.id = int.Parse(data[0]);
            tmp.name = data[1];
            tmp.icon = data[2];
            tmp.type = (ItemData.ItemType)int.Parse(data[3]);
            tmp.eType = (ItemData.EquimentType)int.Parse(data[4]);
            tmp.attaack = int.Parse(data[5]);
            tmp.defense = int.Parse(data[6]);
            tmp.speed = float.Parse(data[7]);
            tmp.hp = int.Parse(data[8]);
            tmp.mp = int.Parse(data[9]);
            tmp.description = data[10];
            itemCfg.Add(tmp.id, tmp);
        }
    }

    public Item GetItem(int id)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i].id == id)
            {
                return items[i];
            }
        }

        return null;
    }

    public Item GetItemByIndex(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            return items[index];
        }
        return null;
    }

    public void AddItem(Item item)
    {
        ItemData cfg = GetCfg(item.id);
        if (cfg != null)
        {
            int i = -1;
            switch (cfg.type)
            {
                case ItemData.ItemType.Equipment:
                {
                    int added = 0;
                    while (++i < ITEM_SLOT_CTN)
                    {
                        if (items[i] == null)
                        {
                            items[i] = new Item() { id = item.id, amount = 1 };
                            if (++added == item.amount)
                            {
                                break;
                            }
                        }
                    }
                    if (added < item.amount)
                    {
                        Debug.Log($"背包已满，有{item.amount - added}个未装入");
                    }
                    break;
                }
                case ItemData.ItemType.Normal:
                {
                    Item exist = null;
                    while (++i < ITEM_SLOT_CTN)
                    {
                        if (items[i] != null)
                        {
                            if (items[i].id == item.id)
                            {
                                exist = items[i];
                                break;
                            }
                        }
                    }
                    if (exist != null)
                    {
                        exist.amount += item.amount;
                    }
                    else
                    {
                        int emptyIndex = GetEmptySlotIndex();
                        if (emptyIndex >= 0)
                        {
                            items[emptyIndex] = item;
                        }
                    }
                    break;
                }
            }
        
        }
        else
        {
            Debug.Log($"{item.id}的配置数据不存在");
        }
    }

    public void AddItem(int id, int amount)
    {
        AddItem(new Item() { id = id, amount = amount });
    }

    public void RemoveItem(int id, int amount)
    {
        Item toRemove = null;
        int i = -1;              
        int removed = 0;
        while (++i < items.Length)
        {
            Item item = items[i];
            if (item.id == id)
            {
                // 计算当前还有多少需要移除
                int currentToRemove = amount - removed;
                // 计算当前item堆最多可移除多少，如果item.amount >= currentToRemove，直接从当前堆移除, 反之移除当前堆后继续遍历移除
                int canRemove = Mathf.Min(item.amount, currentToRemove);
                removed += canRemove;
                item.amount -= canRemove;

                if (item.amount == 0)
                {
                    items[i] = null;
                }
            }
            if (removed == amount)
            {
                break;
            }
        }

        if (removed < amount)
        {
            Debug.Log("背包中物品不足");
        }

        if (toRemove == null)
        {
            Debug.Log($"背包中不存在{id}的物品");
        }
    }

    public void RemoveItem(Item item)
    {
        RemoveItem(item.id, item.amount);
    }

    /// <summary>
    /// 本质是数组中索引位置对换
    /// </summary>
    public void SwapItem(int origin, int target)
    {
        Item item = items[target];
        items[target] = items[origin];
        items[origin] = item;
    }

    public int GetEmptySlotIndex()
    {
        
        int emptyIndex = -1;
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] == null)
            {
                emptyIndex = i;
                break;
            }
        }
        return emptyIndex;
    }

    public Item SpitItem(Item item)
    {
        return default;
    }

    public bool UseItem(int id, int amount)
    {
        return default;
    }

    public ItemData GetCfg(int id)
    {
        if (itemCfg.TryGetValue(id, out ItemData cfg))
        {
            return cfg;
        }
        return null;
    }
}
