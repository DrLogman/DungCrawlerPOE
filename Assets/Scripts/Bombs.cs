using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Bombs : MonoBehaviour
{
    [SerializeField] float damage, horizontalKB, verticalKB, explosionMultiplier, speed, respawnTime, splashRange, damageRadius, initialRadius;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] bool isDestroyed, isRespawnable, isNotRespawnable, isProjectile;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] CircleCollider2D circleCollider;
    void Start()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        initialRadius = circleCollider.radius;
        rend.enabled = true;
    }
    private void Update()
    {
        if(isDestroyed == true)
        {
            StartCoroutine(RespawnBomb());
        }
       if(isProjectile == true)
        {
            DestroyOnGround();
            MoveProjectile();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                SplashDamageObjects();
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
                SplashDamageObjects();
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
                SplashDamageObjects();
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
        circleCollider.radius = initialRadius;
        rend.enabled = true;
    }

    void DestroyOnGround()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), 0, groundLayer);

        if (col != null)
        {
            SplashDamageObjects();
            Destroy(gameObject);
        }
    }
    public void MoveProjectile()
    {
        transform.position += transform.right * Time.deltaTime * speed;
    }
    public void SplashDamageObjects()
    {
        if (splashRange > 0)
        {
            circleCollider.radius = damageRadius;
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRange);
            foreach (var hitCollider in hitColliders)
            {
               
                var enemy = hitCollider.GetComponent<MovingEnemy>();
                var player = hitCollider.GetComponent<PlayerMovement>();
                var flyingEnemy = hitCollider.GetComponent<FlyingEnemy>();
                if (enemy)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);
                    enemy.TakeDamage(transform, damage, horizontalKB, verticalKB);
                }
                if (player)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);
                    player.TakeDamage(transform, damage * explosionMultiplier);
                }
                if(flyingEnemy)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);
                    flyingEnemy.TakeDamage(transform, damage);
                }

            }
        }
    }

}
