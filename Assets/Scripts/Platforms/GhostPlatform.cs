using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlatform : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] float waitTime;
    [SerializeField] float respawnTime;
    [SerializeField] bool isMissing;
    void Start()
    {
        rend = this.gameObject.GetComponent<SpriteRenderer>();
        rend.enabled = true;
    }


    private void Update()
    {
        if(isMissing == true)
        {
            StartCoroutine(RespawnPlatform());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        
            StartCoroutine(DespawnPlatform());
        
    }
    
    private IEnumerator DespawnPlatform()
    {
        Debug.Log("Gone?");
        isMissing = true;
        yield return new WaitForSeconds(waitTime);
        GetComponent<BoxCollider2D>().enabled = false;
        rend.enabled = false;

    }
    private IEnumerator RespawnPlatform()
    {
        Debug.Log("Return?");
        isMissing = false;
        yield return new WaitForSeconds(respawnTime);
        GetComponent<BoxCollider2D>().enabled = true;
        rend.enabled = true;
    }
}
