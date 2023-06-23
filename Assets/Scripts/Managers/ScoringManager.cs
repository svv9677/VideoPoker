using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VideoPoker
{
	public class ScoreHandCombo
    {
		public int score;
		public string hand;

		public ScoreHandCombo() { score = 0; hand = "No matching hand!"; }

		public override string ToString() { return string.Format("{0} - {1}", score, hand); }
    }

	public class ScoringManager
	{
		private List<Card> cardsInHand;

		public ScoringManager() { cardsInHand = null; }

		//-//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Helper functions for calculating scores
		/// </summary>
		private bool IsSameSuit()
		{
			if (cardsInHand == null)
				return false;

			int suitIndex = -1;
			foreach (Card card in cardsInHand)
			{
				// if we haven't set it yet
				if (suitIndex == -1)
					suitIndex = card.SuitIndex;
				// else, compare
				else
				{
					if (suitIndex != card.SuitIndex)
						return false;
				}
			}

			return true;
		}

		private Dictionary<int, int> MatchingRankCount()
		{
			Dictionary<int, int> maxValues = new Dictionary<int, int>();

			if (cardsInHand == null)
				return maxValues;

			foreach (Card card in cardsInHand)
			{
				if (maxValues.ContainsKey(card.RankValue))
					maxValues[card.RankValue]++;
				else
					maxValues[card.RankValue] = 1;
			}

			return maxValues;
		}

		private bool AreUnique(Dictionary<int, int> values)
        {
			foreach(KeyValuePair<int, int> kvp in values)
            {
				if (kvp.Value != 1)
					return false;
            }
			return true;
        }

		private bool AreSequential(Dictionary<int, int> values)
		{
			List<int> keys = new List<int>();
			foreach (KeyValuePair<int, int> kvp in values)
			{
				keys.Add(kvp.Key);
			}

			var sortedKeys = keys.OrderBy(x => x).ToList();
			for(int i=0; i<sortedKeys.Count-1; i++)
            {
				if (sortedKeys[i] != sortedKeys[i + 1] - 1)
					return false;
            }

			return true;
		}

		private bool JacksOrHigher(Dictionary<int, int> values)
		{
			int count = 0;
			foreach (KeyValuePair<int, int> kvp in values)
			{
				if (kvp.Key >= 11 && kvp.Value >= 2)
					count++;
			}

			return (count >= 1);
		}
				

		//-//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Main Function to calculate the score for the given cards in hand
		/// </summary>
		public ScoreHandCombo CalculateScore(List<Card> cards, int bets)
		{
			ScoreHandCombo combo = new ScoreHandCombo();
			// Store the cards internally for re-use
			cardsInHand = cards;

			// Calculate various scoring variables
			bool sameSuit = IsSameSuit();
			Dictionary<int, int> maxMatched = MatchingRankCount();
			bool unique = AreUnique(maxMatched);
			bool sequential = AreSequential(maxMatched);

			// Check the hand conditions in the highest to lowest order
			// Check for Royal Flush
			int royalCount = 0;
			for(int i=0; i<cardsInHand.Count; i++)
            {
				Card card = cardsInHand[i];
				if (card.SuitIndex == 3 && card.RankValue >= 10)
					royalCount++;
            }
			if(sequential && unique && sameSuit && royalCount == 5)
            {
				// bonus!!
				if (bets == 5)
					combo.score = 4000;
				else
					combo.score = 250 * bets;
				combo.hand = "Royal Flush!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Straight Flush
			if (sequential && unique && sameSuit)
            {
				combo.score = 50 * bets;
				combo.hand = "Straight Flush!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Four of a kind
			if (maxMatched.ContainsValue(4))
            {
				combo.score = 25 * bets;
				combo.hand = "Four of a kind!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Full House
			if(maxMatched.ContainsValue(3) && maxMatched.ContainsValue(2))
            {
				combo.score = 9 * bets;
				combo.hand = "Full House!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Flush
			if (sameSuit)
            {
				combo.score = 6 * bets;
				combo.hand = "Flush!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Straight
			if(sequential && unique)
            {
				combo.score = 4 * bets;
				combo.hand = "Straight!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Three of a kind
			if (maxMatched.ContainsValue(3))
            {
				combo.score = 3 * bets;
				combo.hand = "Three of a kind!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for Two pairs
			int pairs = 0;
			foreach(KeyValuePair<int, int> kvp in maxMatched)
            {
				if (kvp.Value == 2)
					pairs++;
            }
			if(pairs >= 2)
            {
				combo.score = 2 * bets;
				combo.hand = "Two Pair!";
				Debug.Log(combo.ToString());
				return combo;
			}

			// Check for jacks or higher
			if (JacksOrHigher(maxMatched))
            {
				combo.score = bets;
				combo.hand = "Jacks or Better!";
				Debug.Log(combo.ToString());
				return combo;
			}

			return combo;
		}
	}
}
