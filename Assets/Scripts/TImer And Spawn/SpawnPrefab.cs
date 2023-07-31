using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    //public GameObject prefabToSpawn;  // 需要生成的预制体
    public GameObject[] prefabsToSpawn;
    public float spacing = 2.0f;
    public GameObject origin;
    private float Radius = 10f;
    private List<GameObject> instantiateList;
    int prefabIndex = 0;
    public void OnSpawnButtonClicked()
    {
        Vector2 CircleRadius = Random.insideUnitCircle * Radius;
        Vector3 position = origin.transform.position + new Vector3(CircleRadius.x, 1, origin.transform.position.z +  1f);
        GameObject game= Instantiate(prefabsToSpawn[prefabIndex], position, Quaternion.identity);
        if (instantiateList == null)
        {
            instantiateList = new List<GameObject>();
        }
        instantiateList.Add(game);
        prefabIndex = (prefabIndex + 1); //% prefabsToSpawn.Length;
        if (prefabIndex==prefabsToSpawn.Length)
        {
            prefabIndex = 0;
        }
    }
    public void OnDeletButtonClicked()
    {
        if (instantiateList != null)
        {
            GameObject temp = instantiateList[instantiateList.Count - 1];
            Destroy(temp);
            instantiateList.Remove(instantiateList[instantiateList.Count - 1]);
        }
    }
    public void OnSpawnRandomButtonClicked()
    {
        prefabIndex = Random.Range(0, 4);
        Vector2 CircleRadius = Random.insideUnitCircle * Radius;
        Vector3 position = origin.transform.position + new Vector3(CircleRadius.x, 1, origin.transform.position.z + 1f);
        GameObject game = Instantiate(prefabsToSpawn[prefabIndex], position, Quaternion.identity);
        if (instantiateList == null)
        {
            instantiateList = new List<GameObject>();
        }
        instantiateList.Add(game);
    }
    public void OnDeletAllClicked()
    {
        if (instantiateList != null)
        {
            foreach (GameObject item in instantiateList)
            {
                Destroy(item);
            }
        }
        instantiateList = null;
    }
    public void OnDeletRandomClicked()
    {
        if (instantiateList != null)
        {
            int num = Random.Range(0, instantiateList.Count);
            GameObject temp = instantiateList[num];
            Destroy(temp);
            instantiateList.RemoveAt(num);
        }
        
    }
}
