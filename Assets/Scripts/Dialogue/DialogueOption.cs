using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class DialogueOption {

    public string Text;
    public int DestinationNodeID;
	public int MonsterID;

    public DialogueOption() { }

    public DialogueOption(string text, int dest) {
        this.Text = text;
        this.DestinationNodeID = dest;
		this.MonsterID = dest;
    }

	public bool IsMonster() {
		if (MonsterID > 0) {
			return true;
		} else{
			return false;
		}
	}

}
