//// This is the Original

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPickup : MonoBehaviour
{
    [SerializeField] private Material rainbowMaterial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerShaderController>();
        if (player != null)
        {
            player.TriggerEffect(rainbowMaterial, 8f); // 8s rainbow (+2fade)
            Destroy(gameObject);
        }
    }
}


