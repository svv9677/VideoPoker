using UnityEngine;
using System.Collections.Generic;

namespace VideoPoker
{
	public enum GameState
    {
		Init,
		WaitingToStart,
		Start,
		SetBets,
		Deal,
		WaitingToDraw,
		Draw
    }

	//-//////////////////////////////////////////////////////////////////////
	/// 
	/// The main game manager
	/// 
	public class GameManager : Singleton<GameManager>
	{
		public const int STARTING_CREDITS = 500;

		private GameState gameState;
		private int credits = 0;

		private List<Card> deck = null;
		private List<Card> cardsThisRound = null;
		private ScoringManager scorer = null;

		public GameState GetState() { return gameState; }

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{
			// kick off initial game state
			gameState = GameState.Init;

			deck = null;
			cardsThisRound = null;
			scorer = new ScoringManager();
		}
		
		//-//////////////////////////////////////////////////////////////////////
		/// Maintain a simple state machine to run the game via different states
		void Update()
		{
			switch(gameState)
            {
				// Initial state, to set/reset UI & holding variables
				// transitions to WaitingToStart
				case GameState.Init:
                    {
						UIManager.Instance.Reset();

						gameState = GameState.WaitingToStart;
                    }
					break;
				// Transitioned from Init state
				// transitions to Start when button is pressed
				case GameState.WaitingToStart:
                    {
						// nothing to do here!
                    }
					break;
				// Triggered by pressing the Start button
                // transitions to SetBets
				case GameState.Start:
                    {
						// give initial credits
						credits = STARTING_CREDITS;
						UIManager.Instance.SetCredits(credits);
						UIManager.Instance.ResetWinnings();

						UIManager.Instance.SetStartButtonState(true);
						UIManager.Instance.SetDrawButtonState(true);
						UIManager.Instance.SetBetButtonState(true);
						UIManager.Instance.SetStartButtonLabel("RESTART");
						UIManager.Instance.SetDrawButtonLabel("DEAL");

						gameState = GameState.SetBets;
                    }
					break;
				// Transitioned from start state
				// transitions to Deal, when Draw/Deal button is pressed
				case GameState.SetBets:
                    {
						// nothing to do here, as we wait for bet and/or deal buttons for interaction
                    }
					break;
				// Triggered by pressing the Deal button
				// transitions to WaitingToDraw state
				case GameState.Deal:
                    {
						// Disable bet switching and set the draw/deal button label to DRAW
						UIManager.Instance.SetBetButtonState(false);
						UIManager.Instance.SetDrawButtonLabel("DRAW");

						// Deal a random shuffle of 5 cards
						GenerateFreshDeck();
						cardsThisRound = ShuffleAndPick(5);

						// Update Card UI
						for(int i=0; i<5; i++)
							UIManager.Instance.SetCard(i, cardsThisRound[i]);

						// Update scores
						credits -= UIManager.Instance.betCount;
						UIManager.Instance.SetCredits(credits);

						gameState = GameState.WaitingToDraw;
					}
					break;
				// Transitioned from Deal state
				// transitions to Draw, when Draw/Deal button is pressed
				case GameState.WaitingToDraw:
                    {
						// nothing to do here!
                    }
					break;
				// Triggered from WaitingToDraw state on Draw/Deal button click
				// transitions to SetBets
				case GameState.Draw:
                    {
						// Draw another random shuffle of cards, keeping the ones on hold in place
						int[] holds = UIManager.Instance.GetHeldCardsCount();
						for(int i=0; i<cardsThisRound.Count; i++)
                        {
							// if this card is held in hand, don't shuffle, if not, shuffle
							if(holds[i] == 0)
                            {
								List<Card> pendingCards = ShuffleAndPick(1);
								cardsThisRound[i] = pendingCards[0];
                            }
                        }
						// Now we have all cards ready to update
						// Update Card UI
						for (int i = 0; i < 5; i++)
							UIManager.Instance.SetCard(i, cardsThisRound[i]);

						// Calculate scores
						ScoreHandCombo creditsThisRound = scorer.CalculateScore(cardsThisRound, UIManager.Instance.betCount);

						credits += creditsThisRound.score;
						UIManager.Instance.SetCredits(credits);
						UIManager.Instance.SetWinnings(creditsThisRound.score, creditsThisRound.hand);

						UIManager.Instance.SetDrawButtonState(true);
						UIManager.Instance.SetBetButtonState(true);
						UIManager.Instance.SetDrawButtonLabel("DEAL");

						gameState = GameState.SetBets;
                    }
					break;
				default:
					break;
            }
		}

		//-//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Main function that creates a deck of cards 
		/// </summary>
		private void GenerateFreshDeck()
        {
			if (deck == null)
				deck = new List<Card>();
			else
				deck.Clear();

			for(int i=0; i<4; i++)
            {
				for(int j=1; j<=13; j++)
                {
					Card card = new Card();
					card.DefineCard((Rank)j, (Suit)i);
					deck.Add(card);
                }
            }
        }

		//-//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Function that shuffles a deck of cards and pick a random set
		/// </summary>
		private List<Card> ShuffleAndPick(int count)
        {
			List<Card> pickedCards = new List<Card>();

			for(int i=0; i<count; i++)
            {
				// pick one from the existing deck
				int index = Random.Range(0, deck.Count);
				// add it to our list
				pickedCards.Add(deck[index]);
				// remove it from the deck
				deck.RemoveAt(index);
            }

			return pickedCards;
        }

		//-//////////////////////////////////////////////////////////////////////
		/// Event that triggers when draw button is pressed
		public void OnDrawButtonPressed()
		{
			// only if we are in setting bets state, we can switch to deal
			if(gameState == GameState.SetBets)
            {
				gameState = GameState.Deal;
            }
			// else if we have already dealt a hand, shuffle and draw 
            else if(gameState == GameState.WaitingToDraw)
			{
				gameState = GameState.Draw;
			}
		}

		//-//////////////////////////////////////////////////////////////////////
		/// Event that triggers when Start button is pressed
		public void OnStartButtonPressed()
		{
			// Only allow starting / restarting game from these states
			if(gameState == GameState.WaitingToStart || gameState == GameState.SetBets)
            {
				UIManager.Instance.Reset();
				gameState = GameState.Start;
			}
		}

	}
}