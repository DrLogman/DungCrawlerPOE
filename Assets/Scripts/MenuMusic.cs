using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [SerializeField] AudioSource menuStart, menuCont;


    void Start()
    {

        StartCoroutine(PlaySounds());
    }

    IEnumerator PlaySounds()
    {
        menuStart.Play();
        yield return new WaitForSeconds(menuStart.clip.length - 1);
        menuCont.Play();
    }

}
