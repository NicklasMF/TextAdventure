using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class Weapon {

    public int WeaponID = -1;
	public string Name;

	public string Damage;
	public int CriticalHit;

	public int GoldPieces;

	public Weapon() { }

	public Weapon(string name) {
		Name = name;
    }
}
