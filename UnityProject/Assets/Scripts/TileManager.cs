using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Umbra.Managers;
using Umbra.Data;
using Umbra.Models;
using System;

namespace Umbra.Utilities {

    public class SpawnPoint
    {
        public int id;
        public int x;
        public int y;
        public SpawnType type;
        public string toSpawn;      // what is being spawned

    }

	public class TileManager : SceneSingleton<TileManager>
	{

	    float size = 2f;  // node diam
	    int max = 100;      // for testing this is already set

        public string defaultSprite = "dev02";
        public string mapName = "default";

        public Dictionary<string, Sprite> sprites;
        public Dictionary<string, Sprite> objects;

	    public Node[][] tiles;
		public int sizeX; // map size on x axis
		public int sizeY; // map size on y axis

        public List<GameObject> liveObjects;

		public Node playerSelectedTile; // tile selected by player
		public Node playerTargetedTile; // tile targeted by player

        public MapModel map; //use the map to load the correct one

        public List<SpawnPoint> spawnpoints;

	    // Use this for initialization
	    void Awake()
	    {
            map = new MapModel();
            liveObjects = new List<GameObject>();
            spawnpoints = new List<SpawnPoint>();

            Sprite[] allSprites = Resources.LoadAll<Sprite>("Tiles");
            sprites = new Dictionary<string, Sprite>();
            for (int i = 0; i < allSprites.Length; i++)
            {
                sprites[allSprites[i].texture.name.ToLower()] = allSprites[i];
            }
            Sprite[] allObjects = Resources.LoadAll<Sprite>("Objects");
            objects = new Dictionary<string, Sprite>();
            for (int i = 0; i < allObjects.Length; i++)
            {
                objects[allObjects[i].texture.name.ToLower()] = allObjects[i];
            }

			sizeX = 0;
			sizeY = 0;

           // if (!loadMap(""))       // this is where you would load the map(default no name one)
            //{
            //    defaultTiles();
            //    mapName = "default";
           // }

	    }

        /// <summary>
        /// Converts 2d grid positions to worldspace positions
        /// </summary>
        /// <param name="x">grid x</param>
        /// <param name="y">grid y</param>
        /// <returns></returns>
        public Vector3 GridToWorldspace(int x, int y) {
            return tiles[x][y].tile.transform.position;
        }

        /// <summary>
        /// Deletes all tiles from the scene
        /// </summary>
        public void ClearMap()
        {
            for (int x = 0; x < max; x++)
            {
                for (int y = 0; y < max; y++)
                {
                    Destroy(tiles[x][y].tile);
                }
            }
            spawnpoints.Clear();
        }

        /// <summary>
        /// Sets up the current map
        /// </summary>
        /// <param name="name">The name of the map file</param>
        /// <returns>If the map was found or not</returns>
		public bool loadMap()
        {
            Node lastTile = null;
            TextAsset mapf = Resources.Load("Maps/" + map.getCurrentMap().filename) as TextAsset;
            if (mapf != null) {
           //     ClearMap();
                Stream ss = new MemoryStream(mapf.bytes);
                using (StreamReader reader = new StreamReader(ss))
                {
                    string l;
                    bool firstLine = true;
                    while ((l = reader.ReadLine()) != null)
                    {

                        if (firstLine)
                        {
                            max = int.Parse(l);
                            defaultTiles(false);
                            firstLine = false;
                        }
                        else
                        {
                            string[] values = l.Split(',');

                            if (values[0] == "spawnpoint")
                            {
                                SpawnPoint sp = new SpawnPoint();
                                sp.id = int.Parse(values[1]);
                                if (values[2] == "Enemy") sp.type = SpawnType.Enemy;
                                else if (values[2] == "Friendly") sp.type = SpawnType.Friendly;
                                else if (values[2] == "Item") sp.type = SpawnType.Item;
                                sp.x = int.Parse(values[3]);
                                sp.y = int.Parse(values[4]);
                                sp.toSpawn = values[5];
                                spawnpoints.Add(sp);
                                //Debug.Log(sp.toSpawn + "at " + sp.x + " " + sp.y + " type: " + sp.type);
                            }

                            if (values[0] == "tile")
                            {
                                string s = values[1].ToLower();
                                int x = int.Parse(values[2]);
                                int y = int.Parse(values[3]);

                                if (tiles[x][y] != null)
                                    Destroy(tiles[x][y].tile);

                                Vector3 position = new Vector3(size * x, size * y, 0);

								sizeX = Mathf.Max(x, sizeX);
								sizeY = Mathf.Max(y, sizeY);

                                tiles[x][y] = new Node(true, x, y);
                                tiles[x][y].tile = (GameObject)Instantiate(Resources.Load("Grid"), position, Quaternion.identity);
                                tiles[x][y].tileScript = tiles[x][y].tile.GetComponent<TileScript>();
                                tiles[x][y].tile.GetComponent<SpriteRenderer>().sprite = sprites[s];
                                tiles[x][y].tile.transform.parent = transform;
                                tiles[x][y].tileScript.tile = tiles[x][y];

                                lastTile = tiles[x][y];
                            }
                            else if (values[0] == "object")
                            {
                                // object creation code
                                // if an object follows a tile then that means the object was on top of it
                                if (lastTile != null)
                                {
                                    lastTile.isPassable = false;
                                    lastTile = null;
                                }

                                string s = values[1].ToLower();
                                Vector3 pos = new Vector3(int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
                                Quaternion rot = new Quaternion(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]));
                                GameObject obj = (GameObject)Instantiate(Resources.Load("Grid"), pos, rot);
                                obj.GetComponent<SpriteRenderer>().sprite = objects[s];
                                obj.transform.parent = transform;
                                liveObjects.Add(obj);
                            }
                            else if (values[0] == "actor")
                            {
                                // actor creation code
                                // if an actor follows a tile then that means the actor was on top of it
                            }
                        }
                    }
                }
                mapName = name;
                OutpostModel outpostModel = new OutpostModel();
                Outpost retOutpost = null;
                MapModel mapM = new MapModel();
                Map currentMap = mapM.getCurrentMap();
                if (!currentMap.isBattleMap && !outpostModel.doesOutpostExist(currentMap.id)) // if map has no outpost, create one and assign it to the Map
                {
                    retOutpost = outpostModel.createOutpost(currentMap.id);
                    currentMap.outpostRef = retOutpost;
                    GameStateManager.Instance.gameState.currentOutpost = currentMap.outpostRef;
                    GameStateManager.Instance.SaveGame();
                }
                if (!currentMap.isBattleMap && outpostModel.getOutpostByID(currentMap.id).hasBeenPlaced) // if outpost has been placed on tile, render it
                {
                    GameStateManager.Instance.gameState.currentOutpost = outpostModel.getOutpostByID(currentMap.id);
                    currentMap.outpostRef = GameStateManager.Instance.gameState.currentOutpost;
                    GameStateManager.Instance.SaveGame();

                    tiles[currentMap.outpostRef.x][currentMap.outpostRef.y].isPassable = false;
                    Quaternion rot = new Quaternion(-0.4331436f, 0.07942633f, -0.2500829f, 0.8622857f);
                    Vector3 pos = GridToWorldspace(currentMap.outpostRef.x, currentMap.outpostRef.y);
                    pos.z = -1f;
                    pos.y -= 0.6f;
                    pos.x -= 0.2f;
                    GameObject obj = (GameObject)Instantiate(Resources.Load("OutpostTemplate"), pos, rot);
                    tiles[currentMap.outpostRef.x][currentMap.outpostRef.y].content = obj;
                    setNodeObject(obj, currentMap.outpostRef.x, currentMap.outpostRef.y);
                    obj.GetComponent<SpriteRenderer>().sprite = TileManager.Instance.objects["outpost"];
                    obj.transform.parent = TileManager.Instance.transform;
                    TileManager.Instance.liveObjects.Add(obj);
				}

                return true;
            }
            return false;
        }

        public void defaultTiles(bool sprite = true)
        {
            tiles = new Node[max][];
            float offset = 0f;

            for (int x = 0; x < max; x++)
            {
                tiles[x] = new Node[max];
                offset -= size / 2;
                for (int y = 0; y < max; y++)
                {
                    // found it easier to just instantiate an existing object
                    Vector3 position = new Vector3(size * x, size * y, 0);
                    tiles[x][y] = new Node(true, x, y);
                    tiles[x][y].tile = (GameObject)Instantiate(Resources.Load("Grid"), position, Quaternion.identity);
                    tiles[x][y].tileScript = tiles[x][y].tile.GetComponent<TileScript>();
                    //tiles[x][y].tile.transform.Rotate(0, 0, rotate);
                    if (sprite)
                    {
                        tiles[x][y].tile.GetComponent<SpriteRenderer>().sprite = sprites[defaultSprite];
                    }
                    else
                    {
                        tiles[x][y].tile.GetComponent<SpriteRenderer>().sprite = null;
                        tiles[x][y].isPassable = false;
                    }
                    //tiles[i][x].isPassable = !(Physics.CheckSphere(position, size / 2, unisPassableMask));

                    tiles[x][y].tile.transform.parent = transform;
                }
            }
        }

        void fillTiles()
        {
            tiles = new Node[max][];
            for (int i = 0; i < max; i++)
            {
                tiles[i] = new Node[max];
            }
        }

        /// <summary>
        /// Creates a path to the requested coords on the grid
        /// </summary>
        /// <param name="currx">The actors current x coord</param>
        /// <param name="curry">The actors current y coord</param>
        /// <param name="x">The new x coord</param>
        /// <param name="y">The new y coord</param>
        /// <returns>A list of nodes</returns>
		public List<Node> createPath(int currx, int curry, int x, int y, bool ignoresObstacles=false)
	    {

	        List<Node> ret = new List<Node>();

	        Node startNode = tiles[currx][curry];
	        Node endNode = tiles[x][y];

			// If the moving actor ignores obstacles (ie projectile), just add the start and end points
			if (ignoresObstacles) {

				ret.Add (startNode);
				ret.Add (endNode);

			// Actor cannot cross obstacles, so calculate a path
			} else {

				List<Node> openSet = new List<Node> ();
				HashSet<Node> closedSet = new HashSet<Node> ();

				openSet.Add (startNode);

				while (openSet.Count > 0) {

					Node currentNode = openSet [0];
					for (int i = 1; i < openSet.Count; i++) {
						if (openSet [i].fCost < currentNode.fCost || openSet [i].fCost == currentNode.fCost && openSet [i].hCost < currentNode.hCost) {
							currentNode = openSet [i];
						}
					}

	            openSet.Remove(currentNode);
	            closedSet.Add(currentNode);

	            if (currentNode == endNode)
	            {
	                ret = retracePath(startNode, endNode);
	                return ret;
	            }

	            foreach (Node neighbour in GetNeighbours(currentNode))
	            {
	                if (!neighbour.isPassable || closedSet.Contains(neighbour))
	                {
	                    continue;
	                }

	                int newMovementCostToN = currentNode.gCost + GetDistance(currentNode, neighbour);
	                if (newMovementCostToN < neighbour.gCost || !openSet.Contains(neighbour))
	                {
	                    neighbour.gCost = newMovementCostToN;
	                    neighbour.hCost = GetDistance(neighbour, endNode);
	                    neighbour.parent = currentNode;

							if (!openSet.Contains (neighbour)) {
								openSet.Add (neighbour);
							}
						}
					}
				}

			}

	        return ret;
	    }

		/*
		 * Return distance between Nodes a and b
		 */
		public double getDistanceBetweenNodes(Node a, Node b) {
			double result = -1;
			if (a != null && b != null) {
				return Math.Sqrt ( 
					Math.Pow(Math.Abs(a.x - b.x), 2) +  
					Math.Pow(Math.Abs(a.y - b.y), 2)
				);
			}
			return -1;
		}

	    public int GetDistance(Node a, Node b)
	    {
	        int dX = Mathf.Abs(a.x - b.x);
	        int dY = Mathf.Abs(a.y - b.y);

	        if (dX > dY)
	        {
	            return 14 * dY + 10 * (dX - dY);
	        }
	        else
	        {
	            return 14 * dX + 10 * (dY - dX);
	        }
	    }

		public List<Node> GetNeighbours(Node node, int radius=1)
		{
		
			List<Node> neighbours = new List<Node>();

			if (radius <= 0) {
				neighbours.Add (node); // just return the node
			} else {

				for (int x = -radius; x <= radius; x++) {
					for (int y = -radius; y <= radius; y++) {
						if (x == 0 && y == 0) {
							continue;
						}

						int checkX = node.x + x;
						int checkY = node.y + y;

						if (checkX >= 0 && checkX < max && checkY >= 0 && checkY < max) {
							neighbours.Add (tiles [checkX] [checkY]);
						}
					}
				}

			}
			return neighbours;
		}


	    List<Node> retracePath(Node start, Node end)
	    {
	        List<Node> path = new List<Node>();
	        Node currentNode = end;
	        while (currentNode != start)
	        {
	            path.Add(currentNode);
	            currentNode = currentNode.parent;
	        }
	        path.Reverse();
	        return path;
	    }

		// Assign object obj to tile at (x,y)
		public void setNodeObject(GameObject obj, int x, int y) {
			tiles[x][y].content = obj;
		}

		// Assign actor to tile at (x,y)
		public void setNodeActor(Actor a, int x, int y) {
			tiles[x][y].actor = a;
			tiles [x] [y].isPassable = false;
			a.parent = tiles [x][y];
		}

		public void clearNodeActor(int x, int y) {
			tiles[x][y].actor = null;
			tiles [x] [y].isPassable = true;
		}

		// Get actor at node (x,y)
		public Actor getNodeActor(int x, int y) {
			return tiles[x][y].actor;
		}

	}

	public class Node
	{
	    public bool isPassable;
	    public int x;
	    public int y;
	    public Node parent;
	    public GameObject tile; // the gameobject sprite
	    public TileScript tileScript;
		public GameObject content; // each tile can hold an object
		public Actor actor; // each tile can hold an actor

	    public int gCost;
	    public int hCost;

	    public int fCost
	    {
	        get
	        {
	            return gCost + hCost;
	        }
	    }

	    int length;

	    public Node(bool walk, int _x, int _y)
	    {
	        isPassable = walk;
	        x = _x;
	        y = _y;
	    }

	    public Node()
	    {
	        // TODO: Complete member initialization
	    }
	}
}