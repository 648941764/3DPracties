using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackSystem : MonoBehaviour
{
    public Transform gridParent;
    public GameObject gridItemPrefab,dragPrefab;
    public List<GridItem> gridList;
    private const int max_Count = 32;
    
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
            if (gridList[i].Isempty)
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
            if (gridList[i].Isempty)
            {
                gridList[i].SetData(data, count);
                return;
            }
        }
    }
}
