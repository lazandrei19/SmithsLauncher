using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CraftingMenu : MonoBehaviour {

	Text BLMaterial;
	Text GDMaterial;
	Text HDMaterial;
	Text PMMaterial;
	Text LengthText;
	InputField SwordName;
	Text MaterialsValue;
	Text SwordPrice;
	Button BuildButton;
	Slider LengthSlider;
	GameObject SwordStatsContainer;
	Image DurabilityStar;
	Image RangeStar;
	Image SpeedStar;
	Image BalanceStar;
	Image DamageStar;
	Sprite EmptyStar;
	Sprite HalfStar;
	Sprite FullStar;

	int BLSelection = 0;
	int GDSelection = 0;
	int HDSelection = 0;
	int PMSelection = 0;

	List<string> craftingMaterials;

	int length = 80;

	void Start () {
		craftingMaterials = Materials.items.Where (delegate(ItemMaterial material) {
			return InventorySystem.controller.Exists (material);
		}).ToList ().Select (delegate(ItemMaterial material) {
			return material.GetName ();
		}).ToList ();
		BLMaterial = GameObject.Find ("MaterialBL").GetComponent<Text> ();
		GDMaterial = GameObject.Find ("MaterialGD").GetComponent<Text> ();
		HDMaterial = GameObject.Find ("MaterialHD").GetComponent<Text> ();
		PMMaterial = GameObject.Find ("MaterialPM").GetComponent<Text> ();
		LengthText = GameObject.Find ("Length").GetComponent<Text> ();
		MaterialsValue = GameObject.Find ("Value").GetComponent<Text> ();
		SwordPrice = GameObject.Find ("Estimate").GetComponent<Text> ();
		BuildButton = GameObject.Find ("BuildButton").GetComponent<Button> ();
		LengthSlider = GameObject.Find ("LengthSlider").GetComponent<Slider> ();
		SwordName = GameObject.Find ("SwordNameSelection").GetComponent <InputField> (); 
		SwordStatsContainer = GameObject.Find ("SwordStats");
		DurabilityStar = GameObject.Find ("DurabilityStar").GetComponent<Image> ();
		RangeStar = GameObject.Find ("RangeStar").GetComponent<Image> ();
		SpeedStar = GameObject.Find ("SpeedStar").GetComponent<Image> ();
		BalanceStar = GameObject.Find ("BalanceStar").GetComponent<Image> ();
		DamageStar = GameObject.Find ("DamageStar").GetComponent<Image> ();
		FullStar = Resources.Load <Sprite> ("CraftingMenu/Sprites/Stars/StarFill");
		HalfStar = Resources.Load <Sprite> ("CraftingMenu/Sprites/Stars/StarHalfFill");
		EmptyStar = Resources.Load <Sprite> ("CraftingMenu/Sprites/Stars/StarEmpty");
		LengthSlider.onValueChanged.AddListener (delegate {
			length = (int) LengthSlider.value;
			LengthText.text = "LENGTH: " + length;
			UpdateMaterialRequirements ();
		});
		BLMaterial.text = craftingMaterials [BLSelection].ToUpper ();
		GDMaterial.text = craftingMaterials [GDSelection].ToUpper ();
		HDMaterial.text = craftingMaterials [HDSelection].ToUpper ();
		PMMaterial.text = craftingMaterials [PMSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void GoBack () {
		SceneManager.LoadScene ("MainMenu");
	}

	public void BLCM () {
		BLSelection = Prev (BLSelection, craftingMaterials.Count);
		BLMaterial.text = craftingMaterials [BLSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void BLCP () {
		BLSelection = Next (BLSelection, craftingMaterials.Count);
		BLMaterial.text = craftingMaterials [BLSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void GDCM () {
		GDSelection = Prev (GDSelection, craftingMaterials.Count);
		GDMaterial.text = craftingMaterials [GDSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void GDCP () {
		GDSelection = Next (GDSelection, craftingMaterials.Count);
		GDMaterial.text = craftingMaterials [GDSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void HDCM () {
		HDSelection = Prev (HDSelection, craftingMaterials.Count);
		HDMaterial.text = craftingMaterials [HDSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void HDCP () {
		HDSelection = Next (HDSelection, craftingMaterials.Count);
		HDMaterial.text = craftingMaterials [HDSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void PMCM () {
		PMSelection = Prev (PMSelection, craftingMaterials.Count);
		PMMaterial.text = craftingMaterials [PMSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void PMCP () {
		PMSelection = Next (PMSelection, craftingMaterials.Count);
		PMMaterial.text = craftingMaterials [PMSelection].ToUpper ();
		UpdateMaterialRequirements ();
	}

	public void SCP () {
		if (++length < 150) {
			LengthSlider.value = (float)length;
		} else {
			length = 150;
		}
		LengthText.text = "LENGTH: " + length;
		UpdateMaterialRequirements ();
	}

	public void SCM () {
		if (--length > 80) {
			LengthSlider.value = (float)length;
		} else {
			length = 80;
		}
		LengthText.text = "LENGTH: " + length;
		UpdateMaterialRequirements ();
	}

	int Next (int current, int max) {
		return (++current == max) ? 0 : current;
	}

	int Prev (int current, int max) {
		return (current == 0)? --max : --current;
	}

	void UpdateMaterialRequirements () {
		Dictionary<IItem, int> requirements = new Dictionary<IItem, int> ();
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [BLSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [BLSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[BLSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [HDSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [HDSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[HDSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [GDSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [GDSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[GDSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [PMSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [PMSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[PMSelection]), 1);
		BuildButton.interactable = InventorySystem.controller.Exists (requirements);
		MaterialsValue.text =
			(Materials.prices[Materials.GetMaterial (craftingMaterials[BLSelection])] +
			Materials.prices[Materials.GetMaterial (craftingMaterials[GDSelection])] +
			Materials.prices[Materials.GetMaterial (craftingMaterials[HDSelection])] +
			Materials.prices[Materials.GetMaterial (craftingMaterials[PMSelection])]).ToString ();
		ItemSword s = new ItemSword (
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[BLSelection]), SwordComponent.Blade, new Dictionary<string, float> {{"length", length}}),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[GDSelection]), SwordComponent.Guard, new Dictionary<string, float> {{"balance", 0.00001f}}),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[HDSelection]), SwordComponent.Handle, new Dictionary<string, float> ()),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[PMSelection]), SwordComponent.Pommel, new Dictionary<string, float> ())
		);
		SwordPrice.text = Mathf.RoundToInt(s.GetBasePrice ()) + " - " + Mathf.RoundToInt(s.GetBasePrice () + s.GetBasePrice () * .25f);
		int dmg = Mathf.RoundToInt(30 * (s.GetDamage () / 160f));
		int rng = Mathf.RoundToInt(20 * ((s.GetRange () - 80) / 80f));
		float spd = Mathf.RoundToInt(20 * (s.GetDisplaySpeed () / 25f));
		float blc = Mathf.RoundToInt(10 * (2 - Mathf.Abs(1 - s.GetBalance ())));
		int dwr = Mathf.RoundToInt(24 * (s.GetDurability () / 36f));
		//
		Debug.Log(s.GetBalance ());
		Debug.Log (s.GetDurability ());
		Debug.Log ("--------------------------------------------------------------------------------------------");
		//
		DestroyAllWithTag ("DamageStar");
		for (int i = 0; i < 15; i++) {
			Image dmgStar = Instantiate (DamageStar) as Image;
			if (dmg >= 2) {
				dmgStar.sprite = FullStar;
			} else if (dmg == 1) {
				dmgStar.sprite = HalfStar;
			} else {
				dmgStar.sprite = EmptyStar;
			}
			dmg -= 2;
			Vector3 originalPos = dmgStar.transform.localPosition;
			dmgStar.transform.SetParent (SwordStatsContainer.transform, true);
			dmgStar.transform.localPosition = originalPos;
			dmgStar.transform.localScale = new Vector3 (1, 1, 1);
			dmgStar.transform.localPosition = new Vector3 (dmgStar.transform.localPosition.x + 32 * i, dmgStar.transform.localPosition.y, dmgStar.transform.localPosition.z);
		}
		DestroyAllWithTag ("RangeStar");
		for (int i = 0; i < 10; i++) {
			Image rngStar = Instantiate (RangeStar) as Image;
			if (rng >= 2) {
				rngStar.sprite = FullStar;
			} else if (rng == 1) {
				rngStar.sprite = HalfStar;
			} else {
				rngStar.sprite = EmptyStar;
			}
			rng -= 2;
			Vector3 originalPos = rngStar.transform.localPosition;
			rngStar.transform.SetParent (SwordStatsContainer.transform, true);
			rngStar.transform.localPosition = originalPos;
			rngStar.transform.localScale = new Vector3 (1, 1, 1);
			rngStar.transform.localPosition = new Vector3 (rngStar.transform.localPosition.x + 32 * i, rngStar.transform.localPosition.y, rngStar.transform.localPosition.z);
		}
		DestroyAllWithTag ("SpeedStar");
		for (int i = 0; i < 10; i++) {
			Image spdStar = Instantiate (SpeedStar) as Image;
			if (spd >= 2) {
				spdStar.sprite = FullStar;
			} else if (spd == 1) {
				spdStar.sprite = HalfStar;
			} else {
				spdStar.sprite = EmptyStar;
			}
			spd -= 2;
			Vector3 originalPos = spdStar.transform.localPosition;
			spdStar.transform.SetParent (SwordStatsContainer.transform, true);
			spdStar.transform.localPosition = originalPos;
			spdStar.transform.localScale = new Vector3 (1, 1, 1);
			spdStar.transform.localPosition = new Vector3 (spdStar.transform.localPosition.x + 32 * i, spdStar.transform.localPosition.y, spdStar.transform.localPosition.z);
		}
		DestroyAllWithTag ("BalanceStar");
		for (int i = 0; i < 10; i++) {
			Image blcStar = Instantiate (BalanceStar) as Image;
			if (blc >= 2) {
				blcStar.sprite = FullStar;
			} else if (blc == 1) {
				blcStar.sprite = HalfStar;
			} else {
				blcStar.sprite = EmptyStar;
			}
			blc -= 2;
			Vector3 originalPos = blcStar.transform.localPosition;
			blcStar.transform.SetParent (SwordStatsContainer.transform, true);
			blcStar.transform.localPosition = originalPos;
			blcStar.transform.localScale = new Vector3 (1, 1, 1);
			blcStar.transform.localPosition = new Vector3 (blcStar.transform.localPosition.x + 32 * i, blcStar.transform.localPosition.y, blcStar.transform.localPosition.z);
		}
		DestroyAllWithTag ("DurabilityStar");
		for (int i = 0; i < 12; i++) {
			Image dwrStar = Instantiate (DurabilityStar) as Image;
			if (dwr >= 2) {
				dwrStar.sprite = FullStar;
			} else if (dwr == 1) {
				dwrStar.sprite = HalfStar;
			} else {
				dwrStar.sprite = EmptyStar;
			}
			dwr -= 2;
			Vector3 originalPos = dwrStar.transform.localPosition;
			dwrStar.transform.SetParent (SwordStatsContainer.transform, true);
			dwrStar.transform.localPosition = originalPos;
			dwrStar.transform.localScale = new Vector3 (1, 1, 1);
			dwrStar.transform.localPosition = new Vector3 (dwrStar.transform.localPosition.x + 32 * i, dwrStar.transform.localPosition.y, dwrStar.transform.localPosition.z);
		}
	}

	void DestroyAllWithTag (string Tag) {
		GameObject[] ComponentsWithTag = GameObject.FindGameObjectsWithTag(Tag);
		for (int i = 1; i < ComponentsWithTag.Length; i++) {
			Destroy (ComponentsWithTag [i]);
		}
	}

	public void BuildSword () {
		Dictionary<IItem, int> requirements = new Dictionary<IItem, int> ();
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [BLSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [BLSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[BLSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [HDSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [HDSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[HDSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [GDSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [GDSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[GDSelection]), 1);
		if (requirements.ContainsKey (Materials.GetMaterial(craftingMaterials [PMSelection])))
			requirements [Materials.GetMaterial(craftingMaterials [PMSelection])] += 1;
		else
			requirements.Add (Materials.GetMaterial(craftingMaterials[PMSelection]), 1);
		ItemSword s = new ItemSword (
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[BLSelection]), SwordComponent.Blade, new Dictionary<string, float> () {{"length", length}}),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[GDSelection]), SwordComponent.Guard, new Dictionary<string, float> () {{"balance", 0.00001f}}),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[HDSelection]), SwordComponent.Handle, new Dictionary<string, float> ()),
			new ItemPartSword (Materials.GetMaterial (craftingMaterials[PMSelection]), SwordComponent.Pommel, new Dictionary<string, float> ())
		);
		GameController.control.SetItem ("furnace/blade", new ItemPartSword (Materials.GetMaterial (craftingMaterials[BLSelection]), SwordComponent.Blade, new Dictionary<string, float> () {{"length", length}}));
		GameController.control.SetItem ("furnace/guard", new ItemPartSword (Materials.GetMaterial (craftingMaterials[GDSelection]), SwordComponent.Guard, new Dictionary<string, float> () {{"balance", 0.00001f}}));
		GameController.control.SetItem ("furnace/handle", new ItemPartSword (Materials.GetMaterial (craftingMaterials[HDSelection]), SwordComponent.Handle, new Dictionary<string, float> ()));
		GameController.control.SetItem ("furnace/pommel", new ItemPartSword (Materials.GetMaterial (craftingMaterials[PMSelection]), SwordComponent.Pommel, new Dictionary<string, float> ()));
		GameController.control.SetItem ("furnace/sword", s);

		GameController.control.SetInt ("furnace/dmg", s.GetDamage ());
		GameController.control.SetInt ("furnace/rng", s.GetRange ());
		GameController.control.SetInt ("furnace/dwr", s.GetDurability ());

		GameController.control.SetString ("sword/name", SwordName.text);
		InventorySystem.controller.RemoveItem (requirements);
		SceneManager.LoadScene ("Furnace");
	}
}
