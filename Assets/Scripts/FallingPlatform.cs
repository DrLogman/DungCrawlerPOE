using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float fallingDelay;
    [SerializeField] float destroyDelay;
    [SerializeField] Rigidbody2D rb2d;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  
        {
            StartCoroutine(Fall());
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(Fall());
        }
    }
    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallingDelay);
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }
}