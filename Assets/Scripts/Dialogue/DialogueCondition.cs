using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class DialogueCondition {

	public int Sneek;

	public int MinimumRoll;
	public string ConditionText;
	public int WinNodeID;
	public int LoseNodeID;

	public DialogueCondition() { }

	public DialogueCondition(string text, int sneek, int win, int lose) {
		this.ConditionText = text;
		this.Sneek = sneek;
		this.WinNodeID = win;
		this.LoseNodeID = lose;
	}


}
