using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    //he fell off
    SpriteRenderer rend;
    [SerializeField] float fallingDelay, playerDelay, respawnDelay, destroyDelay;
    [SerializeField] bool isWithPlayer, isOnGround, isWithSecPlayer, isDamagingPlayer;
    [SerializeField] LayerMask enemyLayer, groundLayer;
    [SerializeField] Transform platformRefPos;
    [SerializeField] GameObject platform;
    [SerializeField] Transform[] particlePoints;
    [SerializeField] GameObject particle;
    
    Rigidbody2D rb2d;

    private void Start()
    {

        
        isWithPlayer = false;
        isOnGround = false;
        isWithSecPlayer = false;
        rb2d = GetComponent<Rigidbody2D>();
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        


    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /* if (collision.gameObject.CompareTag("Player"))  
        {
            StartCoroutine(Fall());
        }
        if (collision.gameObject.layer("Enemy")) // make check for layer instead of tag on gameobject
        {
            StartCoroutine(Fall());
        } */
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null) //makes it so any rigidboy triggers fall, we can add layers or tags or whatever for edge cases like certain projectiles or enemies
        {
            //isWithPlayer = true;
            //isWithSecPlayer = true;
            StartCoroutine(Fall());
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                if (!isWithPlayer && isWithSecPlayer == true && isDamagingPlayer == true)
                {
                    collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, 5f);
                    RespawnPlatform();
                    Destroy(gameObject);
                }
                
            }
            if (collision.gameObject.GetComponent<MovingEnemy>() != null)
            {
                collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6.0f, 2.0f);
                RespawnPlatform();
                Destroy(gameObject);
            }
            if (collision.gameObject.GetComponent<FlyingEnemy>() != null)
            {
                collision.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
                RespawnPlatform();
                Destroy(gameObject);
            } 
        }
       
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            isWithPlayer = false;
            isWithSecPlayer = true;
            

        }
    }
    private IEnumerator Fall()
    {
        
        platformRefPos.transform.SetParent(null);
        for(int i = 0; i < particlePoints.Length; i++)
        {
            Instantiate(particle, particlePoints[i].position, particlePoints[i].rotation);
        }
        yield return new WaitForSeconds(fallingDelay);
        isDamagingPlayer = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(respawnDelay);
        RespawnPlatform();
        CheckForGround();
        DestroyOnGround();

        





    }
    
    void RespawnPlatform()
    {

            Vector3 creationPoint = new Vector3(platformRefPos.position.x, platformRefPos.position.y, platformRefPos.position.z);
            rb2d.bodyType = RigidbodyType2D.Static;
            Instantiate(platform, creationPoint, Quaternion.identity);
        
    }
    
    void DestroyOnGround()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), 0, groundLayer);

        if (col != null && !isWithPlayer && isWithSecPlayer == true)
        {
            Destroy(gameObject);

        }
        if (col != null && isOnGround)
        {

            Destroy(gameObject);
        }
    }
    void CheckForGround()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), 0, groundLayer);
        if(col != null)
        {
            Debug.Log("Grounded");
            isOnGround = true;
        }
    }
}