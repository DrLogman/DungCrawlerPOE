using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : EnemyAI
{
    [SerializeField] float chaseMinDistance, enemySize;
    [SerializeField] Transform fallCollider, touchCollider;
    SpriteRenderer spriteRenderer;
    public float speed;
    string facingDirection;
    bool idle;


    private void Start()
    {
        facingDirection = "left";
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        CheckForChase();
        CheckDirection();
        if (idle == true)
        {
            Debug.Log("Patrolling");
            Patrol();
        }
    }

    void ChasePlayer()
    {
        lineOfSight = chaseLineOfSight;
        if(distanceToPlayer > chaseMinDistance)
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
        } else
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }
        
    }

    void CheckDirection()
    {
        if(facingDirection == "right")
        {
            transform.localScale = new Vector3(enemySize, enemySize, enemySize);
        }
        if(facingDirection == "left")
        {
            transform.localScale = new Vector3(-enemySize, enemySize, enemySize);
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
            Debug.Log("FLIP! " + facingDirection);
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
        Debug.Log("FLIPPED!");
        if (facingDirection == "left")
        {
            Debug.Log("RIGHT");
            facingDirection = "right";
            transform.localScale = new Vector3(enemySize, enemySize, enemySize);
        }
        else if (facingDirection == "right")
        {
            Debug.Log("LEFT");
            facingDirection = "left";
            transform.localScale = new Vector3(-enemySize, enemySize, enemySize);
        }
    }

    IEnumerator WaitThenIdle()
    {
        if(DetectPlayer() == false)
        {
            yield return new WaitForSeconds(1.0f);

            if(DetectPlayer() == false)
            {
                idle = true;
            } else
            {
                idle = false;
            }
        }
    }

    
}
