using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    static Console console;

    public Text text;
    public RectTransform content;
    public Text FPSText;

    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    public static Console GetInstance() {
        return console;
    }

    // Use this for initialization
    void Start() {
        prevS = text.text;

        content.sizeDelta = new Vector2 (content.rect.width, 32);
        Debug.Log("New content size x = " + content.rect.width + ", y = " + content.rect.height);

        if (console != null) {
            // Someone ELSE is the singleton already.
            // So let's just destroy ourselves before we cause trouble.
            Destroy(this.gameObject);
            return;
        }

        // If we get here, the we are "the one". Let's act like it.
        console = this;    // We are a Highlander
        GameObject.DontDestroyOnLoad(this.gameObject);

        if (!FPSText) {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;
    }

    string prevS;
    int fps;
    void Update() {
        if (prevS != text.text) {
            prevS = text.text;
            Debug.Log("rect.height = " + content.rect.height);
        }

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            fps = (int)accum / frames;
            FPSText.text = "FPS: " + fps;
            if (fps < 30)
                FPSText.color = Color.red;
            else
                FPSText.color = Color.white;

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
            spam = false;
        }
    }

    bool spam;
    public void AddString (string s) {
        if (spam)
            return;
        text.text = s + "\n" + text.text;
        content.sizeDelta = new Vector2(content.rect.width, content.rect.height + 23);
        spam = true;
        //content.transform.position = new Vector3(1, 2, 3);
    }

}
