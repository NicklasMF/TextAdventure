using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class Monster {

    public int MonsterID = -1;
	public string Name;

	public int Life;
	public int ArmourClass;
	public int WinNodeId;
	public int LoseNodeId;

	public Monster() { }

	public Monster(int _id) {
		MonsterID = _id;

    }
}
