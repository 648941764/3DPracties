using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public List<CraftItem> items = new List<CraftItem>();
    public List<CraftItem> craftItems = new List<CraftItem>();

    public bool isCrafting;
    public string currentCraftId;

    private CraftItem FetchItemsById(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Id == id)
            {
                return items[i];
            }
        }
        return null;
    }
}

[System.Serializable]
public class CraftItem
{
    public string name;
    public int Id;
    public Sprite sprite;
    public bool canCraftable;
    public string craftId;
}
