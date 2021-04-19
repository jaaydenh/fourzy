//@vadym udod

using ByteSheep.Events;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using TMPro;
using Fourzy._Updates.Serialized;
using UnityEngine;
using Fourzy._Updates.Mechanics._GamePiece;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OpponentWidget : WidgetBase
    {
        public AdvancedEvent onSelected;
        public AdvancedEvent onDeselected;

        public List<MiscGameContentHolder.IconData> labelBGs;
        public List<MiscGameContentHolder.IconData> profileIcons;
        public TMP_Text nameLabel;
        public Image labelBG;
        public RectTransform iconParent;
        public Image icon;

        private bool isSelected = false;

        public int playerID { get; private set; }
        public AIProfile aiProfile { get; private set; }
        public GamePieceData prefabData { get; private set; }

        public OpponentWidget SetData(AIProfile aiProfile, int playerID)
        {
            this.playerID = playerID;
            this.aiProfile = aiProfile;

            if (aiProfile == AIProfile.Player)
                nameLabel.text = LocalizationManager.Value("human");
            else
                nameLabel.text = aiProfile.ToString();

            UpdateLabelBG(aiProfile);

            MiscGameContentHolder.IconData __icon = profileIcons.Find(_icon => _icon.id == aiProfile.ToString());
            if (__icon == null)
            {
                icon.gameObject.SetActive(false);

                prefabData = GameContentManager.Instance.piecesDataHolder.random;
                GamePieceView _gamePiece = Instantiate(prefabData.player1Prefab, iconParent);

                _gamePiece.transform.localPosition = Vector3.zero;
                _gamePiece.transform.localScale = Vector3.one * 110f;
                _gamePiece.StartBlinking();
            }
            else
            {
                icon.gameObject.SetActive(true);
                icon.sprite = __icon.sprite;
            }

            return this;
        }

        public void Select()
        {
            if (isSelected) return;

            isSelected = true;
            onSelected.Invoke();
        }

        public void Deselect()
        {
            if (!isSelected) return;

            isSelected = false;
            onDeselected.Invoke();
        }

        private void UpdateLabelBG(AIProfile aiProfile)
        {
            MiscGameContentHolder.IconData icon = labelBGs.Find(_icon => _icon.id == aiProfile.ToString());
            if (icon != null) labelBG.sprite = icon.sprite;
        }
    }
}