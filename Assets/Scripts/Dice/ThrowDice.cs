using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour {

    public bool canRoll = false;

    public LayerMask dieValueColliderLayer;
    [SerializeField] GameObject diePrefab;
    [SerializeField] Transform dieStartPosition;
    float forceAmount = 10f;
    float angularForce = 10f;

    public int currentValue = 1;
    bool rollComplete = false;
    GameObject die;

    void Update() {

        if (canRoll) {
            if (Input.GetButtonDown("Fire1")) {
                die.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * forceAmount, ForceMode.VelocityChange);
                die.GetComponent<Rigidbody>().AddForce(Vector3.up * Random.Range(forceAmount - 3f, forceAmount + 3f), ForceMode.VelocityChange);
                die.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * Random.Range(angularForce - 3f, angularForce + 13f), ForceMode.VelocityChange);
            }

            RaycastHit hit;
            if (Physics.Raycast(die.transform.position, Vector3.up, out hit, Mathf.Infinity, dieValueColliderLayer)) {
                currentValue = hit.collider.GetComponent<DieValue>().value;
            }

            Debug.DrawRay(die.transform.position, Vector3.up);

            if (die.GetComponent<Rigidbody>().IsSleeping() && !rollComplete) {
                rollComplete = true;
                print(currentValue);
            } else if (!die.GetComponent<Rigidbody>().IsSleeping()) {
                rollComplete = false;
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Return)) {
                SetDieReady();
            }
        }

    }

    public void SetDieReady() {
        if (die == null) {
            die = (GameObject) Instantiate(diePrefab);
        }
        die.transform.position = dieStartPosition.position;

        canRoll = true;
    }

}
