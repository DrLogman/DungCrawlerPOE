using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float destroyTime, speed;
    [SerializeField] LayerMask groundLayer;

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
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), 0, groundLayer);

        if (col != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null && collision.gameObject.GetComponent<PlayerMovement>().invulnerable == false)
            {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, 5);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.GetComponent<MovingEnemy>() != null && collision.gameObject.GetComponent<MovingEnemy>().justShot == false)
        {
            collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6.0f, 2.0f);
            Destroy(gameObject);
        }
    }

    public void MoveProjectile()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }
}
