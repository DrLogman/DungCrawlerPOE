using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    //he fell off
    [SerializeField] float fallingDelay;
    [SerializeField] float destroyDelay;
    [SerializeField] LayerMask enemyLayer;
    Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.layer);

        /* if (collision.gameObject.CompareTag("Player"))  
        {
            StartCoroutine(Fall());
        }
        if (collision.gameObject.CompareTag("Enemy")) // make check for layer instead of tag on gameobject
        {
            StartCoroutine(Fall());
        } */
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null) //makes it so any rigidboy triggers fall, we can add layers or tags or whatever for edge cases like certain projectiles or enemies
        {
            StartCoroutine(Fall());
        }
    }
    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallingDelay);
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(gameObject, destroyDelay);
    }
}