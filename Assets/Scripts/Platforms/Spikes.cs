using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] float damage, horizontalKB, verticalKB;
    
    
    
    //spiky
    
    void Start()
    {
       
    }

    private void OnCollisionEnter2D(Collision2D collision) //make physics collider and check every frame
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if(collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, damage);
            }
            if (collision.gameObject.GetComponent<MovingEnemy>() != null)
            {
                collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, damage, horizontalKB, verticalKB);
            }
        }
    }
     
}
