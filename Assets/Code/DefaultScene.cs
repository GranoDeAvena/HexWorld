using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DefaultScene : MonoBehaviour {

    private AsyncOperation async;

    public GameObject loadingsObjects;

    IEnumerator Start() {
        //Console.GetInstance().AddString("LoadScene is load!");
        SceneManager.UnloadScene("World");
        SceneManager.UnloadScene("Castle");
        async = SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);
        //    i++;
        yield return false;
        loadingsObjects.SetActive(false);
        //SceneManager.UnloadScene("LoadScene");
        GameStatus.GetInstance().ActiveScene = "World";
        //async.allowSceneActivation = false;
        //loadingIcon.SetActive(false);
    }   // Use this for initialization
}