using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Backpack : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    bool isChangePlayerNature;
    enum BtnType { None = -1, Tidy, MinusHP, MinusMp, Clear, Sort, Count }

    private static string[] btnText = new string[]
    {
        "整理", "-HP", "-MP", "清空", "排序"
    };
    [SerializeField] Transform btnParent;
    [SerializeField] RectTransform dragLayer;
    [SerializeField] RectTransform slotPrefab;
    [SerializeField] RectTransform itemPrefab;
    [SerializeField] Text infoText;

    [SerializeField] Image hp, mp;
    [SerializeField] Text hpTxt, mpTxt;

    Dictionary<RectTransform, Image> itemSprites = new Dictionary<RectTransform, Image>();
    Dictionary<RectTransform, Text> itemAmounts = new Dictionary<RectTransform, Text>();

    RectTransform[] itemUIs;
    RectTransform[] itemSlots;

    int clickIndex = -1;

    RectTransform dragItem;
    bool canDrag, beginDrag;
    Vector2 lastMousePos;

    Player p;

    [Range(0.2f, 0.75f)] public float time = 0.75f;
    int currentHP, currentMP;
    float hpTimer, mpTimer;

    private void Awake()
    {
        Button btnPrefab = Resources.Load<Button>("CommonBtn");
        BtnType btnType = BtnType.None;
        while (++btnType < BtnType.Count)
        {
            Button btn = Instantiate(btnPrefab, btnParent);
            btn.GetComponentInChildren<Text>().text = btnText[(int)btnType];
            btn.onClick.AddListener(GetBtnAction(btnType));
        }
        p = new Player(300, 200);
        BackpackManager.Instance.player = p;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            int itemId = Random.Range(1001, 1006);
            int max = itemId <= 1002 ? 6 : 2;
            int amount = Random.Range(1, max);
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
                        ShowAll();
                    }
                    break;
                }
            }
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
                            int useCount = Random.Range(1, amount + 1);
                            BackpackManager.Instance.UseItem(item, useCount);
                            Debug.LogFormat("当前使用了{0}个", useCount);
                            ShowAll();
                        }
                    }
                    else
                    {
                        clickIndex = i;
                    }
                    infoText.text = BackpackManager.Instance.ItemInfo(i);
                    break;
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
        //else if (canDrag && Input.GetMouseButton(0))
        //{

        //    Vector2 mousePos = Input.mousePosition;
        //    if (!beginDrag)
        //    {
        //        if (mousePos == lastMousePos)
        //        {
        //            return;
        //        }
        //        beginDrag = true;
        //        ItemData cfg = BackpackManager.Instance.GetCfg(BackpackManager.Instance.GetItemByIndex(clickIndex).id);
        //        dragItem.GetComponent<Image>().sprite = GetItemSprite(cfg.icon);
        //        RectTransform itemUI = itemUIs[clickIndex];
        //        itemUI.gameObject.SetActive(false);
        //        dragItem.gameObject.SetActive(true);
        //        dragItem.position = itemUI.position;
        //        // Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, itemUI.position);
        //        // RectTransformUtility.ScreenPointToLocalPointInRectangle(dragLayer, screenPos, Camera.main, out Vector2 localPoint);
        //        // dragItem.localPosition = localPoint;  
        //    }

        //    Vector2 deltaPos = mousePos - lastMousePos;
        //    Vector3 pos = dragItem.position;
        //    pos += (Vector3)deltaPos;
        //    dragItem.position = pos;
        //    lastMousePos = mousePos;
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (beginDrag)
        //    {  
        //        Vector2 mousePos = Input.mousePosition;
        //        beginDrag = false;
        //        dragItem.gameObject.SetActive(false);
        //        if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, mousePos))
        //        {
        //            BackpackManager.Instance.RemoveItem(BackpackManager.Instance.GetItemByIndex(clickIndex));
        //            ShowItem(clickIndex);
        //        }
        //        else
        //        {
        //            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePos, null, out Vector2 localPoint);
        //            Debug.Log(localPoint);
        //            int nearest = 0;
        //            for (int i = 1; i < itemSlots.Length; ++i)
        //            {
        //                RectTransform rect = itemSlots[i];
        //                if (Vector2.Distance(rect.localPosition, localPoint) <= Vector2.Distance(itemSlots[nearest].localPosition, localPoint))
        //                {
        //                    nearest = i;
        //                }
        //            }
        //            Item currentItem = BackpackManager.Instance.GetItemByIndex(clickIndex);

        //            Item item = BackpackManager.Instance.GetItemByIndex(nearest);
        //            ItemData cfg = BackpackManager.Instance.GetCfg(currentItem.id);
        //            if (item != null)
        //            {
        //                if (item.id == currentItem.id && cfg.type == ItemData.ItemType.Normal)
        //                {
        //                    BackpackManager.Instance.MergeItem(currentItem, nearest);
        //                    //BackpackManager.Instance.RemoveItem(currentItem);
        //                }
        //                else
        //                {
        //                    BackpackManager.Instance.SwapItem(clickIndex, nearest);
        //                }
        //            }
        //            else
        //            {
        //                 BackpackManager.Instance.SwapItem(clickIndex, nearest);
        //            }

        //            ShowItem(clickIndex);
        //            ShowItem(nearest);

        //        }
        //        clickIndex = -1;
        //    }
        //}
    }

    void LateUpdate()
    {
        UpdatePlayerUI();
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

    public void ShowAll()
    {
        int i = -1;
        while (++i < BackpackManager.ITEM_SLOT_CTN)
        {
            Item item = BackpackManager.Instance.GetItemByIndex(i);
            ShowItem(i);
        }
    }

    void UpdatePlayerUI()
    {
        if (currentHP != p.hp)
        {
            hpTimer += Time.deltaTime;
            float current = Mathf.Lerp(currentHP, p.hp, hpTimer / time);
            hp.fillAmount = current / p.maxHp;
            hpTxt.text = $"{(int)current} / {p.maxHp}";
            if (hpTimer > time)
            {
                currentHP = p.hp;
                hpTimer = 0;
            }
        }

        if (currentMP != p.mp)
        {
            mpTimer += Time.deltaTime;
            float current = Mathf.Lerp(currentMP, p.mp, mpTimer / time);
            mp.fillAmount = current / p.maxMp;
            mpTxt.text = $"{(int)current} / {p.maxMp}";
            if (mpTimer >= time)
            {
                currentMP = p.mp;
                mpTimer = 0f;
            }
        }
    }

    private UnityEngine.Events.UnityAction GetBtnAction(BtnType type)
    {
        UnityEngine.Events.UnityAction action = () => Debug.Log("未添加事件");
        switch (type)
        {
            case BtnType.Tidy:
                action = OnTidyBtnClicked;
                break;
            case BtnType.MinusHP:
                action = OnReduceHpBtnClicked;
                break;
            case BtnType.MinusMp:
                action = OnReduceMpBtnClicked;
                break;
            case BtnType.Clear:
                action = OnClearBtnClick;
                break;
            case BtnType.Sort:
                action = OnSortBtnClick;
                break;
        }
        return action;
    }

    private void OnReduceHpBtnClicked()
    {
        p.ChangeHp(-Random.Range(1, 51));
    }

    private void OnReduceMpBtnClicked()
    {
        p.ChangeMp(-Random.Range(1, 51));
    }

    private void OnTidyBtnClicked()
    {
        BackpackManager.Instance.TidyItem();
        ShowAll();
    }

    private void OnClearBtnClick()
    {
        BackpackManager.Instance.DeletAll();
        ShowAll();
    }

    private void OnSortBtnClick()
    {
        BackpackManager.Instance.SortItme();
        ShowAll();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canDrag && eventData.button == PointerEventData.InputButton.Left)
        {
            if (!beginDrag)
            {
                beginDrag = true;
                //获取点击物品的Data，
                ItemData cfg = BackpackManager.Instance.GetCfg(BackpackManager.Instance.GetItemByIndex(clickIndex).id);
                //dragItem是我要把要拖拽物品的Data数据里面的图片放到dragItem里面
                dragItem.GetComponent<Image>().sprite = GetItemSprite(cfg.icon);
                //这个UI就是我要要拖动的物体
                RectTransform UI = itemUIs[clickIndex];
                UI.gameObject.SetActive(false);
                dragItem.gameObject.SetActive(true);
                //把要拖物品的位置，等于dragItem的位置
                dragItem.position = UI.position;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag && beginDrag && eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 mousePosition = eventData.position;//eventdata.positon 就是BeginDrag点击的位置
            //
            Vector2 currentPosion = mousePosition - lastMousePos;
            //拖动中物品的位置
            Vector3 pos = dragItem.position;
            pos += (Vector3)currentPosion;
            dragItem.position = pos;
            lastMousePos = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canDrag && beginDrag && eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 currentPositon = eventData.position;
            beginDrag = false;
            dragItem.gameObject.SetActive(false);

            if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, currentPositon))
            {
                BackpackManager.Instance.RemoveItem(BackpackManager.Instance.GetItemByIndex(clickIndex));
                Debug.Log("丢弃");
                ShowAll();
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, currentPositon, null, out Vector2 localPoint);
                int nearest = 0;
                for (int i = 0; i < itemSlots.Length; ++i)
                {
                    RectTransform rect = itemSlots[i];
                    if (Vector2.Distance(rect.localPosition, localPoint) <= Vector2.Distance(itemSlots[nearest].localPosition, localPoint))
                    {
                        nearest = i;
                    }
                }
                Item currentItem = BackpackManager.Instance.GetItemByIndex(clickIndex);
                Item nearestItem = BackpackManager.Instance.GetItemByIndex(nearest);
                ItemData currentData = BackpackManager.Instance.GetCfg(currentItem.id);
                if (nearestItem != null)
                {
                    if (currentItem.id == nearestItem.id && currentData.type == ItemData.ItemType.Normal)
                    {
                        nearestItem.amount += currentItem.amount;
                        BackpackManager.Instance.RemoveItem(currentItem);
                    }
                    else
                    {
                        BackpackManager.Instance.SwapItem(clickIndex, nearest);
                    }
                }
                else
                {
                    BackpackManager.Instance.SwapItem(clickIndex, nearest);
                }
                ShowAll();
                ShowItem(clickIndex);
            }
            clickIndex = -1;
        }
    }
}