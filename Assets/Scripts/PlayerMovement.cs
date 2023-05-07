using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed, jumpForce, maxHP, health;
    [SerializeField] float dashStat, dashCooldown, sliceCooldown;
    [SerializeField] SwordPointer swordPointer;
    [SerializeField] LayerMask dashLayer, enemyLayer;
    [SerializeField] Transform groundCheck, leftCollider, rightCollider, sliceTransform;
    public Image healthBarImage;
    Rigidbody2D playerRB;
    private bool canMove, isOnWall, isGrounded, doubleJump, wallJump, wallJumpReset, canSlice;
    private float lastYPos;
    private string lastWallSide, playerDirection;
    public bool invulnerable, dashActive, stopDashCooldown, canExit;
    public Coroutine dashCoroutine = null;
    public Coroutine dashLineCoroutine = null;
    [SerializeField] Animator sliceAnimator;
    LineRenderer lineRenderer;

    private void Start()
    {
        canSlice = true;
        stopDashCooldown = false;
        playerDirection = "left";
        invulnerable = false;
        maxHP = 100;
        health = GameController.savedPlayerHealth;
        playerRB = GetComponent<Rigidbody2D>();
        canMove = true;
        dashActive = true;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        Debug.Log(playerDirection);
    }
    private void Update()
    {
        if (canMove == true)
        {
            PlayerMove();
            Jump();
            Dash();
        }
        
        if(Input.GetMouseButtonDown(0))
        {
            SwordAttack();
        }

        UpdateHealthBar();
        if(dashCoroutine != null && dashLineCoroutine != null && stopDashCooldown == true)
        {
            StopCoroutine(dashCoroutine);
            StopCoroutine(dashLineCoroutine);
            stopDashCooldown = false;
        }

    }

    public void UpdateHealthBar()
    {
        healthBarImage.fillAmount = Mathf.Clamp(health / maxHP, 0, 1f);
    }

    private void CheckPlayerDirection()
    {
        //Add code for turning and all that
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

    private void Jump()
    {
        
            /*
            * Checks if player is grounded, if double jump is active, or if the player is on a wall and the wall jump reset is true
            * If the player is not grounded, check for wall jump and then double jump and remove whichever is used
            * Jumps used in order Ground -> Wall -> Double
            */
            if (Input.GetKeyDown(KeyCode.Space) && (isGrounded == true || doubleJump == true || (wallJump == true && wallJumpReset == true)))
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);

                if (isGrounded == false)
                {
                    if (wallJump == true && wallJumpReset == true)
                    {
                        wallJump = false;
                        wallJumpReset = false;
                    }
                    else
                    {
                        doubleJump = false;
                    }

                }
            }
            /*
             * Checks is space is let go, if so slows down jump velocity to give affect of pressing space longer to jump longer
             * Thanks Trey :^)
             */
            if (Input.GetKeyUp(KeyCode.Space) && playerRB.velocity.y > 0f)
            {
                playerRB.velocity = new Vector2(playerRB.velocity.x, playerRB.velocity.y * 0.5f);
            }
        
    }


   private void PlayerCollision()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.15f, dashLayer) && canMove)
        {
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
        }
        else if(rightWall != null)
        {
            wallJump = true;

            if (lastWallSide != "right")
            {
                wallJumpReset = true;
            }

            lastWallSide = "right";
        }
        else
        {
            wallJump = false;
        }



    }

    public void TakeDamage(Transform attackTransform, float damageTaken) //Used for melee attacks or certain projectiles, called in enemy script
    {
        if(invulnerable == false)
        {
            Vector2 damageForce;
            health -= damageTaken;
            StartCoroutine(PlayerInvulnerable(2.0f));
            if (health > 0)
            {
                if (attackTransform.position.x > transform.position.x)
                {
                    playerDirection = "right";
                    damageForce = new Vector2(-6, 5);
                }
                else
                {
                    playerDirection = "left";
                    damageForce = new Vector2(6, 5);
                }

                StartCoroutine(PlayerKnockback(damageForce, 5));
                

            }
        }   
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
        canMove = true;
    }

    private void Dash()
    {
        if(Input.GetMouseButtonDown(1) && dashActive == true)
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

            float dashDistance = Mathf.Sqrt(Mathf.Pow((playerRB.transform.position.x - point.x), 2) + Mathf.Pow((playerRB.transform.position.y - point.y), 2));

            RaycastHit2D[] enemiesHit = Physics2D.RaycastAll(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, dashDistance, enemyLayer);
            
            foreach(RaycastHit2D enemy in enemiesHit)
            {
                if (enemy.collider.gameObject.GetComponent<MovingEnemy>() != null)
                {
                    enemy.collider.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6, 2.0f);
                }

                if (enemy.collider.gameObject.GetComponent<SkeletonHead>() != null)
                {
                    enemy.collider.gameObject.GetComponent<SkeletonHead>().Break(this);
                }

                if (enemy.collider.gameObject.GetComponent<FlyingEnemy>() != null)
                {
                    enemy.collider.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
                }
            }

            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashStat, Color.green, 0.5f);

            transform.position = Vector2.MoveTowards(playerRB.transform.position, point, dashDistance);

            playerRB.velocity = new Vector2(0, 0);

            Vector3 endLinePosition = transform.position;

            StartCoroutine(FreezePlayer(0.1f));

            dashCoroutine = StartCoroutine(DashCooldown());

            StartCoroutine(PlayerInvulnerable(1));

            dashLineCoroutine = StartCoroutine(DrawDashLine(startLinePosition, endLinePosition));
        }
    }

    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashActive = true;
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
    }

    void SwordAttack()
    {
        if(canSlice == true)
        {
            RaycastHit2D swordRay = Physics2D.Raycast(transform.position, swordPointer.transform.rotation * Vector2.right, 1.75f /* sword length */, enemyLayer);
            if (swordPointer.mousePos.x - transform.position.x >= 0)
            {
                sliceTransform.localScale = new Vector3(8.9f, 8.9f, 8.9f);
            }
            else
            {
                sliceTransform.localScale = new Vector3(8.9f, -8.9f, 8.9f);
            }

            sliceAnimator.SetTrigger("Slice");
            //make slice flip if on left side and make it stay where you start it. also make it faster.

            if (swordRay.collider != null)
            {
                if (swordRay.collider.gameObject.GetComponent<MovingEnemy>() != null)
                {
                    swordRay.collider.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 4, 1.5f);
                }

                if (swordRay.collider.gameObject.GetComponent<FlyingEnemy>() != null)
                {
                    swordRay.collider.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
                }
            }

            canSlice = false;
            StartCoroutine(SliceCooldown());

        }
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
