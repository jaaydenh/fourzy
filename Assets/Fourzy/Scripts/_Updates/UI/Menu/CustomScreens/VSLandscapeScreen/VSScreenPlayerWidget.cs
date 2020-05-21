//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenPlayerWidget : WidgetBase
    {
        public Image profileImage;
        public TMP_Text nameLabel;
        public GameObject difficultiesHolder;
        public GameObject[] difficultyLevels;

        public VSScreenPlayerWidget SetData(GamePieceData data)
        {
            profileImage.sprite = data != null ? data.profilePicture : null;
            profileImage.color = data != null ? Color.white : Color.clear;
            profileImage.SetNativeSize();

            nameLabel.text = data != null ? data.name : "";

            return this;
        }

        public VSScreenPlayerWidget DisplayDifficulty(int level)
        {
            for (int index = 0; index < difficultyLevels.Length; index++) difficultyLevels[index].SetActive(level == index);
            difficultiesHolder.SetActive(level > -1);

            return this;
        }
    }
}