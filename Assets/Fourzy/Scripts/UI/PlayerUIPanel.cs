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

            GamePiece gamePiecePrefab = GameContentManager.Instance.GetGamePiecePrefab(playerGamePieceId);
            playerIcon = Instantiate(gamePiecePrefab, new Vector3(0, 0, 10),
                                        Quaternion.identity, playerIconParent);

            playerIcon.transform.localPosition = new Vector3(0, 0, 10);
            playerIcon.player = player;
        }

        public void InitPlayerIcon(GamePiece gamePiecePrefab)
        {
            testPlayerIcon.SetActive(false);

            if (playerIcon != null)
            {
                Destroy(playerIcon.gameObject);
            }

            playerIcon = Instantiate(gamePiecePrefab);
            playerIcon.gameObject.SetActive(true);
            playerIcon.CachedTransform.parent = playerIconParent;
            playerIcon.CachedTransform.localPosition = new Vector3(0, 0, 10);
            playerIcon.transform.localScale = gamePiecePrefab.transform.localScale;
        }

        public void ShowPlayerTurnAnimation()
        {
            playerIcon.View.ShowTurnAnimation();
        }

        public void StopPlayerTurnAnimation()
        {
            playerIcon.View.StopTurnAnimation();
        }
    }
}

// if (UserManager.Instance.profilePicture)
// {
//     playerProfilePicture.sprite = UserManager.Instance.profilePicture;
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
//     StartCoroutine(UserManager.Instance.GetFBPicture(opponentFacebookId, (sprite) =>
//     {
//         opponentProfilePicture.sprite = sprite;
//     }));
// } else {
//     opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
//         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
//         new Vector2(0.5f, 0.5f));
// }