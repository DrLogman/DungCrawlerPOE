using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : EnemyAI
{
    [SerializeField] float chaseMinDistance;
    [SerializeField] Transform fallCollider, touchCollider;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private GameObject rightProjectilePrefab;
    [SerializeField] private GameObject leftProjectilePrefab;
    [SerializeField] private Transform referencePoint;
    [SerializeField] bool isAttacking;
    [SerializeField] float offset;
    [SerializeField] float DelayBetweenShots;
    [SerializeField] float lastShotTime;
    Rigidbody2D rb2d;
    public float speed, health;
    string facingDirection;
    bool idle, canMove;
    Vector3 enemySize;
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        lastShotTime = Time.time;
        facingDirection = "left";
        canMove = true;
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemySize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        
    }
    
    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        CheckForChase();
        CheckDirection();
        if (idle == true)
        {
            Patrol();
        }
        DamagePlayer();
    }





    void ChasePlayer()
    {
        if (canMove == true)
        {
            lineOfSight = chaseLineOfSight;
            if ((Mathf.Abs(transform.position.x - player.position.x)) > chaseMinDistance)
            {
                if (transform.position.x < player.position.x)
                {
                    rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
                    facingDirection = "right";
                    
                    CheckForRightDelay();
                    
                }
                else
                {
                    rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
                    facingDirection = "left";
                    
                    CheckForLeftDelay();
                }
            }
            else
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }
        }
    }

    void CheckDirection()
    {
        if (facingDirection == "right")
        {
            transform.localScale = new Vector3(enemySize.x, enemySize.y, enemySize.z);
        }
        if (facingDirection == "left")
        {
            transform.localScale = new Vector3(-enemySize.x, enemySize.y, enemySize.z);
        }
    }
    void StopChasingPlayer()
    {
        lineOfSight = idleLineOfSight;

        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
    }

    void CheckForChase()
    {
        if (distanceToPlayer < lineOfSight)
        {
            if (DetectPlayer() == true)
            {
                ChasePlayer();
            }
            
        }
        else
        {
            StopChasingPlayer();
            StartCoroutine(WaitThenIdle());
        }
    }

    void Patrol()
    {
        Collider2D fallCollision = Physics2D.OverlapCircle(fallCollider.position, 0.1f, collideLayer);
        if (fallCollision == null)
        {
            FlipDirection();
        }
        else
        {
            if (facingDirection == "left")
            {
                rb2d.velocity = new Vector2(-speed / 3, rb2d.velocity.y);
            }
            if (facingDirection == "right")
            {
                rb2d.velocity = new Vector2(speed / 3, rb2d.velocity.y);
            }

            Collider2D touchCollision = Physics2D.OverlapCircle(touchCollider.position, 0.2f, collideLayer);

            if (touchCollision != null && fallCollision != null)
            {
                FlipDirection();
            }
        }
    }

    void FlipDirection()
    {

        if (facingDirection == "left")
        {

            facingDirection = "right";
        }
        else if (facingDirection == "right")
        {
            facingDirection = "left";
        }
    }

    IEnumerator WaitThenIdle()
    {
        if (DetectPlayer() == false)
        {
            yield return new WaitForSeconds(1.0f);

            if (DetectPlayer() == false)
            {
                idle = true;
            }
            else
            {
                idle = false;
            }
        }
    }

    public void DamagePlayer()
    {
        if (distanceToPlayer < 1 && playerMovement.invulnerable == false)
        {
            Collider2D touchCollision = Physics2D.OverlapBox(touchCollider.position, new Vector2(0.5f, 1), 0, playerLayer);

            if (touchCollision != null && touchCollision.tag == "Player")
            {
                playerMovement.TakeDamage(transform, 10);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(touchCollider.position, new Vector3(0.5f, 1, 0));
    }

    public void TakeDamage(Transform playerTransform, float damage, float horizontalKB, float verticalKB)
    {
        health -= damage;

        if (health > 0)
        {
            Vector3 damageForce;

            if (playerTransform.position.x > transform.position.x)
            {
                damageForce = new Vector2(-horizontalKB, verticalKB);
            }
            else
            {
                damageForce = new Vector2(horizontalKB, verticalKB);
            }

            StartCoroutine(EnemyKnockback(damageForce, 5));
        }
        else if (health <= 0)
        {
            if (playerMovement.dashActive == false)
            {
                playerMovement.ResetDash();
            }

            Destroy(gameObject);
        }
        
    }
    
    void CheckForRightDelay()
    {
        if((Time.time - lastShotTime > DelayBetweenShots))
        {
            SpawnRightProjectile();
            lastShotTime = Time.time;
        }
    }
    void CheckForLeftDelay()
    {
        if ((Time.time - lastShotTime > DelayBetweenShots))
        {
            SpawnLeftProjectile();
            lastShotTime = Time.time;
        }
    }

    void SpawnRightProjectile()
    {
        Vector3 targetCreationPoint = new Vector3(referencePoint.position.x +offset, referencePoint.position.y, 0);
        Instantiate(rightProjectilePrefab, targetCreationPoint, Quaternion.identity);
    }
    void SpawnLeftProjectile()
    {
        
        Vector3 targetCreationPoint = new Vector3(referencePoint.position.x -offset, referencePoint.position.y, 0);
        Instantiate(leftProjectilePrefab, targetCreationPoint, Quaternion.identity);
    }

    IEnumerator EnemyKnockback(Vector2 velocity, int kbScale)
    {
        canMove = false;
        rb2d.velocity = new Vector2(0, 0);
        for (int i = 0; i < kbScale; i++)
        {
            rb2d.velocity = velocity;
            yield return 0;
        }
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }
}
