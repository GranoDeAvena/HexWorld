using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableTile : MonoBehaviour {

	public int tileX;
	public int tileY;
	public TileMap map;

    private bool swipe = false;
    // Use this for initialization
    void OnMouseUp() {  // Клик Левой КМ по плитке
        if (!EventSystem.current.IsPointerOverGameObject()) { // Если клик не по кнопке UI
            map.GoUnit(tileX, tileY);
            Console.GetInstance().AddString("Mouse click! Turn it of, if app run on Android!");
        }
    }

    //void OnMouseOver() {
    //    if (!EventSystem.current.IsPointerOverGameObject())
    //        if (Input.GetMouseButtonDown(1)) // Клик Правой КМ по плитке
    //            map.AtackUnit(tileX, tileY);
    //    //Debug.Log("Pressed right click.");
    //}

    void Update() {
        if (Input.touchCount > 0) {
            if (GameStatus.GetInstance().ActiveScene == "Castle")
                return;
            if (IsPointerOverUIObject()) {
                Console.GetInstance().AddString("Tap over UI!");
                return;
            }            

            if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                if (swipe) {
                    //Debug.Log("tap return!");
                    Console.GetInstance().AddString("swipe!");
                    swipe = false;
                    return;
                }
                //Debug.Log("tap!");
                //debug.text += " tap continue! ";
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider == GetComponent<MeshCollider>()) {
                        map.GoUnit(tileX, tileY);
                        Console.GetInstance().AddString("Go to (" + tileX + " ; " + tileY + ")");                        
                    }
                }
            }

            if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                swipe = true;
                //Debug.Log("swipe!");
            }
        }
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
