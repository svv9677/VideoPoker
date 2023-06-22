using UnityEngine;
using UnityEngine.UI;

namespace VideoPoker
{
	//-//////////////////////////////////////////////////////////////////////
	///
	/// Manages UI including button events and updates to text fields
	/// 
	public class UIManager : Singleton<UIManager>
	{
		[SerializeField]
		private Text currentBalanceText = null;

		[SerializeField]
		private Text winningText = null;

		[SerializeField]
		private Button betButton = null;

		[SerializeField]
		private Button startButton = null;

		[SerializeField]
		private Button drawButton = null;

		// Handle to the draw button's label to switch between different states
		[SerializeField]
		private Text drawButtonLbl = null;

		// Handle to the bet button's label to switch between different states
		[SerializeField]
		private Text betButtonLbl = null;

		// Handle to the staRT button's label to switch between different states
		[SerializeField]
		private Text startButtonLbl = null;

		[SerializeField]
		private CardUI[] cardObjs = null;

		public int betCount { get; private set; }

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{
			if (betButton == null || drawButton == null || startButton == null || betButtonLbl == null ||
				drawButtonLbl == null || cardObjs == null || currentBalanceText == null || startButtonLbl == null)
			{
				Debug.LogError("ERROR!! Please connect UI Manager's inspector elements properly!");
				return;
			}

			// Bet button click can be totally UI manager driven
			betButton.onClick.AddListener(OnBetButtonPressed);
			// These two need to be tied to the Game Manager's gameState, so let GameManager drive them
			drawButton.onClick.AddListener(GameManager.Instance.OnDrawButtonPressed);
			startButton.onClick.AddListener(GameManager.Instance.OnStartButtonPressed);
		}

		public void SetCard(int index, Card card)
        {
			if (index < 0 || index > 4)
            {
				Debug.LogWarning("invalid index!!");
				return;
			}

			cardObjs[index].SetCard(card);
		}

		public int[] GetHeldCardsCount()
        {
			int[] holds = { 0, 0, 0, 0, 0 };
			for(int i=0; i<5; i++)
            {
				if (cardObjs[i].theCard.isOnHold())
					holds[i] = 1;
            }
			return holds;
        }

		/// <summary>
		/// Reset the UI manager to clean slate
		/// </summary>
		public void Reset()
        {
			// Reset all cards
			cardObjs[0].SetCard(null);
			cardObjs[1].SetCard(null);
			cardObjs[2].SetCard(null);
			cardObjs[3].SetCard(null);
			cardObjs[4].SetCard(null);

			//Set "start game" button enabled and the rest disabled
			SetStartButtonState(true);
			SetDrawButtonState(false);
			SetBetButtonState(false);

			// Set appropriate button labels
			betCount = 1;
			SetDrawButtonLabel("DEAL");
			SetBetButtonLabel("BET " + betCount.ToString());

			ResetWinnings();
		}

		void OnBetButtonPressed()
        {
			// only perform this in the correct game state
			if (GameManager.Instance.GetState() != GameState.SetBets)
				return;

			// Toggle this between 1 and 5
			betCount++;
			if (betCount > 5)
				betCount = 1;

			SetBetButtonLabel("BET " + betCount.ToString());
		}

		public void SetStartButtonState(bool toggle)
        {
			startButton.enabled = toggle;
        }

		public void SetDrawButtonState(bool toggle)
		{
			drawButton.enabled = toggle;
		}

		public void SetBetButtonState(bool toggle)
		{
			betButton.enabled = toggle;
		}

		public void SetStartButtonLabel(string text)
		{
			startButtonLbl.text = text;
			startButtonLbl.SetAllDirty();
		}

		public void SetDrawButtonLabel(string text)
        {
			drawButtonLbl.text = text;
			drawButtonLbl.SetAllDirty();
        }

		public void SetBetButtonLabel(string text)
		{
			betButtonLbl.text = text;
			betButtonLbl.SetAllDirty();
		}

		public void SetCredits(int credits)
        {
			if (currentBalanceText == null)
				return;

			currentBalanceText.text = string.Format("Balance: {0} Credits", credits);
			currentBalanceText.SetAllDirty();
        }

		public void SetWinnings(int credits, string hand)
        {
			if (winningText == null)
				return;

			winningText.text = string.Format("{0} You Won {1} Credits!", hand, credits);
			winningText.SetAllDirty();
        }

		public void ResetWinnings()
        {
			if (winningText == null)
				return;

			winningText.text = "Let's Play!";
			winningText.SetAllDirty();
		}

	}
}