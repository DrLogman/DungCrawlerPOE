using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingEnemy : EnemyAI
{
    [SerializeField] float chaseMinDistance, strength;
    [SerializeField] Transform fallCollider, touchCollider;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject skull;
    [SerializeField] bool projectileEnemy;
    [SerializeField] private GameObject projectilePrefab;
    Animator enemyAnimator;
    Rigidbody2D rb2d;
    public float speed, health;
    public bool justShot;
    string facingDirection;
    bool idle, canMove, canShoot;
    Vector3 enemySize;
    SpriteRenderer spriteRenderer;


    private void Start()
    {
        canShoot = true;
        enemyAnimator = GetComponent<Animator>();
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
        if(!projectileEnemy)
        {
            DamagePlayer();
        } else
        {
            StartCoroutine(CheckProjectile());
        }
        DetectMoving();
    }

    void DetectMoving()
    {
        if(rb2d.velocity.x != 0)
        {
            enemyAnimator.SetBool("IsWalking", true);
        } else
        {
            enemyAnimator.SetBool("IsWalking", false);
        }
    }
    


    void ChasePlayer()
    {
        if(canMove == true)
        {
            lineOfSight = chaseLineOfSight;
            if ((Mathf.Abs(transform.position.x - player.position.x)) > chaseMinDistance)
            {
                if (transform.position.x < player.position.x)
                {
                    rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
                    facingDirection = "right";
                }
                else
                {
                    rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
                    facingDirection = "left";
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
        if(facingDirection == "right")
        {
            transform.localScale = new Vector3(-enemySize.x, enemySize.y, enemySize.z);
        }
        if(facingDirection == "left")
        {
            transform.localScale = new Vector3(enemySize.x, enemySize.y, enemySize.z);
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
            else
            {
                StopChasingPlayer();
                StartCoroutine(WaitThenIdle());
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
        if(fallCollision == null)
        {
            FlipDirection();
        } else
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
        if(DetectPlayer() == false)
        {
            yield return new WaitForSeconds(2.0f);

            if(DetectPlayer() == false)
            {
                idle = true;
                enemyAnimator.speed = 0.5f;
                
            } else
            {
                idle = false;
                enemyAnimator.speed = 1.0f;
            }
        }
    }

    void DamagePlayer()
    {
        if (distanceToPlayer < 1 && playerMovement.invulnerable == false)
        {
            Collider2D touchCollision = Physics2D.OverlapBox(touchCollider.position, new Vector2(0.5f, 1), 0, playerLayer);

            if(touchCollision != null && touchCollision.tag == "Player")
            {
                enemyAnimator.SetTrigger("Attack");
                playerMovement.TakeDamage(transform, strength);
            }
        }
    }

    IEnumerator CheckProjectile()
    { 
        if(canShoot)
        {
            if (Mathf.Abs(transform.position.x - playerMovement.transform.position.x) <= chaseMinDistance && DetectPlayer())
            {
                canShoot = false;
                StartCoroutine(ShootProjectile());
                yield return new WaitForSeconds(2.0f);
                canShoot = true;
            }
            else
            {
                yield return 0;
            }
        }
    }

    IEnumerator ShootProjectile()
    {
        Vector3 vectorToPlayer = playerMovement.transform.position - transform.position;
        if (facingDirection == "left")
        {
            vectorToPlayer = playerMovement.transform.position - transform.position;
        }
        if (facingDirection == "right")
        {
            vectorToPlayer = transform.position - playerMovement.transform.position;
        }
        justShot = true;
        Instantiate(projectilePrefab, transform.position, Quaternion.AngleAxis((Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg), Vector3.forward));
        yield return new WaitForSeconds(0.3f);
        justShot = false;

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
            if(playerMovement.dashActive == false)
            {
                playerMovement.ResetDash();
            }


            Instantiate(skull, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);

            Destroy(gameObject);
        }

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
