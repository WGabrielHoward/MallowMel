//// This is the Original

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPickup : MonoBehaviour
{
    public Material snowMaterial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerShaderController>();
        if (player != null)
        {
            player.TriggerEffect(snowMaterial, 8f); // 10s snow (8s + 1fade in + 1fade out)
            Destroy(gameObject);
        }
    }
}

