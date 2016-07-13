using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour {

	public static QuestSystem q;
	int currentQuest = 0;
	JSONNode quests;
	public KeyValuePair<ItemMaterial, int> lastBought = new KeyValuePair<ItemMaterial, int>();

	void Awake () {
		if (q == null) {
			DontDestroyOnLoad (gameObject);
			q = this;
		} else if (q != this) {
			Destroy (gameObject);
		}
	}

	public JSONClass GetCurrentQuest () {
		return this.quests [currentQuest].AsObject;
	}

	void Start () {
		string questsText = (Resources.Load ("General/Text/quests") as TextAsset).text;
		quests = JSON.Parse (questsText);
		GameController.beforeSave += delegate() {
			GameController.control.SetInt ("quests/currentQuest", currentQuest);
		};
		GameController.afterLoad += delegate() {
			currentQuest = GameController.control.GetInt ("quests/currentQuest");
		};
	}

	void GiveReward (JSONClass reward) {
		if (reward ["type"].ToString() == "\"money\"") {
			GameController.control.SetInt ("currency", GameController.control.GetInt ("currency") + reward ["value"].AsInt);
		} else if (reward ["type"].ToString() == "\"material\"") {
			foreach (JSONNode material in reward ["value"].AsArray) {
				InventorySystem.controller.AddItem (Materials.GetMaterial (material ["material"]), material ["count"].AsInt);
			}
		}
	}

	public bool CheckRequirements (KeyValuePair<ItemMaterial, int> bought) {
		JSONClass requirements = quests [currentQuest]["requirements"].AsObject;
		if (CheckRequirements (bought, requirements ["value"].AsObject)) {
			this.GiveReward (quests [currentQuest] ["reward"].AsObject);
			this.currentQuest++;
			return true;
		} else {
			return false;
		}
	}

	bool CheckRequirements (KeyValuePair<ItemMaterial, int> bought, JSONClass requirements) {
		if (this.lastBought.Key == bought.Key) {
			this.lastBought = new KeyValuePair<ItemMaterial, int> (lastBought.Key, lastBought.Value + bought.Value);
		} else {
			this.lastBought = bought;
		}
		if (this.lastBought.Key == Materials.GetMaterial (requirements ["material"]) && this.lastBought.Value >= requirements ["count"].AsInt) {
			return true;
		} else {
			return false;
		}
	}

	public bool CheckRequirements (ItemSword sword) {
		JSONClass requirements = quests [currentQuest]["requirements"].AsObject;
		if (CheckRequirements (sword, requirements ["value"].AsObject)) {
			this.GiveReward (quests [currentQuest] ["reward"].AsObject);
			this.currentQuest++;
			return true;
		} else {
			return false;
		}
	}

	bool CheckRequirements (ItemSword sword, JSONClass requirements) {
		foreach (KeyValuePair<string, JSONNode> requirement in requirements) {
			if (requirement.Key == "minSpeed" && sword.GetDisplaySpeed () < requirement.Value.AsFloat)
				return false;
			if (requirement.Key == "maxSpeed" && sword.GetDisplaySpeed () > requirement.Value.AsFloat)
				return false;
			if (requirement.Key == "minDurability" && sword.GetDurability () < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "maxDurability" && sword.GetDurability () > requirement.Value.AsInt)
				return false;
			if (requirement.Key == "minBalance" && sword.GetBalance () < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "maxBalance" && sword.GetBalance () < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "minDamage" && sword.GetDamage () < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "maxDamage" && sword.GetDamage () > requirement.Value.AsInt)
				return false;
			if (requirement.Key == "minLength" && sword.GetBlade ().GetExtra ("length") < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "maxLength" && sword.GetBlade ().GetExtra ("length") > requirement.Value.AsInt)
				return false;
			if (requirement.Key == "minSellPrice" && GameController.control.GetInt ("price") < requirement.Value.AsInt)
				return false;
			if (requirement.Key == "maxSellPrice" && GameController.control.GetInt ("price") > requirement.Value.AsInt)
				return false;
			if (requirement.Key == "bladeMaterial") {
				List<string> materials = new List<string>();
				foreach (JSONNode material in requirement.Value.Childs) {
					materials.Add (material);
				}
				if (!materials.Contains (sword.GetBlade ().GetMaterial ().GetName ())) {
					return false;
				}
			}
			if (requirement.Key == "handleMaterial") {
				List<string> materials = new List<string>();
				foreach(JSONNode material in requirement.Value.Childs) {
					materials.Add (material);
				}
				if (!materials.Contains (sword.GetHandle ().GetMaterial ().GetName ())) {
					return false;
				}
			}
			if (requirement.Key == "guardMaterial") {
				List<string> materials = new List<string>();
				foreach(JSONNode material in requirement.Value.Childs) {
					materials.Add (material);
				}
				if (!materials.Contains (sword.GetGuard ().GetMaterial ().GetName ())) {
					return false;
				}
			}
			if (requirement.Key == "pommelMaterial") {
				List<string> materials = new List<string>();
				foreach(JSONNode material in requirement.Value.Childs) {
					materials.Add (material);
				}
				if (!materials.Contains (sword.GetPommel ().GetMaterial ().GetName ())) {
					return false;
				}
			}
		}
		return true;
	}
}
