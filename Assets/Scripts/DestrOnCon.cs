using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestrOnCon : MonoBehaviour
{
    public GameObject exclude;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject != exclude)
        {
            gameObject.SetActive(false);
        }
       
    }
}
