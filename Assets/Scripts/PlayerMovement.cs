using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed, jumpForce, maxHP, health;
    [SerializeField] float dashStat, dashCooldown;
    [SerializeField] SwordPointer swordPointer;
    [SerializeField] LayerMask dashLayer;
    [SerializeField] Transform groundCheck, leftCollider, rightCollider;
    Rigidbody2D playerRB;
    private bool canMove, isOnWall, dashActive, isGrounded, doubleJump, wallJump, wallJumpReset;
    private float lastYPos;
    private string lastWallSide, playerDirection;

    private void Start()
    {
        playerDirection = "left";
        maxHP = 100;
        health = maxHP;
        playerRB = GetComponent<Rigidbody2D>();
        canMove = true;
        dashActive = true;
    }
    private void Update()
    {
        if(canMove == true)
        {
            PlayerMove();
            Jump();
            Dash();
        }

        

        
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
        if (Physics2D.OverlapCircle(groundCheck.position, 0.15f, dashLayer))
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



        Debug.Log(lastWallSide);
    }

    public void TakeDamage(Transform attackTransform, float damageTaken) //Used for melee attacks or certain projectiles, called in enemy script
    {
        Vector2 damageForce;
        health -= damageTaken;
        //Add invulnerability coroutine here
        if(health > 0)
        {
            if (attackTransform.position.x > transform.position.x)
            {
                playerDirection = "right";
                damageForce = new Vector2(-4, 3);
            }
            else
            {
                playerDirection = "left";
                damageForce = new Vector2(4, 3);
            }

            StartCoroutine(PlayerKnockback(damageForce, 5));

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
            playerRB.velocity = new Vector2(0, 0);

            // Casts ray from player, goes in direction of mouse for distance 5, hits layermask 6 (dashCollide)

            RaycastHit2D hit = Physics2D.Raycast(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, dashStat, dashLayer);
            Debug.Log(hit.collider);
            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right, Color.green, 0.5f);
            Ray airDashRay = new Ray(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashStat);


            Vector2 point;

            if (hit.collider != null) 
            {
                point = hit.point;
                //Hit something, print the tag of the object
                Debug.Log("Hitting: " + hit.collider.tag);
            } else
            {
                point = airDashRay.GetPoint(dashStat);
            }

            float dashDistance = Mathf.Sqrt(Mathf.Pow((playerRB.transform.position.x - point.x), 2) + Mathf.Pow((playerRB.transform.position.y - point.y), 2));
            Debug.Log("Dash Distance: " + dashDistance);

            Debug.DrawRay(playerRB.transform.position, swordPointer.transform.rotation * Vector2.right * dashStat, Color.green, 0.5f);

            transform.position = Vector2.MoveTowards(playerRB.transform.position, point, dashDistance);

            playerRB.velocity = new Vector2(0, 0);

            StartCoroutine(FreezePlayer(0.1f));

            dashActive = false;
            StartCoroutine(DashCooldown());
        }
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashActive = true;
    }

    IEnumerator FreezePlayer(float pauseTime)
    {
        canMove = false;
        playerRB.gravityScale = 0;
        yield return new WaitForSeconds(pauseTime);
        canMove = true;
        playerRB.gravityScale = 1;
    }

}
