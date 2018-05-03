using UnityEngine;
using System.Collections;
using Umbra.Models;
using Umbra.Utilities;
using Umbra.Data;
using Umbra.UI;
using Umbra.Scenes.BattleMap;
using System.Collections.Generic;
using Umbra.Xml;
using System;
using UnityEngine.UI;
using Umbra.Managers;

namespace Umbra.Scenes.BattleMap {

	/*
	 * Helper class that keeps track of a unit's abilities and also acts as an element in the actor list
	 */
	public class BattleActor {

		public Actor actor;
		public List<int> abilityCooldowns;
		public List<bool> abilityLockouts;
		public List<string> buffs;
		public List<int> buffDurations;
		public int side;
		public int sideIndex;

		public BattleActor(Actor a, int s, int i) {
			actor = a;
			abilityCooldowns = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0};
			abilityLockouts = new List<bool> { false, false, false, false, false, false, false, false };
			// ensure non existing abilities never get called
			if (!a.unit.isPlayerCharacter) abilityLockouts [5] = true;
			if (!a.unit.isCharacter) abilityLockouts [4] = true;
			buffs = new List<string> ();
			buffDurations = new List<int> ();
			side = s;
			sideIndex = i;
		}

		public void applyColor(Color color) {
			actor.sprite.GetComponent<SpriteRenderer> ().color = color;
		}

		public bool addBuff(string buffID, int duration) {
			int e = buffs.IndexOf (buffID);
			if (e == -1) { // add new buff to list
				buffs.Add (buffID);
				buffDurations.Add (duration);
				return true;
			} else { // refresh duration if buff exists already
				buffDurations [e] = duration;
				return false; // didn't add buff, just refreshed it
			}
		}

		public void removeBuff(int idx) {
			if (idx >= 0 && idx < buffs.Count) {
				buffs.RemoveAt (idx);
				buffDurations.RemoveAt (idx);
			}
		}

		public void removeBuff(string buffID) {
			int e = buffs.IndexOf (buffID);
			if (e != -1) {
				buffs.RemoveAt (e);
				buffDurations.RemoveAt (e);
			}
		}

		public void removeAllBuffs() {
			buffs.Clear ();
			buffDurations.Clear ();
			applyColor (new Color (1f, 1f, 1f));
		}

	}

}