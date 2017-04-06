using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {

	public GameObject introWrapper;
	public GameObject duringWrapper;
	public Monster monster;
	public Player player;

	GameObject gameController;

	// Game Variables //
	List<int> Initiave = new List<int>();
	List<int> InitiaveRolls = new List<int>();
	bool initiavePhase;
	int currentPhase;
	int currentTurn;


	void Awake() {
		gameController = GameObject.FindGameObjectWithTag("GameController");
	}

	public void SetupBattle() {
		Initiave.Clear();
		InitiaveRolls.Clear();

		initiavePhase = true;
		currentPhase = 0;
		currentTurn = 0;
		Initiave.Add(0);
		Initiave.Add(monster.MonsterID);
		DiceController.HasRolledDie += RollFinish;

	}

	void Update() {
		
	}

	public void StartBattle(Monster _monster) {
		player = gameController.GetComponent<GameController>().player;
		monster = _monster;
		duringWrapper.GetComponent<UIBattleDuring>().SetupBattle(player, monster);
		SetupBattle();

		introWrapper.SetActive(true);
		duringWrapper.SetActive(false);

		StartCoroutine(GoToDuringUI());
	}

	IEnumerator GoToDuringUI() {
		yield return new WaitForSeconds(2f);

		introWrapper.SetActive(false);
		duringWrapper.SetActive(true);
		PrepareDice();
	}

	void PrepareDice() {
		duringWrapper.GetComponent<UIBattleDuring>().HideUI();
		string playing;
		int dieNo = 20;
		if (Initiave.IndexOf(currentTurn) == 0) {
			playing = player.Name;
		} else{
			playing = monster.Name;
		}

		duringWrapper.GetComponent<UIBattleDuring>().RoundText.gameObject.SetActive(true);
		duringWrapper.GetComponent<UIBattleDuring>().ActionText.gameObject.SetActive(true);
		if (initiavePhase) {
			duringWrapper.GetComponent<UIBattleDuring>().RoundText.text = "Initiative";
			duringWrapper.GetComponent<UIBattleDuring>().ActionText.text = playing + " goes for initiative";
			if (playing == player.Name) {
				//StartCoroutine(ShowRollText());
			}
		} else {
			if (currentPhase == 0) {
				duringWrapper.GetComponent<UIBattleDuring>().RoundText.text = "Hit Roll";
				duringWrapper.GetComponent<UIBattleDuring>().ActionText.text = playing + " goes for hit";
			} else if (currentPhase == 1) {
				duringWrapper.GetComponent<UIBattleDuring>().RoundText.text = "Damage Roll";
				duringWrapper.GetComponent<UIBattleDuring>().ActionText.text = playing + " goes for damage";

				if (playing == player.Name) {
					int wea = player.Weapons[0].ID;
					dieNo = gameController.GetComponent<WeaponDatabase>().GetDieByWeaponId(wea);
				} else {
					int wea = monster.WeaponID;
					dieNo = gameController.GetComponent<WeaponDatabase>().GetDieByWeaponId(wea);
				}
			}
		}
		gameController.GetComponent<DiceController>().SetDieReady(playing, dieNo);

	}

	IEnumerator ShowRollText() {
		yield return new WaitForSeconds(2f);
		duringWrapper.GetComponent<UIBattleDuring>().TouchPanel.SetActive(true);
		duringWrapper.GetComponent<UIBattleDuring>().TouchPanel.GetComponentInChildren<Text>().text = "Tap to Roll";
	}

	void RollFinish(int sum) {
		//DiceController.HasRolledDie -= RollFinish;

		string whoHit;
		if (Initiave.IndexOf(currentTurn) != 0) {
			whoHit = gameController.GetComponent<NpcDialogue>().dia.Monsters[gameController.GetComponent<NpcDialogue>().dia.GetIndexByMonsterId(Initiave.IndexOf(currentTurn))].Name;
		} else {
			whoHit = player.Name;
		}

		int defenderAC;
		if (whoHit == player.Name) {
			defenderAC = monster.ArmourClass;
		} else {
			defenderAC = player.ArmourClass;
		}

		if (initiavePhase) {
			InitiaveRolls.Add(sum);
			duringWrapper.GetComponent<UIBattleDuring>().AfterHit(whoHit, sum);
			NextTurn();
			if (InitiaveRolls.Count == Initiave.Count) {
				initiavePhase = false;
				if (InitiaveRolls[1] > InitiaveRolls[0]) {
					Debug.Log("Monster starter");
					currentTurn = 1;
				} else {
					Debug.Log("Player starter");
				}
			}
		} else {
			switch (currentPhase) {
			case 0:
				if (sum >= defenderAC) {
					print(whoHit + " hit");
					currentPhase++;
				} else {
					print(whoHit + " missed");
					NextTurn();
				}
				duringWrapper.GetComponent<UIBattleDuring>().AfterHit(whoHit, sum);
				break;
			case 1:
				if (whoHit == player.Name) {
					monster.Life -= sum;
					duringWrapper.GetComponent<UIBattleDuring>().AfterDamage(whoHit, sum, monster.Life);
				} else {
					player.UpdateLife(-sum);
					duringWrapper.GetComponent<UIBattleDuring>().AfterDamage(whoHit, sum, player.Life);
				}
				NextTurn();

				break;
			default:
				Debug.LogError("CurrentPhase out of range");
				break;
			}
		}

		print(whoHit +" rolled " + sum);
		duringWrapper.GetComponent<UIBattleDuring>().TouchPanel.GetComponent<Button>().onClick.AddListener(delegate {
			PrepareDice();
		});
	}

	void NextTurn() {
		int players = Initiave.Count;
		int nextTurn = currentTurn + 1;
		if (nextTurn >= players) {
			currentTurn = 0;
		} else {
			currentTurn = nextTurn;
		}
		currentPhase = 0;

	}
}
