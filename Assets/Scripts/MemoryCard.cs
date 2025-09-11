using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MemoryCard : MonoBehaviour
{
    public string cardText;
    public WordEntry wordEntry;


    public TextMeshProUGUI Kanji;
    public TextMeshProUGUI Kana;
    public TextMeshProUGUI Translation;
    public GameObject front;
    public GameObject back;

    private Button button;
    private MemoryGameController controller;
    private bool isRevealed = false;
    private bool isMatched = false;

    public void Init(WordEntry text, MemoryGameController gameController)
    {
        wordEntry = text;
        cardText = text.kanji;
        Kanji.text = text.kanji;
        Kana.text = text.kana;
        Translation.text = text.translation;
        controller = gameController;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);

        // Start hidden
        front.SetActive(false);
        back.SetActive(true);
    }

    private void OnCardClicked()
    {
        if (controller.inputLocked) return; // block clicks during checking
        if (isMatched || isRevealed) return;

        StartCoroutine(FlipCard(true));
        controller.CardRevealed(this);
    }

    public IEnumerator FlipCard(bool reveal)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 midScale = new Vector3(0f, 1f, 1f);
        Vector3 endScale = Vector3.one;

        // Shrink to middle
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, midScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = midScale;

        // Swap sides
        front.SetActive(reveal);
        back.SetActive(!reveal);

        // Grow back
        time = 0f;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(midScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;

        isRevealed = reveal;
    }

    public void Hide()
    {
        if (!isMatched)
            StartCoroutine(FlipCard(false));
    }

    public void Match()
    {
        isMatched = true;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        button.interactable = false;
    }

    public bool IsMatched() => isMatched;
    public string GetText() => cardText;
}
