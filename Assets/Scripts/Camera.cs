using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float smoothTime = 1;
    [SerializeField] Vector3 cameraSpeed;
    [SerializeField] bool lookAtPlayer;
    [SerializeField] int lowerLimit = 0;


    void FixedUpdate()
    {
       
    }
    void Update()
    {
        
        transform.position = new Vector3(player.position.x, 0, transform.position.z);
     



    }
}
    
