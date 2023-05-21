using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    //schmovin'
    [SerializeField] float platformSpeed;
    [SerializeField] Transform[] points;
    [SerializeField] int i;

    void Start()
    {
        transform.position = transform.position;
    }


    void Update()
    {

        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;

            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, platformSpeed * Time.deltaTime);
    }
    
}
