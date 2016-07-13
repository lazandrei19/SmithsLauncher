
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Furnace : MonoBehaviour {

	public static Furnace furnControl;

	private float score = 0;
	private uint curTime;
	private uint specificTime = 15;
	private float furnTime = 0;

	private Text scoreTx;
	private Text curTimeTx;

	private GameObject you;
	private GameObject limit;	
	private GameObject max;	

	public float q1,p1,q2,p2,flame;
	public float r1,r2;
	private float difficulty;
	private bool start;

	void Start(){
		curTime = specificTime;
		scoreTx = GameObject.Find ("/Canvas/Score").GetComponent<Text>();
		curTimeTx = GameObject.Find ("/Canvas/CurrentTime").GetComponent<Text>();
		you = GameObject.Find ("/Canvas/You");
		limit = GameObject.Find ("/Canvas/Limit");
		max = GameObject.Find ("/Canvas/Max");

		start = false;
		try {
			difficulty = GameController.control.GetItem ("furnace/sword").GetDifficulty ();
		} catch {
			difficulty = 2.25f;
		}
		if (difficulty < 1) {
			limit.transform.localPosition = new Vector3 (0,  20, 0);
			limit.GetComponent<RectTransform> ().offsetMax = new Vector2(0 , 150);
			q1 = 10;
			p1 = 12;
			q2 = 4;
			p2 = 9;
			flame = 10;
		} else if (difficulty >= 1 && difficulty < 1.5) {
			limit.transform.localPosition = new Vector3 (0, 300, 0);
			q1 = 14;
			p1 = 17;
			q2 = 4;
			p2 = 9;
			flame = 23;
		} else if (difficulty >= 1.5) {
			limit.transform.localPosition = new Vector3 (0, 300, 0);
			q1 = 16;
			p1 = 20;
			q2 = 4;
			p2 = 9;
			flame = 50;
		}
		you.GetComponent<RectTransform> ().offsetMax = new Vector2 (0 , -200);
	}

	void Update () {
		scoreTx.text = "Score : " + (int)score;
		curTimeTx.text = "Time : " + curTime;
		if (Input.GetMouseButtonDown (0))
			start = true;
		if (curTime > 0 && start) {
			r1 = -Random.Range (q1, p1);
			r2 = Random.Range (q2, p2);

			if (you.GetComponent<RectTransform> ().offsetMax.y >= max.GetComponent<RectTransform> ().offsetMax.y)
				you.GetComponent<RectTransform> ().offsetMax = new Vector3(0 , max.GetComponent<RectTransform> ().offsetMax.y , 0);	

			if (you.transform.position.y < 70)
				r1 = r2 = 0;
			
			if (Input.GetMouseButton (0))
				you.GetComponent<RectTransform>().offsetMax += new Vector2 (0, flame);
			else
				you.GetComponent<RectTransform>().offsetMax += new Vector2 (0, r1 + r2);
			
			if (you.GetComponent<RectTransform>().offsetMax.y <=  limit.GetComponent<RectTransform> ().offsetMax.y && 
				you.GetComponent<RectTransform>().offsetMax.y >=  (limit.GetComponent<RectTransform> ().offsetMax.y - limit.GetComponent<RectTransform> ().rect.height))
				score += 0.05f;
			curTime = specificTime - (uint)furnTime; 
			furnTime += 0.02f;

		} else if (curTime <= 0) {
			ItemPartSword b = (ItemPartSword) GameController.control.GetItem ("furnace/blade");
			ItemPartSword g = (ItemPartSword) GameController.control.GetItem ("furnace/guard");
			ItemPartSword h = (ItemPartSword) GameController.control.GetItem ("furnace/handle");
			ItemPartSword p = (ItemPartSword) GameController.control.GetItem ("furnace/pommel");

			b.GetMaterial ().SetQuality (b.GetMaterial ().GetBaseQuality () + b.GetMaterial ().GetBaseQuality () * (score/400));
			g.GetMaterial ().SetQuality (g.GetMaterial ().GetBaseQuality () + g.GetMaterial ().GetBaseQuality () * (score/400));
			h.GetMaterial ().SetQuality (h.GetMaterial ().GetBaseQuality () + h.GetMaterial ().GetBaseQuality () * (score/400));
			p.GetMaterial ().SetQuality (p.GetMaterial ().GetBaseQuality () + p.GetMaterial ().GetBaseQuality () * (score/400));

			ItemSword sword = new ItemSword (b, g, h, p);
			GameController.control.SetItem ("hammer/blade", b);
			GameController.control.SetItem ("hammer/guard", g);
			GameController.control.SetItem ("hammer/handle", h);
			GameController.control.SetItem ("hammer/pommel", p);
			GameController.control.SetItem ("hammer/sword", sword);
			Debug.Log (sword.GetBasePrice ()); 
			SceneManager.LoadScene ("FurnaceStats");
		}
	}
}
