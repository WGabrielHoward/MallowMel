//// This is the Original

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPickup : MonoBehaviour
{
    public Material rainbowMaterial;

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

//using UnityEngine;

//public class RainbowPickup : MonoBehaviour
//{
//    public Material rainbowMaterial;
//    public float duration = 10f;

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        var powerups = other.GetComponent<PlayerPowerups>();
//        if (powerups != null)
//        {
//            powerups.ApplyMaterial(rainbowMaterial, duration);

//            var audio = other.GetComponent<PlayerAudio>();
//            audio?.PlayPickup();

//            Destroy(gameObject);
//        }
//    }
//}
