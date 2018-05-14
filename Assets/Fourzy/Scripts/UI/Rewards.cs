using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class Rewards : MonoBehaviour
    {

        public GameObject winnerReward;
        public Text playedPiecesTitleText;
        public Text playedPiecesRewardText;
        public Text ratingDeltaText;

        private void OnEnable()
        {
            ChallengeManager.OnReceivedRatingDelta += SetRatingDelta;
        }

        private void OnDisable()
        {
            ChallengeManager.OnReceivedRatingDelta -= SetRatingDelta;
        }

        public void Open(bool isWinner, int piecesPlayed)
        {
            if (isWinner)
            {
                winnerReward.SetActive(true);
            }
            else
            {
                winnerReward.SetActive(false);
            }
            playedPiecesTitleText.text = "Played " + piecesPlayed + " Pieces";
            playedPiecesRewardText.text = "+" + piecesPlayed + " Coins";

            gameObject.SetActive(true);
        }

        private void SetRatingDelta(int ratingDelta) {
            Debug.Log("Rewards Screen: SetRatingDelta: " + ratingDelta.ToString());
            if (ratingDelta >= 0) {
                ratingDeltaText.text = "+" + ratingDelta.ToString();    
            } else {
                ratingDeltaText.text = ratingDelta.ToString();
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

