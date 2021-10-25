using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Player player;

	void Awake() {
		//DeletePlayerPrefs();
        SetDefaultPlayerPrefs();

		GetComponent<NavigationController>().ShowDialogue();

		//DiceController.HasRolledDie += FinishedRolling;
    }

	void Start() {
		GetComponent<NavigationController>().ShowDialogue();
	}

    void SetDefaultPlayerPrefs() {
		player = new Player();
		player.Awake();
    }

	void DeletePlayerPrefs() {
		PlayerPrefs.DeleteAll();
	}

	public void PrepareDice() {
		GetComponent<NavigationController>().ShowDice();
		//diceController.GetComponent<DiceController>().txtWinCondition = ""

		GetComponent<NavigationController>().BeforeCondition();
		GetComponent<DiceController>().SetDieReady("player", 20);
	}


	void FinishedRolling(int sum) {
		int nextNode;
		int monsterId = GetComponent<NpcDialogue>().current_monster;
		int monsterIndex = GetComponent<NpcDialogue>().dia.GetIndexByMonsterId(monsterId);
		int winCondition = GetComponent<NpcDialogue>().dia.Monsters[monsterIndex].ArmourClass;

		string txt = "Du slog " + sum + ". ";

		if (sum >= winCondition) {
			nextNode = GetComponent<NpcDialogue>().dia.Monsters[monsterIndex].WinNodeId;
			//dialogueController.GetComponent<NpcDialogue>().RunDialogue(nextNode);
			txt += "Du vandt.";
		} else {
			nextNode = GetComponent<NpcDialogue>().dia.Monsters[monsterIndex].LoseNodeId;
			//dialogueController.GetComponent<NpcDialogue>().RunDialogue(nextNode);
			txt += "Det var ikke nok. Du tabte.";
		}

		GetComponent<DiceController>().txtStatus.text = txt;
			
	}

	void MoveOn() {
		print("Let move on");
	}
}
