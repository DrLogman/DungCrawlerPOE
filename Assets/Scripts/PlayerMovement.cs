using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed, jumpForce, maxHP;
    public float gravityDownScale;
    [SerializeField] float dashStat, dashCooldown, sliceCooldown;
    [SerializeField] SwordPointer swordPointer;
    [SerializeField] LayerMask dashLayer, enemyLayer;
    [SerializeField] Transform groundCheck, leftCollider, rightCollider, sliceTransform;
    public Image healthBarImage, dashBarImage;
    Rigidbody2D playerRB;
    private bool canMove, isOnWall, isGrounded, doubleJump, wallJump, wallJumpReset, canSlice;
    private float lastYPos;
    public string lastWallSide, playerDirection;
    public bool invulnerable, dashActive, stopDashCooldown, canExit, isStickyWallStuck, isWeightedDown, isDead;
    public Coroutine dashCoroutine = null;
    public Coroutine invulnCoroutine;
    public Coroutine dashLineCoroutine = null;
    [SerializeField] Animator sliceAnimator;
    [SerializeField] GameObject hitParticle, dashParticle;
    LineRenderer lineRenderer;
    public float health;
    [SerializeField] AudioSource jumpSound, doubleJumpSound, wallJumpSound, landSound, swingSound, dashSound, damageSound;
    public AudioSource critSound;
    Animator playerAnimator;
    private DamageFlash damageFlash;
    float dashCooldownValue;
    [SerializeField] Color cyan, yellow;

    private void Start()
    {
        
        isDead = false;
        damageFlash = GetComponent<DamageFlash>();
        playerAnimator = GetComponent<Animator>();
        GameController.staticPlayer = this;
        canSlice = true;
        stopDashCooldown = false;
        playerDirection = "right";
        invulnerable = false;
        maxHP = 100;
        health = GameController.savedPlayerHealth;
        playerRB = GetComponent<Rigidbody2D>();
        canMove = true;
        dashActive = true;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        canExit = false;
        dashCooldownValue = 1;
    }
    private void Update()
    {
        CheckPlayerDirection();
        WallStickyRun();
        if (canMove == true)
        {
            PlayerMove();
            Jump();
            Dash();
        }
        CheckAnimations();

        if (Input.GetAxis("Fire1") != 0)
        {
            SwordAttack();
        }

        UpdateHealthBar();
        UpdateDashBar();
        if (dashCoroutine != null && dashLineCoroutine != null && stopDashCooldown == true)
        {
            StopCoroutine(dashCoroutine);
            StopCoroutine(dashLineCoroutine);
            stopDashCooldown = false;
        }
        if(dashActive)
        {
            dashCooldownValue = 1;
        }
    }

    void CheckAnimations()
    {
        if(canMove)
        {
            if (playerRB.velocity.y > 0.001)
            {
                playerAnimator.SetBool("Falling", false);
                playerAnimator.SetBool("Jumping", true);
            }
            else if (playerRB.velocity.y < -0.001)
            {
                playerAnimator.SetBool("Falling", true);
                playerAnimator.SetBool("Jumping", false);
            }
            else
            {
                playerAnimator.SetBool("Falling", false);
                playerAnimator.SetBool("Jumping", false);
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                playerAnimator.SetBool("Moving", true);
                playerAnimator.SetBool("IsLeft", false);
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                playerAnimator.SetBool("Moving", true);
                playerAnimator.SetBool("IsLeft", true);
            }
            else if(Input.GetAxisRaw("Horizontal") == 0)
            {
                playerAnimator.SetBool("Moving", false);
            }

            if (playerDirection == "left")
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            if (playerDirection == "right")
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }

    public void UpdateHealthBar()
    {
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHP, 0, 1f);
    }

    public void UpdateDashBar()
    {
        dashBarImage.fillAmount = Mathf.Clamp(dashCooldownValue / 1, 0, 1f);
    }

    private void CheckPlayerDirection()
    {
        if(!playerAnimator.GetBool("WallSlide") && !playerAnimator.GetBool("Swinging"))
        {
            if (playerRB.velocity.x > 0.1)
            {
                playerDirection = "right";
            }
            if (playerRB.velocity.x < -0.1)
            {
                playerDirection = "left";
            }
        }
    }


    private void FixedUpdate()
    {
        PlayerCollision();
        
    }

    private void PlayerMove()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal"); //Add Move method and canMove variable, make it so you can't move for a bit after dashing
        
        
        playerRB.velocity = new Vector2(horizontalMovement * speed, playerRB.velocity.y);
        
    }
    public void WallStickyRun()
    {
        if (isStickyWallStuck == true)
        {
            
            playerRB.gravityScale = 0.0f;
            playerAnimator.SetBool("Climbing", true);
            float verticalMovement = Input.GetAxisRaw("Vertical");
            playerRB.velocity = new Vector2(playerRB.velocity.x, verticalMovement * speed);
        }
        if (isStickyWallStuck == false && isWeightedDown == true)
        {
            playerRB.gravityScale = 1.0f;
            playerAnimator.SetBool("Climbing", false);
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            playerAnimator.SetBool("ClimbMoving", true);
        }
        else if (Input.GetAxisRaw("Vertical") == 0)
        {
            playerAnimator.SetBool("ClimbMoving", false);
        }
    }

    private void Jump()
    {
            /*
            * Checks if player is grounded, if double jump is active, or if the player is on a wall and the wall jump reset is true
            * If the player is not grounded, check for wall jump and then double jump and remove whichever is used
            * Jumps used in order Ground -> Wall -> Double
            */
            if (Input.GetButtonDown("Jump") && (isGrounded == true || doubleJump == true || (wallJump == true && wallJumpReset == true)))
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
                

                if (isGrounded == false)
                {
                    if (wallJump == true && wallJumpReset == true)
                    {
                        wallJump = false;
                        wallJumpReset = false;
                        playerAnimator.SetTrigger("DoubleJump");
                        wallJumpSound.Play();
                    }
                    else
                    {
                        doubleJump = false;
                        playerAnimator.SetTrigger("DoubleJump");
                        doubleJumpSound.Play();
                    }

                    } else
                    {
                    jumpSound.Play();
                    playerAnimator.SetTrigger("Jump");
                    }
            }
            /*
             * Checks is space is let go, if so slows down jump velocity to give affect of pressing space longer to jump longer
             * Thanks Trey :^)
             */
            if (Input.GetButtonUp("Jump") && playerRB.velocity.y > 0f)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * 0.7f);
            }
        
    }


   private void PlayerCollision()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.15f, dashLayer) && canMove)
        {
            if(isGrounded == false)
            {
                landSound.Play();
            }
            isGrounded = true;
            doubleJump = true;
            wallJumpReset = true;
        } else
        {
            isGrounded = false;
        }

        Collider2D leftWall = Physics2D.OverlapCircle(leftCollider.position, 0.2f, dashLayer);
        Collider2D rightWall = Physics2D.OverlapCircle(rightCollider.position, 0.2f, dashLayer);

        if (leftWall != null)
        {
            wallJump = true;

            if (lastWallSide != "left")
            {
                wallJumpReset = true;
            }

            lastWallSide = "left";
            
            if(playerRB.velocity.y < 0)
            {
                playerAnimator.SetBool("WallSlide", true);
            }

            playerDirection = "left";
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if(rightWall != null)
        {
            wallJump = true;

            if (lastWallSide != "right")
            {
                wallJumpReset = true;
            }

            lastWallSide = "right";

            if (playerRB.velocity.y < 0)
            {
                playerAnimator.SetBool("WallSlide", true);
            }

            playerDirection = "right";
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            wallJump = false;
            playerAnimator.SetBool("WallSlide", false);
        }



    }

    public void TakeDamage(Transform attackTransform, float damageTaken) //Used for melee attacks or certain projectiles, called in enemy script
    {
        if(invulnerable == false)
        {
            
            Vector2 damageForce;
            health -= damageTaken;
            damageSound.Play();
            if (invulnCoroutine != null)
            {
                StopCoroutine(invulnCoroutine);
            }
            invulnCoroutine = StartCoroutine(PlayerInvulnerable(2.0f));
            if (health > 0)
            {
                damageFlash.CallPlayerDamageFlash();
                damageForce = new Vector2(-6, 5);
                if (attackTransform.position.x >= transform.position.x)
                {
                    playerDirection = "right";
                    damageForce = new Vector2(-6, 5);
                }
                else if (attackTransform.position.x < transform.position.x)
                {
                    playerDirection = "left";
                    damageForce = new Vector2(6, 5);
                }

                StartCoroutine(PlayerKnockback(damageForce, 5));
                

            } else
            {
                isDead = true;
                damageForce = new Vector2(-3, 2.5f);
                if (attackTransform.position.x >= transform.position.x)
                {
                    playerDirection = "right";
                    damageForce = new Vector2(-3, 2.5f);
                }
                else if (attackTransform.position.x < transform.position.x)
                {
                    playerDirection = "left";
                    damageForce = new Vector2(3, 2.5f);
                }
                StartCoroutine(PlayerKnockback(damageForce, 3));
                damageFlash.CallDamageFlash();
                playerAnimator.SetBool("Dead", true);
                StartCoroutine(GameOver());
            }
        }   
    }

    private IEnumerator GameOver()
    {
        canMove = false;
        canSlice = false;
        dashActive = false;
        yield return new WaitForSeconds(1.0f);
        GameController.GameOver();
    }

    IEnumerator PlayerKnockback(Vector2 velocity, int kbScale)
    {
        canMove = false;
        playerRB.velocity = new Vector2(0, 0);

        for (int i = 0; i < kbScale; i++)
        {
            playerRB.velocity = velocity;
            yield return 0;
        }

        yield return new WaitForSeconds(0.4f);
        if(isDead == false)
        {
            canMove = true;
        }
    }

    private void Dash()
    {
        if(Input.GetButtonDown("Fire2") && dashActive == true)
        {
            dashActive = false;

            playerRB.velocity = new Vector2(0, 0);

            

            Vector3 startLinePosition = transform.position;

            // Casts ray from player, goes in direction of mouse for distance 5, hits layermask 6 (dashCollide)

            RaycastHit2D hit = Physics2D.Raycast(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, dashStat, dashLayer);
            
            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, Color.green, 0.5f);
            Ray airDashRay = new Ray(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashStat);


            Vector2 point;

            if (hit.collider != null) 
            {
                point = hit.point;
                //Hit something, print the tag of the object
                
            } else
            {
                point = airDashRay.GetPoint(dashStat);
            }

            float dashDistance = Mathf.Sqrt(Mathf.Pow((playerRB.transform.position.x - point.x), 2) + Mathf.Pow((playerRB.transform.position.y - point.y), 2)) - 0.1f;

            RaycastHit2D[] enemiesHit = Physics2D.RaycastAll(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, dashDistance, enemyLayer);
            
            foreach(RaycastHit2D enemy in enemiesHit)
            {
                if (enemy.collider.gameObject.GetComponent<MovingEnemy>() != null)
                {
                    Instantiate(dashParticle, enemy.transform);
                    enemy.collider.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6, 2.0f);
                }

                if (enemy.collider.gameObject.GetComponent<SkeletonHead>() != null)
                {
                    enemy.collider.gameObject.GetComponent<SkeletonHead>().Break(this);
                }

                if (enemy.collider.gameObject.GetComponent<FlyingEnemy>() != null)
                {
                    Instantiate(dashParticle, enemy.transform);
                    enemy.collider.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
                }
            }

            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashStat, Color.green, 0.5f);

            transform.position = Vector2.MoveTowards(playerRB.transform.position, point, dashDistance);

            playerRB.velocity = new Vector2(0, 0);

            Vector3 endLinePosition = transform.position;

            StartCoroutine(FreezePlayer(0.1f));

            dashCoroutine = StartCoroutine(DashCooldown());

            if (invulnCoroutine != null)
            {
                StopCoroutine(invulnCoroutine);
            }

            invulnCoroutine = StartCoroutine(PlayerInvulnerable(1));

            if(dashLineCoroutine != null)
            {
                StopCoroutine(dashLineCoroutine);
            }

            dashLineCoroutine = StartCoroutine(DrawDashLine(startLinePosition, endLinePosition));

            dashSound.Play();
        }
    }

    public IEnumerator DashCooldown()
    {
        dashCooldownValue = 0;
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(dashCooldown/100);
            dashCooldownValue += 0.01f;
        }

        
        dashActive = true;
        StartCoroutine(DashBarFlash());
    }

    public IEnumerator SliceCooldown()
    {
        yield return new WaitForSeconds(sliceCooldown);
        canSlice = true;
    }

    IEnumerator FreezePlayer(float pauseTime)
    {
        canMove = false;
        playerRB.gravityScale = 0;
        yield return new WaitForSeconds(pauseTime);
        canMove = true;
        playerRB.gravityScale = 1;
    }

    IEnumerator PlayerInvulnerable(float seconds)
    {
        invulnerable = true;
        yield return new WaitForSeconds(seconds);
        invulnerable = false;
    }

    public void ResetDash() //dont work :(
    {
        
        stopDashCooldown = true;
        Debug.Log("REeseyt");
        dashActive = true;
        dashCooldownValue = 1;
        StartCoroutine(DashBarFlash());
    }

    public void AddHealth(float healthAdded)
    {
        health += healthAdded;
        if (health > 100)
        {
            health = 100;
        }
    }

    public void ResetInvuln() //dont work :(
    {
        stopDashCooldown = true;
        Debug.Log("REeseyt");
        dashActive = true;
    }

    void SwordAttack()
    {
        if(canSlice == true)
        {
            RaycastHit2D swordRay = Physics2D.Raycast(transform.position, swordPointer.transform.rotation * Vector2.right, 1.6f /* sword length */, enemyLayer);
            Debug.DrawRay(transform.position, swordPointer.transform.rotation * Vector2.right * 1.6f, Color.blue, 2f);
            if (playerDirection == "right")
            {
                sliceTransform.localScale = new Vector3(1.78f, 1.78f, 1.78f);
            }
            else
            {
                sliceTransform.localScale = new Vector3(1.78f, -1.78f, 1.78f);
            }

            sliceAnimator.SetTrigger("Slice");
            swingSound.Play();
            StartCoroutine(SwingPlayerAnimation());
            if(transform.position.x - swordPointer.mousePos.x < 0) 
            {
                playerDirection = "right";
            } else
            {
                playerDirection = "left";
            }
            if (swordRay.collider != null)
            {
                if (swordRay.collider.gameObject.GetComponent<MovingEnemy>() != null)
                {
                    Instantiate(hitParticle, swordRay.collider.transform);
                    swordRay.collider.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 4, 1.5f);
                }

                if (swordRay.collider.gameObject.GetComponent<FlyingEnemy>() != null)
                {
                    Instantiate(hitParticle, swordRay.collider.transform);
                    swordRay.collider.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
                }
            }

            canSlice = false;
            StartCoroutine(SliceCooldown());

        }
    }

    IEnumerator DashBarFlash()
    {
        dashBarImage.color = yellow;
        yield return new WaitForSeconds(0.1f);
        dashBarImage.color = cyan;
    }

    IEnumerator SwingPlayerAnimation()
    {
        playerAnimator.SetBool("Swinging", true);
        yield return new WaitForSeconds(0.15f);
        playerAnimator.SetBool("Swinging", false);
    }

    IEnumerator DrawDashLine(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        yield return new WaitForSeconds(1.0f);
        lineRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ExitDoor")
        {
            canExit = true;
        }
        if(collision.tag == "Challenge")
        {
            collision.GetComponent<ChallengeWalls>().MoveUp();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "ExitDoor")
        {
            canExit = false;
        }
    }
    
}
