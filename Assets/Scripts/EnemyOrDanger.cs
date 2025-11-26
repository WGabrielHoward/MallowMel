using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrDanger : MonoBehaviour
{
    private ObjectPool pool;

    void Start()
    {
        pool = FindObjectOfType<ObjectPool>();
    }

    // Example: when enemy goes off-screen or "dies"
    void OnBecameInvisible()
    {
        pool.ReturnObject(gameObject);
    }
}
