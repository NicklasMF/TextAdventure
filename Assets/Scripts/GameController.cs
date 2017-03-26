using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	void Awake() {
        SetDefaultPlayerPrefs();
    }


    void SetDefaultPlayerPrefs() {
        if (!PlayerPrefs.HasKey("Player_Level")) {
            PlayerPrefs.SetString("Player_Name", "Jamison");

            PlayerPrefs.SetInt("Player_Level", 1);



        }
    }
}
