using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FurnaceStatsScript : MonoBehaviour {

	float dmgThings;
	float rngThings;
	float dwrThings;

	Image dmgThing;
	Image rngThing;
	Image dwrThing;

	Sprite None;
	Sprite Plus;
	Sprite Minus;

	GameObject Canvas;

	float t;

	public void Advance () {
		if (t > 0.75)
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Hammer");
	}

	void Update () {
		t += Time.deltaTime;
	}

	void Start () {
		ItemSword finalSword = (ItemSword) GameController.control.GetItem ("hammer/sword");
		dmgThings = ((float) finalSword.GetDamage () / GameController.control.GetInt ("furnace/dmg")) * 100 - 100;
		rngThings = ((float) finalSword.GetRange () / GameController.control.GetInt ("furnace/rng")) * 100 - 100;
		dwrThings = ((float) finalSword.GetDurability () / GameController.control.GetInt ("furnace/dwr")) * 100 - 100;

		dmgThing = GameObject.Find ("dmgThing").GetComponent <Image> ();
		rngThing = GameObject.Find ("rngThing").GetComponent <Image> ();
		dwrThing = GameObject.Find ("dwrThing").GetComponent <Image> ();

		None = GameObject.Find ("None").GetComponent <Image> ().sprite;
		Minus = GameObject.Find ("Minus").GetComponent <Image> ().sprite;
		Plus = GameObject.Find ("Plus").GetComponent <Image> ().sprite;

		Canvas = GameObject.Find ("Canvas"); ;

		if (dmgThings < 0) {
			dmgThing.sprite = Minus;
		} else if (dmgThings > 0) {
			dmgThing.sprite = Plus;
		}
		for (int i = 5; i < Mathf.Abs (dmgThings); i += 30) {
			Image a = Instantiate (dmgThing) as Image;
			Vector3 initialPos = a.transform.localPosition;
			a.transform.SetParent (Canvas.transform, true);
			a.transform.localScale = new Vector3 (1, 1, 1);
			a.transform.localPosition = initialPos;
			a.transform.localPosition = new Vector3 (a.transform.localPosition.x + 70 * (i / 30 + 1), 80, a.transform.localPosition.z);
		}

		if (rngThings < 0) {
			rngThing.sprite = Minus;
		} else if (rngThings > 0) {
			rngThing.sprite = Plus;
		}
		for (int i = 1; i < Mathf.Abs (rngThings); i++) {
			Image a = Instantiate (rngThing) as Image;
			Vector3 initialPos = a.transform.localPosition;
			a.transform.SetParent (Canvas.transform, true);
			a.transform.localScale = new Vector3 (1, 1, 1);
			a.transform.localPosition = initialPos;
			a.transform.localPosition = new Vector3 (a.transform.localPosition.x + 70 * i, -30, a.transform.localPosition.z);
			a.transform.Translate (new Vector3 (0, -307, 0));
		}

		if (dwrThings < 0) {
			dwrThing.sprite = Minus;
		} else if (dwrThings > 0) {
			dwrThing.sprite = Plus;
		}
		for (int i = 15; i < Mathf.Abs (dwrThings); i += 70) {
			Image a = Instantiate (dwrThing) as Image;
			Vector3 initialPos = a.transform.localPosition;
			a.transform.SetParent (Canvas.transform, true);
			a.transform.localScale = new Vector3 (1, 1, 1);
			a.transform.localPosition = initialPos;
			a.transform.localPosition = new Vector3 (a.transform.localPosition.x + 70 * (i / 70 + 1), -140, a.transform.localPosition.z);
			a.transform.Translate (new Vector3 (0, -307, 0));
		}
	}
}
