using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static string[] sceneNames = { "Scene1", "Scene2" , "Scene3" , "Scene4" };
    public static int savedPlayerHealth = 100;
    static List<string> scenesChosen = new List<string>();
    static int currentSceneNumber = 0;

    public static void RandomSceneList()
    {
        if(scenesChosen != null)
        {
            scenesChosen.Clear();
        }
        while(scenesChosen.Count < 3)
        {
            int rand = (int)(Random.Range(0, sceneNames.Length));
            if (!scenesChosen.Contains(sceneNames[rand]))
            {
                scenesChosen.Add(sceneNames[rand]);
            }
        }
        currentSceneNumber = 0;
    }

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(scenesChosen[currentSceneNumber]);
        currentSceneNumber++;
    }
    
} 
