using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static string[] sceneNames = { "(Beg)ParkourFocus", "(Int)Spiky"};
    public static string[] sceneNames2 = { "(Adv)DrummerScene", "(Adv)JoblessScene" };
    public static float savedPlayerHealth = 100;
    public static List<string> scenesChosen = new List<string>();
    static int currentSceneNumber = 0;
    public static PlayerMovement staticPlayer;
    public static bool created;

    public static void RandomSceneList()
    {
        savedPlayerHealth = 100;

        currentSceneNumber = 0;

        if (scenesChosen != null)
        {
            scenesChosen.Clear();
        }
        scenesChosen.Add("IntroLevel");
        scenesChosen.Add("(Int)SpikesScene");
        while (scenesChosen.Count < sceneNames.Length + 2)
        {
            int rand = (int)(Random.Range(0, sceneNames.Length));
            if (!scenesChosen.Contains(sceneNames[rand]))
            {
                scenesChosen.Add(sceneNames[rand]);
            }
        }
        while (scenesChosen.Count < sceneNames2.Length + sceneNames.Length + 2)
        {
            int rand = (int)(Random.Range(0, sceneNames2.Length));
            if (!scenesChosen.Contains(sceneNames2[rand]))
            {
                scenesChosen.Add(sceneNames2[rand]);
            }
        }
        scenesChosen.Add("ENDLEVEL");

        created = true;

        
    }

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(scenesChosen[currentSceneNumber]);
        currentSceneNumber++;
    }

    public static void LoadEnd()
    {
        SceneManager.LoadScene("WIN");
    }

    public static void GameOver()
    {
        SceneManager.LoadScene("DeathScene");
    }
    
} 
