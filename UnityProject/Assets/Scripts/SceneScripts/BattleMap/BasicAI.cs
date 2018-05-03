using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Umbra.Managers;
using Umbra.Utilities;
using Umbra.UI;
using Umbra.Data;
using Umbra.Models;

namespace Umbra.Scenes.BattleMap
{

	/*
	 * Class containing the results of an AI's "thinking" for a turn
	 */
	public class AIAction {
		
		public BattleActor caster;
		public BattleActor target;
		public Ability ability;
		public Node destination;

		public AIAction(BattleActor c, BattleActor t, Ability a, Node d) {
			caster = c;
			target = t;
			ability = a;
			destination = d;
		}

	}

	/*
	 * The AI of the game, that picks an enemy target, a friendly unit to attack the enemy, and either an ability to
	 * attack the enemy with, or move towards the enemy.
	 * 
	 * Very basic AI, using randomization for decisions.
	 */
	public class AI : MonoBehaviour
	{

		System.Random random;

		public AI() {
			Debug.Log ("Hello from the AI!");
			random = new System.Random ();
		}

		/*
		 * Make the AI do something based on the two battlegroups in the battle map
		 */
		public AIAction doSomething (List<BattleActor> friendlyUnits, List<BattleActor> enemyUnits) {

			BattleActor target = pickTarget (enemyUnits);
			BattleActor caster = null;
			Ability ability = null;
			Node dest = null;

			// Try to find a friendly unit that can attack target, or move towards it
			int attempts = 0; 
			// limit to 100; should only happen if the AI has really bad luck, or its units are disabled
			while (ability == null && dest == null && attempts < 100) {
				attempts++;
				caster = pickUnit (friendlyUnits);
				ability = pickAbility (caster, target);
				if (ability == null) {
					dest = moveTowardsTarget (caster, target);
				}
			}

			return new AIAction (caster, target, ability, dest);

		}

		/*
		 * Return list of living units from units; additionally filter out invisible and invincible units
		 */
		private List<BattleActor> getLivingActors(List<BattleActor> units, bool validUnits=false) {

			List<BattleActor> result = new List<BattleActor> ();

			foreach (BattleActor a in units) {
				if (!a.actor.unit.isDead && !a.actor.unit.isDisabled && a.actor.unit.health > 0) {
					if (
						!validUnits ||
						(validUnits && a.actor.unit.isVisible && !a.actor.unit.isInvincible)) {
						result.Add (a);
					}
				}
			}

			return result;

		}

		/*
		 * Randomly select a friendly unit from units
		 */
		private BattleActor pickUnit(List<BattleActor> units) {
			BattleActor u = null;
			List<BattleActor> living = getLivingActors (units);
			if (living.Count > 0)
				u = living[random.Next(0, living.Count)];
			// else something broke and the AI will just stay there
			return u;
		}

		/*
		 * Randomly select an enemy unit from enemies
		 */
		private BattleActor pickTarget(List<BattleActor> enemies) {
			BattleActor enemy = null;
			List<BattleActor> living = getLivingActors (enemies, true);
			if (living.Count > 0)
				enemy = living [random.Next (0, living.Count)];
			// else something broke and the AI will just stay there
			return enemy;
		}

		/*
		 * Randomly select the first available ability (by AI priority) from actor a
		 */
		private Ability pickAbility(BattleActor caster, BattleActor target) {

			if (caster != null && target != null) {

				Ability ability = null;
				AbilityModel _abilityModel = new AbilityModel ();
				double dist = ActorManager.Instance.getDistanceBetweenActors (caster.actor, target.actor);

				// Scan all 3-6 abilities (not handling items)
				for (int i = 0; i < 6; i++) {

					// ensure ability is not locked out or on a cooldown
					if (caster.abilityCooldowns [i] == 0 && !caster.abilityLockouts [i]) {
					
						Ability tmp = _abilityModel.getAbilityById (caster.actor.unit.abilities [i]);
						if (tmp != null) {
							// If an ability was grabbed already, look for a higher priority one
							if ((ability == null) || (ability != null && tmp.AIPriority < ability.AIPriority)) {
								if (_abilityModel.getAbilityState (
									   tmp,
									   false,
									   false,
									   0,
									   caster.actor.unit.altitude == target.actor.unit.altitude,
									   dist,
									   true,
									   false, // bypass self-cast abilities
									   true)) {
									ability = tmp;
								}
							}
						}
				
					}

				}

				return ability;

			}

			return null;

		}

		/*
		 * Move unit as close as possible to picked target
		 */
		private Node moveTowardsTarget(BattleActor caster, BattleActor target) {

			// only bother if caster is movable
			if (caster.actor.unit.isMovable) {

				TileManager tm = TileManager.Instance;

				// Calculate point along vector between caster and target at movement range value
				// Basically find the closest point to the target
				Vector2 r = Vector2.MoveTowards (
					new Vector2(caster.actor.x, caster.actor.y), 
					new Vector2(target.actor.x, target.actor.y), 
					caster.actor.unit.movementRange
				);
				Node dest = tm.tiles [(int)r.x] [(int)r.y];

				if (!dest.isPassable) {

					Node result = null;

					// Look at destination's nearest neighbours and randomly pick one that is passable
					// If none is found, increase search radius
					int searchRadius = 1;
					// should never reach 1000, unless the map is completely unpassable
					while (result == null && searchRadius < 1000) {
						List<Node> neighbours = tm.GetNeighbours (dest, searchRadius);
						int attempts = 0;
						while (result == null && attempts < neighbours.Count) {
							Node tmp = neighbours [random.Next (0, neighbours.Count - 1)];
							if (tmp.isPassable)	result = tmp;
							attempts++;
						}
						searchRadius++;
					}

					return result;

				}

				return dest;

			} 

			return null;

		}

	}

}