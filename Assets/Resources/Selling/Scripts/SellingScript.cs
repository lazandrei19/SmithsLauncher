using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SellingScript : MonoBehaviour {

	GameObject Zone;
	GameObject Dot;
	RectTransform Bar;
	Text ScoreText;
	Text TimeText;
	int Score = 0;
	float TimeLeft;
	float ZoneScale = 2;
	bool started = false;
	public float ScaleStep = .005f;
	public float MoveStep = 2f;
	float diff;

	public void ClickDot () {
		started = true;
	}

	void Start () {
		try {
			diff = GameController.control.GetItem ("selling/sword").GetDifficulty ();
		} catch {
			diff = 1;
		}
		TimeLeft = 15f; //15f * diff;
		Zone = GameObject.Find ("Zone");
		Dot = GameObject.Find ("Dot");
		ScoreText = GameObject.Find ("Score").GetComponent <Text> ();
		TimeText = GameObject.Find ("Time").GetComponent <Text> ();
		TimeText.text = "Time: " + Mathf.RoundToInt (TimeLeft);
		Bar = (RectTransform) GameObject.Find ("Bar").transform;
	}

	float ValueBetween (float value, float min, float max) {
		return Mathf.Min (Mathf.Max (min, value), max);
	}

	void MoveDot () {
		float mousePosX = ValueBetween (Input.mousePosition.x, Bar.position.x - Bar.rect.width / 2 - Dot.GetComponent<RectTransform>().rect.width, Bar.position.x + Bar.rect.width / 2 + Dot.GetComponent<RectTransform>().rect.width);
		Dot.transform.position = new Vector3 (mousePosX, Dot.transform.position.y, Dot.transform.position.z);
	}

	int DotInsideZone () {
		float ZoneWidth = ((RectTransform) Zone.transform).rect.width;
		if (Zone.transform.localPosition.x - (ZoneWidth / 2) < Dot.transform.localPosition.x && Zone.transform.localPosition.x + (ZoneWidth / 2) > Dot.transform.localPosition.x) {
			return 1;
		} else {
			return -1;
		}
	}

	void MoveZone () {
		float variation;
		if (diff < .75f) {
			variation = 1;
		} else if (diff < 1.5f) {
			variation = Random.Range (-1.2f, 2f);
		} else {
			variation = Random.Range (-1.75f, 4f);
		}
		Zone.transform.localPosition = new Vector3 (ValueBetween(Zone.transform.localPosition.x + MoveStep * DotInsideZone () * variation, -405f - (240f - 120f * Zone.transform.localScale.x) - 200 , 405f + (240f - 120f * Zone.transform.localScale.x) + 200), Zone.transform.localPosition.y, Zone.transform.localPosition.z);
	}

	void BuildSword () {
		int price = Mathf.RoundToInt(((ItemSword) GameController.control.GetItem("selling/sword")).GetBasePrice () + ((ItemSword) GameController.control.GetItem("selling/sword")).GetBasePrice () * (Score / 200f));
		GameController.control.SetInt ("price", price);
		GameController.control.SetInt ("currency", GameController.control.GetInt ("currency") + price);
		GameController.control.SetBool ("sold", true);
		//QuestSystem.q.CheckRequirements ((ItemSword) GameController.control.GetItem("selling/sword"));
		SceneManager.LoadScene("MainMenu");
	}

	void FixedUpdate () {
		if (started) {
			Debug.Log (TimeLeft);
			if (Mathf.RoundToInt (TimeLeft) >= 1) {
				MoveDot ();
				MoveZone ();
				ZoneScale -= ScaleStep;
				Zone.transform.localScale = new Vector3 (Mathf.Max(.6f, ZoneScale), 2f, 1f);
				TimeLeft -= Time.fixedDeltaTime;
				TimeText.text = "Time: " + Mathf.RoundToInt (TimeLeft);
				Score = Mathf.RoundToInt ((Zone.transform.localPosition.x / 570f) * 20f);
				ScoreText.text = "Score: " + Score;
			} else {
				BuildSword ();
			}
		}
	}
}
