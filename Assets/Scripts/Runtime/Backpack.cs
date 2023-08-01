using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Backpack : MonoBehaviour
{
    private bool isSwapping;
    private float swapDuration = 0.5f;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float swapStartTime;

    [SerializeField] RectTransform slotPrefab;
    [SerializeField] RectTransform itemPrefab;
    [SerializeField] RectTransform dragLayer;
    [SerializeField] Text infoText;

    [SerializeField] Image hp, mp,hpBuffer,mpBuffer;

    Dictionary<RectTransform, Image> itemSprites = new Dictionary<RectTransform, Image>();
    Dictionary<RectTransform, Text> itemAmounts = new Dictionary<RectTransform, Text>();

    RectTransform[] itemUIs;
    RectTransform[] itemSlots;

    int clickIndex = -1;

    RectTransform dragItem;
    bool canDrag, beginDrag;
    Vector2 lastMousePos;

    Player p;

    private void Awake()
    {
        p = new Player(300, 200);
        BackpackManager.Instance.player = p;
        UpdatePlayerInfo();
        infoText.text = "";
        itemUIs = new RectTransform[BackpackManager.ITEM_SLOT_CTN];
        itemSlots = new RectTransform[BackpackManager.ITEM_SLOT_CTN];
        int i = -1;
        while (++i < BackpackManager.ITEM_SLOT_CTN)
        {
            RectTransform slot = Instantiate(slotPrefab, transform);
            slot.name = slotPrefab.name;
            itemSlots[i] = slot;
        }
        dragItem = Instantiate(itemPrefab, dragLayer);
        dragItem.GetChild(0).gameObject.SetActive(false);
        dragItem.gameObject.SetActive(false);
    }

    private void Update()
    {
        hpBuffer.fillAmount = Mathf.Lerp(hpBuffer.fillAmount, hp.fillAmount, 2.5f * Time.deltaTime);
        mpBuffer.fillAmount = Mathf.Lerp(mpBuffer.fillAmount, mp.fillAmount, 2.5f * Time.deltaTime);

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
            ShowAll();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;

            for (int i = 0; i < itemSlots.Length; i++)
            {
                RectTransform rect = itemSlots[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
                {
                    Item item = BackpackManager.Instance.GetItemByIndex(i);
                    if (item != null)
                    {
                        Item splitItem = BackpackManager.Instance.SplitItem(item);
                        if (splitItem != null)
                        {
                            ShowItem(i);
                        }
                    }
                    break;
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            p.ChangeHp(-Random.Range(1, 51));
            p.ChangeMp(-Random.Range(1, 31));
            UpdatePlayerInfo();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            for (int i = 0; i < itemSlots.Length; ++i)
            {
                RectTransform rect = itemSlots[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
                {
                    if (clickIndex == i)
                    {
                        Item item = BackpackManager.Instance.GetItemByIndex(i);
                        if (item != null)
                        {
                            int amount = BackpackManager.Instance.GetItemTotalCount(item.id);
                            BackpackManager.Instance.UseItem(item.id, Random.Range(1, amount + 1));
                            UpdatePlayerInfo();
                            ShowItem(i);
                        }
                    }
                    else
                    {
                        clickIndex = i;
                        infoText.text = BackpackManager.Instance.ItemInfo(i);
                    }
                }
            }

            canDrag = false;
            for (int i = 0; i < itemUIs.Length; ++i)
            {
                if (itemUIs[i] != null && RectTransformUtility.RectangleContainsScreenPoint(itemUIs[i], mousePos))
                {
                    Item item = BackpackManager.Instance.GetItemByIndex(i);
                    if (item != null)
                    {
                        canDrag = true;
                        lastMousePos = mousePos;
                    }
                }
            }
        }
        else if (canDrag && Input.GetMouseButton(0))
        {

            Vector2 mousePos = Input.mousePosition;
            if (!beginDrag)
            {
                if (mousePos == lastMousePos)
                {
                    return;
                }
                beginDrag = true;
                ItemData cfg = BackpackManager.Instance.GetCfg(BackpackManager.Instance.GetItemByIndex(clickIndex).id);
                dragItem.GetComponent<Image>().sprite = GetItemSprite(cfg.icon);
                RectTransform itemUI = itemUIs[clickIndex];
                itemUI.gameObject.SetActive(false);
                dragItem.gameObject.SetActive(true);
                dragItem.position = itemUI.position;
                // Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, itemUI.position);
                // RectTransformUtility.ScreenPointToLocalPointInRectangle(dragLayer, screenPos, Camera.main, out Vector2 localPoint);
                // dragItem.localPosition = localPoint;  
            }

            Vector2 deltaPos = mousePos - lastMousePos;
            Vector3 pos = dragItem.position;
            pos += (Vector3)deltaPos;
            dragItem.position = pos;
            lastMousePos = mousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (beginDrag)
            {  
                Vector2 mousePos = Input.mousePosition;
                beginDrag = false;
                dragItem.gameObject.SetActive(false);
                if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, mousePos))
                {
                    BackpackManager.Instance.RemoveItem(BackpackManager.Instance.GetItemByIndex(clickIndex));
                    ShowItem(clickIndex);
                }
                else
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePos, null, out Vector2 localPoint);
                    Debug.Log(localPoint);
                    int nearest = 0;
                    for (int i = 1; i < itemSlots.Length; ++i)
                    {
                        RectTransform rect = itemSlots[i];
                        if (Vector2.Distance(rect.localPosition, localPoint) <= Vector2.Distance(itemSlots[nearest].localPosition, localPoint))
                        {
                            nearest = i;
                        }
                    }

                    BackpackManager.Instance.SwapItem(clickIndex, nearest);
                    ShowItem(clickIndex);
                    ShowItem(nearest);

                }
                clickIndex = -1;
            }

        }
    }

    private RectTransform CreateItemUI(int index)
    {
        RectTransform itemUI = Instantiate(itemPrefab, transform.GetChild(index));
        itemUI.anchoredPosition = Vector2.zero;
        itemSprites.Add(itemUI, itemUI.GetComponent<Image>());
        itemAmounts.Add(itemUI, itemUI.GetComponentInChildren<Text>());
        itemUIs[index] = itemUI;
        return itemUI;
    }

    private void ShowItem(int index)
    {
        RectTransform itemUI = itemUIs[index];
        Item item = BackpackManager.Instance.GetItemByIndex(index);
        if (item != null && itemUI == null)
        {
            itemUI = CreateItemUI(index);
        }

        if (item == null && itemUI == null) { return; }

        if (itemUI.gameObject.activeSelf != (item != null))
        {
            itemUI.gameObject.SetActive(item != null);
        }
        if (item != null)
        {
            ItemData data = BackpackManager.Instance.GetCfg(item.id);
            itemSprites[itemUI].sprite = GetItemSprite(data.icon); // Resources.Load<Sprite>("Images/Icon/" + data.icon);
            itemAmounts[itemUI].text = item.amount.ToString();
        }
    }

    private Sprite GetItemSprite(string icon)
    {
        return Resources.Load<Sprite>("Images/Icon/" + icon);
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

    private void UpdatePlayerInfo()
    {
        float targetHp = (float)p.hp / p.maxHp;
        float targetMp = (float)p.mp / p.maxMp;

        hp.fillAmount = targetMp;
        mp.fillAmount = targetMp;
    }
}
