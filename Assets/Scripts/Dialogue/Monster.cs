using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Monster {

    public int MonsterID = -1;
	public string Name;

	public int WinCondition;
	public int WinNodeId;
	public int LoseNodeId;

	public Monster() { }

	public Monster(string name) {
		Name = name;
    }
}
