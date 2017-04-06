using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Player {

    public int ID = 0;
	public string Name;

	public int Level;
	public int Life;
	public int ArmourClass;

	public List<Weapon> Weapons = new List<Weapon>();

	public Player() {
	}

	public void Awake() {
		if (!PlayerPrefs.HasKey("Player_Level")) {
			PlayerPrefs.SetString("Player_Name", "Jamison");
			PlayerPrefs.SetInt("Player_Level", 1);
			PlayerPrefs.SetInt("Player_Life", 20);
			PlayerPrefs.SetInt("Player_ArmourClass", 10);
		}

		Name = PlayerPrefs.GetString("Player_Name");
		Level = PlayerPrefs.GetInt("Player_Level");
		Life = PlayerPrefs.GetInt("Player_Life");
		ArmourClass = PlayerPrefs.GetInt("Player_ArmourClass");
	}

	public Player(int _id) {
		ID = _id;
    }

	public void UpdateLife(int _sum) {
		int life;
		if (PlayerPrefs.HasKey("Player_Life")) {
			life = PlayerPrefs.GetInt("Player_Life");
			Life += _sum;
			PlayerPrefs.SetInt("Player_Life", life);
		} else {
			Debug.LogError("Liv kunne ikke opdateres.");
		}
	}

	public int GetLife() {
		return PlayerPrefs.GetInt("Player_Life");
	}

	public void AddWeapon(Weapon _weapon) {
		Weapons.Add(_weapon);
		PlayerPrefs.SetInt("Weapon_id"+_weapon.ID, 1);
	}
}
