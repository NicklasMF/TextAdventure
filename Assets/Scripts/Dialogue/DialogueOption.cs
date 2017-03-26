using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class DialogueOption {

    public string Text;
    public int DestinationNodeID;

    public DialogueOption() { }

    public DialogueOption(string text, int dest) {
        this.Text = text;
        this.DestinationNodeID = dest;
    }

}
