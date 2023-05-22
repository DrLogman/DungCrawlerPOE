using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButton : MonoBehaviour
{
    public void OnStartClick()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Reset");
    }
    
}
