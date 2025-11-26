using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;   // The prefab to pool
    public int poolSize = 10;   // How many to pre-instantiate

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Start inactive
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector2 position, Quaternion rotation)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Optional: Expand pool if empty
            GameObject obj = Instantiate(prefab, position, rotation);
            return obj;
        }
    }

    

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
