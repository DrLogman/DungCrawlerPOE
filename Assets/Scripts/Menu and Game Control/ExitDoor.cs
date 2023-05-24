using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorFinal : MonoBehaviour
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
                    GameController.LoadEnd();
                }
            }
            else
            {
                buttonSprite.enabled = false;
            }
        }
        
    }
}
