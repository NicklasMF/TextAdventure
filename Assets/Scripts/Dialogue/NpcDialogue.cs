using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogue : MonoBehaviour {


	[HideInInspector] GameObject gameController;
	[HideInInspector] public Dialogue dia;

    GameObject txtStory;
    GameObject option_1;
    GameObject option_2;
    GameObject option_3;


	public int selected_type = 0;
	public int current_node = -1;
	public int current_monster = -1;

    public string DialogueDataFilePath;
	[SerializeField] GameObject uiDialogueWindow;
	[SerializeField] GameObject uiGoingToThrow;
	//[SerializeField] GameObject DialogueWindowPrefab;

    void Start() {
        dia = Dialogue.LoadDialogue("Assets/" + DialogueDataFilePath);
		gameController = GameObject.FindGameObjectWithTag("GameController");

        //dialogue_window = Instantiate<GameObject>(DialogueWindowPrefab);
        //dialogue_window.transform.SetParent(canvas.transform, false);
        RectTransform dia_windows_transform = (RectTransform)uiDialogueWindow.transform;
        dia_windows_transform.localPosition = new Vector3(0, 0, 0);

		txtStory = uiDialogueWindow.GetComponent<UIDialogue>().txtStory;
		option_1 = uiDialogueWindow.GetComponent<UIDialogue>().btn1;
		option_2 = uiDialogueWindow.GetComponent<UIDialogue>().btn2;
		option_3 = uiDialogueWindow.GetComponent<UIDialogue>().btn3;

        RunDialogue(0);
    }

    public void RunDialogue(int _nodeid) {
		RunNode(_nodeid);
    }

	void SetSelectedOption(int nodeId, int monsterId) {
		if (monsterId != 0) {
			current_monster = monsterId;
			selected_type = 1;
		} else if (nodeId != 0) {
			current_monster = -1;
			selected_type = 0;
		}
    }

	void RunNode(int _nodeid) {
		current_node = _nodeid;
		if (_nodeid < 0)	return;

		gameController.GetComponent<NavigationController>().ShowDialogue();
		DialogueNode _node = dia.Nodes[dia.GetIndexByNodeId(_nodeid)];

		// Der skal slås med en terning //
		if (_node.Conditions != null && _node.Conditions.MinimumRoll > 0) {
			uiGoingToThrow.SetActive(true);
			uiDialogueWindow.SetActive(false);

			uiGoingToThrow.GetComponent<GoingToThrow>().txtStory.text = _node.Text;
			uiGoingToThrow.GetComponent<GoingToThrow>().txtStatus.text = _node.Conditions.ConditionText;
			uiGoingToThrow.GetComponent<GoingToThrow>().btnRollDie.onClick.AddListener(delegate { PrepareForThrow(); });
		} else {
			uiGoingToThrow.SetActive(false);
			uiDialogueWindow.SetActive(true);
			display_node(dia.Nodes[dia.GetIndexByNodeId(current_node)]);
		}
	}

	void RunBattle(int _monsterID) {
		Monster monster = dia.Monsters[dia.GetIndexByMonsterId(_monsterID)];
		gameController.GetComponent<NavigationController>().BeforeBattle(monster);
	}

	void PrepareForThrow() {
		DiceController.HasRolledDie += CheckResultOfThrow;
		gameController.GetComponent<NavigationController>().uiCondition.GetComponent<UICondition>().txtWinCondition.text = dia.Nodes[dia.GetIndexByNodeId(current_node)].Conditions.ConditionText;
		gameController.GetComponent<GameController>().PrepareDice();
	}

	void CheckResultOfThrow(int sum) {
		DiceController.HasRolledDie -= CheckResultOfThrow;

		gameController.GetComponent<NavigationController>().AfterCondition();
		gameController.GetComponent<NavigationController>().uiCondition.GetComponent<UICondition>().txtSum.text = sum.ToString();

		DialogueNode node = dia.Nodes[dia.GetIndexByNodeId(current_node)];
		int winCase = node.Conditions.MinimumRoll;
		int nextNode = (sum >= winCase) ? node.Conditions.WinNodeID : node.Conditions.LoseNodeID;
		Button toContinue = gameController.GetComponent<NavigationController>().uiCondition.GetComponent<UICondition>().ContinuePanel.GetComponent<Button>();
		toContinue.onClick.AddListener(delegate { RunNode(nextNode); });
	}

    void display_node(DialogueNode node) {
		txtStory.GetComponent<Text>().text = node.Text;
        option_1.SetActive(false);
        option_2.SetActive(false);
        option_3.SetActive(false);

		if (node.Options.Count > 0) {
			uiDialogueWindow.GetComponent<UIDialogue>().tapToContinue.SetActive(false);
		} else {
			uiDialogueWindow.GetComponent<UIDialogue>().tapToContinue.SetActive(true);
			if (node.MonsterID > 0) {
				uiDialogueWindow.GetComponent<UIDialogue>().tapToContinue.GetComponent<Button>().onClick.AddListener(delegate { RunBattle(node.MonsterID); });
			} else {
				uiDialogueWindow.GetComponent<UIDialogue>().tapToContinue.GetComponent<Button>().onClick.AddListener(delegate { RunNode(node.DestinationNodeID); });
			}
		}

        for(int i = 0; i < node.Options.Count; i++) {
            switch(i) {
                case 0:
                set_option_button(option_1, node.Options[i]);
                break;
                case 1:
                set_option_button(option_2, node.Options[i]);
                break;
                case 2:
                set_option_button(option_3, node.Options[i]);
                break;
            }
        }
    }

	void display_monster(Monster monster) {


		txtStory.GetComponent<Text>().text = "Du står over for " + monster.Name;

		option_1.SetActive(false);
		option_2.SetActive(false);
		option_3.SetActive(false);

		option_1.GetComponentInChildren<Text>().text = "Jeg er klar!";
		option_1.GetComponent<Button>().onClick.AddListener( delegate { gameController.GetComponent<GameController>().PrepareDice(); } );
	}

    void set_option_button(GameObject button, DialogueOption opt) {
        button.SetActive(true);
        button.GetComponentInChildren<Text>().text = opt.Text;
		button.GetComponent<Button>().onClick.AddListener(delegate { RunNode(opt.DestinationNodeID); });
    }

}
