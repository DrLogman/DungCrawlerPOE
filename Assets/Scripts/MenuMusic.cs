using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [SerializeField] AudioSource menuStart, menuCont;


    void Start()
    {
        menuStart.Play();
        
    }

    private void Update()
    {
        if (!menuStart.isPlaying && !menuCont.isPlaying)
        {
            menuCont.Play();
        }
    }

}
