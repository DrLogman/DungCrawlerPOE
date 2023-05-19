using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSign : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite controllerSign, pcSign;
    void Update()
    {
        if (!SwordPointer.staticConnected)
        {
            sr.sprite = pcSign;
        }
        else
        {
            sr.sprite = controllerSign;
        }
    }
}
