using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHead : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] LayerMask groundLayer;
    Rigidbody2D skullRB;

    private void Start()
    {
        skullRB = GetComponent<Rigidbody2D>();
        skullRB.velocity = new Vector2(0, 5);
        skullRB.AddTorque(5);
    }

    private void Update()
    {
        DetectGround();
    }
    public void Break(PlayerMovement pm)
    {
        playerMovement = pm;

        if (playerMovement.dashActive == false && playerMovement != null)
        {
            playerMovement.ResetDash();
        }

        Destroy(gameObject);
    }
    void DetectGround()
    {
        Collider2D groundCollision = Physics2D.OverlapBox(transform.position, new Vector2(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f), 0, groundLayer);

        if(groundCollision != null)
        {
            Destroy(gameObject, 1.5f);
        }
    }
}
