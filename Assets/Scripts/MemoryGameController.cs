using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemoryGameController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform gridParent;
    public TextMeshProUGUI winTextJP;
    public TextMeshProUGUI winTextEN;



    public VocabularyLoader loader;

    private List<MemoryCard> revealedCards = new List<MemoryCard>();
    private List<MemoryCard> allCards = new List<MemoryCard>();

    public bool inputLocked = false; // <--- NEW


    void Start()
    {
         
        SetupGame();
    }

    void SetupGame()
    {
        WordEntry[] words = loader.GetWords(8).ToArray();
        List<WordEntry> deck = new List<WordEntry>();

        foreach (WordEntry w in words)
        {
            deck.Add(w);
            deck.Add(w); // pair
        }

        // Shuffle
        for (int i = 0; i < deck.Count; i++)
        {
            WordEntry temp = deck[i];
            int rand = Random.Range(i, deck.Count);
            deck[i] = deck[rand];
            deck[rand] = temp;
        }

        // Create cards
        foreach (WordEntry text in deck)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridParent);
            MemoryCard card = cardObj.GetComponent<MemoryCard>();
            card.Init(text, this);
            allCards.Add(card);
        }
    }

    public void CardRevealed(MemoryCard card)
    {
        if (inputLocked) return; // block clicks during checking
        revealedCards.Add(card);

        if (revealedCards.Count == 2)
        {
            inputLocked = true;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (revealedCards[0].GetText() == revealedCards[1].GetText())
        {
            revealedCards[0].Match();
            revealedCards[1].Match();
            winTextJP.text = revealedCards[0].wordEntry.jp;
            winTextEN.text = revealedCards[0].wordEntry.en;

        }
        else
        {
            revealedCards[0].Hide();
            revealedCards[1].Hide();
        }

        revealedCards.Clear();
        inputLocked = false;
        CheckWin();
    }

    void CheckWin()
    {
        foreach (var card in allCards)
        {
            if (!card.IsMatched()) return;
        }

        
         
    }
}
