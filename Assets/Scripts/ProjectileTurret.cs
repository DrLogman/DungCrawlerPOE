using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTurret : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform projectile;
    [SerializeField] float destroyTime;
    [SerializeField] bool isGoingLeft;


    private void Start()
    {
        StartCoroutine(DestroyProjectile());
    }
    private void Update()
    {
        MoveProjectile();
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerMovement>() != null)
            {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(transform, 5);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.GetComponent<MovingEnemy>() != null)
        {
            collision.gameObject.GetComponent<MovingEnemy>().TakeDamage(transform, 5, 6.0f, 2.0f);
            Destroy(gameObject);
        }
       



    }
    public void MoveProjectile()
    {
        if (isGoingLeft == true)
        {
            transform.position += new UnityEngine.Vector3(-speed, 0, 0) * Time.deltaTime;
        }
        else
        {
            transform.position += new UnityEngine.Vector3(speed, 0, 0) * Time.deltaTime;
        }
    }
   
    
        private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
    

}
