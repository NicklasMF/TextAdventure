using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[System.Serializable]
public class Dialogue {

    public List<DialogueNode> Nodes;
	public List<Monster> Monsters;
	public List<Weapon> Weapons;

	public int GetIndexByNodeId(int nodeId) {
		foreach (DialogueNode node in Nodes) {
			if (node.NodeID == nodeId) {
				return Nodes.IndexOf(node);
			}
		}
		return -1;
	}

	public int GetIndexByMonsterId(int monsterId) {
		foreach (Monster monster in Monsters) {
			if (monster.MonsterID == monsterId) {
				return Monsters.IndexOf(monster);
			}
		}
		return -1;
	}

	public int GetIndexByWeaponId(int weaponId) {
		foreach (Weapon weapon in Weapons) {
			if (weapon.WeaponID == weaponId) {
				return Weapons.IndexOf(weapon);
			}
		}
		return -1;
	}

	public void AddNode(DialogueNode node) {
		if (node == null) return;

		Nodes.Add(node);
		node.NodeID = Nodes.IndexOf(node);
	}

	public void AddOption(DialogueNode node) {
		DialogueOption opt = new DialogueOption();
		node.Options.Add(opt);
	}

	public void AddMonster(Monster monster) {
		if (monster == null) return;

		Monsters.Add(monster);
		monster.MonsterID = Monsters.IndexOf(monster);
	}

	public void AddWeapon(Weapon weapon) {
		if (weapon == null) return;

		Weapons.Add(weapon);
		weapon.WeaponID = Weapons.IndexOf(weapon);
	}

	public DialogueOption AddOption(DialogueNode node, bool _returnObj) {
		if (_returnObj) {
			DialogueOption opt = new DialogueOption();
			node.Options.Add(opt);
			return opt;
		}
		return null;
	}

    public Dialogue() {
        Nodes = new List<DialogueNode>();
		Monsters = new List<Monster>();
    }

    public static Dialogue LoadDialogue(string path) {
        XmlSerializer seri = new XmlSerializer(typeof(Dialogue));
        StreamReader reader = new StreamReader(path);

        Dialogue dia = (Dialogue)seri.Deserialize(reader);
        return dia;
    }

}
