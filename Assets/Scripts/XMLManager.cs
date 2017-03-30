using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class XMLManager : MonoBehaviour {

	public static XMLManager ins;
	public Dialogue dialogue;

	[SerializeField] GameObject NodePrefab;
	[SerializeField] GameObject AnswerPrefab;
	[SerializeField] Transform NodesWrapper;

	void Awake() {
		ins = this;
		LoadItems();
	}

	public void SaveItems() {
		XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
		FileStream stream = new FileStream(Application.dataPath + "/new1Dialogue.xml", FileMode.Create);
		serializer.Serialize(stream, dialogue);
		stream.Close();
		print("Saved");
	}

	public void LoadItems() {
		XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
		FileStream stream = new FileStream(Application.dataPath + "/newDialogue.xml", FileMode.Open);
		dialogue = serializer.Deserialize(stream) as Dialogue;
		stream.Close();

		DrawNodes();
	}

	void DrawNodes() {
		int posX = -100;
		int posNX = posX;
		int posY = -20;
		int posNY = posY;

		for (int currentNode = 0; currentNode < dialogue.Nodes.Count; currentNode++) {

			int currentIndex = dialogue.GetIndexByNodeId(currentNode);
			DialogueNode node = dialogue.Nodes[currentIndex];
			GameObject go = Instantiate(NodePrefab, NodesWrapper);
			go.transform.localPosition = new Vector3(posX, posY, 0);

			go.GetComponent<XMLNodePrefab>().nodeId.text = "NodeID: "+ node.NodeID.ToString();
			go.GetComponent<XMLNodePrefab>().storyText.text = node.Text.ToString();



			if (node.Options.Count > 0) {
				posNX = posX + 280;
				posNY = posY;
				go.GetComponent<XMLNodePrefab>().destinationID.text = "";

				foreach(DialogueOption opt in dialogue.Nodes[currentIndex].Options) {
					GameObject newOpt = (GameObject) Instantiate(AnswerPrefab, NodesWrapper);
					newOpt.transform.localPosition = new Vector3(posNX, posNY, 0);
					newOpt.GetComponent<XMLAnswerPrefab>().StoryText.text = opt.Text;
					newOpt.GetComponent<XMLAnswerPrefab>().NextDestinationID.text = opt.DestinationNodeID.ToString();
					posNY -= 70;
				}
			} else {
				go.GetComponent<XMLNodePrefab>().destinationID.text = node.DestinationNodeID.ToString();
			}

			if (node.Conditions != null) {
				go.GetComponent<XMLNodePrefab>().ConditionText.text = node.Conditions.ConditionText;
				go.GetComponent<XMLNodePrefab>().MinimumRollInput.text = node.Conditions.MinimumRoll.ToString();
				go.GetComponent<XMLNodePrefab>().WinInput.text = node.Conditions.WinNodeID.ToString();
				go.GetComponent<XMLNodePrefab>().LoseInput.text = node.Conditions.LoseNodeID.ToString();


			} else {
				go.GetComponent<XMLNodePrefab>().ConditionText.transform.parent.parent.gameObject.SetActive(false);

			}
			posX = -100;
			posY -= 200;
		}

		/*
		foreach(DialogueNode node in dialogue.Nodes) {

			GameObject go = Instantiate(NodePrefab, NodesWrapper);
			go.transform.localPosition = new Vector3(posX, posY, 0);

			go.GetComponent<XmlNodePrefab>().nodeId.text = node.NodeID.ToString();
			go.GetComponent<XmlNodePrefab>().storyText.text = node.Text.ToString();

			if (node.Options.Count > 0) {
				go.GetComponent<XmlNodePrefab>().destinationID.text = "";
			} else {
				go.GetComponent<XmlNodePrefab>().destinationID.text = node.DestinationNodeID.ToString();

			}


			posY -= 170;
		}*/
	}
}