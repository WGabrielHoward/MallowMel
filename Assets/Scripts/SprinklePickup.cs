//// This is the Original

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklePickup : MonoBehaviour
{
    [SerializeField] private Material sprinkleMaterial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerShaderController>();
        if (player != null)
        {
            player.TriggerEffect(sprinkleMaterial, 8f); // 10s sprinkle (8+1fin+1fout)
            Destroy(gameObject);
        }
    }
}


