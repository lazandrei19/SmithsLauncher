using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {

	public static GameController control;
	public delegate void BeforeSave ();
	public static event BeforeSave beforeSave;
	public delegate void AfterLoad ();
	public static event AfterLoad afterLoad;
	SaveData data = new SaveData ();

	// Use this for initialization of the singleton pattern
	void Awake () {
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}
	}

	void Start () {
		Tasks.Initialize ();
	}

	//Sets the value of an integer
	public void SetInt(string key, int value) {
		if (data.intStorage.ContainsKey (key) == false)
			data.intStorage.Add (key, value);
		else
			data.intStorage [key] = value;
	}

	//Sets the value of a float
	public void SetFloat(string key, float value) {
		if (data.floatStorage.ContainsKey (key) == false)
			data.floatStorage.Add (key, value);
		else
			data.floatStorage [key] = value;
	}

	//Sets the value of a string
	public void SetString(string key, string value) {
		if (data.stringStorage.ContainsKey (key) == false)
			data.stringStorage.Add (key, value);
		else
			data.stringStorage [key] = value;
	}

	//Sets the value of a bool
	public void SetBool(string key, bool value) {
		if (data.boolStorage.ContainsKey (key) == false)
			data.boolStorage.Add (key, value);
		else
			data.boolStorage [key] = value;
	}

	//Sets the value of an item
	public void SetItem(string key, IItem value) {
		if (data.itemStorage.ContainsKey (key) == false)
			data.itemStorage.Add (key, value);
		else
			data.itemStorage [key] = value;
	}

	//Sets the value of the inventory
	public void SetInventory(Dictionary<IItem, int> inventory) {
		data.inventory = inventory;
	}

	//Gets the value of an integer
	public int GetInt(string key) {
		if (data.intStorage.ContainsKey(key))
			return data.intStorage [key];
		else
			throw new System.ArgumentException("Specified key doesn't exist", "key");
	}

	//Gets the value of a float
	public float GetFloat(string key) {
		if (data.floatStorage.ContainsKey(key))
			return data.floatStorage [key];
		else
			throw new System.ArgumentException("Specified key doesn't exist", "key");
	}

	//Gets the value of a string
	public string GetString(string key) {
		if (data.stringStorage.ContainsKey(key))
			return data.stringStorage [key];
		else
			throw new System.ArgumentException("Specified key doesn't exist", "key");
	}

	//Gets the value of a bool
	public bool GetBool(string key) {
		if (data.boolStorage.ContainsKey(key))
			return data.boolStorage [key];
		else
			throw new System.ArgumentException("Specified key doesn't exist", "key");
	}

	//Gets the value of an item
	public IItem GetItem(string key) {
		if (data.itemStorage.ContainsKey(key))
			return data.itemStorage [key];
		else
			throw new System.ArgumentException("Specified key doesn't exist", "key");
	}

	//Gets the value of the inventory
	public Dictionary<IItem, int> GetInventory() {
		return data.inventory;
	}

	//Saves + calls the BeforeSave event
	public void Save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream saveFile = File.Create (Application.persistentDataPath + "/save.dat");
		if (beforeSave != null) {
			beforeSave ();
		}
		bf.Serialize (saveFile, data);
		saveFile.Close ();
	}

	//Loads + calls the AfterLoad event
	public bool Load() {
		if (File.Exists (Application.persistentDataPath + "/save.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream saveFile = File.Open (Application.persistentDataPath + "/save.dat", FileMode.Open);

			data = (SaveData)bf.Deserialize (saveFile);
			saveFile.Close ();
			if (afterLoad != null) {
				afterLoad ();
			}
			return true;
		} else {
			return false;
		}
	}
}

//The data holder object
//Will add more types if necesary
[Serializable]
class SaveData {
	public Dictionary<string, int> intStorage = new Dictionary<string, int> ();
	public Dictionary<string, float> floatStorage = new Dictionary<string, float> ();
	public Dictionary<string, string> stringStorage = new Dictionary<string, string> ();
	public Dictionary<string, bool> boolStorage = new Dictionary<string, bool> ();
	public Dictionary<string, IItem> itemStorage = new Dictionary<string, IItem> ();
	public Dictionary<IItem, int> inventory = new Dictionary<IItem, int> ();
}
