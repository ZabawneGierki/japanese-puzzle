using UnityEngine;
using Ink.Runtime;

public static class StoryStateManager
{
    private static string savedStoryJson;

    public static void SaveState(Story story)
    {
        savedStoryJson = story.state.ToJson();
    }

    public static void LoadState(Story story)
    {
        if (!string.IsNullOrEmpty(savedStoryJson))
        {
            story.state.LoadJson(savedStoryJson);
        }
    }

    public static bool HasSavedState => !string.IsNullOrEmpty(savedStoryJson);
}
