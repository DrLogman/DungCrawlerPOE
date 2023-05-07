using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyAI
{
    [SerializeField] float health, speed, turnSpeed;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] LayerMask playerLayer;
    bool idle, canMove;
    Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        canMove = true;
    }

    private void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        ChasePlayer();
        DamagePlayer();
    }

    

    void ChasePlayer()
    {
        if (DetectPlayer() && canMove)
        { 
            RotateToPlayer();
            lineOfSight = chaseLineOfSight;
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    void RotateToPlayer()
    {
        Vector3 vectorToPlayer = player.position - transform.position;
        float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
        Quaternion turnQuat = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, turnQuat, Time.deltaTime * turnSpeed);   
    }
    void DamagePlayer()
    {
        if (distanceToPlayer < 1 && playerMovement.invulnerable == false)
        {
            Collider2D touchCollision = Physics2D.OverlapBox(transform.position, new Vector2(4, 2.0f), 0, playerLayer);

            if (touchCollision != null && touchCollision.tag == "Player")
            {
                playerMovement.TakeDamage(transform, 10);
            }
        }
    }

    void Patrol()
    {

    }

    public void TakeDamage(Transform playerTransform, float damage)
    {
        health -= damage;

        if (health > 0)
        {
            Vector3 damageForce;

            damageForce = (transform.position - player.position);

            StartCoroutine(EnemyKnockback(damageForce * 2f, 4));
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
        rb2d.velocity = new Vector2(0, 0);
        canMove = true;
    }
}
