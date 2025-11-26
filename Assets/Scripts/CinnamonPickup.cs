using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinnamonPickup : MonoBehaviour
{
    public Material CinnamonMaterial;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerShaderController>();
        if (player != null)
        {
            player.TriggerEffect(CinnamonMaterial, 8f); // 8s Cinnamon (+2fade)
            Destroy(gameObject);
        }
    }
}
