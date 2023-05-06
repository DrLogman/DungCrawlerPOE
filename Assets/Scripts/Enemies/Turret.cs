using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] float delayTime, repeatRate, turnSpeed;
    [SerializeField] bool rangeless, canRotate;

    void Start()
    {
        InvokeRepeating(nameof(SpawnProjectile), delayTime, repeatRate);
    }

    private void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        RotateToPlayer();
    }

    void RotateToPlayer()
    {
        if(DetectPlayer() && canRotate)
        {
            Vector3 vectorToPlayer = player.position - transform.position;
            float angle = Mathf.Atan2(vectorToPlayer.y, vectorToPlayer.x) * Mathf.Rad2Deg;
            Quaternion turnQuat = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, turnQuat, Time.deltaTime * turnSpeed);
        }
    }
    void SpawnProjectile()
    {
        if (DetectPlayer() || rangeless)
        {
            Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
    }
}
