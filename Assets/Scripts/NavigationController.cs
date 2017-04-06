using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour {

	public GameObject uiDialogue;
	public GameObject uiBattle;
	public GameObject uiCondition;

	List<GameObject> uis = new List<GameObject>();

	void Awake() {
		Setup();
	}

	public void ShowDialogue() {
		DeactiveAll();
		uiDialogue.SetActive(true);
	}

	public void ShowDice() {
		DeactiveAll();
		uiCondition.SetActive(true);
	}

	public void AfterCondition() {
		uiCondition.GetComponent<UICondition>().AfterBattle.SetActive(true);
		uiCondition.GetComponent<UICondition>().BeforeBattle.SetActive(false);
	}

	public void DuringCondition() {
		uiCondition.GetComponent<UICondition>().AfterBattle.SetActive(false);
		uiCondition.GetComponent<UICondition>().BeforeBattle.SetActive(false);
	}

	public void BeforeCondition() {
		uiCondition.GetComponent<UICondition>().AfterBattle.SetActive(false);
		uiCondition.GetComponent<UICondition>().BeforeBattle.SetActive(true);
		uiCondition.GetComponent<UICondition>().txtTapToRoll.SetActive(false);
		uiCondition.GetComponent<UICondition>().ShowTapToRoll(3);
	}

	public void BeforeBattle(Monster _monster) {
		DeactiveAll();
		uiBattle.SetActive(true);
		uiBattle.GetComponent<BattleController>().StartBattle(_monster);
	}

	void DeactiveAll() {
		foreach(GameObject ui in uis) {
			ui.SetActive(false);
		}
	}

	void Setup() {
		uis.Add(uiDialogue);
		uis.Add(uiBattle);
		uis.Add(uiCondition);
	}

}
