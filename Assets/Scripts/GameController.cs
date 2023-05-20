using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static string[] sceneNames = {"Scene1"};
    public static float savedPlayerHealth = 100;
    public static List<string> scenesChosen = new List<string>();
    static int currentSceneNumber = 0;
    public static PlayerMovement staticPlayer;
    public static bool created;

    public static void RandomSceneList()
    {
        if(scenesChosen != null)
        {
            scenesChosen.Clear();
        }
        scenesChosen.Add("IntroLevel");
        while(scenesChosen.Count < sceneNames.Length + 1)
        {
            int rand = (int)(Random.Range(0, sceneNames.Length));
            if (!scenesChosen.Contains(sceneNames[rand]))
            {
                scenesChosen.Add(sceneNames[rand]);
            }
        }
        currentSceneNumber = 0;

        created = true;
    }

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(scenesChosen[currentSceneNumber]);
        currentSceneNumber++;
    }
    
} 
