using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

	Text alertMessage;
	Text currency;
	Text questName;
	Text questDescription;
	Text questReward;

	public void craftSword () {
		if (InventorySystem.controller.GetItems ().Count > 0)
			SceneManager.LoadScene ("CraftingMenu");
		else{
			alertMessage.text = "Buy some materials from the shop first!";
			alertMessage.color = new Color(0.9f,0.7f,0.6f);
		}
	}

	public void shop () {
		SceneManager.LoadScene ("ShopMenu");
	}

	public void save () {
		GameController.control.Save ();
	}

	public void load () {
		GameController.control.Load ();
	}

	public void quit () {
		Application.Quit ();
	}

	void Start () {
		try {
			QuestSystem.q.CheckRequirements ((ItemSword) GameController.control.GetItem("selling/sword"));
		} catch {
		}
		alertMessage = GameObject.Find ("Message").GetComponent<Text> ();
		currency = GameObject.Find ("Currency").GetComponent<Text> ();
		questName = GameObject.Find ("QuestName").GetComponent<Text> ();
		questDescription = GameObject.Find ("QuestDescription").GetComponent<Text> ();
		questReward = GameObject.Find ("QuestReward").GetComponent<Text> ();
		if (GameController.control.GetBool ("sold")) {
			alertMessage.text = ((GameController.control.GetString ("sword/name") == "")? "Your sword" : GameController.control.GetString ("sword/name")) + " sold for " + GameController.control.GetInt ("price") + " coins";
			alertMessage.color = new Color(0.2f,0.8f,0.3f);
		}

		GameController.afterLoad += delegate() {
			currency.text = GameController.control.GetInt("currency") + " Coins";
			questName.text = QuestSystem.q.GetCurrentQuest () ["name"];
			questDescription.text = QuestSystem.q.GetCurrentQuest () ["description"];
			if (QuestSystem.q.GetCurrentQuest () ["reward"] ["type"].ToString () == "\"money\"") { 
				questReward.text = "Reward: " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"].AsInt + " Coins";
			} else {
				questReward.text = "Reward: " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"] ["count"] + " " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"] ["material"];
			}
		};

		currency.text = GameController.control.GetInt("currency") + " Coins";
		questName.text = QuestSystem.q.GetCurrentQuest () ["name"];
		questDescription.text = QuestSystem.q.GetCurrentQuest () ["description"];
		if (QuestSystem.q.GetCurrentQuest () ["reward"] ["type"].ToString () == "\"money\"") { 
			questReward.text = "Reward: " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"].AsInt + " Coins";
		} else {
			questReward.text = "Reward: " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"] ["count"] + " " + QuestSystem.q.GetCurrentQuest () ["reward"] ["value"] ["material"];
		}
	}
}
