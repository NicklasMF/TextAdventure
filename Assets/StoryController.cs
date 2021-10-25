using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class StoryController : MonoBehaviour
{
    [SerializeField] TextAsset inkStory;
    [SerializeField] TextMeshProUGUI storyTxt;
    [SerializeField] Transform buttonsParent;

    Story story;

    void Awake() {
        StartStory();
    }

    void StartStory() {
        story = new Story(inkStory.text);
        RefreshView();
    }

    void RefreshView() {
        ResetView();

        while(story.canContinue) {
            string text = story.Continue();
            text = text.Trim();
            SetContentView(text);
        }

        if (story.currentChoices.Count > 0) {
            for (int i = 0; i < story.currentChoices.Count; i++) {
                SetChoice(i);
            }
        } else {
            Debug.Log("We are done");
        }
    }

    void ResetView() {
        foreach (Transform button in buttonsParent) {
            button.gameObject.SetActive(false);
        }
    }

    void SetContentView(string text) {
        storyTxt.text = text;
    }

    void SetChoice(int i) {
        Choice choice = story.currentChoices[i];
        Transform button = buttonsParent.GetChild(i);
        button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
        button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            story.ChooseChoiceIndex(choice.index);
            RefreshView();
        });
        button.gameObject.SetActive(true);
    }
}
