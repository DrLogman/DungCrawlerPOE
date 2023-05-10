using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Bombs : MonoBehaviour
{
    [SerializeField] float damage, horizontalKB, verticalKB, explosionMultiplier, respawnTime;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] bool isDestroyed, isRespawnable, isNotRespawnable;
    void Start()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        rend.enabled = true;
    }
    private void Update()
    {
        if(isDestroyed == true)
        {
            StartCoroutine(RespawnBomb());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, damage * explosionMultiplier);
                if(isNotRespawnable == true)
                {
                    Destroy(gameObject);
                }
                if(isRespawnable == true)
                {
                    isDestroyed = true;
                    GetComponent<Collider2D>().enabled = false;
                    rend.enabled = false;
                }
            }
            if (collision.gameObject.GetComponent<MovingEnemy>() != null)
            {
                collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, damage, horizontalKB, verticalKB);
                if (isNotRespawnable == true)
                {
                    Destroy(gameObject);
                }
                if (isRespawnable == true)
                {
                    isDestroyed = true;
                    GetComponent<Collider2D>().enabled = false;
                    rend.enabled = false;
                }

            }
            if (collision.gameObject.GetComponent<FlyingEnemy>() != null)
            {
                collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, damage, horizontalKB, verticalKB);
                if (isNotRespawnable == true)
                {
                    Destroy(gameObject);
                }
                if (isRespawnable == true)
                {
                    isDestroyed = true;
                    GetComponent<Collider2D>().enabled = false;
                    rend.enabled = false;
                }

            }
        }
    }
    private IEnumerator RespawnBomb()
    {
     
        isDestroyed = false;
        yield return new WaitForSeconds(respawnTime);
        GetComponent<CircleCollider2D>().enabled = true;
        rend.enabled = true;
    }


}
