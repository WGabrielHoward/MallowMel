using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps; // for ViewBlocker example

[DisallowMultipleComponent]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerPowerups powerups;
    [SerializeField] private PlayerAudio audioFx;

    [Header("Pickups")]
    public Material rainbowMaterial;
    public float   rainbowDuration = 15f;
    public Material candyCaneMaterial;
    public float   candyCaneDuration = 10f;

    private void Reset()
    {
        movement = GetComponent<PlayerMovement>();
        stats    = GetComponent<PlayerStats>();
        powerups = GetComponent<PlayerPowerups>();
        audioFx  = GetComponent<PlayerAudio>();
    }

    // --- Collision (non-trigger) ---
    private void OnCollisionEnter2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "BigBounce":
                movement.SetBigBounce(true);
                break;
            case "Ramp":
                movement.SetRamp(true);
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "BigBounce":
                movement.SetBigBounce(false);
                break;
            case "Ramp":
                movement.SetRamp(false);
                break;
        }
    }

    // --- Triggers ---
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Ground":
                movement.SetGrounded(true); // preserves your old trigger-based grounding
                break;

            case "KillBox":
                stats.TeleportToSpawnAndDamage(0.5f);
                break;

            case "Collectible":
                stats.AddScore(1);
                col.gameObject.SetActive(false);
                audioFx?.PlayPickup();
                break;

            case "NextLevel":
                stats.BankLevelScoreAndLoad(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            case "PreviousLevel":
                stats.LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
                break;

            case "EndTutorial":
                stats.LoadLevel(0);
                break;

            case "CheckPoint":
                stats.SetSpawn(col.transform.position);
                break;

            case "Rainbow":
                powerups.ApplyMaterial(rainbowMaterial, rainbowDuration);
                stats.SetInvincible(rainbowDuration);
                col.gameObject.SetActive(false);
                audioFx?.PlayPickup();
                break;

            case "CCMallow":
                powerups.ApplyMaterial(candyCaneMaterial, candyCaneDuration);
                stats.SetCandyCane(candyCaneDuration);
                col.gameObject.SetActive(false);
                audioFx?.PlayPickup();
                break;

            case "ViewBlocker":
                var tm = col.GetComponent<TilemapRenderer>();
                if (tm) tm.enabled = false;
                break;

            case "Hazard":
                stats.EnterHazard(); // your tick/DoT logic lives in PlayerStats
                audioFx?.PlayDamage();
                break;

            case "Fire":
                stats.EnterFire();
                audioFx?.PlayFire();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Ground":
                movement.SetGrounded(false);
                break;
            case "ViewBlocker":
                var tm = col.GetComponent<TilemapRenderer>();
                if (tm) tm.enabled = true;
                break;
            case "Hazard":
                stats.ExitHazard();
                break;
            case "Fire":
                stats.ExitFire();
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Hazard")) stats.TickHazard();
        else if (col.CompareTag("Fire")) stats.TickFire();
    }
}
