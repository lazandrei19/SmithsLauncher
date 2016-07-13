using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Materials : MonoBehaviour {

	static JSONArray materials;
	public static List<ItemMaterial> items = new List<ItemMaterial>();
	public static Dictionary<ItemMaterial, int> prices = new Dictionary<ItemMaterial, int> ();

	public static void InitializeMaterials () {
		string materialsText = (Resources.Load ("General/Text/materials") as TextAsset).text;
		materials = JSON.Parse (materialsText).AsArray;
		int i = 0;
		foreach (JSONNode material in materials) {
			items.Add (new ItemMaterial (new List<SwordComponent>{SwordComponent.Blade, SwordComponent.Guard, SwordComponent.Pommel, SwordComponent.Handle}, material["quality"].AsFloat, material["name"], material["weight"].AsFloat, material["sharpness"].AsFloat, material["difficulty"].AsFloat, material["description"], "General/Sprites/Materials/" + material["name"].ToString ().ToLower ().Substring (1, material["name"].ToString ().Length - 2)));
			prices.Add (items[i], material["price"].AsInt);
			i++;
		}
	}

	public static ItemMaterial GetMaterial (string name) {
		return items.Find (material => name == material.GetName ());
	}
}
