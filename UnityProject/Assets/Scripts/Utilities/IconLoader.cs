using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Umbra.Utilities
{
    /// <summary>
    /// Loads and Stores all types of Icon images (Avatar, Item, Perks)
    /// </summary>
    public class IconLoader : MonoBehaviour
    {
        //AVATAR ICON MEMBERS
        public static int numAvatars; //length of array
        public static int currentAvatar; //keeps track of viewed avatar on screen for navigation
        public static Sprite[] avatarSprites;

		public static Dictionary<string, Sprite> defaults;

        /// <summary>
        /// Loads all Avatar images located in Resources/Avatars
        /// </summary>
		public static void Load(bool loadAvatars = false, List<string> playerAvatars = null)
        {

			defaults = new Dictionary<string, Sprite> ();
			defaults ["Avatars"] = Resources.Load<Sprite> ("IconImages/Avatars/DefaultAvatar");
			defaults ["Items"] = Resources.Load<Sprite> ("IconImages/Items/DefaultItem");
			defaults ["Perks"] = Resources.Load<Sprite> ("IconImages/Perks/DefaultPerk");
			defaults ["Units"] = Resources.Load<Sprite> ("IconImages/Units/DefaultUnit");
			defaults ["NoIcon"] = Resources.Load<Sprite> ("IconImages/Other/DefaultIcon");

			if (loadAvatars) {
				if (numAvatars == 0) {
					if (playerAvatars != null) {
						avatarSprites = new Sprite[playerAvatars.Count];
						for (int i=0; i < playerAvatars.Count; i++) {
							avatarSprites[i] = Resources.Load<Sprite> ("IconImages/Avatars/" + playerAvatars[i]);
						}
					} else {
						avatarSprites = Resources.LoadAll<Sprite> ("IconImages/Avatars/");
					}
					numAvatars = avatarSprites.Length - 1;
				}
			}

        }

        public static Sprite NavigateLeft()
        {
            if (numAvatars > 0)
            {
                currentAvatar--;
                if (currentAvatar < 0) currentAvatar = numAvatars;
                return avatarSprites[currentAvatar];
            }
            return null;
        }

        public static Sprite NavigateRight()
        {
            if (numAvatars > 0)
            {
                currentAvatar++;
                if (currentAvatar > numAvatars) currentAvatar = 0;
                return avatarSprites[currentAvatar];
            }
            return null;
        }

        public static Sprite GetCurrentAvatarSprite()
        {
            if (numAvatars > 0) return avatarSprites[currentAvatar];
            return null;
        }

        public static string GetCurrentAvatarName()
        {
            if (numAvatars > 0) return avatarSprites[currentAvatar].name;
            return null;
        }

		public static Sprite GetSpriteByName(string name, string type)
		{

			Sprite result;

			if (defaults.ContainsKey(type)) {
				result = Resources.Load<Sprite> ("IconImages/" + type + "/" + name);
				if (result == null) result = defaults [type];
			} else {
				result = defaults ["NoIcon"];
			}

			return result;

		}
    }
}