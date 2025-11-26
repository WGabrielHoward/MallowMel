using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [Header("UI (optional)")]
    public Text scoreText;
    public Text healthText;
    public Text totalScoreText;

    [Header("Health/Score")]
    [SerializeField] private float maxHealth = 6f;
    [SerializeField] private float damagePerTick = 0.5f;
    [SerializeField] private float tickRate = 1f;

    private float health;
    private int levelScore;

    [Header("Timers")]
    private float nextDamageTime;
    private float invincibleUntil;
    private float candyCaneUntil;

    [Header("State")]
    public Vector3 spawnPoint;
    private bool inHazard;
    private bool inFire;

    private PlayerAudio audioFx;
    private SpriteRenderer sprite;
    private Color originalColor = Color.white;

    private void Awake()
    {
        audioFx = GetComponent<PlayerAudio>();
        sprite = GetComponent<SpriteRenderer>();
        spawnPoint = transform.position;
        health = maxHealth;
        UpdateUI();
    }

    // --- Public API called by interaction/pickups ---
    public void AddScore(int amount) { levelScore += amount; UpdateUI(); }

    public void SetSpawn(Vector3 pos) { spawnPoint = pos; }

    public void TeleportToSpawnAndDamage(float dmg)
    {
        transform.position = spawnPoint;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        ApplyDamage(dmg);
    }

    public void BankLevelScoreAndLoad(int buildIndex)
    {
        PlayerScoring.totalScore += levelScore;
        levelScore = 0;
        UpdateUI();
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void LoadLevel(int buildIndex)
    {
        UpdateUI();
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }

    public void SetInvincible(float duration) => invincibleUntil = Time.time + duration;
    public void SetCandyCane(float duration) => candyCaneUntil = Time.time + duration;

    public bool Invincible => Time.time < invincibleUntil;
    public bool CandyCane => Time.time < candyCaneUntil;

    // --- Hazard / Fire zones ---
    public void EnterHazard() { inHazard = true; nextDamageTime = Time.time; Tint(Color.red); }
    public void ExitHazard() { inHazard = false; Tint(originalColor); }
    public void TickHazard()
    {
        if (Invincible || CandyCane) return;
        if (inHazard && Time.time >= nextDamageTime)
        {
            ApplyDamage(damagePerTick);
            nextDamageTime = Time.time + tickRate;
        }
    }

    public void EnterFire() { inFire = true; nextDamageTime = Time.time; Tint(Color.red); }
    public void ExitFire() { inFire = false; Tint(originalColor); }
    public void TickFire()
    {
        if (Invincible || CandyCane) return;
        if (inFire && Time.time >= nextDamageTime)
        {
            ApplyDamage(damagePerTick);
            nextDamageTime = Time.time + tickRate;
        }
    }

    // --- Damage / death ---
    private void ApplyDamage(float amount)
    {
        if (Invincible || CandyCane) return;
        health -= amount;
        audioFx?.PlayDamage();
        if (health <= 0f)
        {
            // respawn logic
            transform.position = spawnPoint;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            health = maxHealth;
            levelScore -= 1;
            Tint(originalColor);
        }
        UpdateUI();
    }

    // --- Helpers ---
    private void Tint(Color c)
    {
        if (sprite) sprite.color = c;
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + levelScore;
        if (healthText) healthText.text = $"Health: {health}/{maxHealth}";
        if (totalScoreText) totalScoreText.text = "Total Score: " + PlayerScoring.totalScore;
    }
}
