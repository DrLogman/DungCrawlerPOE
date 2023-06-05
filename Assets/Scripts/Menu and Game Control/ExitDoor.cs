using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SpriteRenderer buttonSprite;

    private void Update()
    {
        if (GameController.created)
        {
            if (playerMovement.canExit == true)
            {
                buttonSprite.enabled = true;
                if (Input.GetButtonDown("Fire3"))
                {
                    GameController.savedPlayerHealth = playerMovement.health;
                    GameController.LoadNextScene();
                }
            }
            else
            {
                buttonSprite.enabled = false;
            }
        }

        if (Input.GetButtonDown("Fire3") && Input.GetKey(KeyCode.I))
        {
            SceneManager.LoadScene("win");
        }

    }
}
