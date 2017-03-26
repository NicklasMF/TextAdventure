using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class Dialogue {

    public List<DialogueNode> Nodes;

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
    }

    public static Dialogue LoadDialogue(string path) {
        XmlSerializer seri = new XmlSerializer(typeof(Dialogue));
        StreamReader reader = new StreamReader(path);

        Dialogue dia = (Dialogue)seri.Deserialize(reader);
        return dia;
    }

}
