using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform referencePoint;
    [SerializeField] float delayTime;
    [SerializeField] float repeatRate;
    [SerializeField] bool isAttacking;
    
 
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnProjectile), delayTime, repeatRate);

        
        
    }
    

    void SpawnProjectile()
    {
        Vector3 targetCreationPoint = new Vector3(referencePoint.position.x, referencePoint.position.y, 0);
        Instantiate(projectilePrefab, targetCreationPoint, Quaternion.identity);
    }
  
}
