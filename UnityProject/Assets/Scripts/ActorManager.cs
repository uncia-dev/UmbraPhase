using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Umbra.Utilities;
using Umbra.Data;
using Umbra.Models;
using System;

namespace Umbra.Managers
{

    public class Actor
    {
		public string name;
        public GameObject GameObject;
        //public ActorScript script;
        public GameObject sprite;
        public List<Node> path;
        public bool isWalking;
        public int cPathPos;
        public float lerp;
        public Vector3 prevPos;

		public Unit unit;
		public Character character;
		public Item item;
		public int side;
		public int sideIndex;
		public bool isHostile;

        public int x;
        public int y;

        // for switching movement
        public bool swap;
        public int tempX;
        public int tempY;

		public bool isProjectile;

		public int direction;

		public Node parent;

        public void SetGridPos(int _x, int _y) {
            x = _x;
            y = _y;
        }

        public Actor(GameObject g, string n, int _x, int _y)
        {
			name = n;
            GameObject = g;
            lerp = 0;
            cPathPos = 0;
            isWalking = false;
            x = _x;
            y = _y;
            swap = false;
			side = 0;
			sideIndex = 0;
			isHostile = false;
			isProjectile = false;
			direction = 0;
        }

    }

    public class ActorManager : SceneSingleton<ActorManager>
    {
        const float MOVE_SPEED = 4f;
        const float DIST_AWAY_FROM_GRID = -1f;

        public Dictionary<string, Actor> actors;
        Dictionary<string, Sprite> actorSprites;
		public List<Actor> moveActors;
        private TileManager tm;
		private CharacterModel _characterModel;
		private ItemModel _itemModel;
		private MapModel _mapModel;
		private UnitModel _unitModel;

		public int actorCount;
		public Actor selectedActor; // actor selected by the player
		public Actor targetedActor; // actor targeted by the player (only works if s/he selected an actor)

        void Awake()
        {
            tm = TileManager.Instance;
			_characterModel = new CharacterModel ();
			_itemModel = new ItemModel ();
			_mapModel = new MapModel ();
			_unitModel = new UnitModel ();

            Sprite[] s = Resources.LoadAll<Sprite>("Actors");

            actorSprites = new Dictionary<string, Sprite>();
            for (int i = 0; i < s.Length; i++)
            {
				actorSprites[s[i].texture.name] = s[i];
            }

            actors = new Dictionary<string, Actor>();
            moveActors = new List<Actor>();
        }

        // Use this for initialization
        void Start()
        {
			actorCount = 0;
        }

        /// <summary>
        /// Clears all the actors in the scene
        /// </summary>
        public void ClearActors()
        {
            moveActors.Clear();
            foreach (Actor a in actors.Values)
            {
                Destroy(a.GameObject);
            }
            actors.Clear();
        }

		public void changeActorDirection(Actor a, int x=-1) {

			if (x == -1)
				a.sprite.GetComponent<SpriteRenderer> ().flipX = (a.direction == 0) ? false : true; // 0 = right, 1 = left
			else {

				if (x < a.x) {
					a.direction = 1;
					a.sprite.GetComponent<SpriteRenderer> ().flipX = true;
				}

				if (x > a.x) {
					a.direction = 0;
					a.sprite.GetComponent<SpriteRenderer> ().flipX = false;
				}

			}

		}

        // Update is called once per frame
        void Update()
        {

			for (int i = 0; i < moveActors.Count; i++)
            {
                Actor a = moveActors[i];
				changeActorDirection (a);

				// Output location on grid - noticed some bugs with the actor skipping tiles in the code below
				// Debug.Log (" >> " + a.x + ", " + a.y);

				// if actor is in center of current node
				if ((Vector2)a.GameObject.transform.position == (Vector2)a.path [a.cPathPos].tile.transform.position) {

					// do we need to recalculate new path?
					if (!a.swap) {
						// Begin moving to next node in the list
						a.cPathPos++;
						a.prevPos = a.GameObject.transform.position;
						a.lerp = 0;

						// if we've reached the last node in the list
						if (a.cPathPos >= a.path.Count) {

							changeActorDirection(a, a.path[a.cPathPos -1].x);

							if (!a.isProjectile) tm.clearNodeActor (a.x, a.y);
							a.SetGridPos (a.path [a.cPathPos - 1].x, a.path [a.cPathPos - 1].y);
							// only change world locations on exploration maps
							if (a.character != null && !_mapModel.getCurrentMap().isBattleMap) a.character.worldloc.setValues (a.x, a.y, a.direction);
							if (!a.isProjectile) tm.setNodeActor (a, a.x, a.y);
							moveActors.Remove (a);
							a.isWalking = false;

							// remove projectile from game world
							if (a.isProjectile) {
								actors.Remove (a.name);
								playTargetAnimation (a);
							}

						} else {
							
							changeActorDirection (a, a.path [a.cPathPos].x);
							// set the actors x,y coords along the way
							if (!a.isProjectile) tm.clearNodeActor (a.x, a.y);
							a.SetGridPos (a.path [a.cPathPos].x, a.path [a.cPathPos].y);
							// only change world locations on exploration maps
							if (a.character != null && !_mapModel.getCurrentMap().isBattleMap) a.character.worldloc.setValues (a.x, a.y, a.direction);
							if (!a.isProjectile) tm.setNodeActor (a, a.x, a.y);

						}
					} else {
						// recreate a new path based on new x and y
						moveActors.Remove (a);
						a.isWalking = false;
						a.swap = false;
						//a.SetGridPos(a.path[a.cPathPos].x, a.path[a.cPathPos].y);
						MoveActor (a.name, a.tempX, a.tempY);
					}

				} else {
					changeActorDirection (a, a.path [a.cPathPos].x);
					// Lerp the actor towards the center of the node
					a.lerp += Time.deltaTime;
					Vector3 pos = Vector2.MoveTowards (moveActors [i].prevPos, a.path [a.cPathPos].tile.transform.position, MOVE_SPEED * a.lerp * (a.isProjectile ? 3 : 1));
					pos.z = DIST_AWAY_FROM_GRID;
					a.GameObject.transform.position = pos;
				}

            }
        }
			
		/*
		 * Play animation assigned to ability that cast projectile in a
		 */
		void playTargetAnimation(Actor a) {
			// TODO needs further implementation; for now just destroy actor
			Destroy(a.GameObject); // remove sprite from game world
		}

        /// <summary>
        /// Creates and actor and places it on the grid
        /// </summary>
        /// <param name="name">Name of the actor</param>
        /// <param name="sp">Actor sprite you want to use</param>
        /// <param name="x">X coord on grid</param>
        /// <param name="y">Y coord on grid</param>
        /// <returns>The actor object, you dont need to use this but you can have it if you want.</returns>
		public Actor CreateActor(string name, string sp, int x = 0, int y = 0, Unit u=null, Character c=null, int facing=0, Item i=null)
        {
			Vector3 pos = tm.GridToWorldspace(x, y);
            pos.z = DIST_AWAY_FROM_GRID;

            Quaternion rot = new Quaternion(-0.4331f, 0.07942f, -0.25f, 0.8622f);                           // dont ask
            actors[name] = new Actor((GameObject)Instantiate(Resources.Load("Actor"), pos, Quaternion.identity), name, x, y);
            actors[name].sprite = (GameObject)Instantiate(Resources.Load("ActorSprite"), pos, Quaternion.identity);

			try {
				actors[name].sprite.GetComponent<SpriteRenderer>().sprite = actorSprites[sp];
			} catch (KeyNotFoundException) {
				actors[name].sprite.GetComponent<SpriteRenderer>().sprite = actorSprites["devActor"];
			}
           
			// for now just using two directions for sprite direction: left and right
			// this could be expanded later on to work with 8+ directions
			if (facing == 0) {
				actors [name].sprite.GetComponent<SpriteRenderer> ().flipX = false;
			} else {
				actors [name].sprite.GetComponent<SpriteRenderer> ().flipX = true;
			}

			actors [name].direction = facing;

			actors[name].sprite.transform.rotation = rot;
            actors[name].GameObject.transform.parent = transform;
            actors[name].sprite.transform.parent = actors[name].GameObject.transform;
			actors[name].character = c;

			if (c != null)
				actors [name].unit = _characterModel.getCharacterSelectedUnit (c);
			else
				actors [name].unit = u;

			actors [name].item = i;

            return actors[name];
        }

		/*
		 * Creates a projectile actor
		 */
		public Actor CreateActorWithProjectile(string projectile, int x = 0, int y = 0) {
			Actor a = CreateActor ("projectile" + actorCount++, projectile, x, y);
			a.isProjectile = true;
			return a;
		}

		/*
		 * Crates an actor and attaches unit c's visuals to it
		 */
		public Actor CreateActorWithUnit(
			Unit u, int x=0, int y=0, int side=-1, int index=-1, bool isHostile=false, int facing = 0
		) {
			Actor a = CreateActor("actor" + actorCount++ + u.id, u.getVisuals(), x, y, u, null, facing);
			a.side = side;
			a.sideIndex = index;
			a.isHostile = isHostile;
			TileManager.Instance.setNodeActor (a, x, y);
			return a;
		}

		/*
		 * Creates an actor and attaches character c's active unit's visuals to it
		 */
		public Actor CreateActorWithCharacter(
			Character c, int x=0, int y=0, int side=-1, int index=-1, bool isHostile=false, int facing = 0
		) {
			Actor a = CreateActor(
				"actor" + actorCount++ + c.id, 
				_characterModel.getCharacterSelectedUnit(c).getVisuals(),
				x, y, 
				_characterModel.getCharacterSelectedUnit(c), 
				c,
				facing);
			a.side = side;
			a.sideIndex = index;
			a.isHostile = isHostile;
			TileManager.Instance.setNodeActor (a, x, y);
			return a;
		}

		/*
		 * Create an actor that holds an item
		 */
		public Actor CreateActorWithItem(
			Item i, int x=0, int y=0
		) {
			Actor a = CreateActor (
				"actor" + actorCount++ + i.id,
				i.sprite,
				x, y,
				null,
				null,
				0,
				i
			);
			TileManager.Instance.setNodeActor (a, x, y);
			a.isHostile = true; // to display context menu
			return a;
		}

		/*
		 * Return distance between Actors a and b
		 */
		public double getDistanceBetweenActors(Actor a, Actor b) {
			if (a != null && b != null) {
				return tm.getDistanceBetweenNodes (a.parent, b.parent);
			}
			return -1;
		}

        /// <summary>
        /// will move a character to a tile
        /// </summary>
        /// <param name="name">The actors name that you want to move</param>
        /// <param name="x">The X coord on the map you want it to move</param>
        /// <param name="y">The Y coord on the map you want it to move</param>
		public void MoveActor(string name, int x, int y, bool ignoresObstacles=false)
        {
            
			Actor a = actors[name];

			//if (!moveActors.Exists(actor => actor.name == name))
            if (!actors[name].isWalking)
			{
				a.path = tm.createPath(a.x, a.y, x, y, ignoresObstacles);
                if (a.path.Count > 0)
                {
                    a.prevPos = actors[name].GameObject.transform.position;
                    a.isWalking = true;
                    a.lerp = 0;
                    a.cPathPos = 0;
                    moveActors.Add(a);
                }
            }
            else
            {
                // actor is already moving, need to change its course. 
                // We only set the swap x and y here just incase the new pos is changed multiple times
                // before the actor reaches the center of a node=
                a.tempX = x;
                a.tempY = y;
                a.swap = true;
            }

        }

        /// <summary>
        /// will move a character to a tile
        /// </summary>
        /// <param name="a">The actors name that you want to move</param>
        /// <param name="x">The X coord on the map you want it to move</param>
        /// <param name="y">The Y coord on the map you want it to move</param>
		public void MoveActor(Actor a, int x, int y)
        {
			MoveActor(a.name, x, y);
        }

		/*
		 * Move projectile p to (x, y)
		 */
		public void MoveProjectile(Actor p, int x, int y) {
			MoveActor (p.name, x, y, true);
		}

		/*
		 * Return list of nearest units to Actor a (including a's unit), within a range of radius tiles.
		 * This is used for AoE abilities
		 */
		public List<Actor> getNearestActors(Actor a, int radius) {

			List<Actor> result = new List<Actor> ();

			if (a.unit != null) {

				// If radius > 0, scan the square grid around a, and all units to result
				if (radius > 0) {

					int xl = Mathf.Max (0, a.x - radius); // left
					int xr = Mathf.Min (tm.sizeX, a.x + radius); // right
					int yb = Mathf.Max (0, a.y - radius); // down
					int yt = Mathf.Min (tm.sizeY, a.y + radius); // up

					for (int x = xl; x <= xr; x++) {
						for (int y = yb; y <= yt; y++) {
							if (tm.tiles [x] [y].actor != null)
								result.Add (tm.tiles [x] [y].actor);
						}
					}

					// If radius is 0 or less, just add the actor's unit; skip the above calculations
				} else {
					result.Add (a);
				}

			}

			return result;

		}

		/*
		 *  Return actor from tile				 selected by the player
		 */
		public Actor getPlayerSelectedActor() {
			return (tm.playerSelectedTile == null ? null : tm.playerSelectedTile.actor);
		}

		/*
		 * Clear selected actor
		 */
		public void clearPlayerSelectedActor() {
			tm.playerSelectedTile = null;
			tm.playerTargetedTile = null; // ensure target is cleared too
		}

		/*
		 * Clear targeted actor
		 */
		public void clearPlayerTargetedActor() {
			tm.playerTargetedTile = null;
		}

		/*
		 *  Return unit from tile selected by the player
		 */
		public Unit getPlayerSelectedUnit() {
			if (tm.playerSelectedTile != null) {
				if (tm.playerSelectedTile.actor != null) {
					return tm.playerSelectedTile.actor.unit;
				}
			}
			return null;
		}

		/*
		 *  Return character from tile selected by the player
		 */
		public Character getPlayerSelectedCharacter() {
			if (tm.playerSelectedTile != null) {
				if (tm.playerSelectedTile.actor != null) {
					return tm.playerSelectedTile.actor.character;
				}
			}
			return null;
		}

		/*
		 *  Return actor from tile selected by the player
		 */
		public Actor getPlayerTargetedActor() {
			return (tm.playerTargetedTile == null ? null : tm.playerTargetedTile.actor);
		}

		/*
		 *  Return unit from tile selected by the player
		 */
		public Unit getPlayerTargetedUnit() {

			if (tm.playerTargetedTile != null) {
				if (tm.playerTargetedTile.actor != null) {
					return tm.playerTargetedTile.actor.unit;
				}
			}
			return null;
		}

		public int getPlayerSelectedActorSide() {
			Actor a = getPlayerSelectedActor ();
			if (a == null)
				return -1;
			else
				return a.side;
		}

		public int getPlayerTargetedActorSide() {
			Actor a = getPlayerTargetedActor ();
			if (a == null)
				return -1;
			else
				return a.side;
		}

		/*
		 * Return item from tile selected by the player
		 */
		public Item getPlayerTargetedItem() {
			if (tm.playerTargetedTile != null) {
				if (tm.playerTargetedTile.actor != null) {
					return tm.playerTargetedTile.actor.item;
				}
			}
			return null;
		}

		/*
		 *  Return character from tile selected by the player
		 */
		public Character getPlayerTargetedCharacter() {
			if (tm.playerTargetedTile != null) {
				if (tm.playerTargetedTile.actor != null) {
					return tm.playerTargetedTile.actor.character;
				}
			}
			return null;
		}

		/*
		 * Check if player selected actor is hostile. If none is selected, return false.
		 */
		public bool isPlayerSelectedActorHostile() {
			return getPlayerSelectedActor ().isHostile;
		}

		/*
		 * Check if player targeted actor is hostile. If none is selected, return false.
		 */
		public bool isPlayerTargetedActorHostile() {
			return getPlayerTargetedActor ().isHostile;
		}

    }
}
