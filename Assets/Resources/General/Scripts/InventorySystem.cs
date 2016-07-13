using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour {
	public static InventorySystem controller;
	Dictionary<IItem, int> inventory = new Dictionary<IItem, int>();

	void Awake () {
		if (controller == null) {
			DontDestroyOnLoad (gameObject);
			controller = this;
		} else if (controller != this) {
			Destroy (gameObject);
		}
	}

	void Start () {
		GameController.beforeSave += saveInventory;
		GameController.afterLoad += loadInventory;
		InventorySystem.controller.AddItem (Materials.GetMaterial ("Stone"), 10);
	}

	void saveInventory () {
		GameController.control.SetInventory (inventory);
	}

	void loadInventory () {
		inventory = new Dictionary<IItem, int> ();
		foreach (KeyValuePair<IItem, int> obj in GameController.control.GetInventory ()) {
			InventorySystem.controller.AddItem (Materials.GetMaterial (obj.Key.GetName ()), obj.Value);
		}
	}

	public void AddItem (IItem item, int size = 1) {
		if (inventory.ContainsKey (item))
			inventory [item] += size;
		else
			inventory.Add (item, size);
	}

	public void AddItem (Dictionary<IItem,int> itemStack) {
		foreach (KeyValuePair<IItem,int> item in itemStack) {
			if (inventory.ContainsKey (item.Key))
				inventory [item.Key] += item.Value;
			else
				inventory.Add (item.Key, item.Value);
		}
	}

	public bool RemoveItem(IItem item, int size = 1) {
		if (item != (IItem)Materials.GetMaterial ("Stone")) {
			if (inventory.ContainsKey (item)) {
				if (inventory [item] >= size) {
					inventory [item] -= size;
					if (inventory [item] == 0) {
						inventory.Remove (item);
					}
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		} else {
			return true;
		}
	}

	public bool RemoveItem(Dictionary<IItem,int> itemStack) {
		if (InventorySystem.controller.Exists (itemStack)) {
			foreach (KeyValuePair<IItem,int> item in itemStack) {
				InventorySystem.controller.RemoveItem (item.Key, item.Value);
			}
			return true;
		} else {
			return false;
		}
	}

	public int Count (IItem item) {
		if (inventory.ContainsKey (item)) {
			return inventory [item];
		} else {
			return 0;
		}
	}

	public bool Exists (IItem item, int size = 1) {
		if (size != 0) {
			if (item != (IItem)Materials.GetMaterial ("Stone")) {
				if (inventory.ContainsKey (item)) {
					if (inventory [item] >= size) {
						return true;
					} else {
						return false;
					}
				} else {
					return false;
				}
			} else {
				return true;
			}
		} else {
			return true;
		}
	}

	public bool Exists (Dictionary<IItem,int> itemStack) {
		foreach (KeyValuePair<IItem,int> item in itemStack) {
			if (!InventorySystem.controller.Exists (item.Key, item.Value)) {
				return false;
			}
		}
		return true;
	}

	public int Missing (IItem item, int size) {
		return size - this.Count (item);
	}

	public List<ItemMaterial> GetRawItems() {
		return this.GetItems (ItemType.Material).Select (delegate(IItem item) {
			return (ItemMaterial) item;
		}).ToList ();
	}

	public List<ItemMaterial> GetProcessedItems() {
		return this.GetItems (ItemType.Material).Where (delegate (IItem item) {
			ItemMaterial itemMaterial = (ItemMaterial) item;
			return itemMaterial.GetQuality () != itemMaterial.GetBaseQuality ();
		}).ToList ().Select (delegate(IItem item) {
			return (ItemMaterial) item;
		}).ToList ();
	}

	public List<IItem> GetItems () {
		return new List<IItem>(inventory.Keys);
	}

	public List<IItem> GetItems (ItemType type) {
		return new List<IItem>(inventory.Keys).Where(delegate(IItem item) {
			return item.GetType () == type;
		}).ToList ();
	}
}

public interface IItem {
	string GetName ();
	ItemType GetType ();
	Sprite GetTexture();
	float GetDifficulty ();
}

[Serializable]
public enum ItemType {
	Fuel,
	Material,
	PartSword,
	Sword
}

[Serializable]
public enum SwordComponent {
	Blade,
	Guard,
	Handle,
	Pommel
}

[Serializable]
public class ItemFuel : IItem{
	float burnTime;
	float burnSpeed;
	float minUp;
	float maxUp;
	string name;
	float difficulty;
	Sprite texture;

	public ItemFuel (float burnTime, float burnSpeed, string name, float minUp, float maxUp, float difficulty, string textureName = "") {
		this.burnTime = Mathf.Round (burnTime * 100f) / 100f;
		this.burnSpeed = Mathf.Round (burnSpeed * 100f) / 100f;
		this.name = name;
		this.minUp = Mathf.Round (minUp * 100f) / 100f;
		this.maxUp = Mathf.Round (maxUp * 100f) / 100f;
		this.difficulty = difficulty;
		this.texture = Resources.Load <Sprite> (textureName);
	}

	public float Up () {
		return UnityEngine.Random.Range (this.minUp, this.maxUp);
	}

	string IItem.GetName () {
		return this.name;
	}

	public string GetName () {
		return this.name;
	}

	public float GetBurnTime () {
		return this.burnTime;
	}

	public float GetBurnSpeed () {
		return this.burnSpeed;
	}

	ItemType IItem.GetType() {
		return ItemType.Fuel;
	}

	Sprite IItem.GetTexture() {
		return this.texture;
	}

	public Sprite GetTexture() {
		return this.texture;
	}

	float IItem.GetDifficulty () {
		return this.difficulty;
	}

	public float GetDifficulty () {
		return this.difficulty;
	}
}

[Serializable]
public class ItemMaterial : IItem{
	List<SwordComponent> forms;
	float baseQuality;
	float quality;
	float weight;
	float sharpness;
	float difficulty;
	public string name;
	string textureName;
	string description;

	public ItemMaterial (List<SwordComponent> forms, float baseQuality, string name, float weight, float sharpness, float difficulty, string description = "", string textureName = "") {
		this.forms = forms;
		this.baseQuality = Mathf.Round (baseQuality * 100f) / 100f;
		this.quality = this.baseQuality;
		this.name = name;
		this.weight = Mathf.Round (weight * 100f) / 100f;
		this.sharpness = Mathf.Round (sharpness * 100f) / 100f;
		this.difficulty = Mathf.Round (difficulty * 100f) / 100f;
		this.textureName = textureName;
		this.description = description;
	}

	public ItemMaterial (ItemMaterial baseItem) {
		this.forms = baseItem.forms;
		this.baseQuality = baseItem.baseQuality;
		this.quality = this.baseQuality;
		this.name = baseItem.name;
		this.weight = baseItem.weight;
		this.sharpness = baseItem.sharpness;
		this.difficulty = baseItem.difficulty;
		this.textureName = baseItem.textureName;
	}

	public void SetQuality(float quality) {
		this.quality = Mathf.Round (quality * 100f) / 100f;
	}

	string IItem.GetName () {
		return this.name;
	}

	public string GetName () {
		return this.name;
	}

	public float GetQuality () {
		return this.quality;
	}

	public float GetBaseQuality () {
		return this.baseQuality;
	}

	public float GetWeight () {
		return this.weight;
	}

	public float GetSharpness () {
		return this.sharpness;
	}

	public float GetDifficulty () {
		return this.difficulty;
	}

	public List<SwordComponent> GetForms () {
		return this.forms;
	}

	ItemType IItem.GetType() {
		return ItemType.Material;
	}

	Sprite IItem.GetTexture() {
		return Resources.Load <Sprite> (this.textureName);
	}

	public Sprite GetTexture() {
		return Resources.Load <Sprite> (this.textureName);
	}

	float IItem.GetDifficulty () {
		return this.difficulty;
	}

	public string GetDescription () {
		return this.description;
	}
}

[Serializable]
public class ItemPartSword : IItem{
	ItemMaterial material;
	SwordComponent part;
	Dictionary<string, float> extra = new Dictionary<string, float>();
	Sprite texture;

	public ItemPartSword(ItemMaterial material, SwordComponent part, Dictionary<string, float> extra, string textureName = "") {
		if (material.GetForms ().Contains (part)) {
			this.material = new ItemMaterial(material);
			this.part = part;
			this.texture = Resources.Load <Sprite> (textureName);
			this.extra = extra;
		} else {
			throw new System.ArgumentException("You can't make this part from the selected material", "material");
		}
	}

	public SwordComponent GetComponent () {
		return this.part;
	}

	public ItemMaterial GetMaterial () {
		return this.material;
	}

	string IItem.GetName () {
		return this.material.name + " Blade";
	}

	public string GetName () {
		return this.material.name + " Blade";
	}

	public float GetExtra(string key) {
		return Mathf.Round (this.extra[key] * 100f) / 100f;
	}

	ItemType IItem.GetType() {
		return ItemType.PartSword;
	}

	Sprite IItem.GetTexture() {
		return this.texture;
	}

	public Sprite GetTexture() {
		return this.texture;
	}

	float IItem.GetDifficulty () {
		return this.GetMaterial ().GetDifficulty ();
	}

	public float GetDifficulty () {
		return this.GetMaterial ().GetDifficulty ();
	}
}

[Serializable]
public class ItemSword : IItem {
	ItemPartSword blade;
	ItemPartSword handle;
	ItemPartSword guard;
	ItemPartSword pommel;
	string name;
	Sprite texture;

	public ItemSword(ItemPartSword blade, ItemPartSword guard, ItemPartSword handle, ItemPartSword pommel, string name = "", string textureName = "") {
		if (blade.GetComponent () == SwordComponent.Blade) {
			this.blade = blade;
		}  else {
			throw new System.ArgumentException("The first argument should be a blade", "blade");
		}
		if (guard.GetComponent () == SwordComponent.Guard) {
			this.guard = guard;
		}  else {
			throw new System.ArgumentException("The second material should be a guard", "guard");
		}
		if (handle.GetComponent () == SwordComponent.Handle) {
			this.handle = handle;
		}  else {
			throw new System.ArgumentException("The second material should be a handle", "handle");
		}
		if (pommel.GetComponent () == SwordComponent.Pommel) {
			this.pommel = pommel;
		}  else {
			throw new System.ArgumentException("The second material should be a pommel", "pommel");
		}
		this.name = (name == "") ? this.blade.GetMaterial ().name + " Sword" : name;
		this.texture = Resources.Load <Sprite> (textureName);
	}

	string IItem.GetName () {
		return this.name;
	}

	public string GetName () {
		return this.name;
	}

	public int GetDurability () {
		return (int) (Mathf.Pow ((this.blade.GetMaterial ().GetQuality () * .5f + this.handle.GetMaterial ().GetQuality () * .25f + this.guard.GetMaterial ().GetQuality () * .15f + this.pommel.GetMaterial ().GetQuality () * .1f), 2f));
	}

	public int GetRange () {
		return Mathf.FloorToInt (((this.blade.GetExtra ("length") / 100f) * this.blade.GetMaterial ().GetQuality ()) + this.blade.GetExtra ("length"));
	}

	public float GetSpeed () {
		return Mathf.Round ((this.blade.GetMaterial ().GetWeight () * 2+ this.handle.GetMaterial ().GetWeight ()+ this.guard.GetMaterial ().GetWeight ()+ (this.pommel.GetMaterial ().GetWeight () / 2f)) * 100f) / 100f;
	}

	public float GetDisplaySpeed () {
		return 25f - this.GetSpeed ();
	}

	public int GetDamage () {
		return Mathf.FloorToInt ((this.blade.GetMaterial ().GetQuality () * this.GetRange () * this.blade.GetMaterial ().GetSharpness ()) / this.GetSpeed ());
	}

	public float GetBalance () {
		Vector2 balance = new Vector2 (0, 0);
		balance += new Vector2 (this.guard.GetExtra ("balance"), 0);
		balance += new Vector2(0, (this.blade.GetMaterial ().GetWeight () * this.blade.GetExtra ("length")) / (this.guard.GetMaterial ().GetWeight () * 30 + this.handle.GetMaterial ().GetWeight () * 50 + this.pommel.GetMaterial ().GetWeight () * 20));
		return (balance.magnitude == 0f) ? .0001f : Mathf.Round (balance.magnitude * 100f) / 100f;
	}

	public float GetBasePrice () {
		int durability = this.GetDurability ();
		int range = this.GetRange ();
		float speed = this.GetSpeed ();
		int damage = this.GetDamage ();
		float balance = this.GetBalance ();
		return (durability * damage) / (50 * (Mathf.Abs(1 - balance) + 1) * speed) * 1500;
	}

	ItemType IItem.GetType() {
		return ItemType.Sword;
	}

	Sprite IItem.GetTexture() {
		return this.texture;
	}

	public Sprite GetTexture() {
		return this.texture;
	}

	float IItem.GetDifficulty () {
		return this.blade.GetDifficulty () * .5f + this.guard.GetDifficulty () * .15f + this.pommel.GetDifficulty () * .1f + this.handle.GetDifficulty () * .25f;
	}

	public float GetDifficulty () {
		return (this.blade.GetDifficulty () + this.guard.GetDifficulty () + this.pommel.GetDifficulty () + this.handle.GetDifficulty ()) / 4;
	}

	public ItemPartSword GetBlade () {
		return this.blade;
	}

	public ItemPartSword GetPommel () {
		return this.pommel;
	}

	public ItemPartSword GetGuard () {
		return this.guard;
	}

	public ItemPartSword GetHandle () {
		return this.handle;
	}
}
