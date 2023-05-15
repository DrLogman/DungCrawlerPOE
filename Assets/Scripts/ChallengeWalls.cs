using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeWalls : MonoBehaviour
{
    [SerializeField] GameObject[] enemiesNeeded;
    [SerializeField] bool startUp;
    bool movedUp, deleteAfter;

    private void Start()
    {
        deleteAfter = false;
        if(startUp)
        {
            movedUp = true;
        } else
        {
            StartCoroutine(MoveDown());
        }
    }
    private void Update()
    {
        CheckIfEnemiesLeft();
    }

    public void MoveUp()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
        movedUp = true;
    }

    IEnumerator MoveDown()
    {
        yield return new WaitForSeconds(1);
        Vector3 startPosition = transform.position;
        Vector3 destination = new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z);
        float t = 0.0f;
        while(t < 2)
        {
            t += Time.deltaTime / 3;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            yield return 0;
        }
        movedUp = false;
        if(deleteAfter == true)
        {
            Destroy(gameObject);
        }
    }

    void CheckIfEnemiesLeft()
    {
        if (movedUp == true)
        {
            int enemyCount = 0;
            foreach (GameObject enemy in enemiesNeeded)
            {
                if (enemy != null)
                {
                    enemyCount++;
                }
            }
            if (enemyCount == 0)
            {
                if (movedUp == true)
                {
                    deleteAfter = true;
                    StartCoroutine(MoveDown());
                }
            }
        }
    }
}
