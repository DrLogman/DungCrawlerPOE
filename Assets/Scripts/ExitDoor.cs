using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SpriteRenderer buttonSprite;

    private void Update()
    {
        if(playerMovement.canExit == true)
        {
            buttonSprite.enabled = true;
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameController.LoadNextScene();
            }
        } else
        {
            buttonSprite.enabled = false;
        }
    }
}
