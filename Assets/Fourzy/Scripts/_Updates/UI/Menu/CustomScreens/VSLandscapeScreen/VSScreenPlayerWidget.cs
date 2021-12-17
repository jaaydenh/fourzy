//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenPlayerWidget : WidgetBase
    {
        [SerializeField]
        private Image profileImage;
        [SerializeField]
        private TMP_Text nameLabel;
        [SerializeField]
        private Badge magic;
        [SerializeField]
        private GameObject difficultiesHolder;
        [SerializeField]
        private GameObject random;
        [SerializeField]
        private GameObject[] difficultyLevels;
        [SerializeField]
        private LocalizedText localizedText;
        [SerializeField]
        private AlphaTween outlineTween;
        [SerializeField]
        private GameObject extraView;

        [Header("Spells")]
        [SerializeField]
        private TokenWidgetSmall spellPrefab;
        [SerializeField]
        private RectTransform spellsParent;

        private List<TokenWidgetSmall> spells = new List<TokenWidgetSmall>();
        private GamePieceData data;
        private GamePieceData prevData;

        public VSScreenPlayerWidget SetData(GamePieceWidgetLandscape widget)
        {
            if (widget)
            {
                data = widget.data;

                profileImage.sprite = data?.profilePicture;
                profileImage.color = data != null ? Color.white : Color.clear;
                profileImage.rectTransform.pivot = data?.ProfilePicturePivot ?? Vector2.one * .5f;
                profileImage.rectTransform.anchoredPosition = Vector2.zero;

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

            if (extraView)
            {
                extraView.SetActive(!widget);
            }

            if (data != null)
            {
                if (prevData != data)
                {
                    ClearMagic();

                    //add spells
                    foreach (SpellId spellId in data.Spells)
                    {
                        spells.Add(
                            Instantiate(spellPrefab, spellsParent)
                                .SetData(GameContentManager.Instance.tokensDataHolder.GetTokenData(spellId)));
                    }

                    magic.SetValue(data.Magic);
                    magic.SetState(true);

                    prevData = data;
                }
            }
            else
            {
                ClearMagic();
                magic.SetState(false);
            }

            return this;
        }

        public VSScreenPlayerWidget DisplayDifficulty(int level)
        {
            for (int index = 0; index < difficultyLevels.Length; index++)
            {
                difficultyLevels[index].SetActive(level == index);
            }

            difficultiesHolder.SetActive(level > -1);

            return this;
        }

        public void SetOutline(bool state)
        {
            if (state)
            {
                outlineTween.PlayForward(true);
            }
            else
            {
                outlineTween.PlayBackward(true);
            }
        }

        public bool CheckDroppedPiece(VSScreenDragableGamepiece target)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, target.transform.position);
        }

        private void ClearMagic()
        {
            //clear spells list
            foreach (TokenWidgetSmall spell in spells)
            {
                Destroy(spell.gameObject);
            }
            spells.Clear();
        }
    }
}