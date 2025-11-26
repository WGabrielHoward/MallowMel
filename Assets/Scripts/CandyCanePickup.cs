using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCanePickup : MonoBehaviour
{
    public Material CandyCaneMaterial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerShaderController>();
        if (player != null)
        {
            player.TriggerEffect(CandyCaneMaterial, 8f); // 8s CandyCane (+2fade)
            Destroy(gameObject);
        }
    }
}
