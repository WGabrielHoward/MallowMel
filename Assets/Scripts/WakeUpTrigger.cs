using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WakeUpTrigger : MonoBehaviour
{
    [Tooltip("The object you want to wake up when the player enters.")]
    public Rigidbody2D targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure it was the player
        if (other.CompareTag("Player"))
        {
            if (targetObject != null)
            {
                // If object is sleeping, wake it up
                targetObject.WakeUp();

                // Optional: if the object was inactive, enable it
                targetObject.gameObject.SetActive(true);

                //Debug.Log(targetObject.name + " woke up!");
            }
        }
    }
}
