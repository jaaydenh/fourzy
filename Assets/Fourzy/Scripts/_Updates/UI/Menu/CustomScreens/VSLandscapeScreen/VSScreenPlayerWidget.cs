//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Fourzy._Updates.Serialized.TokensDataHolder;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenPlayerWidget : WidgetBase
    {
        public Image profileImage;
        public TMP_Text nameLabel;
        public Badge magic;
        public GameObject difficultiesHolder;
        public GameObject random;
        public GameObject[] difficultyLevels;
        public LocalizedText localizedText;

        [Header("Spells")]
        public TokenWidgetSmall spellPrefab;
        public RectTransform spellsParent;

        private List<TokenWidgetSmall> spells = new List<TokenWidgetSmall>();
        private GamePieceData data;
        private GamePieceData prevData;

        public VSScreenPlayerWidget SetData(GamePieceWidgetLandscape widget)
        {
            if (widget)
            {
                data = widget.data;

                profileImage.sprite = data != null ? data.profilePicture : null;
                profileImage.color = data != null ? Color.white : Color.clear;
                profileImage.rectTransform.anchoredPosition = data != null ? data.profilePictureOffset : Vector2.zero;
                profileImage.SetNativeSize();
                random.SetActive(data == null);

                nameLabel.text = data != null ? data.name : "Random";
            }
            else
            {
                data = null;

                profileImage.sprite = null;
                profileImage.color = Color.clear;

                random.SetActive(false);
                localizedText.UpdateLocale();
            }

            _Update();

            return this;
        }

        public override void _Update()
        {
            //toggle magic visibility
            bool _magic = SettingsManager.Get(SettingsManager.KEY_MAGIC) && data != null;

            if (_magic && prevData != data)
            {
                //clear spells list
                foreach (TokenWidgetSmall spell in spells) Destroy(spell.gameObject);
                spells.Clear();

                //add spells
                foreach (SpellId spellId in data.spells) spells.Add(Instantiate(spellPrefab, spellsParent).SetData(GameContentManager.Instance.tokensDataHolder.GetTokenData(spellId)));

                magic.SetValue(data.startingMagic);
                spellsParent.gameObject.SetActive(true);

                prevData = data;
            }

            magic.SetState(_magic);
            spellsParent.gameObject.SetActive(_magic);
        }

        public VSScreenPlayerWidget DisplayDifficulty(int level)
        {
            for (int index = 0; index < difficultyLevels.Length; index++) difficultyLevels[index].SetActive(level == index);
            difficultiesHolder.SetActive(level > -1);

            return this;
        }
    }
}