using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{

    public class LeaderboardPlayer : MonoBehaviour {

        public Text playerNameLabel, rankLabel, ratingLabel;
        public Image playerProfilePicture;
        public Texture2D defaultProfilePicture;
        public Image onlineTexture;
        public bool isOnline;

        public string playerId, facebookId;

    	void Start () {
    		
            UpdatePlayer();
    	}

        public void UpdatePlayer()
        {
            onlineTexture.color = isOnline ? Color.green : Color.gray;

            if (facebookId != null) {
                StartCoroutine(UserManager.instance.GetFBPicture(facebookId, (sprite)=>
                    {
                        playerProfilePicture.sprite = sprite;
                    }));
            } else {
                playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture, 
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height), 
                    new Vector2(0.5f, 0.5f));
            }

        }
    }
}
