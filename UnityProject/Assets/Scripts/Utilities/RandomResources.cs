using System;
using System.Collections.Generic;

namespace Umbra.Utilities
{
	public class RandomResources
	{
		/*
		 * Return a list of resources, with randomly generated amounts
		 */
		public List<int> generateRandomResources(int enemyFaction) {

			List<int> result = new List<int> ();
			System.Random r = new System.Random ();

			int fc = r.Next (100, 500);

			result.Add (r.Next (10, 150)); // minerals
			result.Add (r.Next(10, 100)); // gas
			result.Add (r.Next(0, 50)); // fuel
			result.Add (r.Next(0, 75)); // water
			result.Add (r.Next(0, 50)); // food
			result.Add (r.Next(5, 75)); // meds
			result.Add (r.Next(0, 2)); // people
			result.Add (0); // Faction 1 currency
			result.Add (enemyFaction == 2 ? fc : 0); // Faction 2 currency
			result.Add (enemyFaction == 3 ? fc : 0); // Faction 3 currency
			result.Add (enemyFaction == 4 ? fc : 0); // Faction 4 currency
			result.Add (enemyFaction == 5 ? fc : 0); // Faction 5 currency
			result.Add (enemyFaction == 6 ? fc : 0); // Faction 6 currency

			return result;

		}

	}
		
}

