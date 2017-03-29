using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class Dialogue {

    public List<DialogueNode> Nodes;
	public List<Monster> Monsters;

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

    public void AddNode(DialogueNode node) {
        if (node == null) return;

        Nodes.Add(node);
        node.NodeID = Nodes.IndexOf(node);
    }
	
    public void AddOption(string text, DialogueNode node, DialogueNode dest) {
        if (!Nodes.Contains(dest)) {
            AddNode(dest);
        }

        if (!Nodes.Contains(node)) {
            AddNode(node);
        }

        DialogueOption opt;

        if (dest == null) {
            opt = new DialogueOption(text, -1);
        } else {
            opt = new DialogueOption(text, dest.NodeID);
        }

        node.Options.Add(opt);
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
