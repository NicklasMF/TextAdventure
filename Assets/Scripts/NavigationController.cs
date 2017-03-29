using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationController : MonoBehaviour {

	public GameObject uiDialogue;
	public GameObject uiBattle;

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
		uiBattle.SetActive(true);
	}

	void DeactiveAll() {
		foreach(GameObject ui in uis) {
			ui.SetActive(false);
		}
	}

	void Setup() {
		uis.Add(uiDialogue);
		uis.Add(uiBattle);
	}

}
