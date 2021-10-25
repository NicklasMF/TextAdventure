using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class WeaponDatabase : MonoBehaviour {

	public List<Weapon> database = new List<Weapon>();
	JsonData itemData;

	void Start() {
		itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Weapons.json"));
		ConstructItemDatabase();

		GetPlayerWeapons();

	}

	void ConstructItemDatabase() {
		for (int i = 0; i < itemData.Count; i++) {
			database.Add(new Weapon((int)itemData[i]["id"], itemData[i]["title"].ToString(), itemData[i]["damage"]["die"].ToString(), (int)itemData[i]["damage"]["criticalHit"], (int)itemData[i]["goldPieces"]));
		}
	}

	void GetPlayerWeapons() {
		Player player = GetComponent<GameController>().player;

		for (int i = 0; i < database.Count; i++) {
			if (PlayerPrefs.GetInt("Weapon_id"+database[i].ID) == 1) {
				player.Weapons.Add(database[i]);
			}
		}
	}

	public Weapon FetchItemById(int id) {
		for (int i = 0; i < database.Count; i++) {
			if (database[i].ID == id) {
				return database[i];
			}
		}
		return null;
	}

	public int GetDieByWeaponId(int id) {
		Weapon weapon = FetchItemById(id);
		List<string> array = new List<string>(weapon.Die.Split("d".ToCharArray()));
		return int.Parse(array[1]);
	}

}

public class Weapon {
	public int ID { get; set; }
	public string Title { get; set; }
	public string Die { get; set; }
	public int CriticalHit { get; set; }
	public int GoldPieces { get; set; }

	public Weapon(int id, string title, string die, int criticalHit, int goldPieces) {
		ID = id;
		Title = title;
		Die = die;
		CriticalHit = criticalHit;
		GoldPieces = goldPieces;
	}

	public Weapon() {
		ID = -1;
	}
}