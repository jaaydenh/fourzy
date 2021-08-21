//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using FourzyGameModel.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSGamePromptProfileWidget : WidgetBase
    {
        public Image bg;
        public Image content;
        public TMP_Text nameLabel;
        public VSGamePromptBossPowerWidget bossPowerWidget;

        private GamePieceView gamePiece;

        public VSGamePromptProfileWidget SetColor(Color color)
        {
            if (color.a != 0f) bg.color = color;

            return this;
        }

        public VSGamePromptProfileWidget SetProfile(Player profile)
        {
            if (gamePiece)
            {
                Destroy(gamePiece.gameObject);
            }

            GamePieceData data = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(profile.HerdId);

            if (data.profilePicture != null)
            {
                content.color = Color.white;
                content.rectTransform.pivot = data.ProfilePicturePivot;
                content.sprite = data.profilePicture;
            }
            else
            {
                content.color = Color.clear;

                gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(profile.HerdId).player1Prefab, content.rectTransform);

                gamePiece.transform.localPosition = Vector3.zero;
                gamePiece.transform.localScale = Vector3.one * .5f;
                gamePiece.StartBlinking();
            }

            SetName(profile.DisplayName);

            bossPowerWidget.SetData(profile);

            return this;
        }

        public VSGamePromptProfileWidget SetName(string name)
        {
            int charLimit = 9;

            //shorten if needed
            if (name.Length > charLimit)
                nameLabel.text = name.Substring(0, charLimit) + "...";
            else
                nameLabel.text = name;

            return this;
        }
    }
}