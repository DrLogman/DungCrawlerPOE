using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] SwordPointer swordPointer;
    [SerializeField] LayerMask dashLayer;
    [SerializeField] float dashStat;
    private bool isOnWall;
    public bool dashActive;
    public bool isGrounded;
    public bool doubleJump;
    public bool wallJump;
    public bool wallJumpReset;
    public float lastYPos;
    int lastWallID;
    //set most of these to private when done testing, only set to public to see if they change correctly. as of right now all are tested thoroughly except for dash related variables

    private void Update()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal"); //Add Move method and canMove variable, make it so you can't move for a bit after dashing

        playerRB.velocity = new Vector2(horizontalMovement * speed, playerRB.velocity.y);

        Jump();
        Dash();
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

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            doubleJump = true;
            wallJumpReset = true;

        }

        if (collision.gameObject.tag == "Wall")
        {
            wallJump = true;

            if(lastWallID != collision.collider.GetInstanceID())
            {
                wallJumpReset = true;
            }

            lastWallID = collision.collider.GetInstanceID();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }

        if (collision.gameObject.tag == "Wall")
        {
            wallJump = false;
        }
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

            dashActive = false;
            StartCoroutine(DashCooldown(2.0f));
        }
    }

    IEnumerator DashCooldown(float cooldownSeconds)
    {
        yield return new WaitForSeconds(cooldownSeconds);
        dashActive = true;
    }

}
