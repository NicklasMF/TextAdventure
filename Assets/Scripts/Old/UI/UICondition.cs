using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICondition : MonoBehaviour {

	public GameObject BeforeBattle;
	public GameObject AfterBattle;

	public GameObject txtTapToRoll;
	public Text txtHeader;
	public Text txtWinCondition;
	public Text txtSum;
	public GameObject ContinuePanel;

	public void ShowTapToRoll(int sec) {
		StartCoroutine(TapToRoll(sec));
	}

	IEnumerator TapToRoll(int sec) {
		yield return new WaitForSeconds(sec);

		txtTapToRoll.SetActive(true);
	}

}
