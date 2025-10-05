using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneList
{
    Menu,
    Tutorial
}
public class LevelManager : MonoBehaviour
{
    public GameObject spawnPosition;
    public SceneList nextScene;

    public static void LoadFirstScene()
    {
        SceneManager.LoadScene(((int)SceneList.Tutorial));
    }

    public void SwitchScene(SceneList nextScene, GameObject spawnPosition)
    {
        StartCoroutine(SwitchSceneAsync());
        
    }

    IEnumerator SwitchSceneAsync ()
    {

        yield return null;
    }
}
