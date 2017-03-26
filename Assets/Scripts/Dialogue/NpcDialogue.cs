using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogue : MonoBehaviour {

    Dialogue dia;

    GameObject dialogue_window;

    GameObject node_text;
    GameObject option_1;
    GameObject option_2;
    GameObject option_3;
    GameObject exit;

    int selected_option = -2;

    public string DialogueDataFilePath;
    public GameObject DialogueWindowPrefab;

    void Start() {
        dia = Dialogue.LoadDialogue("Assets/" + DialogueDataFilePath);

        var canvas = GameObject.Find("Canvas");

        dialogue_window = Instantiate<GameObject>(DialogueWindowPrefab);
        dialogue_window.transform.SetParent(canvas.transform, false);
        RectTransform dia_windows_transform = (RectTransform)dialogue_window.transform;
        dia_windows_transform.localPosition = new Vector3(0, 0, 0);

        node_text = GameObject.Find("Txt_DiaNodeText");
        option_1 = GameObject.Find("Button_Option1");
        option_2 = GameObject.Find("Button_Option2");
        option_3 = GameObject.Find("Button_Option3");
        exit = GameObject.Find("Button_End");

        RunDialogue(0);
    }

    public void RunDialogue(int _nodeid) {
        StartCoroutine(run(_nodeid));
    }

    void SetSelectedOption(int x) {
        selected_option = x;
    }

    IEnumerator run(int _nodeid) {
        int node_id = _nodeid;

        while (node_id != -1) {
            display_node(dia.Nodes[node_id]);

            selected_option = -2;
            while (selected_option == -2) {
                yield return new WaitForSeconds(0.25f);
            }

            node_id = selected_option;
        }

        dialogue_window.SetActive(false);
    }

    void display_node(DialogueNode node) {
        node_text.GetComponent<Text>().text = node.Text;

        option_1.SetActive(false);
        option_2.SetActive(false);
        option_3.SetActive(false);

        for(int i = 0; i < node.Options.Count; i++) {
            switch(i) {
                case 0:
                set_option_button(option_1, node.Options[i]);
                break;
                case 1:
                set_option_button(option_2, node.Options[i]);
                break;
                case 2:
                set_option_button(option_3, node.Options[i]);
                break;

            }
        }
    }

    void set_option_button(GameObject button, DialogueOption opt) {
        button.SetActive(true);
        button.GetComponentInChildren<Text>().text = opt.Text;
        button.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedOption(opt.DestinationNodeID); });
    }
}
