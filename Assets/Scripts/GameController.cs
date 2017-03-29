using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	void Awake() {
        SetDefaultPlayerPrefs();

		GetComponent<NavigationController>().ShowDialogue();

		//DiceController.HasRolledDie += FinishedRolling;
    }

	void Start() {
		GetComponent<NavigationController>().ShowDialogue();
	}

    void SetDefaultPlayerPrefs() {
        if (!PlayerPrefs.HasKey("Player_Level")) {
            PlayerPrefs.SetString("Player_Name", "Jamison");

            PlayerPrefs.SetInt("Player_Level", 1);

        }
    }


	public void PrepareDice() {
		GetComponent<NavigationController>().ShowDice();
		//diceController.GetComponent<DiceController>().txtWinCondition = ""
		//GetComponent<DiceController>().uiTouchPanel.GetComponent<Button>().onClick.AddListener(MoveOn);

		GetComponent<DiceController>().SetDieReady();
	}


	void FinishedRolling(int sum) {
		int nextNode;
		int monsterId = GetComponent<NpcDialogue>().current_monster;
		int monsterIndex = GetComponent<NpcDialogue>().dia.GetIndexByMonsterId(monsterId);
		int winCondition = GetComponent<NpcDialogue>().dia.Monsters[monsterIndex].WinCondition;

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
