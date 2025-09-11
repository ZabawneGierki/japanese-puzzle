using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System.IO;

public class VnDialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Button nextButton;
    public Button quickSaveButton;
    public Button quickLoadButton;

    private Story story;
    private string quickSavePath;

    void Start()
    {
        // Load ink file
        TextAsset inkJSONAsset = Resources.Load<TextAsset>("Story");
        story = new Story(inkJSONAsset.text);

        quickSavePath = System.IO.Path.Combine(Application.persistentDataPath, "quicksave.json");

        // Load from static state if available
        if (StoryStateManager.HasSavedState)
            StoryStateManager.LoadState(story);

        nextButton.onClick.AddListener(DisplayNextLine);
        quickSaveButton.onClick.AddListener(QuickSave);
        quickLoadButton.onClick.AddListener(QuickLoad);

        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            dialogueText.text = text;
        }
    }

    // --- Quick Save/Load ---
    void QuickSave()
    {
        string saveJson = story.state.ToJson();
        File.WriteAllText(quickSavePath, saveJson);
        Debug.Log("QuickSaved at " + quickSavePath);
    }

    void QuickLoad()
    {
        if (File.Exists(quickSavePath))
        {
            string saveJson = File.ReadAllText(quickSavePath);
            story.state.LoadJson(saveJson);
            DisplayNextLine();
            Debug.Log("QuickLoaded");
        }
    }

    // --- Automatic cross-scene save ---
    void OnDisable()
    {
        StoryStateManager.SaveState(story);
    }
}


