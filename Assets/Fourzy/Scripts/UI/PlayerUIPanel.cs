using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class PlayerUIPanel : MonoBehaviour
    {
        [SerializeField]
        private Text playerName;

        [SerializeField]
        private Transform playerIconParent;

        [SerializeField]
        private GameObject testPlayerIcon;

        private GamePiece playerIcon;

        public void SetupPlayerName(string name)
        {
            playerName.text = name;
        }

        public string GetPlayerName()
        {
            return playerName.text;
        }

        public void InitPlayerIcon(PlayerEnum player, int playerGamePieceId = 0)
        {
            testPlayerIcon.SetActive(false);

            GameObject gamePiecePrefab = GamePieceSelectionManager.Instance.GetGamePiecePrefab(playerGamePieceId);
            GameObject go = Instantiate(gamePiecePrefab, new Vector3(0, 0, 10),
                                        Quaternion.identity, playerIconParent);
            playerIcon = go.GetComponent<GamePiece>();

            playerIcon.transform.localPosition = new Vector3(0, 0, 10);
            playerIcon.SetupPlayer(player, PieceAnimState.ASLEEP);
        }

        public void InitPlayerIcon(GameObject gamePiecePrefab)
        {
            testPlayerIcon.SetActive(false);

            if (playerIcon != null)
            {
                Destroy(playerIcon.gameObject);
            }

            GameObject go = Instantiate(gamePiecePrefab);
            go.SetActive(true);
            go.transform.parent = playerIconParent;
            go.transform.localPosition = new Vector3(0, 0, 10);

            playerIcon = go.GetComponent<GamePiece>();
        }

        public void ShowPlayerTurnAnimation(Color outlineColor)
        {
            playerIcon.View.ShowTurnAnimation(outlineColor);
        }

        public void StopPlayerTurnAnimation()
        {
            playerIcon.View.StopTurnAnimation();
        }
    }
}

// if (UserManager.instance.profilePicture)
// {
//     playerProfilePicture.sprite = UserManager.instance.profilePicture;
// }
// else
// {
//     playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
//         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
//         new Vector2(0.5f, 0.5f));
// }
// if (opponentProfileSprite != null)
// {
//     opponentProfilePicture.sprite = opponentProfileSprite;
// }
// else if (opponentFacebookId != "")
// {
//     StartCoroutine(UserManager.instance.GetFBPicture(opponentFacebookId, (sprite) =>
//     {
//         opponentProfilePicture.sprite = sprite;
//     }));
// } else {
//     opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
//         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
//         new Vector2(0.5f, 0.5f));
// }