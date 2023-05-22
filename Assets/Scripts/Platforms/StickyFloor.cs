using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StickyFloor : MonoBehaviour
{
    float savedJumpForce = 0;
    float savedSpeed = 0;
    
    [SerializeField] float stickyStatus;

    private void OnCollisionEnter2D(Collision2D collision) //make physics collider and check every frame
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().isStickyWallStuck = true;
                savedSpeed = collision.gameObject.GetComponent<PlayerMovement>().speed;
                collision.gameObject.GetComponent<PlayerMovement>().speed /= stickyStatus;
                savedJumpForce = collision.gameObject.GetComponent<PlayerMovement>().jumpForce;
                collision.gameObject.GetComponent<PlayerMovement>().jumpForce = 0;
                
                


            }

            if (collision.gameObject.GetComponent<MovingEnemy>() != null)
            {

            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().isStickyWallStuck = false;
                collision.gameObject.GetComponent<PlayerMovement>().isWeightedDown = true;
                collision.gameObject.GetComponent<PlayerMovement>().speed = savedSpeed;
                collision.gameObject.GetComponent<PlayerMovement>().jumpForce = savedJumpForce;
                

            }
        }
    }
}