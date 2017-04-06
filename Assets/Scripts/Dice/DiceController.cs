using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour {

	public delegate void HasRolled(int sum);
	public static event HasRolled HasRolledDie; 

    public LayerMask dieValueColliderLayer;
	[SerializeField] GameObject die6PlayerPrefab;
	[SerializeField] GameObject die20PlayerPrefab;
	[SerializeField] GameObject die6MonsterPrefab;
	[SerializeField] GameObject die20MonsterPrefab;
    [SerializeField] Transform dieStartPosition;
    float forceAmount = 15f;
    float angularForce = 30f;

	[HideInInspector] public int currentValue = 0;

	string currentTurn;
	bool canRoll = false;
	bool hasRolled = false;
	bool dieStill = false;
    bool rollComplete = false;
	Vector3 rotationPoint;
	GameObject die;

	public Text txtStatus;
	Vector3 velocity = Vector3.zero;

    void Update() {

		if (!die || rollComplete) {
			return;
		}

		if (canRoll && !hasRolled) {
			if (currentTurn == GetComponent<GameController>().player.Name && Input.GetButtonDown("Fire1")) {
				RollDie();
			} else {
				die.GetComponent<Rigidbody>().isKinematic = true;
				die.GetComponent<Rigidbody>().useGravity = false;
				die.transform.Rotate(Vector3.up * Time.deltaTime * 80);
				die.transform.Rotate(rotationPoint * Time.deltaTime * 50);
			}
		}

		if (dieStill) {
			RaycastHit hit;
			if (Physics.Raycast(die.transform.position, Vector3.up, out hit, Mathf.Infinity, dieValueColliderLayer)) {
				currentValue = hit.collider.GetComponent<DieValue>().value;
			}
		}

		if (hasRolled) {
			Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, die.transform.position + new Vector3(0, 16f, 0), ref velocity, 0.7f);

			if (currentValue != 0 && dieStill) {
				HasRolledDie(currentValue);
			}
			if (!dieStill && die.GetComponent<Rigidbody>().IsSleeping()) {
				dieStill = true;
			}
		} else {
			Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, new Vector3(0, 16f, 0), ref velocity, 1.5f);
		}
    }

	public void SetDieReady(string person, int dieNo) {
		
		currentTurn = person;
		//Camera.main.transform.position = new Vector3(0, 40f, 0);

		if (die == null) {
			die = (GameObject) Instantiate(die20PlayerPrefab);
		}

		if (currentTurn == GetComponent<GameController>().player.Name) {
			Destroy(die);
			die = (GameObject) Instantiate(die20PlayerPrefab);
		} else {
			Destroy(die);
			die = (GameObject) Instantiate(die20MonsterPrefab);
			StartCoroutine(MonsterRollDie());
		}

		die.transform.position = dieStartPosition.position;
		rotationPoint = (Random.Range(0,1) == 0) ? Vector3.left : Vector3.right;
        canRoll = true;
		hasRolled = false;
		dieStill = false;
		rollComplete = false;
		HasRolledDie += RollFinish;
    }

	void RollFinish(int sum) {
		rollComplete = true;
	}

	IEnumerator MonsterRollDie() {
		float waitingTime = Random.Range(2.0f, 4.0f);
		yield return new WaitForSeconds(waitingTime);

		RollDie();
	}

	void RollDie() {
		hasRolled = true;
		die.GetComponent<Rigidbody>().isKinematic = false;
		die.GetComponent<Rigidbody>().useGravity = true;
		die.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 3f, ForceMode.VelocityChange);
		die.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(forceAmount - 3f, forceAmount + 3f), ForceMode.VelocityChange);
		die.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * Random.Range(angularForce - 3f, angularForce + 13f), ForceMode.VelocityChange);
		//GetComponent<NavigationController>().DuringCondition();
	}
}
