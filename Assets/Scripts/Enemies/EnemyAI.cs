using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float idleLineOfSight, chaseLineOfSight, distanceToPlayer, lineOfSight;
    public LayerMask collideLayer;
    

    public bool DetectPlayer()
    {
        if(distanceToPlayer < lineOfSight)
        {
            Vector2 directionToPlayer = new Vector2((GameController.staticPlayer.transform.position.x - transform.position.x), (GameController.staticPlayer.transform.position.y - transform.position.y));
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, collideLayer);

            if(playerHit.collider == null)
            {
                return true;
            }
        }

        return false;
    }

    


    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, GameController.staticPlayer.transform.position);
        if(player == null)
        {
            player = GameController.staticPlayer.transform;
        }
    }

}
