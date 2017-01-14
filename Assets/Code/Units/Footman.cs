using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Footman : MonoBehaviour {

    public string name;
    public int tileX;
    public int tileY;
    public TileMap map;
    public Text stepRemText;
    public Canvas underUnitLabel;
    public GameObject model;


    public Unit attackingTarget;
    public Node tileTarget;
    public int healthPoint = 100;  // Текущее здоровье
    public int maxHealth = 100;
    public int force = 2;  // Сила атаки
    private int stepAmount = 2;
    public float remainingSteps = 5; // Шагов осталось 
    public float remainingStepsDefoult = 5; // Шагов по умолчанию
    public bool alive = true;

    private LineRenderer line;

    public List<Node> currentPath = null;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public bool stop;

    void Start() {
        stop = true;
        tileX = (int)transform.position.x;
        tileY = (int)transform.position.y;

        stepRemText.text = "" + remainingSteps;
        transform.position = map.TileCoordToWorldCoord(tileX, tileY);

        line = new LineRenderer();
        line = gameObject.AddComponent<LineRenderer>();
        //line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetColors(c1, c2);
        line.SetWidth(0.05F, 0.05F);

    }

    void Update() {
        if (!alive)
            return;
        if (healthPoint <= 0) {  // Умер
            underUnitLabel.enabled = false;
            tileX = 0;
            tileY = 0;
            alive = false;
            model.SetActive(false);
            //GetComponent<GameObject>().SetActive(false);
            //Destroy(this.GetComponent<GameObject>());
            return;
            //Destroy(this);
        }
        if (currentPath == null || stop)
            return;  // Если путь пуст или горит стоп - выйти        

        // Если до клетки большое расстояние                                          
        if (Vector3.Distance(transform.position, map.TileCoordToWorldCoord(tileX, tileY)) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord(tileX, tileY),
                                              5f * Time.deltaTime);// Двигать юнита в сторону нужной клетки
        else {  // Иначе
            if (currentPath.Count == 2 && attackingTarget != null) {
                Attacking(attackingTarget);  // если предпоследняя клетка и есть цель для атаки
                //attackingTarget = null;  // то Атаковать
                stop = true;
                return;
            }
            if (currentPath.Count > 1) // Если ход не последний
                AdvancePathing();
            else {
                if (Vector3.Distance(transform.position, map.TileCoordToWorldCoord(tileX, tileY)) < 0.02f) {
                    transform.position = map.TileCoordToWorldCoord(tileX, tileY);
                    currentPath = null; // Остановиться
                    stop = true;
                    Debug.Log("Обнулить путь!");
                    //if (attackingTarget != null) { // и если нужно атаковать
                    //    Attacking(attackingTarget);
                    //    attackingTarget = null;
                    //}
                }
                else       // Медленно двигать юнита в сторону последней клетки
                    transform.position = Vector3.Lerp(transform.position, map.TileCoordToWorldCoord(tileX, tileY),
                                                        4f * Time.deltaTime);
            }
        }
    }

    int currNode;
    public void DrowPath() {
        currNode = 0;
        line.SetVertexCount(currentPath.Count);
        while (currNode < currentPath.Count) {  // Нарисовать путь
            Vector3 v = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
                new Vector3(0, 0, -0.1f);
            //Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
            //    new Vector3(0, 0, -0.1f);

            line.SetPosition(currNode, v);
            currNode++;
        }
    }

    void AdvancePathing() {
        //Debug.Log("Ход!");

        if (remainingSteps <= 0) {
            transform.position = map.TileCoordToWorldCoord(tileX, tileY);
            stop = true;
            //Debug.Log("Кончились ходы!");
            return;
        }

        // Teleport us to our correct "current" position, in case we
        // haven't finished the animation yet.
        transform.position = map.TileCoordToWorldCoord(tileX, tileY);

        // Get cost from current tile to next tile
        remainingSteps -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);
        stepRemText.text = "" + remainingSteps;

        // Move us to the next tile in the sequence
        tileX = currentPath[1].x;
        tileY = currentPath[1].y;

        // Remove the old "current" tile from the pathfinding list
        currentPath.RemoveAt(0);
        DrowPath();
    }

    public void Attacking(Unit victim) {
        victim.healthPoint -= force;
    }

    public void MoveNextTile() {
        float remainningMovement = stepAmount; // 2

        if (currentPath == null)
            return;

        remainningMovement -= map.CostToEnterTile(currentPath[0].x, currentPath[0].y, currentPath[1].x, currentPath[1].y);
        tileX = currentPath[1].x;
        tileY = currentPath[1].y;

        transform.position = map.TileCoordToWorldCoord(currentPath[0].x, currentPath[0].y);

        currentPath.RemoveAt(0);

        if (currentPath.Count == 1) {
            currentPath = null;
        }
    }
}
