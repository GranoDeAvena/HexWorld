using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public ListOfUnits listOfUnits;
    public Text debug;
    private AsyncOperation async;
    public Unit currentUnit;

    public GameObject nextBtn;
    public GameObject castleBtn;
    public GameObject backBtn;

    // Use this for initialization
    //   void Start () {

    //}

    public void NextBtnClick () {
        Console.GetInstance().AddString("Next btn Click!");
        foreach (Unit unit in listOfUnits.units)
            if (unit != null) {
                unit.remainingSteps = unit.remainingStepsDefoult;
                unit.stepRemText.text = "" + unit.remainingStepsDefoult;
            }
    }

    public void GoBtnClick() {
        currentUnit.stop = false;
    }

    public void CastleBtnClick () {
        Console.GetInstance().AddString("Castle btn Click!");
        GameStatus.GetInstance().ActiveScene = "Castle";
        StartCoroutine(AddScene("Castle"));
        //if (nextBtn)
        //    nextBtn.SetActive (false);
        //if (castleBtn)
        //    castleBtn.SetActive(false);
        if (backBtn)
            backBtn.SetActive(true);

    }

    public void BackBtnCastle () {
        Console.GetInstance().AddString("Back btn Click!");
        GameStatus.GetInstance().ActiveScene = "World";
        SceneManager.UnloadScene("Castle");
        if (nextBtn)
            nextBtn.SetActive(true);
        if (castleBtn)
            castleBtn.SetActive(true);
        if (backBtn)
            backBtn.SetActive(false);

    }
    
    IEnumerator AddScene(string sceneName) {
        async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return async;
        //async.allowSceneActivation = false;
        //loadingIcon.SetActive(false);
    }
}
