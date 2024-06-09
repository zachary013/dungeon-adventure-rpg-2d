using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject objectPrefab;
    private Queue<GameObject> poolQueue;
    private int initialPoolSize;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        objectPrefab = prefab;
        initialPoolSize = initialSize;
        poolQueue = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(objectPrefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (poolQueue.Count > 0)
        {
            return poolQueue.Dequeue();
        }
        else
        {
            GameObject obj = Object.Instantiate(objectPrefab);
            obj.SetActive(false);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
