using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class XMLManager : MonoBehaviour {

	public static XMLManager ins;
	public Dialogue dialogue;

	[SerializeField] GameObject MonsterPrefab;
	[SerializeField] GameObject NodePrefab;
	[SerializeField] GameObject AnswerPrefab;
	[SerializeField] GameObject ConditionPrefab;
	[SerializeField] GameObject WeaponPrefab;
	[SerializeField] Transform nodeContent;
	[SerializeField] Transform monsterContent;
	[SerializeField] Transform weaponContent;

	[SerializeField] GameObject MonsterWrapper;
	[SerializeField] GameObject NodeWrapper;
	[SerializeField] GameObject WeaponWrapper;

	int lastY;
	int monLastY;
	int weaLastY;

	void Awake() {
		ins = this;
		LoadItems();
		ShowNodes();
	}

	#region Load/Save
	public void SaveItems() {
		XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
		FileStream stream = new FileStream(Application.dataPath + "/Chapter1.xml", FileMode.Create);
		serializer.Serialize(stream, dialogue);
		stream.Close();
		print(System.DateTime.Now + ": Saved");
	}

	public void LoadItems() {
		foreach(Transform child in nodeContent) {
			Destroy(child.gameObject);
		}
		foreach(Transform child in monsterContent) {
			Destroy(child.gameObject);
		}
		foreach(Transform child in weaponContent) {
			Destroy(child.gameObject);
		}
		XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
		FileStream stream = new FileStream(Application.dataPath + "/Chapter1.xml", FileMode.Open);
		dialogue = serializer.Deserialize(stream) as Dialogue;
		stream.Close();

		DrawNodes();
		DrawMonsters();
	}
	#endregion
	#region Draw
	void DrawNodes() {
		int posX = -100;
		int posNX = posX;
		int posY = -20;
		int posNY = posY;

		List<int> nodeArray = new List<int>();
		foreach (DialogueNode node in dialogue.Nodes) {
			nodeArray.Add(node.NodeID);
		}
		nodeArray.Sort();

		foreach(int currentNode in nodeArray) {
			int currentIndex = dialogue.GetIndexByNodeId(currentNode);

			DialogueNode node = dialogue.Nodes[currentIndex];
			GameObject go = Instantiate(NodePrefab, nodeContent);
			go.transform.localPosition = new Vector3(posX, posY, 0);
			go.name = "NodeID " + node.NodeID;

			go.GetComponent<XMLNodePrefab>().nodeId.text = node.NodeID.ToString();
			go.GetComponent<XMLNodePrefab>().storyText.text = node.Text.ToString();
			go.GetComponent<XMLNodePrefab>().nodeId.onEndEdit.AddListener(delegate {
				ChangeInput(node, "NodeID", go.GetComponent<XMLNodePrefab>().nodeId.text);
			});
			go.GetComponent<XMLNodePrefab>().storyText.onEndEdit.AddListener(delegate {
				ChangeInput(node, "Text", go.GetComponent<XMLNodePrefab>().storyText.text);
			});
			go.GetComponent<XMLNodePrefab>().destinationID.onEndEdit.AddListener(delegate {
				ChangeInput(node, "DestinationID", go.GetComponent<XMLNodePrefab>().destinationID.text);
			});
			go.GetComponent<XMLNodePrefab>().monsterID.onEndEdit.AddListener(delegate {
				ChangeInput(node, "MonsterID", go.GetComponent<XMLNodePrefab>().monsterID.text);
			});
			go.GetComponent<XMLNodePrefab>().AddAnswerBtn.onClick.AddListener(delegate {
				AddAnswer(node, go);
			});
			go.GetComponent<XMLNodePrefab>().DeleteBtn.onClick.AddListener(delegate {
				DeleteNode(go, node);
			});


			if (node.Options.Count > 0) {
				posNX = posX + 310;
				posNY = posY;
				go.GetComponent<XMLNodePrefab>().destinationID.text = "";

				foreach(DialogueOption opt in dialogue.Nodes[currentIndex].Options) {
					GameObject newOpt = (GameObject) Instantiate(AnswerPrefab, nodeContent);
					newOpt.transform.localPosition = new Vector3(posNX, posNY, 0);
					newOpt.name = "NodeID "+ node.NodeID + " Option";
					newOpt.GetComponent<XMLAnswerPrefab>().StoryText.text = opt.Text;
					newOpt.GetComponent<XMLAnswerPrefab>().NextDestinationID.text = opt.DestinationNodeID.ToString();
					newOpt.GetComponent<XMLAnswerPrefab>().StoryText.onEndEdit.AddListener(delegate {
						ChangeInput(opt, "StoryText", newOpt.GetComponent<XMLAnswerPrefab>().StoryText.text);
					});
					newOpt.GetComponent<XMLAnswerPrefab>().NextDestinationID.onEndEdit.AddListener(delegate {
						ChangeInput(opt, "DestinationID", newOpt.GetComponent<XMLAnswerPrefab>().NextDestinationID.text);
					});
					newOpt.GetComponent<XMLAnswerPrefab>().DeleteBtn.onClick.AddListener(delegate {
						DeleteAnswer(newOpt, node, opt);
					});
					posNX += 235;
				}
			} else {
				if (node.DestinationNodeID != 0) {
					go.GetComponent<XMLNodePrefab>().destinationID.text = node.DestinationNodeID.ToString();
				}
				if (node.MonsterID != 0) {
					go.GetComponent<XMLNodePrefab>().destinationID.text = node.MonsterID.ToString();
				}
			}

			if (node.Conditions != null && node.Conditions.MinimumRoll > 0) {
				GameObject cond = Instantiate(ConditionPrefab, nodeContent);
				cond.transform.localPosition = new Vector3(posX - 290, posY,0);
				cond.name = "NodeID " + node.NodeID + " Condition";

				cond.GetComponent<XMLConditionPrefab>().Header.text = "Condition: NodeID: " +node.NodeID.ToString();
				cond.GetComponent<XMLConditionPrefab>().ConditionText.text = node.Conditions.ConditionText;
				cond.GetComponent<XMLConditionPrefab>().MinimumRollInput.text = node.Conditions.MinimumRoll.ToString();
				cond.GetComponent<XMLConditionPrefab>().WinInput.text = node.Conditions.WinNodeID.ToString();
				cond.GetComponent<XMLConditionPrefab>().LoseInput.text = node.Conditions.LoseNodeID.ToString();
				cond.GetComponent<XMLConditionPrefab>().DeleteBtn.onClick.AddListener(delegate {
					DeleteCondition(cond, node.Conditions);
				});

				AddConditionListeners(node, cond);

				go.GetComponent<XMLNodePrefab>().AddConditionBtn.gameObject.SetActive(false);
			} else {
				
				go.GetComponent<XMLNodePrefab>().AddConditionBtn.onClick.AddListener(delegate {
					AddCondition(go);
				});

			}
			posX = -100;
			posY -= 170;
			lastY = posY;
		}
	}

	void DrawMonsters() {
		int monPosX = -100;
		int monPosY = -20;

		List<int> monsterArray = new List<int>();
		foreach (Monster monster in dialogue.Monsters) {
			monsterArray.Add(monster.MonsterID);
		}
		monsterArray.Sort();

		foreach(int currentMonster in monsterArray) {
			int currentIndex = dialogue.GetIndexByMonsterId(currentMonster);

			Monster monster = dialogue.Monsters[currentIndex];
			GameObject go = Instantiate(MonsterPrefab, monsterContent);
			go.transform.localPosition = new Vector3(monPosX, monPosY, 0);
			go.name = "MonsterID: " + monster.MonsterID;

			go.GetComponent<XMLMonsterPrefab>().monsterId.text = monster.MonsterID.ToString();
			go.GetComponent<XMLMonsterPrefab>().name.text = monster.Name.ToString();
			go.GetComponent<XMLMonsterPrefab>().life.text = monster.Life.ToString();
			go.GetComponent<XMLMonsterPrefab>().armourClass.text = monster.ArmourClass.ToString();
			go.GetComponent<XMLMonsterPrefab>().winID.text = monster.WinNodeId.ToString();
			go.GetComponent<XMLMonsterPrefab>().loseID.text = monster.LoseNodeId.ToString();

			go.GetComponent<XMLMonsterPrefab>().monsterId.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "MonsterID", go.GetComponent<XMLMonsterPrefab>().monsterId.text);
			});
			go.GetComponent<XMLMonsterPrefab>().name.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "Name", go.GetComponent<XMLMonsterPrefab>().name.text);
			});
			go.GetComponent<XMLMonsterPrefab>().life.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "Life", go.GetComponent<XMLMonsterPrefab>().life.text);
			});
			go.GetComponent<XMLMonsterPrefab>().armourClass.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "ArmourClass", go.GetComponent<XMLMonsterPrefab>().armourClass.text);
			});
			go.GetComponent<XMLMonsterPrefab>().winID.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "WinID", go.GetComponent<XMLMonsterPrefab>().winID.text);
			});
			go.GetComponent<XMLMonsterPrefab>().loseID.onEndEdit.AddListener(delegate {
				ChangeInput(monster, "LoseID", go.GetComponent<XMLMonsterPrefab>().loseID.text);
			});
			go.GetComponent<XMLMonsterPrefab>().DeleteBtn.onClick.AddListener(delegate {
				DeleteMonster(go, monster);
			});

		}

		monPosX = -100;
		monPosY -= 170;
		monLastY = monPosY;
	}

	#endregion
	#region Node
	public void AddNode() {
		DialogueNode node = new DialogueNode();
		GameObject go = (GameObject) Instantiate(NodePrefab, nodeContent);
		go.transform.localPosition = new Vector3(-100, lastY, 0);
		dialogue.AddNode(node);

		int id = GetEmptyID("node");
		go.GetComponent<XMLNodePrefab>().nodeId.text = id.ToString();
		go.GetComponent<XMLNodePrefab>().storyText.text = "";
		dialogue.Nodes[dialogue.Nodes.Count - 1].NodeID = id;
		dialogue.Nodes[dialogue.Nodes.Count - 1].Text = "";

		go.name = "NodeID " + node.NodeID;

		go.GetComponent<XMLNodePrefab>().nodeId.text = node.NodeID.ToString();
		go.GetComponent<XMLNodePrefab>().storyText.text = node.Text.ToString();
		go.GetComponent<XMLNodePrefab>().nodeId.onEndEdit.AddListener(delegate {
			ChangeInput(node, "NodeID", go.GetComponent<XMLNodePrefab>().nodeId.text);
		});
		go.GetComponent<XMLNodePrefab>().storyText.onEndEdit.AddListener(delegate {
			ChangeInput(node, "Text", go.GetComponent<XMLNodePrefab>().storyText.text);
		});
		go.GetComponent<XMLNodePrefab>().destinationID.onEndEdit.AddListener(delegate {
			ChangeInput(node, "DestinationID", go.GetComponent<XMLNodePrefab>().destinationID.text);
		});
		go.GetComponent<XMLNodePrefab>().AddAnswerBtn.onClick.AddListener(delegate {
			AddAnswer(node, go);
		});


		lastY -= 170;
	}

	void DeleteNode(GameObject go, DialogueNode node) {
		dialogue.Nodes.Remove(node);
		Destroy(go);
	}

	void AddConditionListeners(DialogueNode node, GameObject cond) {
		cond.GetComponent<XMLConditionPrefab>().ConditionText.onEndEdit.AddListener(delegate {
			ChangeInput(node.Conditions, "StoryText", cond.GetComponent<XMLConditionPrefab>().ConditionText.text);
		});
		cond.GetComponent<XMLConditionPrefab>().WinInput.onEndEdit.AddListener(delegate {
			ChangeInput(node.Conditions, "WinID", cond.GetComponent<XMLConditionPrefab>().WinInput.text);
		});
		cond.GetComponent<XMLConditionPrefab>().LoseInput.onEndEdit.AddListener(delegate {
			ChangeInput(node.Conditions, "LoseID", cond.GetComponent<XMLConditionPrefab>().LoseInput.text);
		});
		cond.GetComponent<XMLConditionPrefab>().MinimumRollInput.onEndEdit.AddListener(delegate {
			ChangeInput(node.Conditions, "MinimumRoll", cond.GetComponent<XMLConditionPrefab>().MinimumRollInput.text);
		});
		cond.GetComponent<XMLConditionPrefab>().ConditionText.onEndEdit.AddListener(delegate {
			ChangeInput(node.Conditions, "Type", cond.GetComponent<XMLConditionPrefab>().TypeDropdown.value.ToString());
		});

	}
	void AddCondition(GameObject go) {
		GameObject cond = (GameObject) Instantiate(ConditionPrefab, nodeContent);
		cond.transform.localPosition = go.transform.localPosition - new Vector3(290, 0, 0);
		cond.GetComponent<XMLConditionPrefab>().MinimumRollInput.text = "";
		cond.GetComponent<XMLConditionPrefab>().WinInput.text = "";
		cond.GetComponent<XMLConditionPrefab>().LoseInput.text = "";
		go.GetComponent<XMLNodePrefab>().AddConditionBtn.gameObject.SetActive(false);

		DialogueNode node = dialogue.Nodes[dialogue.GetIndexByNodeId(int.Parse(go.GetComponent<XMLNodePrefab>().nodeId.text))];
		AddConditionListeners(node, cond);
	}

	void DeleteCondition(GameObject go, DialogueCondition cond) {
		cond.MinimumRoll = 0;
		cond.LoseNodeID = 0;
		cond.WinNodeID = 0;
		cond.ConditionText = "";
		Destroy(go);
	}

	void AddAnswer(DialogueNode node, GameObject go) {
		GameObject answ = (GameObject) Instantiate(AnswerPrefab, nodeContent);
		int optCount;
		if (node.Options.Count != null) {
			optCount = 0;
		} else {
			optCount = node.Options.Count;
		}
		answ.transform.localPosition = go.transform.localPosition + new Vector3(230 * (optCount + 1) + 80, 0, 0);
		DialogueOption opt = dialogue.AddOption(node, true);

		//answ.GetComponent<XMLAnswerPrefab>().StoryText.text = "";
		answ.GetComponent<XMLAnswerPrefab>().NextDestinationID.text = "";
		answ.GetComponent<XMLAnswerPrefab>().StoryText.onEndEdit.AddListener(delegate {
			ChangeInput(opt, "StoryText", answ.GetComponent<XMLAnswerPrefab>().StoryText.text);
		});
		answ.GetComponent<XMLAnswerPrefab>().NextDestinationID.onEndEdit.AddListener(delegate {
			ChangeInput(opt, "DestinationID", answ.GetComponent<XMLAnswerPrefab>().NextDestinationID.text);
		});
		answ.GetComponent<XMLAnswerPrefab>().DeleteBtn.onClick.AddListener(delegate {
			DeleteAnswer(answ, node, node.Options[optCount - 1]);
		});

	}

	void DeleteAnswer(GameObject go, DialogueNode node, DialogueOption opt) {
		for (int i = 0; i < node.Options.Count; i++) {
			if (node.Options[i] == opt) {
				node.Options.Remove(opt);
			}
		}
		Destroy(go);
	}
	#endregion
	#region Monster
	public void AddMonster() {
		Monster monster = new Monster();
		GameObject go = (GameObject) Instantiate(MonsterPrefab, monsterContent);
		go.transform.localPosition = new Vector3(-100, monLastY, 0);
		dialogue.AddMonster(monster);

		int id = GetEmptyID("monster");
		go.GetComponent<XMLMonsterPrefab>().monsterId.text = id.ToString();
		go.GetComponent<XMLMonsterPrefab>().name.text = monster.Name;
		dialogue.Monsters[dialogue.Monsters.Count - 1].MonsterID = id;

		monLastY -= 170;

	}

	void DeleteMonster(GameObject go, Monster monster) {
		dialogue.Monsters.Remove(monster);
		Destroy(go);
	}

	#endregion

	#region ChangeInput

	void ChangeInput(DialogueNode node, string type, string text) {
		switch(type) {
		case "Text":
			node.Text = text;
			break;
		case "NodeID":
			node.NodeID = int.Parse(text);
			break;
		case "DestinationID":
			if (text == "") {
				node.DestinationNodeID = 0;
			} else {
				node.DestinationNodeID = int.Parse(text);
			}
			break;
		case "MonsterID":
			if (text == "") {
				node.MonsterID = 0;
			} else {
				node.MonsterID = int.Parse(text);
			}
			break;
		default:
			print("ChangeInput gik galt med nodeID: " + node.NodeID + ", " + type + ", " + text);
			break;
		}
	}

	void ChangeInput(DialogueOption opt, string type, string text) {
		switch(type) {
		case "StoryText":
			opt.Text = text;
			break;
		case "DestinationID":
			if (text == "") {
				opt.DestinationNodeID = 0;
			} else {
				opt.DestinationNodeID = int.Parse(text);
			}
			break;
		default:
			print("ChangeInput gik galt med destNodeID: " + opt.DestinationNodeID + ", " + type + ", " + text);
			break;
		}
	}

	void ChangeInput(DialogueCondition cond, string type, string text) {
		switch(type) {
		case "StoryText":
			cond.ConditionText = text;
			break;
		case "WinID":
			if (text == "") {
				cond.WinNodeID = 0;
			} else {
				cond.WinNodeID = int.Parse(text);
			}
			break;
		case "LoseID":
			if (text == "") {
				cond.LoseNodeID = 0;
			} else {
				cond.LoseNodeID = int.Parse(text);
			}
			break;
		case "MinimumRoll":
			if (text == "") {
				cond.MinimumRoll = 0;
			} else {
				cond.MinimumRoll = int.Parse(text);
			}
			break;
		case "Type":
			cond.Sneek = 0;
			switch(text) {
			case "Sneek":
				cond.Sneek = 1;
				break;
			}
			break;
		default:
			print("ChangeInput gik galt med destNodeID: " + cond.ConditionText + ", " + type + ", " + text);
			break;
		}
	}

	void ChangeInput(Monster monster, string type, string text) {
		switch(type) {
		case "Name":
			monster.Name = text;
			break;
		case "Life":
			if (text == "") {
				monster.Life = 0;
			} else {
				monster.Life = int.Parse(text);
			}
			break;
		case "MonsterID":
			if (text == "") {
				monster.MonsterID = 0;
			} else {
				monster.MonsterID = int.Parse(text);
			}
			break;
		case "ArmourClass":
			if (text == "") {
				monster.ArmourClass = 0;
			} else {
				monster.ArmourClass = int.Parse(text);
			}
			break;
		case "WinID":
			if (text == "") {
				monster.WinNodeId = 0;
			} else {
				monster.WinNodeId = int.Parse(text);
			}
			break;
		case "LoseID":
			if (text == "") {
				monster.LoseNodeId = 0;
			} else {
				monster.LoseNodeId = int.Parse(text);
			}
			break;
		default:
			print("ChangeInput gik galt med nodeID: " + monster.MonsterID + ", " + type + ", " + text);
			break;
		}
	}


	#endregion
	#region Navigation

	public void ShowMonsters() {
		MonsterWrapper.SetActive(true);
		NodeWrapper.SetActive(false);
		WeaponWrapper.SetActive(false);
	}

	public void ShowNodes() {
		MonsterWrapper.SetActive(false);
		NodeWrapper.SetActive(true);
		WeaponWrapper.SetActive(false);
	}

	public void ShowWeapons() {
		MonsterWrapper.SetActive(false);
		NodeWrapper.SetActive(false);
		WeaponWrapper.SetActive(true);
	}

	#endregion

	int GetEmptyID(string type) {
		int x = 0;

		if (type == "node") {
			List<int> nodeArray = new List<int>();
			foreach (DialogueNode node in dialogue.Nodes) {
				nodeArray.Add(node.NodeID);
			}
			nodeArray.Sort();
			foreach(int i in nodeArray) {
				if (x != i) {
					break;
				} else {
					x++;
				}
			}
		} else if (type == "monster") {
			List<int> monsterArray = new List<int>();
			foreach (Monster monster in dialogue.Monsters) {
				monsterArray.Add(monster.MonsterID);
			}
			monsterArray.Sort();
			foreach(int i in monsterArray) {
				if (x != i) {
					break;
				} else {
					x++;
				}
			}
		}
		return x;
	}

}