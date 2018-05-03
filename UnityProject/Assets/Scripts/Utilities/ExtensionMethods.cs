using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

	public static bool IsDirectChildOf(this Transform transform, Transform parent){
		foreach (Transform t in parent) {
			if (t == transform) {
				return true;
			}
		}

		return false;
	}

//	public static void IsParent
}
