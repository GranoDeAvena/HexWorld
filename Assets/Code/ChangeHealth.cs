using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeHealth : MonoBehaviour {

    public GameObject unit;
    public RectTransform healthBar;
    public Text hpUpText;
    public Text hpDownText;
    

    int behaviorHealthPoint;
    float percentHP;
    // Use this for initialization
    void Start () {
        behaviorHealthPoint = unit.GetComponent<Unit>().healthPoint;
        hpUpText.enabled = false;
        hpDownText.enabled = false;        
    }
	
	// Update is called once per frame
	void Update () {
        if (unit.GetComponent<Unit>() == null) return;
        if (behaviorHealthPoint != unit.GetComponent<Unit>().healthPoint) {
            if (behaviorHealthPoint > unit.GetComponent<Unit>().healthPoint) {
                hpDownText.text = "-" + (behaviorHealthPoint - unit.GetComponent<Unit>().healthPoint);
                Console.GetInstance().AddString("Attack unit, " + hpDownText.text + " of damage!");
                StartCoroutine(ShowFewSecond(hpDownText));
            }
            else {
                hpUpText.text = "+" + (unit.GetComponent<Unit>().healthPoint - behaviorHealthPoint);
                StartCoroutine(ShowFewSecond(hpDownText));
            }
            Debug.Log(unit.GetComponent<Unit>().healthPoint);
            percentHP = (float)unit.GetComponent<Unit>().healthPoint / (float)unit.GetComponent<Unit>().maxHealth;
            if (percentHP > 1)
                percentHP = 1;
            healthBar.localScale = new Vector3( percentHP, 1f, 1f);
            behaviorHealthPoint = unit.GetComponent<Unit>().healthPoint;
        }


    }

    Vector3 defPos;
    IEnumerator ShowFewSecond(Text text) {
        Text textPref = (Text)Instantiate(text, text.transform.position, text.transform.rotation);
        textPref.transform.SetParent(text.transform.parent, true);
        textPref.transform.localScale = text.transform.localScale;
        //textPref.transform.position = text.transform.position;
        defPos = text.transform.position;
        textPref.enabled = true;
        
        while (textPref.transform.position.y - defPos.y - 0.3 < 0.1f) {
               textPref.transform.position = Vector3.Lerp(textPref.transform.position,
                                                      new Vector3(textPref.transform.position.x,
                                                                  defPos.y + 0.42f, 
                                                                  textPref.transform.position.z),
                                                     2f * Time.deltaTime);

            yield return new WaitForSeconds(0.02f);
            //yield return null;
        }
        Destroy(textPref);
        Debug.Log("Done!");
        yield break;   
    }
}
