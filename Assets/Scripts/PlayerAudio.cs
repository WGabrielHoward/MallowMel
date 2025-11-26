using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class PlayerAudio : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip jumpBigClip;   // maps to your bigJumpSnd
    public AudioClip damageClip;
    public AudioClip pickupClip;
    public AudioClip fireClip;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayJump() { if (jumpClip) source.PlayOneShot(jumpClip); }
    public void PlayJumpBig() { if (jumpBigClip) source.PlayOneShot(jumpBigClip); }
    public void PlayDamage() { if (damageClip) source.PlayOneShot(damageClip); }
    public void PlayPickup() { if (pickupClip) source.PlayOneShot(pickupClip); }
    public void PlayFire() { if (fireClip) source.PlayOneShot(fireClip); }
}
