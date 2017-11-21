using UnityEngine;
using UnityEngine.UI;

public class Rewards : MonoBehaviour {

    public GameObject winnerReward;
    public Text playedPiecesTitleText;
    public Text playedPiecesRewardText;

    public void Open(bool isWinner, int piecesPlayed)
    {
        if (isWinner) {
            winnerReward.SetActive(true);
        } else {
            winnerReward.SetActive(false);
        }
        playedPiecesTitleText.text = "Played " + piecesPlayed + " Pieces";
        playedPiecesRewardText.text = "+" + piecesPlayed + " Coins";

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
