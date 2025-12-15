using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifespanActive : MonoBehaviour
{
    public float lifetime = 3f;         // Seconds after wake before removal
    public bool disableInstead = false; // If true, deactivate instead of destroy

    private Rigidbody2D rb;
    private bool timerStarted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // If not awake yet, do nothing
        if (!rb.IsAwake() || timerStarted)
            return;

        // Start timer once object wakes
        timerStarted = true;
        Invoke(nameof(RemoveObject), lifetime);
    }

    private void RemoveObject()
    {
        if (disableInstead)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}
