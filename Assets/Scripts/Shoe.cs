using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoe : MonoBehaviour
{

    private enum Suit
    {
        CLUBS,
        DIAMONDS,
        HEARTS,
        SPADES
    }

    private enum Rank
    {
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING
    }

    Suit suit;
    Rank rank;

    const int MAXDECKS = 8;
    const int CARDSINDECK = 52;

    private int numOfDecks = 3, numOfCards;
    private List<string> shoeCards;
    private string cardString;

    public void FillTheShoe()
    {
        // Get the chosen num of decks for the new game from the slider
        numOfDecks = int.Parse(GameObject.Find("DeckCountText").GetComponent<Text>().text);

        numOfCards = numOfDecks * CARDSINDECK;

        shoeCards = new List<string>(numOfCards);

        // Each time through the loop creates 52 strings representing one deck of cards
        for (int i = 0; i < numOfDecks; i++)
        {
            // Create 52 cards in a deck as strings of rank and suit
            for (suit = Suit.CLUBS; suit <= Suit.SPADES; suit++)
            {
                for (rank = Rank.ACE; rank <= Rank.KING; rank++)
                {
                    cardString = rank + " of " + suit;

                    shoeCards.Add(cardString);
                }
            }
        }

        Shuffle();

        return;
    }

    // Returns the next card object on the deck
    public string Deal()
    {
        string nextCard = shoeCards[0];

        //If no cards are left in the shoe, repopulate and shuffle
        if (shoeCards.Count == 0)
        {
            FillTheShoe();
            Shuffle();
        }

        // Remove the dealt card from the list
        shoeCards.RemoveAt(0);
        
        return nextCard;
    }

    public void Shuffle()
    {
        string tempCard;
        int randomCard, i;

        // Seed the random generator using current time
        Random.InitState((int)System.DateTime.Now.Ticks); 

        //  Go through each card, and swap it with a random card
        for (i = 0; i <= numOfCards - 1; i++)
        {
            randomCard = Random.Range(1, numOfCards);                                          

            tempCard = shoeCards[i];
            shoeCards[i] = shoeCards[randomCard];
            shoeCards[randomCard] = tempCard;
        }

        return;
    }
}