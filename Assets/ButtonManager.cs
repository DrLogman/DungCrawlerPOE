using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void OnStartClick()
    {
        GameController.RandomSceneList();
        GameController.LoadNextScene();
    }
}
