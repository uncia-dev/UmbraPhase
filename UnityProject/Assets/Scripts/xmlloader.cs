using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using Umbra.Data;
using Umbra.Utilities;
////using https://blog.udemy.com/csharp-serialize-to-xml/ from infomation

namespace Umbra.Xml
{
	public class LoadFromXml
	{
		//Pass in the name of the file and what class you are loading i.e perk, ability etc..
		public static Dictionary<string, V> loadFromXml<V>(string name)
		{
		
			Dictionary<string, V> objectdic = new Dictionary<string, V>();
			UmbraCollection<V> classlist = new UmbraCollection<V>();
			string ids = "";

			XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
			XmlAttributes attrs = new XmlAttributes();

			// Create an XmlElementAttribute to override so the xml will load into right class
			XmlElementAttribute attr = new XmlElementAttribute();
			attr.ElementName = typeof(V).Name;
			attr.Type = typeof(V);

			attrs.XmlElements.Add(attr);
			attrOverrides.Add(typeof(UmbraCollection<V>), "collection", attrs);

			TextAsset xmlresources = Resources.Load("Xml/" + name) as TextAsset;


			if (xmlresources != null)
			{

				using (var r = new StringReader (xmlresources.text)) {

					XmlSerializer d = new XmlSerializer (typeof(UmbraCollection<V>), attrOverrides);
					classlist = (UmbraCollection<V>)d.Deserialize(r);

					foreach (V info in classlist.collection) {
						var type = info.GetType();
						ids = type.GetProperty("id").GetValue(info, null).ToString();
						objectdic.Add(ids, info);

						/* This breaks the game - causes ArgumentException: failed to convert parameters
						//sets the icon value to the value from iconloader
						if (type.GetProperty("icon") != null)
						{
							type.GetProperty("icon").SetValue(info, IconLoader.GetCurrentAvatarSprite(), null);
						}
						*/

					}

				}

			}
			return objectdic;
		}

		public static void Serializedata<V>(string filename, V obj)
		{
			XmlSerializer ser = new XmlSerializer(typeof(V));
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				ser.Serialize(stream, obj);
			}

		}
	}

	[Serializable, XmlRoot("xmldb")]
	public class UmbraCollection<T>
	{   
		public List<T> collection { get; set; }
	}

}