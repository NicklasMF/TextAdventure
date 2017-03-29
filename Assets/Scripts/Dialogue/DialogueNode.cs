﻿using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class DialogueNode {

    public int NodeID = -1;

    public string Text;
	public int DestinationNodeID;

    public List<DialogueOption> Options;

	public DialogueCondition Conditions;

    public DialogueNode() { }

    public DialogueNode(string text) {
        Text = text;
        Options = new List<DialogueOption>();
		Conditions = new DialogueCondition();
    }
}
