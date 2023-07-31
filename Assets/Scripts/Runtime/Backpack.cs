using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    [SerializeField] RectTransform slotPrefab;
    [SerializeField] RectTransform itemPrefab;

    Dictionary<RectTransform, Image> itemSprites = new Dictionary<RectTransform, Image>();
    Dictionary<RectTransform, Text> itemAmounts = new Dictionary<RectTransform, Text>();

    RectTransform[] itemUIs;

    private void Awake() 
    {
        itemUIs = new RectTransform[BackpackManager.ITEM_SLOT_CTN];
        int i = -1;
        while (++i < BackpackManager.ITEM_SLOT_CTN)
        {
            RectTransform slot = Instantiate(slotPrefab, transform);
            slot.name = slotPrefab.name;
        }
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int itemId = Random.Range(1001, 1006);
            int amount = Random.Range(1, 5);
            BackpackManager.Instance.AddItem(itemId, amount);
            Debug.Log($"向背包添加id = {itemId}, amount = {amount}");
            ShowAll();
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {

        }

        
        if (Input.GetKeyDown(KeyCode.B))
        {
        }     
    }

    private RectTransform CreateItemUI(int index)
    {
        RectTransform itemUI = Instantiate(itemPrefab, transform.GetChild(index));
        itemUI.anchoredPosition = Vector2.zero;
        itemSprites.Add(itemUI, itemUI.GetComponentInChildren<Image>());
        itemAmounts.Add(itemUI, itemUI.GetComponentInChildren<Text>());
        itemUIs[index] = itemUI;
        return itemUI;
    }

    private void ShowItem(int index)
    {
        RectTransform itemUI = itemUIs[index];
        if (itemUI == null)
        {
            itemUI = CreateItemUI(index);
        }
        Item item = BackpackManager.Instance.GetItemByIndex(index);
        ItemData data = BackpackManager.Instance.GetCfg(item.id);
        itemSprites[itemUI].sprite = Resources.Load<Sprite>("Images/Icon/" + data.icon);
        itemAmounts[itemUI].text = item.amount.ToString();
    }

    private void ShowAll()
    {
        int i = -1;
        while (++i < BackpackManager.ITEM_SLOT_CTN)
        {
            Item item = BackpackManager.Instance.GetItemByIndex(i);
            if (item != null)
            {
                ShowItem(i);
            }
        }
    }
}
