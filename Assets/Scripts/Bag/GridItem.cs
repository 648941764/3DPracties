using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public bool IsEmpty { get { return data == null; } }
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text amount;
    private ItemData data;

    public int index;
    public int itemAmount;
    public bool isDragging;
    private BackPackSystem backPack;
    private GameObject dragObject;

    public void SetData(ItemData itemData,int amount = 1)
    {
        this.data = itemData;
        itemAmount = amount;
        UpdateGrid();
    }

    public void SetBackpackSystem(BackPackSystem s)
    {
        backPack = s;
    }

    public ItemData GetData()
    {
        return data;
    }

    public int GetItemCount()
    {
        return itemAmount;
    }

    public bool AddCount(int count,bool isAdd = true)
    {
        if(data == null || itemAmount > 99)
        {
            return false;
        }
        itemAmount = isAdd ? itemAmount + count : itemAmount - count;
        amount.text = itemAmount.ToString();
        return true;
    }

    private void UpdateGrid()
    {
        if (data == null)
        {
            icon.gameObject.SetActive(false);
            amount.gameObject.SetActive(false);
        }
        else
        {
            icon.sprite = Resources.Load<Sprite>("Images/Icon/" + data.icon);
            icon.gameObject.SetActive(true);
            if (itemAmount > 1)
            {
                amount.gameObject.SetActive(true);
                amount.text = itemAmount.ToString();
            }
            else
            {
                amount.gameObject.SetActive(false);
            }
        }
    }

    public void Clean()
    {
        data = null;
        UpdateGrid();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            return;
        }
        dragObject = GameObject.Instantiate(backPack.dragPrefab, backPack.transform);
        dragObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Icon/" + data.icon);
        isDragging = true;
        backPack.isDrag = true;
        backPack.HideTip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && dragObject != null)
        {
            dragObject.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        Destroy(dragObject);
        dragObject = null;
        backPack.isDrag = true;
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            var temObject = eventData.pointerCurrentRaycast.gameObject;
            if (temObject.CompareTag("GridItem"))
            {
                GridItem tmpGrid = temObject.GetComponent<GridItem>();
                if (tmpGrid.IsEmpty)
                {
                    tmpGrid.SetData(data, itemAmount);
                    Clean();
                }
                else
                {
                    if (tmpGrid.GetData().id == data.id)
                    {
                        if (data.type == ItemData.ItemType.Normal)
                        {
                            if (tmpGrid.AddCount(itemAmount))
                            {
                                Clean();
                                return;
                            }
                        }
                    }
                    backPack.SwitchGrid(this, tmpGrid);
                }
            }
        }
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backPack.isDrag)
        {
            return;
        }
        if (data == null)
        {
            return;
        }
        string tipCount = data.name + "\n" + data.description;
        if (data.attaack > 0)
        {
            tipCount += "\n攻击力：+" + data.attaack; 
        }
        if (data.defense > 0)
        {
            tipCount += "\n防御力：+" + data.speed;
        }
        if (data.speed > 0)
        {
            tipCount += "\n速度：+" + data.speed.ToString("F1");
        }
        if (data.speed > 0)
        {
            tipCount += "\n回血：+" + data.hp;
        }
        if (data.mp > 0)
        {
            tipCount += "\n回蓝：+" + data.hp;
        }
        backPack.ShowTipContent(tipCount, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backPack.isDrag && data == null)
        {
            return;
        }
        backPack.HideTip();
    }
}
