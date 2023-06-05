using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorFinal : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovements;
    [SerializeField] SpriteRenderer buttonSprites;

    private void Update()
    {
        
            if (playerMovements.canExit == true)
            {
                buttonSprites.enabled = true;
                if (Input.GetButtonDown("Fire3"))
                {
                    SceneManager.LoadScene("win");
                }
            }
            else
            {
                buttonSprites.enabled = false;
            }

        
    }
}
