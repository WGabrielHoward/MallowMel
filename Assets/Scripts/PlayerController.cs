using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using System;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float jumpMultiple = 2f;
    public Transform groundCheck;
    private Vector3 spawnPoint;
    private Vector2 moveInput;
    //private Vector2 jumpVect = new Vector2(0f,1f);

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isBigBounce;
    private bool isRamp = false;
    private bool isHazard = false;
    public bool isOnIce = false;
    private Animator playerAnimation;

    [Header("Texts")]
    public Text scoreText;
    public Text healthText;
    public Text totalScoreText;

    private float playerHealth = 6f;

    [Header("MobileControls")]
    public VirtualJoystick joystick;
    public JumpButton jumpButton;
    public float deadZone = 0.4f;


    private AudioSource audioSource;
    [Header("Audio")]
    public AudioClip bigJumpSnd;
    public AudioClip damageSnd;
    public AudioClip scoreSnd;
    public AudioClip fireSnd;

    private float damagePerTick = .5f;
    private double tickRate = 1;
    private bool playerInside = false;
    private double nextDamageTime = 0;
    private bool invincible = false;
    private bool isCandyCane = false;
    private bool isSprinkle = false;
    private bool isSnow = false;
    private bool isCinnamon = false;
    private double invincibleLasts = 10;
    private double candyCaneLasts = 10;
    private double sprinkleLasts = 10;
    private double snowLasts = 10;
    private double cinnamonLasts = 10;
    private double invincibleTil = 0;
    private double candyCaneTil = 0;
    private double sprinkleTil = 0;
    private double snowTil = 0;
    private double cinnamonTil = 0;
    private bool rolling;

    private Vector2 rampNormal;

    [Header("Ice Settings")]
    public float iceAcceleration = 5f;   // How fast you speed up on ice
    public float iceDeceleration = 0.5f; // How fast you slow down on ice
    public float maxIceSpeed = 8f;

    // Need to set up States (maybe use enumeration) rather than damage, invincible, candyCane, etc.

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Graphics")]
    public Color burntBrown = new Color(0.6f, 0.3f, 0.1f);
    public Color frozenBlue = new Color(0.73f, .98f, .9f);
    public GameObject smoke;

    public LevelChange levelChange;
    // Some of the collison and trigger interactions could probably be moved to other scripts.
    // The only reason I haven't is because the thing changing its internals is player, 
    // So other items shouldn't necessarily know about those.
    // I also don't want to run two scripts at the same time if one of them is changing the scene

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        spawnPoint = this.transform.position;
        playerAnimation = GetComponent<Animator>();
        scoreText.text = "Score: " + PlayerScoring.levelScore;
        healthText.text = "Health: " + playerHealth + "/6";
        totalScoreText.text = "Total Score: " + PlayerScoring.totalScore;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        smoke.SetActive(false);

        #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR || UNITY_WEBGL
                joystick.gameObject.SetActive(true);
                jumpButton.gameObject.SetActive(true);
        #else
                joystick.gameObject.SetActive(false);
                jumpButton.gameObject.SetActive(false);
        #endif

    }


    void FixedUpdate()
    {

        Move();

        Jump();

        invincible = Time.time < invincibleTil;
        isCandyCane = Time.time < candyCaneTil;
        isSprinkle = Time.time < sprinkleTil;
        isSnow = Time.time < snowTil;
        isCinnamon = Time.time < cinnamonTil;
    }

    
    void Move()
    {
        moveInput = GetMoveInput();
        if(moveInput.x!=0 && moveInput.y < 0)
        {
            rolling = true;
        }
        else { rolling = false; }

        if (isOnIce == true && isSprinkle == false)
        {
            HandleIceMovement(moveInput);
            
        }
        else if (isRamp)
        {
            HandleRampMovement(moveInput);
        }
        else
        {
            HandleNormalMovement(moveInput);
        }


        // animation variables
        playerAnimation.SetFloat("DirX", rb.linearVelocity.x);
        playerAnimation.SetBool("MovementBool", moveInput.x != 0);
        playerAnimation.SetBool("DownArrow", moveInput.y < 0);

        //rolling and squish
        HandleRollingAndSquish(moveInput);

    }
    Vector2 GetMoveInput()
    {
        Vector2 joyInput = Vector2.zero;

        // Keyboard fallback
        joyInput.x = Input.GetAxisRaw("Horizontal");
        joyInput.y = Input.GetAxisRaw("Vertical");

    #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR || UNITY_WEBGL
            if (joystick != null)
            {
                joyInput = joystick.Direction;
            }
    #endif

        // Apply deadzone
        if (joyInput.magnitude < deadZone)
        {
            joyInput = Vector2.zero;
        }
        else if (Math.Abs(joyInput.x) < deadZone)
        {
            joyInput.x = 0f;
        }


        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        if (joyInput.x != 0) { moveInput.x = joyInput.x; }
        if (joyInput.y != 0) { moveInput.y = joyInput.y; }

        return moveInput;
    }

    void HandleIceMovement(Vector2 moveInput)
    {
        // accelerate towards target velocity instead of snapping
        float targetX = moveInput.x * maxIceSpeed;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, (moveInput.x != 0 ? iceAcceleration : iceDeceleration) * Time.deltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    void HandleRampMovement(Vector2 moveInput)
    {
        if (rolling)
        {
            float rampForce = 15f;
            // Skip if no valid ramp normal yet
            if (rampNormal == Vector2.zero)
                return;

            // Find the tangent of the ramp (perpendicular to normal)
            Vector2 rampTangent = new Vector2(rampNormal.y, -rampNormal.x);

            // Determine if we're going up or down the ramp
            float moveDirection = Mathf.Sign(Vector2.Dot(rb.linearVelocity, rampTangent));
            

            // Ensure tangent points the same direction as movement
            if (moveDirection < 0)
            {
                rampTangent = -rampTangent;
            }
            
            // Apply force along tangent
            rb.AddForce(rampTangent * rampForce, ForceMode2D.Force);

        }
        else
        {
            HandleNormalMovement(moveInput);
        }
    }

    void HandleNormalMovement(Vector2 moveInput)
    {
        // normal ground movement
        rb.gravityScale = 1.2f;
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void HandleRollingAndSquish(Vector2 moveInput)
    {
        // ROLLING AS DOWN + SIDE
        // doing this the fast and dirty way, where trigger is capcol[0], and bounding is capcol[1]
        CapsuleCollider2D[] capCol = rb.GetComponents<CapsuleCollider2D>();
        if (rolling)
        {   // if move true and down arrow true then make ground detection a circle, enable rotation
            rb.constraints = RigidbodyConstraints2D.None;
            capCol[0].size = new Vector2(.85f, .85f);
            capCol[0].offset = new Vector2(-.02f, 0f);

            // unSquish
            capCol[1].size = new Vector2(.8f, .8f);
            capCol[1].offset = new Vector2(-.02f, 0f);

        }
        // Not rolling
        else
        {
            // otherwise return to upright position, freeze rotation
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.rotation = 0f;
            if (moveInput.y < 0)
            {   // if down arrow true, (but move is not), squish object
                capCol[1].size = new Vector2(.8f, .25f);
                capCol[1].offset = new Vector2(-.02f, 0f);

                capCol[0].size = new Vector2(.6f, .3f);
                capCol[0].offset = new Vector2(-.02f, -.1f);
            }
            else
            {   // down false
                // unSquish
                capCol[1].size = new Vector2(.8f, .8f);
                capCol[1].offset = new Vector2(-.02f, 0f);

                capCol[0].size = new Vector2(.75f, .15f);
                capCol[0].offset = new Vector2(-.02f, -.35f);
            }
        }
    }

    void Jump()
    {   // Note: this does not work with AddForce, and directly changes velocity
        bool jumpInput = Input.GetButton("Jump"); // keyboard
        #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR || UNITY_WEBGL
             if (jumpButton != null) jumpInput |= jumpButton.IsHeld; // OR with mobile button
        #endif

        if (jumpInput)
        {

            if (isGrounded == true)
            {
                //UnityEngine.Debug.Log("Vertical = "+ Input.GetAxis("Vertical") + "moveInput = " + moveInput + "\n");
                if (moveInput.y < 0 && Math.Abs(moveInput.x)<0.2f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpMultiple * jumpForce);
                    audioSource.PlayOneShot(bigJumpSnd);
                    //UnityEngine.Debug.Log("SuperJump \n");
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    //Debug.Log("Jump");
                }
            }
            else if (isBigBounce == true)
            {   //Seems redundant but is necessary for bigbounce

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpMultiple * jumpForce);
                //Debug.Log("Jump");
            }
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "BigBounce":
                isBigBounce = true;
                jumpMultiple *= 1.5f;
                //Debug.Log("Grounded");
                break;
            case "NextLevel":
                UnityEngine.Debug.Log("\nNext Level!");
                levelChange.NextLevel();
                break;
            case "EndTutorial":
                totalScoreText.text = "Total Score: " + PlayerScoring.totalScore;
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                break;
            case "PreviousLevel":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                spawnPoint = transform.position;
                break;
            case "Hazard":
                // make knockback
                isHazard = true;
                nextDamageTime = Time.time;
                if (invincible != true && isCandyCane != true && isSprinkle != true)
                {
                    audioSource.PlayOneShot(damageSnd);
                    spriteRenderer.color = Color.red;
                    if (playerHealth <= 0)
                    {
                        transform.position = spawnPoint;
                        rb.linearVelocity = new Vector2(0, 0);
                        //Debug.Log("Dead!\n");
                        playerHealth = 6;
                        PlayerScoring.SubtractfromLevelScore(1);
                        scoreText.text = "Score: " + PlayerScoring.levelScore;
                        originalColor = Color.white;
                        spriteRenderer.color = originalColor;
                    }
                    healthText.text = "Health: " + playerHealth + "/6";
                }
                //UnityEngine.Debug.Log("Hazard \n");
                break;
            case "Ice":
                isOnIce = true;
                break;
            case "Collectible":
                PlayerScoring.AddtoLevelScore(1);
                col.gameObject.SetActive(false);
                scoreText.text = "Score: " + PlayerScoring.levelScore;
                audioSource.PlayOneShot(scoreSnd);
                //UnityEngine.Debug.Log("Collectible\n");
                break;
            case "Rainbow":
                invincibleTil = Time.time + invincibleLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                invincible = true;
                //UnityEngine.Debug.Log("Rainbow\n");
                break;
            case "CCMallow":
                candyCaneTil = Time.time + candyCaneLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isCandyCane = true;
                //UnityEngine.Debug.Log("CCMallow\n");
                break;
            case "Cinnamon":
                cinnamonTil = Time.time + cinnamonLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isCinnamon = true;
                //UnityEngine.Debug.Log("CinMallow\n");
                break;
            case "Sprinkle":
                sprinkleTil = Time.time + sprinkleLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isSprinkle = true;
                //UnityEngine.Debug.Log("Sprinkle\n");
                break;
            case "Snowflake":
                snowTil = Time.time + snowLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isSnow = true;
                //UnityEngine.Debug.Log("Sprinkle\n");
                break;
            case "Ramp":
                isRamp = true;
                break;

        }

    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == ("Hazard"))
        {
            if (isHazard && Time.time >= nextDamageTime)
            {
                if (invincible != true && isCandyCane != true && isSprinkle != true)
                {
                    audioSource.PlayOneShot(damageSnd);

                    spriteRenderer.color = Color.red;
                    //originalColor = burntBrown;
                    playerHealth -= damagePerTick;
                    nextDamageTime = Time.time + tickRate;
                    //UnityEngine.Debug.Log("Hazard Damage\n");
                    if (playerHealth <= 0)
                    {
                        transform.position = spawnPoint;
                        rb.linearVelocity = new Vector2(0, 0);
                        playerHealth = 6;
                        PlayerScoring.SubtractfromLevelScore(1);
                        scoreText.text = "Score: " + PlayerScoring.levelScore;
                        originalColor = Color.white;
                        spriteRenderer.color = originalColor;
                    }
                    healthText.text = "Health: " + playerHealth + "/6";
                }
            }
        }
        if (col.gameObject.tag == ("Ice"))
        {
            isOnIce = true;
        }
        foreach (var contact in col.contacts)
        {
            if (col.collider.CompareTag("Ramp"))
            {
                rampNormal = contact.normal; // The surface normal at that point
                isRamp = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        switch (col.gameObject.tag)
        {
            case "BigBounce":
                isBigBounce = false;
                jumpMultiple /= 1.5f;
                //Debug.Log("UnGrounded");
                break;
            case "Ramp":
                //rb.AddForce(rampNormal * launchStrength, ForceMode2D.Impulse);
                isRamp = false; 
                break;
            case "Hazard":
                isHazard = false;
                spriteRenderer.color = originalColor;
                break;
            case "Ice":
                isOnIce = false;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Side note, maybe add a bubble bounce object:
        //  you jump on it, bounce up, and the object disappears

        // I think that it is double tapping objects due to the two collison shapes.
        // I'm just going to change the trigger enter for health stuff to .5 since it is hitting twice

        switch (col.gameObject.tag)
        {
            case "Ground":
                isGrounded = true;
                playerAnimation.SetBool("IsGrounded", true);
                //Debug.Log("Grounded");
                break;
            case "KillBox":
                transform.position = spawnPoint;
                rb.linearVelocity = new Vector2(0, 0);
                playerHealth -= .5f;
                originalColor = Color.white;
                spriteRenderer.color = originalColor;
                if (playerHealth <= 0)
                {
                    //Debug.Log("Dead!\n");
                    playerHealth = 6;
                    PlayerScoring.SubtractfromLevelScore(1); 
                    scoreText.text = "Score: " + PlayerScoring.levelScore;

                }
                else
                {
                    //Debug.Log("Fell off!");
                }
                healthText.text = "Health: " + playerHealth + "/6";
                break;

            case "CheckPoint":
                spawnPoint = col.gameObject.transform.position;
                break;
            case "Hazard":
                //playerHealth -= .5f;
                // need to make knockback
                isHazard = true;
                nextDamageTime = Time.time;
                if (invincible != true && isCandyCane != true )
                {
                    audioSource.PlayOneShot(damageSnd);
                    spriteRenderer.color = Color.red;
                    if (playerHealth <= 0)
                    {
                        transform.position = spawnPoint;
                        rb.linearVelocity = new Vector2(0, 0);
                        //Debug.Log("Dead!\n");
                        playerHealth = 6;
                        PlayerScoring.SubtractfromLevelScore(1);
                        scoreText.text = "Score: " + PlayerScoring.levelScore;
                        originalColor = Color.white;
                        spriteRenderer.color = originalColor;
                    }
                    healthText.text = "Health: " + playerHealth + "/6";
                }
                //UnityEngine.Debug.Log("Hazard \n");
                break;
            case "Fire":
                playerInside = true;
                nextDamageTime = Time.time;
                if (invincible != true && isSnow != true)
                {
                    audioSource.PlayOneShot(fireSnd);

                    spriteRenderer.color = Color.red;
                    smoke.SetActive(true);
                }
                //UnityEngine.Debug.Log("trigger the fire\n");
                break;
            case "Frost":
                playerInside = true;
                nextDamageTime = Time.time;
                if (invincible != true && isCinnamon != true)
                {
                    audioSource.PlayOneShot(fireSnd);

                    spriteRenderer.color = Color.red;
                }
                //UnityEngine.Debug.Log("trigger the fire\n");
                break;
            case "Rainbow":
                invincibleTil = Time.time + invincibleLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                invincible = true;
                //UnityEngine.Debug.Log("Rainbow\n");
                break;
            case "CCMallow":
                candyCaneTil = Time.time + candyCaneLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isCandyCane = true;
                //UnityEngine.Debug.Log("CCMallow\n");
                break;
            case "Cinnamon":
                cinnamonTil = Time.time + cinnamonLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isCinnamon = true;
                //UnityEngine.Debug.Log("CCMallow\n");
                break;
            case "Snowflake":
                snowTil = Time.time + snowLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isSnow = true;
                //UnityEngine.Debug.Log("CCMallow\n");
                break;
            case "Ice":
                isOnIce = true;
                break;
            case "Sprinkle":
                sprinkleTil = Time.time + sprinkleLasts;
                col.gameObject.SetActive(false);
                audioSource.PlayOneShot(scoreSnd);
                isSprinkle = true;
                //UnityEngine.Debug.Log("Sprinkle\n");
                break;
            case "ViewBlocker":
                //col.gameObject.SetRender(false);
                TilemapRenderer tilemap = col.GetComponent<TilemapRenderer>();
                tilemap.enabled = false;
                //Debug.Log("ViewBlock Disabled\n");
                break;
            case "Untagged":
                break;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {   // The bools are keeping fire or hazard from triggering if player is inside when powerup ends
        if (invincible != true && isCandyCane != true && isSprinkle != true && isSnow != true)
        {
            if (col.gameObject.tag == ("Fire"))
            {
                if (playerInside && Time.time >= nextDamageTime)
                {
                    if (invincible != true && isSnow != true)
                    {
                        audioSource.PlayOneShot(fireSnd);

                        spriteRenderer.color = Color.red;
                        smoke.SetActive(true);
                        originalColor = burntBrown;
                        playerHealth -= damagePerTick;
                        nextDamageTime = Time.time + tickRate;
                        //UnityEngine.Debug.Log("in the fire\n");
                        if (playerHealth <= 0)
                        {
                            transform.position = spawnPoint;
                            rb.linearVelocity = new Vector2(0, 0);
                            playerHealth = 6;
                            PlayerScoring.SubtractfromLevelScore(1);
                            scoreText.text = "Score: " + PlayerScoring.levelScore;
                            originalColor = Color.white;
                            spriteRenderer.color = originalColor;
                        }
                        healthText.text = "Health: " + playerHealth + "/6";
                    }

                }

            }
            else if (col.gameObject.tag == ("Frost"))
            {
                if (playerInside && Time.time >= nextDamageTime)
                {
                    if (invincible != true && isCinnamon != true)
                    {
                        audioSource.PlayOneShot(fireSnd);

                        spriteRenderer.color = Color.red;
                        
                        originalColor = frozenBlue;
                        playerHealth -= damagePerTick;
                        nextDamageTime = Time.time + tickRate;
                        //UnityEngine.Debug.Log("in the fire\n");
                        if (playerHealth <= 0)
                        {
                            transform.position = spawnPoint;
                            rb.linearVelocity = new Vector2(0, 0);
                            playerHealth = 6;
                            PlayerScoring.SubtractfromLevelScore(1);
                            scoreText.text = "Score: " + PlayerScoring.levelScore;
                            originalColor = Color.white;
                            spriteRenderer.color = originalColor;
                        }
                        healthText.text = "Health: " + playerHealth + "/6";
                    }

                }

            }
            else if (col.gameObject.tag == ("Hazard"))
            {
                if (isHazard && Time.time >= nextDamageTime)
                {
                    if (invincible != true && isCandyCane != true && isSprinkle != true)
                    {
                        audioSource.PlayOneShot(damageSnd);

                        spriteRenderer.color = Color.red;
                        //originalColor = burntBrown;
                        playerHealth -= damagePerTick;
                        nextDamageTime = Time.time + tickRate;
                        //UnityEngine.Debug.Log("Hazard Damage\n");
                        if (playerHealth <= 0)
                        {
                            transform.position = spawnPoint;
                            rb.linearVelocity = new Vector2(0, 0);
                            playerHealth = 6;
                            PlayerScoring.SubtractfromLevelScore(1);
                            scoreText.text = "Score: " + PlayerScoring.levelScore;
                            originalColor = Color.white;
                            spriteRenderer.color = originalColor;
                        }
                        healthText.text = "Health: " + playerHealth + "/6";
                    }

                }
            }
        }
        if(col.gameObject.tag == ("Ice"))
        {
            isOnIce = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Ground":
                isGrounded = false;
                playerAnimation.SetBool("IsGrounded", false);
                //Debug.Log("UnGrounded");
                break;
            case "ViewBlocker":
                TilemapRenderer tilemap = col.GetComponent<TilemapRenderer>();
                tilemap.enabled = true;
                break;
            case "Fire":
                playerInside = false;

                spriteRenderer.color = originalColor;
                smoke.SetActive(false);
                break;
            case "Frost":
                playerInside = false;
                spriteRenderer.color = originalColor;
                break;
            case "Hazard":
                isHazard = false;
                spriteRenderer.color = originalColor;
                break;
            case "Ice":
                isOnIce = false;
                break;
        }
        
    }


    public void ResetTotalScore()
    {
        PlayerScoring.totalScore = 0;
    }


}

