using System;
using System.Diagnostics;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    [Header("Pooling")]
    public ObjectPool pool;          // Reference to the object pool

    [Header("Spawning")]
    public float spawnInterval = 2f; // Time between spawns
    public Collider2D spawnArea;
    
    private bool playerInside = false;
    private float timer;
    
    // Rework this so that SpawnRandom goes off input bounding area
    void Update()
    {
        if (!playerInside) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            Spawn();
            timer = 0f;
        }
    }

    void Spawn()
    {
        Vector2 spawnPos = GetRandomPointInCollider(spawnArea);
        pool.GetObject(spawnPos, Quaternion.identity);
    }

    Vector2 GetRandomPointInCollider(Collider2D area)
    {
        // Get box center in world space
        Vector2 center = area.bounds.center;
        Vector2 size = area.bounds.size;

        float x = UnityEngine.Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = UnityEngine.Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

        return new Vector2(x, y);
    }

    //void SpawnRandom()
    //{
    //    // Pick random prefab
    //    int index = UnityEngine.Random.Range(0, prefabs.Length);

    //    Vector2 spawnPos = GetRandomPointInCollider(spawnArea);

    //    Instantiate(prefabs[index], spawnPos, Quaternion.identity);
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //UnityEngine.Debug.Log("Exited: " + other.name + " with tag " + other.tag);

        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
        else if (other.CompareTag("Fire"))
        {
            if (pool == null)
            {
                //UnityEngine.Debug.LogError("Pool reference not set on TriggerSpawner!");
                return;
            }
            pool.ReturnObject(other.gameObject);
        }
    } 
}
