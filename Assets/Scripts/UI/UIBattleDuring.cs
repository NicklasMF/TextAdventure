using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleDuring : MonoBehaviour {

	[SerializeField] Text playerName;
	[SerializeField] Text monsterName;
	[SerializeField] Text playerAC;
	[SerializeField] Text monsterAC;
	[SerializeField] Image playerbackground;
	[SerializeField] Image monsterbackground;
	[SerializeField] GameObject playerLifeWrapper;
	[SerializeField] GameObject monsterLifeWrapper;
	[SerializeField] GameObject lifePrefab;

	[SerializeField] Text WhoHitText;
	[SerializeField] Text HitSum;
	public GameObject TouchPanel;
	public Text RoundText;
	public Text ActionText;


	GameObject playerPreviousLife;
	GameObject monsterPreviousLife;
	[SerializeField] Font crossFont;

	Monster monster;
	Player player;

	public void SetupBattle(Player _player, Monster _monster) {
		player = _player;
		monster = _monster;
		monsterName.text = monster.Name;
		playerName.text = _player.Name;
		playerAC.text = _player.ArmourClass.ToString();
		monsterAC.text = monster.ArmourClass.ToString();
		AddNewLife(player.Name, _player.Life);
		AddNewLife(monster.Name, monster.Life);

	}

	public void AfterHit(string _who, int _sum) {
		ShowAfterRoll();
		HitSum.text = _sum.ToString();
		TouchPanel.SetActive(true);
		WhoHitText.text = _who + " hit";
	}

	public void AfterDamage(string _who, int _sum, int _newLife) {
		HitSum.text = _sum.ToString();
		TouchPanel.SetActive(true);
		WhoHitText.text = _who + " gives damage of " + _sum;
		AddNewLife(_who, _newLife);
	}

	void ShowAfterRoll() {
		TouchPanel.GetComponent<Button>().onClick.RemoveAllListeners();
		WhoHitText.gameObject.SetActive(true);
		HitSum.gameObject.SetActive(true);
		RoundText.gameObject.SetActive(false);
		ActionText.gameObject.SetActive(false);
	}

	public void HideUI() {
		WhoHitText.gameObject.SetActive(false);
		HitSum.gameObject.SetActive(false);
		TouchPanel.SetActive(false);
	}

	void AddNewLife(string _who, int sum) {
		if (_who != player.Name) {
			if (playerPreviousLife != null) {
				playerPreviousLife.GetComponent<BattleLife>().life.font = crossFont;
			}

			GameObject _life1 = (GameObject) Instantiate(lifePrefab, playerLifeWrapper.transform);
			_life1.GetComponent<BattleLife>().life.text = sum.ToString();
			playerPreviousLife = _life1;
		} else {
			if (monsterPreviousLife != null) {
				monsterPreviousLife.GetComponent<BattleLife>().life.font = crossFont;
			}
			GameObject _life2 = (GameObject) Instantiate(lifePrefab, monsterLifeWrapper.transform);
			_life2.GetComponent<BattleLife>().life.text = sum.ToString();
			monsterPreviousLife = _life2;

		}
	}

}
