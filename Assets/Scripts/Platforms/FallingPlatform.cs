using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    //he fell off
    SpriteRenderer rend;
    [SerializeField] float fallingDelay;
    [SerializeField] float destroyDelay;
    [SerializeField] float respawnTime;
    [SerializeField] bool isDestroyed;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform platformRefPos;
    [SerializeField] GameObject platform;
    Rigidbody2D rb2d;

    private void Start()
    {
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
            
            StartCoroutine(Fall());
        }
        if (isDestroyed == true && respawnTime != 0)
        {
            StartCoroutine(Respawn());

        }
    }
    private IEnumerator Fall()
    {
        isDestroyed = true;
        platformRefPos.transform.SetParent(null);
        yield return new WaitForSeconds(fallingDelay);
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(gameObject, destroyDelay);
        
        
        
    }
    private IEnumerator Respawn()
    {
        Debug.Log("Activated");
        yield return new WaitForSeconds(respawnTime);
        Vector3 creationPoint = new Vector3(platformRefPos.position.x, platformRefPos.position.y, platformRefPos.position.z);
        rb2d.bodyType = RigidbodyType2D.Static;
        Instantiate(platform, creationPoint, Quaternion.identity);
        

    }
}