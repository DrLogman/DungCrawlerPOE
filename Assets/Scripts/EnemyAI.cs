using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    private float lineOfSight;
    [SerializeField] float idleLineOfSight, chaseLineOfSight;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] float speed;



    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < lineOfSight)
        {
            ChasePlayer();
        }
        else
        {
            StopChasingPlayer();
        }
    }
    void ChasePlayer()
    {
        lineOfSight = chaseLineOfSight;

        if (transform.position.x < player.position.x)
        {
            rb2d.velocity = new Vector2(speed, 0);

        }
        else
        {
            rb2d.velocity = new Vector2(-speed, 0);
        }
    }
    void StopChasingPlayer()
    {
        lineOfSight = idleLineOfSight;

        rb2d.velocity = new Vector2(0, 0);

    }
}
