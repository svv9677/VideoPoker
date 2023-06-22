using UnityEngine;

namespace VideoPoker
{
    /// <summary>
    /// All the ranks for cards in ascending order 
    /// </summary>
    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        J,
        Q,
        K,
        A
    }

    /// <summary>
    /// Types of suits available for cards
    /// </summary>
    public enum Suit
    {
        Spades = 0,
        Clubs,
        Diamonds,
        Hearts
    }

    /// <summary>
    /// Main class that handles the card functionality and its accompanying UI
    /// Attach this to the Card prefab and link the UI elements
    /// </summary>
    public class Card
    {
        /////////////////////////////////////////////////////////////// 
        #region Functionality

        public static readonly string[] SuitFileNames = { "s", "c", "d", "h" };

        private Rank rank;
        private Suit suit;

        private bool onHold;
        #endregion

        /////////////////////////////////////////////////////////////// 
        public void DefineCard(Rank _rank, Suit _suit)
        {
            // Set the member variables that define this card
            rank = _rank;
            suit = _suit;
            // default to not on hold
            onHold = false;
        }

        public string AssetString()
        {
            if(rank == Rank.A)
                return SuitFileNames[((int)suit)] + "01";
            return SuitFileNames[((int)suit)] + ((int)rank).ToString("D2");
        }

        public bool isOnHold() { return onHold; }
        public void ToggleHold() { SetHold(!onHold); }
        public void SetHold(bool flag) { onHold = flag; }

        public int SuitIndex { get { return (int)suit; } }
        public int RankValue { get { return (int)rank; } }
    }
}
