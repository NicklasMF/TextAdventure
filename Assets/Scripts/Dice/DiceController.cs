using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceController : MonoBehaviour {

	public delegate void HasRolled(int sum);
	public static event HasRolled HasRolledDie;

    public LayerMask dieValueColliderLayer;
    [SerializeField] GameObject diePrefab;
    [SerializeField] Transform dieStartPosition;
    float forceAmount = 15f;
    float angularForce = 10f;

	[HideInInspector] public int currentValue = 0;
	bool canRoll = false;
	bool hasRolled = false;
	bool dieStill = false;
    bool rollComplete = false;
    GameObject die;


	public GameObject uiTouchPanel;
	public Text txtStatus;

    void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			SetDieReady();
		}

		if (!die || rollComplete) {
			return;
		}
		if (canRoll && !hasRolled) {
            if (Input.GetButtonDown("Fire1")) {
				hasRolled = true;
                die.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * 3f, ForceMode.VelocityChange);
                die.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(forceAmount - 3f, forceAmount + 3f), ForceMode.VelocityChange);
                die.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * Random.Range(angularForce - 3f, angularForce + 13f), ForceMode.VelocityChange);
            }
		}

		if (dieStill) {
			RaycastHit hit;
			if (Physics.Raycast(die.transform.position, Vector3.up, out hit, Mathf.Infinity, dieValueColliderLayer)) {
				currentValue = hit.collider.GetComponent<DieValue>().value;
			}
		}

		if (hasRolled) {
			if (currentValue != 0 && dieStill) {
				HasRolledDie(currentValue);
				uiTouchPanel.SetActive(true);
				rollComplete = true;
			}
			if (!dieStill && die.GetComponent<Rigidbody>().IsSleeping()) {
				dieStill = true;
			}
		}

    }

    public void SetDieReady() {
        if (die == null) {
            die = (GameObject) Instantiate(diePrefab);
        }
		uiTouchPanel.SetActive(false);

        die.transform.position = dieStartPosition.position;

        canRoll = true;
		hasRolled = false;
		dieStill = false;
		rollComplete = false;
    }
}
