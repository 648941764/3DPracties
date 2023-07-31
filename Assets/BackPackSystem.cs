using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackSystem : MonoBehaviour
{
    public Transform gridParent;
    public GameObject gridItemPrefab,dragPrefab;
    public List<GridItem> gridList;
    private const int max_Count = 32;
    public ItemTip tip;
    public bool isDrag = false;
    private void Awake()
    {
        gridList = new List<GridItem>();
        for (int i = 0; i < max_Count; i++)
        {
            GridItem grid = GameObject.Instantiate(gridItemPrefab, gridParent).GetComponent<GridItem>();
            grid.index = i;
            gridList.Add(grid);
            grid.SetBackpackSystem(this);
        }
    }

    public bool IsEmptyBackPack()
    {
        for (int i = 0; i < max_Count; i++)
        {
            if (gridList[i].IsEmpty)
            {
                return true;
            }
        }
        return false;
    }

    public void GetItem(int id,int count = 1)
    {
        ItemData data = ItemManager.instance.GetItemByID(id);
        for (int i = 0; i < max_Count; i++)
        {
            if (gridList[i].IsEmpty)
            {
                gridList[i].SetData(data, count);
                return;
            }
        }
    }

    public void SwitchGrid(GridItem g1, GridItem g2)
    {
        var temData = g1.GetData();
        var tmpAmount = g1.GetItemCount();
        g1.SetData(g2.GetData(), g2.GetItemCount());
        g2.SetData(temData, tmpAmount);
    }

    public void ShowTipContent(string content,Vector3 pos)
    {
        tip.gameObject.SetActive(true);
        tip.SetCountent(content);
        tip.transform.position = pos;
    }

    public void HideTip()
    {
        tip.gameObject.SetActive(false);
    }
}
