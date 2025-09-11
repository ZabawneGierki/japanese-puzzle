using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class VNDialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public Image backgroundImage;
    public Image characterImage;

    public Button nextButton;

    private Story story;

    void Start()
    {
        // Load Ink JSON compiled by Unity Ink plugin
        TextAsset inkJSON = Resources.Load<TextAsset>("Story/myStory");
        story = new Story(inkJSON.text);

        nextButton.onClick.AddListener(NextLine);

        NextLine();
    }

    void NextLine()
    {
        if (story.canContinue)
        {
            // Get next line
            string line = story.Continue().Trim();

            // Show dialogue
            ParseDialogueLine(line);

            // Handle tags (like "# sprite Alice happy")
            foreach (string tag in story.currentTags)
            {
                HandleTag(tag);
            }
        }
        else
        {
            // End of story
            dialogueText.text = "";
            speakerText.text = "";
        }
    }

    void ParseDialogueLine(string line)
    {
        // Check if line has "Name: Text"
        if (line.Contains(":"))
        {
            string[] parts = line.Split(new char[] { ':' }, 2);
            speakerText.text = parts[0];
            dialogueText.text = parts[1].Trim();
        }
        else
        {
            // Narration (no speaker)
            speakerText.text = "";
            dialogueText.text = line;
        }
    }

    void HandleTag(string tag)
    {
        // Example: "scene classroom_day"
        if (tag.StartsWith("scene"))
        {
            string bg = tag.Split(' ')[1];
            backgroundImage.sprite = Resources.Load<Sprite>("Backgrounds/" + bg);
        }
        // Example: "sprite Alice happy"
        else if (tag.StartsWith("sprite"))
        {
            string[] parts = tag.Split(' ');
            string character = parts[1];
            string expression = parts[2];
            characterImage.sprite = Resources.Load<Sprite>("Characters/" + character + "_" + expression);
        }
    }
}
