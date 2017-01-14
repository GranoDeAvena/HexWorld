using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

    static GameStatus gameStatus;

    protected string activeScene;
    protected string activeWindow;

    public static GameStatus GetInstance() {
        return gameStatus;
    }

    // Use this for initialization
    void Start () {
        if (gameStatus != null) {
            // Someone ELSE is the singleton already.
            // So let's just destroy ourselves before we cause trouble.
            Destroy(this.gameObject);
            return;
        }

        // If we get here, the we are "the one". Let's act like it.
        gameStatus = this;    // We are a Highlander
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    public string ActiveScene { get; set; }
    public string ActiveWindow { get; set; }

}
