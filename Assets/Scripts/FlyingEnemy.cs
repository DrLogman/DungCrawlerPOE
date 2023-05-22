using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyAI
{
    [SerializeField] float health, speed, turnSpeed;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject deathParticle;
    private DamageFlash damageFlash;
    bool idle, canMove;
    Rigidbody2D rb2d;

    private void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
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
        if (distanceToPlayer < 1 && GameController.staticPlayer.invulnerable == false)
        {
            Collider2D touchCollision = Physics2D.OverlapBox(transform.position, new Vector2(4, 2.0f), 0, playerLayer);

            if (touchCollision != null && touchCollision.tag == "Player")
            {
                GameController.staticPlayer.TakeDamage(transform, 10);
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

            damageForce = (transform.position - GameController.staticPlayer.transform.position);

            StartCoroutine(EnemyKnockback(damageForce * 2f, 4));
            damageFlash.CallDamageFlash();
        }
        else if (health <= 0)
        {
            if (GameController.staticPlayer.dashActive == false)
            {
                GameController.staticPlayer.ResetDash();
            }
            Instantiate(deathParticle, transform.position, transform.rotation);
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
