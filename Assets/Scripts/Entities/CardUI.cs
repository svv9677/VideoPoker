using UnityEngine;
using UnityEngine.UI;

namespace VideoPoker
{
    /// <summary>
    /// Main class that handles the card's UI functionality
    /// Attach this to the Card prefab and link the UI elements
    /// </summary>
    public class CardUI : MonoBehaviour
    {
        /////////////////////////////////////////////////////////////// 
        #region Inspector / UI
        [SerializeField]
        private Image image = null;

        [SerializeField]
        private GameObject holdTextObj = null;
        #endregion

        ///////////////////////////////////////////////////////////////
        #region Functionality
        [HideInInspector]
        public Card theCard = null;

        #endregion

        /////////////////////////////////////////////////////////////// 
        public void SetCard(Card _card)
        {
            theCard = _card;

            // update the prefab's image
            // if the image is set in the prefab
            string filename_suffix = "back";
            if (theCard != null)
                filename_suffix = theCard.AssetString();
            if (image != null)
                image.sprite = Resources.Load<Sprite>("Art/Cards/img_card_" + filename_suffix);
            if (holdTextObj != null)
                holdTextObj.SetActive(false);
        }

        public void OnCardClicked()
        {
            // Enable hold toggle only during DEAL
            if(GameManager.Instance.GetState() == GameState.WaitingToDraw)
            {
                ToggleHold();
            }
        }

        private void ToggleHold()
        {
            if (theCard == null)
                return;

            theCard.ToggleHold();
            holdTextObj.SetActive(theCard.isOnHold());
        }

    }
}
