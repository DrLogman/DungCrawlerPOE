using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float destroyTime, speed;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool ignorePlayer, ignoreEnemies;

    private void Start()
    {
        StartCoroutine(DestroyProjectile());
    }

    private void Update()
    {
        DestroyOnGround();
        MoveProjectile();
    }
    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    void DestroyOnGround()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x * 1.1f, gameObject.GetComponent<BoxCollider2D>().size.y * 1.1f), 0, groundLayer);

        if (col != null)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null && collision.gameObject.GetComponent<PlayerMovement>().invulnerable == false && !ignorePlayer)
            {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, 5);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.GetComponent<MovingEnemy>() != null && collision.gameObject.GetComponent<MovingEnemy>().justShot == false && !ignoreEnemies)
        {
            collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6.0f, 2.0f);
            Destroy(gameObject);
        }
        if (collision.gameObject.GetComponent<FlyingEnemy>() != null && !ignoreEnemies)
        {
            collision.gameObject.GetComponent<FlyingEnemy>().TakeDamage(transform, 5);
            Destroy(gameObject);
        }
    }

    public void MoveProjectile()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }
}
