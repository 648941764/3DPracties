using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public bool Isempty { get { return data == null; } }
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

    public void Clear()
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
        //if ()
        //{

        //}
    }
}
