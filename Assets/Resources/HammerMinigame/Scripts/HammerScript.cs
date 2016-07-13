using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class HammerScript : MonoBehaviour {
	struct TileData {
		public int Value;
		public int Length;

		public TileData (int Value, int Length) {
			this.Value = Value;
			this.Length = Length;
		}
	}

	List <int> tileValues = new List <int> () {-2, -1, 1, 2, 1, -1, -2};
	float speed = 450f;
	public int frames = 8;
	public AudioClip goodSound;
	public AudioClip badSound;
	int moveDirection = 1;
	int score = 0;
	bool triggered = false;
	int state = 0;
	Text time;
	Text score1;
	int i=0;
	float scale1;
	float diff;
	int hits=0;
	int randzone;
	int randpos;
	bool started = false;
	GameObject Canvas;
	Image HammerTile;
	Dictionary <Image, TileData> tiles = new Dictionary<Image, TileData> () {};
	Sprite p2;
	Sprite p1;
	Sprite m1;
	Sprite m2;
	float tm;
	Image Hm;
	AudioSource source;

	void DestroyAllWithTag (string Tag) {
		GameObject[] ComponentsWithTag = GameObject.FindGameObjectsWithTag(Tag);
		for (int i = 1; i < ComponentsWithTag.Length; i++) {
			Destroy (ComponentsWithTag [i]);
		}
	}

	Sprite GetSpriteFromValue (int Value) {
		if (Value == -2) {
			return m2;
		} else if (Value == -1) {
			return m1;
		} else if (Value == 1) {
			return p1;
		} else if (Value == 2) {
			return p2;
		} else {
			return new Sprite ();
		}
	}

	// Use this for initialization
	void Start () {
		Canvas = GameObject.Find ("Canvas");
		gameObject.transform.localPosition = new Vector3 (-705f, 50f, 0f);
		HammerTile = GameObject.Find ("HammerTile").GetComponent <Image> ();
		p2 = GameObject.Find ("+2").GetComponent <Image> ().sprite;
		p1 = GameObject.Find ("+1").GetComponent <Image> ().sprite;
		m1 = GameObject.Find ("-1").GetComponent <Image> ().sprite;
		m2 = GameObject.Find ("-2").GetComponent <Image> ().sprite;
		time = GameObject.Find ("Time Left").GetComponent<Text>();
		score1 = GameObject.Find ("Score").GetComponent<Text>();
		try {
			diff = GameController.control.GetItem ("hammer/sword").GetDifficulty ();
		} catch {
			diff = 2f;
		}
		GameController.control.SetFloat ("hammer/time", 20f * diff);
		source = GetComponent<AudioSource> ();
		for (int i = 0; i < 7; i++) {
			Image currentTile = Instantiate (HammerTile) as Image;
			int j = Mathf.RoundToInt (Random.Range (0f, (tileValues.Count - 1)));
			currentTile.sprite = GetSpriteFromValue (tileValues [j]);
			tiles.Add (currentTile, new TileData (tileValues [j], 237));
			tileValues.RemoveAt (j);
			Vector3 originalPos = currentTile.transform.localPosition;
			currentTile.transform.SetParent (Canvas.transform, true);
			currentTile.transform.localPosition = originalPos;
			currentTile.transform.localScale = new Vector3 (1, 1, 1);
			currentTile.transform.localPosition = new Vector3 (currentTile.transform.localPosition.x + 237 * i, -147, currentTile.transform.localPosition.z);
		}
	}

	int ScoreToAdd (float x) {
		x += 823.5f;
		x += 100.5f * moveDirection;
		foreach (TileData l in tiles.Values) {
			x -= l.Length;
			if (x <= 0) {
				return l.Value;
			}
		}
		return 0;
	}
	void RotateHammer () {
		if (triggered) {
			if (Mathf.Abs (gameObject.transform.localPosition.x) < 700)
				triggered = false;
		} else {
			if (Mathf.Abs (gameObject.transform.localPosition.x) > 705) {
				moveDirection *= -1;
				gameObject.transform.localRotation = Quaternion.Euler (0, (transform.localRotation.eulerAngles.y + 180) % 360, 0);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Down ();
		}
		if (started) {
			if (GameController.control.GetFloat ("hammer/time") > 0) {
				if (diff < 1) {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x + moveDirection * Time.deltaTime * speed * diff, gameObject.transform.position.y, gameObject.transform.position.z);
				} else if (diff >= 1 && diff < 1.5f) {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x + moveDirection * Time.deltaTime * speed * diff * Random.Range (1f, 5f), gameObject.transform.position.y, gameObject.transform.position.z);
				} else if (diff >= 1.5f) {
					gameObject.transform.position = new Vector3 (gameObject.transform.position.x + moveDirection * Time.deltaTime * speed * diff * Random.Range (1f, 10f), gameObject.transform.position.y, gameObject.transform.position.z);
				}
				RotateHammer ();

				if (state == 1) {
					transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y - 50, transform.localPosition.z);
					state++;
				}
				if (state > 1)
					state++;
				if (state == frames) {
					RotateBack ();
					transform.localPosition = new Vector3 (transform.localPosition.x, 50, transform.localPosition.z);
					state = 0;
				}
				GameController.control.SetFloat ("hammer/time", GameController.control.GetFloat ("hammer/time") - Time.deltaTime);
				tm = tm - Time.deltaTime;
			} else {
				BuildSword ();
			}
		
			score1.text = "Score: " + score;
			time.text = "Time left: " + Mathf.RoundToInt(GameController.control.GetFloat ("hammer/time")) ;
		}
	}

	void BuildSword () {
		ItemPartSword b = (ItemPartSword) GameController.control.GetItem ("hammer/blade");
		ItemPartSword g = (ItemPartSword) GameController.control.GetItem ("hammer/guard");
		ItemPartSword h = (ItemPartSword) GameController.control.GetItem ("hammer/handle");
		ItemPartSword p = (ItemPartSword) GameController.control.GetItem ("hammer/pommel");

		ItemSword s = new ItemSword (b, g, h, p);

		GameController.control.SetInt ("hammer/dmg", s.GetDamage ());
		GameController.control.SetInt ("hammer/rng", s.GetRange ());
		GameController.control.SetInt ("hammer/dwr", s.GetDurability ());

		Debug.Log (((ItemSword) GameController.control.GetItem("hammer/sword")).GetBasePrice ()); 

		b.GetMaterial ().SetQuality (b.GetMaterial ().GetQuality () + b.GetMaterial ().GetQuality () * (score / 1000f));

		ItemSword sword = new ItemSword (b, g, h, p);
		GameController.control.SetItem ("selling/sword", sword);
		SceneManager.LoadScene ("HammerStats");
	}

	void Shakeup(){
		GameObject.Find("+2").transform.localPosition = new Vector3 (0f,1.5f,0f);	 
	}
	void ShakeDown(){
		GameObject.Find ("+2").transform.localPosition = new Vector3 (0f, 0.5f, 0f);
	}

	public void Down (){
		if (started) {
			if (state == 0) {
				state = 1;
				i++;
				RotateLeft ();
				hits += 1;

				int scoreToAdd = ScoreToAdd (transform.localPosition.x);
				source.clip = (scoreToAdd > 0) ? goodSound : badSound;
				source.Play ();
				score += scoreToAdd;

				if (diff >= 1) {
					if (i % 2 == 1) {
						scale1 = Random.Range (0.5f, 1f);
						int j = 0;
						List <Image> buffer = new List<Image> (tiles.Keys);
						foreach (Image tile in buffer) {
							if (j % 2 == 0) {
								tile.transform.localScale = new Vector3 (scale1, 1f, 1f);
								tiles[tile] = new TileData(tiles[tile].Value, Mathf.RoundToInt(237 * scale1));
							} else {
								tile.transform.localScale = new Vector3 (2 - scale1, 1f, 1f);
								tiles[tile] = new TileData(tiles[tile].Value, Mathf.RoundToInt(237 * (2 - scale1)));
							}
							j++;
						}
					}
					if (i % 2 == 0) {
						scale1 = Random.Range (1f, 1.5f);
						int j = 0;
						List <Image> buffer = new List<Image> (tiles.Keys);
						foreach (Image tile in buffer) {
							if (j % 2 == 1) {
								tile.transform.localScale = new Vector3 (scale1, 1f, 1f);
								tiles[tile] = new TileData(tiles[tile].Value, Mathf.RoundToInt(237 * scale1));
							} else {
								tile.transform.localScale = new Vector3 (2 - scale1, 1f, 1f);
								tiles[tile] = new TileData(tiles[tile].Value, Mathf.RoundToInt(237 * (2 - scale1)));
							}
							j++;
						}
					}
				}
			}
		} else {
			started = true;
		}
	}

	void RotateLeft () {
		transform.localRotation = Quaternion.Euler (new Vector3 (0, gameObject.transform.localRotation.eulerAngles.y, -45));
	}
	void RotateBack (){
		transform.localRotation = Quaternion.Euler (new Vector3 (0, gameObject.transform.localRotation.eulerAngles.y, 0));
	}

}
