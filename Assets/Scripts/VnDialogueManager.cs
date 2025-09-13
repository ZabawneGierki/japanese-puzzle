using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class VNDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public Image backgroundImage;
    public Image characterImage;

    public Button nextButton;
    public Button saveButton;
    public Button loadButton;

    private Story story;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine;

    [Header("Typing Effect")]
    public float typingSpeed = 0.03f; // seconds between letters

    private const string SaveKeyStory = "VNQuickSave_Story";
    private const string SaveKeyBG = "VNQuickSave_BG";
    private const string SaveKeySprite = "VNQuickSave_Sprite";

    // Track current visuals
    private string currentBackground = "";
    private string currentSprite = "";


    private string quickSavePath;

    void Start()
    {
        // Load Ink JSON compiled by Unity Ink plugin
        TextAsset inkJSON = Resources.Load<TextAsset>("Story/myStory");
        story = new Story(inkJSON.text);

        // Register external function for scene switching
        story.BindExternalFunction("LoadScene", (string sceneName) => {
            // Save the current story state before switching scenes
            StoryStateManager.SaveState(story);

            // Load the requested scene
            SceneManager.LoadScene(sceneName);
        });

        nextButton.onClick.AddListener(OnNextPressed);
        saveButton.onClick.AddListener(SaveGame);
        loadButton.onClick.AddListener(LoadGame);

        // Define quick save path
        quickSavePath = System.IO.Path.Combine(Application.persistentDataPath, "quicksave.json");

        // Load from static state if available
        if (StoryStateManager.HasSavedState)
            StoryStateManager.LoadState(story);
        
        NextLine();
    }

    // --- Automatic cross-scene save ---
    void OnDisable()
    {
        StoryStateManager.SaveState(story);
    }

    void OnNextPressed()
    {
        if (isTyping)
        {
            // Skip typing → instantly show full line
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentFullLine;
            isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    void NextLine()
    {
        if (story.canContinue)
        {
            string line = story.Continue().Trim();

            ParseDialogueLine(line);

            foreach (string tag in story.currentTags)
            {
                HandleTag(tag);
            }
        }
        else
        {
            dialogueText.text = "";
            speakerText.text = "";
        }
    }

    void ParseDialogueLine(string line)
    {
        string speaker = "";
        string text = line;

        if (line.Contains(":"))
        {
            string[] parts = line.Split(new char[] { ':' }, 2);
            speaker = parts[0];
            text = parts[1].Trim();
        }

        speakerText.text = speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentFullLine = text;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void HandleTag(string tag)
    {
        if (tag.StartsWith("scene"))
        {
            string bg = tag.Split(' ')[1];
            currentBackground = bg;
            backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + bg);
        }
        else if (tag.StartsWith("sprite"))
        {
            string[] parts = tag.Split(' ');
            string character = parts[1];
            string expression = parts[2];
            currentSprite = character + "_" + expression;
            characterImage.sprite = Resources.Load<Sprite>("Characters/" + currentSprite);
        }
    }

    // --- Save / Load ---
    void SaveGame()
    {
        string savedState = story.state.ToJson();
        PlayerPrefs.SetString(SaveKeyStory, savedState);
        PlayerPrefs.SetString(SaveKeyBG, currentBackground);
        PlayerPrefs.SetString(SaveKeySprite, currentSprite);
        PlayerPrefs.Save();
        Debug.Log("Game saved.");
    }

    void LoadGame()
    {
        if (PlayerPrefs.HasKey(SaveKeyStory))
        {
            string savedState = PlayerPrefs.GetString(SaveKeyStory);
            story.state.LoadJson(savedState);

            currentBackground = PlayerPrefs.GetString(SaveKeyBG, "");
            currentSprite = PlayerPrefs.GetString(SaveKeySprite, "");

            if (!string.IsNullOrEmpty(currentBackground))
            {
                backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + currentBackground);
            }
            if (!string.IsNullOrEmpty(currentSprite))
            {
                characterImage.sprite = Resources.Load<Sprite>("Characters/" + currentSprite);
            }

            Debug.Log("Game loaded.");
            NextLine();
        }
    }
}
