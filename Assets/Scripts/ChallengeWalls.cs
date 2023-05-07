using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeWalls : MonoBehaviour
{
    [SerializeField] GameObject[] enemiesNeeded;
    bool movedUp, deleteAfter;
    private void Start()
    {
        StartCoroutine(MoveDown());
        deleteAfter = false;
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
        Vector3 startPosition = transform.position;
        Vector3 destination = new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z);
        float t = 0.0f;
        while(t < 1)
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
