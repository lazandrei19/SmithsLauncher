using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopMenuScript : MonoBehaviour {

	int SlotsCount = 6;

	int currentSlot = 1;

	Text TotalCost;

	Button BuyButton;
	Button SellButton;

	Sprite SlotSelected;
	Sprite SlotDeselected;

	Text MaterialName;
	Image MaterialSlotMaterial;
	Text MaterialDescription;

	GameObject Canvas;
	Image QualityStar;
	Text WeightText;
	Image SharpnessStar;
	Image DifficultyStar;

	Text InInventory;
	Text Price;
	InputField Quantity;

	Sprite EmptyStar;
	Sprite HalfStar;
	Sprite FullStar;

	Dictionary<IItem, int> ShoppingCart = new Dictionary<IItem, int> ();

	void Start () {
		InitializeVariables ();
		InitializeSidebar ();
		ClickSlot (1);
	}

	void InitializeSidebar () {
		for (int i = 1; i <= SlotsCount; i++) {
			GameObject.Find ("Slot" + i + "Material").GetComponent <Image> ().sprite = Materials.items [i].GetTexture ();
		}
	}

	void InitializeVariables () {
		TotalCost = GameObject.Find ("TotalCost").GetComponent <Text> ();

		BuyButton = GameObject.Find ("BuyButton").GetComponent <Button> ();
		SellButton = GameObject.Find ("SellButton").GetComponent <Button> ();

		SlotSelected = Resources.Load <Sprite> ("ShopMenu/Sprites/SlotSelected");
		SlotDeselected = Resources.Load <Sprite> ("ShopMenu/Sprites/SlotDeselected");

		MaterialName = GameObject.Find ("MaterialName").GetComponent <Text> ();
		MaterialSlotMaterial = GameObject.Find ("MaterialSlotMaterial").GetComponent <Image> ();
		MaterialDescription = GameObject.Find ("MaterialDescription").GetComponent <Text> ();

		Canvas = GameObject.Find ("Canvas");
		QualityStar = GameObject.Find ("QualityStar").GetComponent <Image> ();
		WeightText = GameObject.Find ("Weight").GetComponent <Text> ();
		SharpnessStar = GameObject.Find ("SharpnessStar").GetComponent <Image> ();
		DifficultyStar = GameObject.Find ("DifficultyStar").GetComponent <Image> ();

		InInventory = GameObject.Find ("In Inventory").GetComponent <Text> ();
		Price = GameObject.Find ("Price").GetComponent <Text> ();
		Quantity = GameObject.Find ("QuantityInput").GetComponent <InputField> ();

		FullStar = Resources.Load <Sprite> ("ShopMenu/Sprites/StarFilled");
		HalfStar = Resources.Load <Sprite> ("ShopMenu/Sprites/StarHalfFilled");
		EmptyStar = Resources.Load <Sprite> ("ShopMenu/Sprites/StarEmpty");

		Quantity.text = "000";

		ShoppingCart.Clear ();
		for (int i = 1; i <= SlotsCount; i++) {
			ShoppingCart.Add (Materials.items[i], 0);
		}
	}

	public void ClickSlot (int slot) {
		currentSlot = slot;
		SelectSlot (slot);
		ShowDetails (Materials.items [slot]);
		Quantity.text = LeftPad (ShoppingCart[Materials.items [slot]].ToString (), 3);
	}

	void UpdateShoppingCart () {
		ShoppingCart [Materials.items [currentSlot]] = Int32.Parse (Quantity.text);
		CheckBalance ();
		CheckInventory ();
	}

	void CheckInventory () {
		foreach (KeyValuePair<IItem, int> item in ShoppingCart) {
			if (!InventorySystem.controller.Exists (item.Key, item.Value)) {
				SellButton.interactable = false;
				return;
			}
		}
		SellButton.interactable = true;
	}

	void CheckBalance () {
		int price = 0;
		foreach (KeyValuePair<IItem, int> item in ShoppingCart) {
			price += Materials.prices [(ItemMaterial) item.Key] * item.Value;
		}
		if (price > GameController.control.GetInt ("currency")) {
			BuyButton.interactable = false;
		} else {
			BuyButton.interactable = true;
		}
		TotalCost.text = "Total Value: " + price;
	}

	public void PlusQuantity () {
		try {
			int val = Int32.Parse(Quantity.text);
			if (++val < 1000)
				Quantity.text = LeftPad (val.ToString (), 3);
			else
				Quantity.text = "999";
		} catch {
			Quantity.text = "000";
		}
		UpdateShoppingCart ();
	}

	public void Buy () {
		foreach (KeyValuePair<IItem, int> item in ShoppingCart) {
			GameController.control.SetInt("currency", GameController.control.GetInt ("currency") - Materials.prices [(ItemMaterial) item.Key] * item.Value);
			InventorySystem.controller.AddItem (item.Key, item.Value);
			QuestSystem.q.CheckRequirements (new KeyValuePair<ItemMaterial, int> ((ItemMaterial) item.Key, item.Value));
		}
		UpdateShoppingCart ();
		SetMaterialRighSidebarInfo (Materials.items [currentSlot]);
	}

	public void Sell () {
		foreach (KeyValuePair<IItem, int> item in ShoppingCart) {
			GameController.control.SetInt("currency", GameController.control.GetInt ("currency") + Materials.prices [(ItemMaterial) item.Key] * item.Value);
			InventorySystem.controller.RemoveItem (item.Key, item.Value);
		}
		UpdateShoppingCart ();
		SetMaterialRighSidebarInfo (Materials.items [currentSlot]);
	}

	public void Back () {
		SceneManager.LoadScene ("MainMenu");
	}

	public void MinusQuantity () {
		try {
			int val = Int32.Parse(Quantity.text);
			if (--val >= 0)
				Quantity.text = LeftPad (val.ToString (), 3);
			else
				Quantity.text = "000";
		} catch {
			Quantity.text = "000";
		}
		UpdateShoppingCart ();
	}

	public void ValueChanged () {
		try {
			int val = Int32.Parse(Quantity.text);
			if (val < 1000)
				Quantity.text = LeftPad (val.ToString (), 3);
			else
				Quantity.text = "999";
		} catch {
			Quantity.text = "000";
		}
		Quantity.caretPosition++;
		UpdateShoppingCart ();
	}

	void ShowDetails (ItemMaterial m) {
		SetMaterialTextInfo (m);
		SetMaterialPropInfo (m);
		SetMaterialRighSidebarInfo (m);
	}

	void DestroyAllWithTag (string Tag) {
		GameObject[] ComponentsWithTag = GameObject.FindGameObjectsWithTag(Tag);
		for (int i = 1; i < ComponentsWithTag.Length; i++) {
			Destroy (ComponentsWithTag [i]);
		}
	}

	void SetMaterialPropInfo (ItemMaterial m) {
		SetQualityStars (m);
		WeightText.text = "Weight:\n" + m.GetWeight () + " kg";
		SetSharpnessStars (m);
		SetDifficultyStars (m);
	}

	void SetQualityStars (ItemMaterial m) {
		DestroyAllWithTag ("QualityStar");
		int qlt = Mathf.RoundToInt(20 * (m.GetQuality () / 6f));
		for (int i = 0; i < 10; i++) {
			Image qltStar = Instantiate (QualityStar) as Image;
			if (qlt >= 2) {
				qltStar.sprite = FullStar;
			} else if (qlt == 1) {
				qltStar.sprite = HalfStar;
			} else {
				qltStar.sprite = EmptyStar;
			}
			qlt -= 2;
			Vector3 originalPos = qltStar.transform.localPosition;
			qltStar.transform.SetParent (Canvas.transform, true);
			qltStar.transform.localPosition = originalPos;
			qltStar.transform.localScale = new Vector3 (1, 1, 1);
			qltStar.transform.localPosition = new Vector3 (qltStar.transform.localPosition.x + 70 * i, qltStar.transform.localPosition.y, qltStar.transform.localPosition.z);
		}
	}

	void SetSharpnessStars (ItemMaterial m) {
		DestroyAllWithTag ("SharpnessStar");
		int spn = Mathf.RoundToInt(20 * (m.GetSharpness () / 1.75f));
		for (int i = 0; i < 10; i++) {
			Image spnStar = Instantiate (SharpnessStar) as Image;
			if (spn >= 2) {
				spnStar.sprite = FullStar;
			} else if (spn == 1) {
				spnStar.sprite = HalfStar;
			} else {
				spnStar.sprite = EmptyStar;
			}
			spn -= 2;
			Vector3 originalPos = spnStar.transform.localPosition;
			spnStar.transform.SetParent (Canvas.transform, true);
			spnStar.transform.localPosition = originalPos;
			spnStar.transform.localScale = new Vector3 (1, 1, 1);
			spnStar.transform.localPosition = new Vector3 (spnStar.transform.localPosition.x + 70 * i, spnStar.transform.localPosition.y, spnStar.transform.localPosition.z);
		}
	}

	void SetDifficultyStars (ItemMaterial m) {
		DestroyAllWithTag ("DifficultyStar");
		int dif = Mathf.RoundToInt(20 * (m.GetDifficulty () / 2f));
		for (int i = 0; i < 10; i++) {
			Image difStar = Instantiate (DifficultyStar) as Image;
			if (dif >= 2) {
				difStar.sprite = FullStar;
			} else if (dif == 1) {
				difStar.sprite = HalfStar;
			} else {
				difStar.sprite = EmptyStar;
			}
			dif -= 2;
			Vector3 originalPos = difStar.transform.localPosition;
			difStar.transform.SetParent (Canvas.transform, true);
			difStar.transform.localPosition = originalPos;
			difStar.transform.localScale = new Vector3 (1, 1, 1);
			difStar.transform.localPosition = new Vector3 (difStar.transform.localPosition.x + 70 * i, difStar.transform.localPosition.y, difStar.transform.localPosition.z);
		}
	}

	void SetMaterialRighSidebarInfo (ItemMaterial m) {
		InInventory.text = "In Inventory:\n" + InventorySystem.controller.Count (m) + " Piece" + ((InventorySystem.controller.Count (m) == 1)? "" : "s");
		Price.text = "Price:\n" + Materials.prices[m] + " Coins";
	}

	void SetMaterialTextInfo (ItemMaterial m) {
		MaterialName.text = m.GetName ().ToUpper ();
		MaterialSlotMaterial.sprite = m.GetTexture ();
		MaterialDescription.text = m.GetDescription ();
	}

	void SelectSlot (int slot) {
		DeselectAllSlots ();
		GameObject.Find ("Slot" + slot).GetComponent <Image> ().sprite = SlotSelected;
	}

	void DeselectAllSlots () {
		for (int i = 1; i <= SlotsCount; i++) {
			GameObject.Find ("Slot" + i).GetComponent <Image> ().sprite = SlotDeselected;
		}
	}

	string LeftPad (string t, int s) {
		return t.PadLeft (s, '0');
	}
}
