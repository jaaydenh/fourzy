﻿//@vadym udod

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenPlayerWidget : WidgetBase
    {
        public Image profileImage;
        public TMP_Text nameLabel;

        public VSScreenPlayerWidget SetData(GamePieceData data)
        {
            profileImage.sprite = data != null ? data.profilePicture : null;
            profileImage.color = data != null ? Color.white : Color.clear;
            profileImage.SetNativeSize();

            nameLabel.text = data != null ? data.name : "";

            return this;
        }
    }
}