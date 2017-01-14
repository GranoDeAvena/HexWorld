using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TileMap : MonoBehaviour {

	public Unit selectedUnit;

	public TileType[] tileTypes;
    public ListOfUnits listOfUnits;

    public Text goAttackBtn;

	int[,] tiles;
	Node[,] graph;

	int mapSizeX = 30;
	int mapSizeY = 30;

	void Start() {		
        
		GenerateMapData();
		GeneratePathfindingGraph();
		GenerateMapVisual();
	}

	void GenerateMapData() {
		// Allocate our map tiles
		tiles = new int[mapSizeX,mapSizeY];

		int x,y;

		// Initialize our map tiles to be grass
		for(x=0; x < mapSizeX; x++) {
			for(y=0; y < mapSizeY; y++) {
				tiles[x,y] = 0;
			}
		}

		// Make a big swamp area
		for(x=3; x < 5; x++) {
			for(y=0; y < 8; y++) {
				tiles[x,y] = 2;
			}
		}
		for(x=3; x < 8; x++) {
			tiles[x,9] = 2;
		}
		for(y=6; y < 15; y++) {
			tiles[7,y] = 1;
		}
		for(y=11; y < 18; y++) {
			tiles[12,y] = 2;
		}
		// Let's make a u-shaped mountain range
		tiles[14, 4] = 2;
		tiles[5, 4] = 2;
		tiles[16, 14] = 2;
		tiles[7, 4] = 2;
		tiles[8, 14] = 2;

		tiles[14, 15] = 2;
		tiles[14, 6] = 2;
		tiles[8, 15] = 2;
		tiles[8, 6] = 2;

	}

    void GeneratePathfindingGraph() {
        // Initialize the array
        graph = new Node[mapSizeX, mapSizeY];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // Now that all the nodes exist, calculate their neighbours
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                // We have a 4-way connected map
                // This also works with 6-way hexes and 8-way tiles and n-way variable areas (like EU4)

                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < mapSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);

                if (y % 2 == 0) { // Четный столбец
                    if (y < mapSizeY - 1 && x > 0) // Вверх Влево
                        graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                    if (y < mapSizeY - 1)  // Вверх Вправо
                        graph[x, y].neighbours.Add(graph[x, y + 1]);

                    if (y > 0 && x > 0)
                        graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                    if (y > 0)
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                }
                else {
                    if (y < mapSizeY - 1) // Вверх Влево
                        graph[x, y].neighbours.Add(graph[x, y + 1]);
                    if (y < mapSizeY - 1 && x < mapSizeX - 1)  // Вверх Вправо
                        graph[x, y].neighbours.Add(graph[x + 1, y + 1]);

                    if (y > 0)
                        graph[x, y].neighbours.Add(graph[x, y - 1]);
                    if (y > 0 && x < mapSizeX - 1)
                        graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                }

            }
        }
    }

    public GameObject[,] go;
    void GenerateMapVisual() {
        go = new GameObject[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeX; y++) {
                TileType tt = tileTypes[tiles[x, y]];
                go[x,y] = (GameObject)Instantiate(tt.tileVisualPrefab, TileCoordToWorldCoord(x, y), Quaternion.identity);

                ClickableTile ct = go[x, y].GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
                ct.transform.SetParent(this.transform);
            }
        }
    }

    bool UnitOnTile (int x, int y) {

        foreach (Unit unit in listOfUnits.units)
            if (unit != null && x == unit.tileX && y == unit.tileY)
                return true;
        return false;
    }

	public float CostToEnterTile(int x, int y) {
		TileType tt = tileTypes [tiles [x, y]];

		if(UnitCanEnterTile(x, y) == false)
			return Mathf.Infinity;

		return tt.movementCost;
	}

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY) {

        TileType tt = tileTypes[tiles[targetX, targetY]];

        if (UnitCanEnterTile(targetX, targetY) == false)
            return Mathf.Infinity;

        float cost = tt.movementCost;

        return cost;
    }

    public bool UnitCanEnterTile(int x, int y) {
        // Не гора или другая непрозодимая клетка и нет юнитов, либо тот на кого нападаем
		return tileTypes[ tiles[x,y] ].isWalkable && (!UnitOnTile(x, y) || ItIsVictim(x, y));
	}

	float offsetX = 0f;
	float offsetY = 1f - 0.2f;

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		if (y % 2 == 0)
			return new Vector3(x, y*offsetY, 0);
		else 
			return new Vector3(x+0.5f, y*offsetY, 0);
	}


    private Unit victim;

    public void GoUnit(int x, int y) {
        if (selectedUnit.currentPath != null && selectedUnit.tileTarget.x == x && selectedUnit.tileTarget.y == y) {
            selectedUnit.stop = false;
            return;
        }
        foreach (Unit unit in listOfUnits.units)  // Есть ли юнит в точке назначения?
            if (unit != null && x == unit.tileX && y == unit.tileY) {
                victim = unit;
                Debug.Log("Click on enemy!");
            }
        if (!victim) { // Если нет - просто идти к клетке
            WalkTo(x, y);
            goAttackBtn.text = "Go";
            return;
        }

        List <Node> path = GeneratePathTo(x, y);
        if (path.Count <= 1) // Чтобы не встать на ту же клетку, где стоит враг
            return;
        //if (path.Count == 2) { // Если стоит вплотную к врагу
        //    selectedUnit.Attacking(victim); // Атаковать
        //    victim = null;
        //    return;
        //}

        //path.RemoveAt(path.Count-1);
        selectedUnit.attackingTarget = victim;
        victim = null;

        selectedUnit.currentPath = path;
        if (path != null)
            selectedUnit.DrowPath();
        goAttackBtn.text = "Attack!";
    }

    bool ItIsVictim (int x, int y) {
        if (victim == null)
            return false;
        if (x == victim.tileX && y == victim.tileY)
            return true;
        return false;
    }

    public void WalkTo (int x, int y) {
        selectedUnit.attackingTarget = null;
        selectedUnit.currentPath = GeneratePathTo(x, y);
        if (selectedUnit.currentPath != null)
            selectedUnit.DrowPath();
    }

    public List<Node> GeneratePathTo(int x, int y) {
		// Clear out our unit's old path.
		selectedUnit.currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) {
			// We probably clicked on a mountain or something, so just quit out.
			return null;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();

		Node source = graph[
			selectedUnit.tileX, 
			selectedUnit.tileY
		];

		Node target = graph[
			x, 
			y
		];

		dist[source] = 0;
		prev[source] = null;

		// Initialize everything to have INFINITY distance, since
		// we don't know any better right now. Also, it's possible
		// that some nodes CAN'T be reached from the source,
		// which would make INFINITY a reasonable value
		foreach(Node v in graph) {
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) {
			// "u" is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach(Node possibleU in unvisited) {
				if(u == null || dist[possibleU] < dist[u]) {
					u = possibleU;
				}
			}

			if(u == target) {
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) {
				//float alt = dist[u] + u.DistanceTo(v);
				float alt = dist[u] + CostToEnterTile(v.x, v.y);
				if( alt < dist[v] ) {
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
		// to our target, or there is no route at ALL to our target.

		if(prev[target] == null) {
			// No route between our target and the source
			return null;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		// Step through the "prev" chain and add it to our path
		while(curr != null) {
			currentPath.Add(curr);
			curr = prev[curr];
		}

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!
        selectedUnit.tileTarget = currentPath[0];
        currentPath.Reverse();

        return currentPath;
		
	}
}
