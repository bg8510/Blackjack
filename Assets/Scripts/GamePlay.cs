using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour
{

    const int CARDOFFSET = 50;
    const float PLAYERROW = 109;
    const float DEALERROW = 450;
    const float STARTINGCOLUMN = 190;
    const float INITIALZOFFSET = 20;

    int wins, losses;
    int numOfDecks;
    string currentResult;

    public List<string> playerHand = new List<string>();
    public List<string> dealerHand = new List<string>();

    string dealtCard;
    private Shoe gameShoe;
    private List<GameObject> gameCards = new List<GameObject>();
    private bool SkipDealersTurn;

    public GameObject DeckCountText;
    public Button HoldButton;
    public Button HitButton;
    public Slider DeckSlider;
    public Button DealButton;
    public Text PlayerWins;
    public Text DealerWins;
    public Text ResultText;
    public Text GameInstructions;
    public Button WildcardOne;
    public Button WildcardTwo;
    public Button WildcardThree;
    List<Button> wildcards = new List<Button>();

    // For positioning cards on the screen:
    float xOffset;
    float PlayerXOffset;
    float DealerXOffset;
    float yOffset;
    float zOffset;
    float smallShift = 10;

    string[] winText = new string[] { "You Win!", "You have great skills!", "Hooray for you!", "Victory is yours!", "Hire the man who wrote this code!" };
    string[] loseText = new string[] { "You Lose", "You Stink", "You FAIL", "You're making me sad.", "Your incompetence makes me weep and moan" };

    // Use this for initialization
    void Start () {

        // Reset statistics to zero
        wins = 0;
        losses = 0;

        numOfDecks = int.Parse(GameObject.Find("DeckCountText").GetComponent<Text>().text);

        gameShoe = new Shoe();
        gameShoe.FillTheShoe();
        HitButton.interactable = false;
        HoldButton.interactable = false;
        GameInstructions.enabled = true;

        wildcards.Add(WildcardOne);
        wildcards.Add(WildcardTwo);
        wildcards.Add(WildcardThree);
    }

    // DEAL NEW HAND
    public void DealNewHand()
    {
        DealButton.interactable = false;

        // If the numOfDecks selection has changed, start a new game with the new value
        if (numOfDecks != int.Parse(GameObject.Find("DeckCountText").GetComponent<Text>().text))
        {
            wins = 0;
            losses = 0;
            EndOfRound();
            Start();
        }

        HitButton.interactable = true;
        HoldButton.interactable = true;
        DeckSlider.interactable = false;
        StartCoroutine("FadeText");

        // This alternates the dealer's hand position to not go directly on top of
        // the previous hand, to give a visual clue that it's a new hand
        smallShift = -smallShift;

        ClearCardsFromScreen();

        // Reset the onscreen position for cards
        PlayerXOffset = STARTINGCOLUMN;
        DealerXOffset = STARTINGCOLUMN + smallShift;
        zOffset = INITIALZOFFSET;

        playerHand.Clear();
        dealerHand.Clear();
        ResultText.text = "";

        // Deal the initial 2 cards to each player
        playerHand.Add(DealCard("Player"));
        dealerHand.Add(DealCard("Dealer"));
        playerHand.Add(DealCard("Player"));
        dealerHand.Add(DealCard("Dealer", false));

        foreach (Button button in wildcards)
        {
            button.interactable = true;
        }

        return;
    }

	// PLAYER HITS method
	public void PlayerHits()
	{
        // Deal a card for the player
        dealtCard = DealCard("Player");

        // If the deal was successful, add the card to player's hand
        if (dealtCard != null)
        {
            playerHand.Add(dealtCard);
        }

        if (CheckForPlayerWin())
        {
            SkipDealersTurn = true;
            EndOfRound();
        }
        else
        {
            SkipDealersTurn = false;
        }
        
        return;
	}

    // DEALERS Turn begins when the player holds
    public void DealerHits()
    {
        FlipFaceDownCards();

        // Run this loop until CheckForWin() finds a result
        do
        {
            HitButton.interactable = false;
            HoldButton.interactable = false;

            dealtCard = DealCard("Dealer");

            // If the deal was successful, add the card to dealer's hand
            if (dealtCard != null)
                dealerHand.Add(dealtCard);
        }
        while (!CheckForWin());

        EndOfRound();
    }


    // Check the score and finish the game, if necessary
    private bool CheckForWin()
    {
        int playerScore = CalculateHand(playerHand);
        int dealerScore = CalculateHand(dealerHand);
        int randomIndex = Random.Range(0, 5);

        if (playerScore > 21)   // Player busted
        {
            losses++;
            currentResult = "Win";
            ResultText.text = loseText[randomIndex];
            return true;
        }

        if (dealerScore > 21)   // Dealer busted
        {
            wins++;
            currentResult = "Win";
            ResultText.text = winText[randomIndex];
            return true;
        }
        else if (dealerScore >= 17)  // Dealer is finished
        {
            if (playerScore == dealerScore)
            {
                ResultText.text = "Push! (It's a tie) Try again.";
                return true;
            }
            else if (playerScore > dealerScore)
            {
                wins++;
                currentResult = "Win";
                ResultText.text = winText[randomIndex];
                return true;
            }
            else
            {
                losses++;
                currentResult = "Lose";
                ResultText.text = loseText[randomIndex];
                return true;
            }
        }
        else if (dealerScore > playerScore)
        {
            losses++;
            currentResult = "Lose";
            ResultText.text = loseText[randomIndex];
            return true;
        }

        return false;
    }

    // Check the score and finish the game, if necessary
    private bool CheckForPlayerWin()
    {
        int playerScore = CalculateHand(playerHand);
        int randomIndex = Random.Range(0, 5);

        if (playerScore > 21)   // Player busted
        {
            losses++;
            currentResult = "Lose";
            ResultText.text = "Busted!";
            FlipFaceDownCards();
            return true;
        }
        else if (playerHand.Count >= 5)
        {
            wins++;
            currentResult = "Win";
            ResultText.text = winText[randomIndex] + " (You win with 5 cards under 21)";
            FlipFaceDownCards();
            return true;
        }

        return false;
    }

    private void EndOfRound()
    {
        PlayerWins.text = wins.ToString();
        DealerWins.text = losses.ToString();

        HitButton.interactable = false;
        HoldButton.interactable = false;
        DealButton.interactable = true;
        DeckSlider.interactable = true;
        GameInstructions.enabled = false;
    }

    // CALCULATE HAND method
    private int CalculateHand(List<string> currentHand)
    {
        int currentPoints = 0, numOfAces = 0, start = 0, i;

        for (i = start; i < currentHand.Count; i++)
        {
            // Add to the score based on the first 3 chars of each card string
            // Aces are handled separately below
            switch (currentHand[i].Substring(0, 3))
            {
                case "ACE":
                    numOfAces++;
                    break;

                case "TWO":
                    currentPoints = currentPoints + 2;
                    break;

                case "THR":
                    currentPoints = currentPoints + 3;
                    break;

                case "FOU":
                    currentPoints = currentPoints + 4;
                    break;

                case "FIV":
                    currentPoints = currentPoints + 5;
                    break;

                case "SIX":
                    currentPoints = currentPoints + 6;
                    break;

                case "SEV":
                    currentPoints = currentPoints + 7;
                    break;

                case "EIG":
                    currentPoints = currentPoints + 8;
                    break;

                case "NIN":
                    currentPoints = currentPoints + 9;
                    break;

                case "TEN":
                case "JAC":
                case "QUE":
                case "KIN":
                    currentPoints = currentPoints + 10;
                    break;

                default:
                    break;

            }
        }

        // Add 1 point for each Ace; currentPoints is now the minimum possible score
        currentPoints = currentPoints + numOfAces;

        if (currentPoints > 21)
            return currentPoints;

        // Add 10 more points for each Ace until you can't anymore
        for (i = 1; i <= numOfAces; i++)
        {
            if (currentPoints + 10 <= 21)
                currentPoints = currentPoints + 10;
        }

        // If player gets 5 cards without busting, send back a negative score
        // The calling function must know how to handle the negative
        //if (currentHand.Count == 5 && currentPoints <= 21)
        //    newScore = -1 * newScore;

        return currentPoints;
    }

    private string DealCard(string whichPlayer, bool faceUp = true)
    {
        // Set the X and Y based on whether it's the Dealer's or Player's
        // card, and increment X accordingly for the next deal
        if (whichPlayer.ToUpper().StartsWith("D"))
        {
            xOffset = DealerXOffset + UnityEngine.Random.Range(-6, 6);     // Add some randomness for more realistic card placement;;
            yOffset = DEALERROW + UnityEngine.Random.Range(-6, 6);
            DealerXOffset += CARDOFFSET;
        }
        else
        {
            xOffset = PlayerXOffset + UnityEngine.Random.Range(-6, 6);     // Add some randomness for more realistic card placement;
            yOffset = PLAYERROW + UnityEngine.Random.Range(-6, 6);
            PlayerXOffset += CARDOFFSET;
        }

        zOffset -= 1;

        Vector3 position = new Vector3(xOffset, yOffset, zOffset);

        string nextCard = gameShoe.Deal();

        // Place the card image, either the card corresponding to the name, or the card back object
        GameObject cardImage = Instantiate(faceUp ? GameObject.Find(nextCard.ToString()) : GameObject.Find("cardBack"), position, Quaternion.identity);

        gameCards.Add(cardImage);

        return nextCard;
    }

    private void ClearCardsFromScreen()
    {
        // The try-catch is here in case no clones have been created - as when the game is first run
        try
        {
            foreach (GameObject card in gameCards)
            {
                Destroy(card);
            }
        }
        catch
        {
            return;
        }

        return;
    }

    private void FlipFaceDownCards()
    {
        DealerXOffset -= CARDOFFSET;    // Move back in the X direction to place the new card approx where the cardBack was

        Vector3 position = new Vector3(DealerXOffset, DEALERROW, zOffset);

        foreach (GameObject card in gameCards)
        {
            if (card.ToString().Contains("cardBack"))
            {
                Destroy(card);
            }
        }

        // Create the face up card GameObject and add it to the gameCards List
        gameCards.Add(Instantiate(GameObject.Find(dealerHand[1]), position, Quaternion.identity));
        
        DealerXOffset += CARDOFFSET;
  
    }

    public void OnTokenClick()
    {
        // This button is only useable after a loss
        if (currentResult == "Win")
        {
            return;
        }

        string lastCard = playerHand[playerHand.Count - 1];

        // Set this button to inactive, which will show the "X" behind it
        EventSystem.current.currentSelectedGameObject.SetActive(false);

        foreach (Button button in wildcards)
        {
            button.interactable = false;
        }

        foreach (GameObject card in gameCards)
        {
            if (card.ToString().Contains(lastCard))
            {
                PlayerXOffset = card.transform.position.x;  // Capture the X position of the card to use for the new card
                Destroy(card);
            }
        }

        // Remove the Dealer's win from the scoreboard
        losses--;
        DealerWins.text = losses.ToString();

        // Remove the last card from the List of card strings
        playerHand.RemoveAt(playerHand.Count-1);

        PlayerHits();

        // If "PlayerHits" doesn't achieve a score result (like a bust),
        // only then proceed to replay the dealer's hand
        if (SkipDealersTurn == false)
        {
            DealerHits();
        }

        return;
    }

    private IEnumerator FadeText()
    {
        GameInstructions.color = new Color(GameInstructions.color.r, GameInstructions.color.g, GameInstructions.color.b, 1);

        while (GameInstructions.color.a > 0.0f)
        {
            GameInstructions.color = new Color(GameInstructions.color.r, GameInstructions.color.g, GameInstructions.color.b, GameInstructions.color.a - 0.01f);
            yield return null;
        }
    }
}