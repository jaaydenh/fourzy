//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GeneratorTesterPrompt : MenuScreen
    {
        public ScrollRect areasContainer;
        public PracticeScreenAreaSelectWidget areaWidgetPrefab;
        public GeneratorTesterTokenView generatorTesterTokenView;
        public RectTransform tokensParent;
        public Slider low;
        public TMP_Text lowText;
        public Slider high;
        public TMP_Text highText;
        public Slider percentage;
        public TMP_Text percentageText;
        public TMP_InputField recipe;
        public TMP_Dropdown aiProfileDropdown;
        public ButtonExtended toggleMagicButton;
        public ButtonExtended toggleRandomElementsButton;
        public ButtonExtended toggleDynamicElementsButton;

        private List<GeneratorTesterTokenView> tokenViews;
        private Dictionary<int, AIProfile> aiProfiles;
        private int currentProfileIndex = -1;

        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }
        public bool AllowMagic { get; private set; } = true;
        public bool AllowRandomElements { get; private set; } = true;
        public bool AllowDynamicElements { get; private set; } = true;

        public override void OnBack()
        {
            base.OnBack();

            CloseSelf();
        }

        public void OnLowValueChange(float value)
        {
            if (value > high.value)
            {
                high.value = value + 1;
            }

            lowText.text = $"Low {value}";
        }

        public void OnHighValueChange(float value)
        {
            if (value < low.value)
            {
                low.value = value - 1;
            }

            highText.text = $"High {value}";
        }

        public void OnPercentageValueChange(float value)
        {
            percentageText.text = $"% {value}/100";
        }

        public void OnAIProfileSelected(int value)
        {
            currentProfileIndex = value;
        }
        
        public void ToggleMagic()
        {
            AllowMagic = !AllowMagic;

            toggleMagicButton.GetBadge().badge.SetState(AllowMagic);
        }

        public void ToggleRandomElements()
        {
            AllowRandomElements = !AllowRandomElements;

            toggleRandomElementsButton.GetBadge().badge.SetState(AllowRandomElements);
        }

        public void ToggleDynamicElements()
        {
            AllowDynamicElements = !AllowDynamicElements;

            toggleDynamicElementsButton.GetBadge().badge.SetState(AllowDynamicElements);
        }

        public void GenerateAndTry()
        {
            Area area = Area.NONE;
            if (currentAreaWidget != null)
            {
                area = currentAreaWidget.area;
            }

            BoardGenerationPreferences preferences = new BoardGenerationPreferences(area, (int)percentage.value);
            
            //preferences.RequestedRecipe = recipe.text;
            preferences.RequestedRecipe = null;
            preferences.AllowedTokens = tokenViews
                .Where(_view => _view.IsAllowed)
                .Select(_view => _view.TokenType)
                .ToList();
            preferences.ForbiddenTokens = tokenViews
                .Where(_view => _view.IsForbidden)
                .Select(_view => _view.TokenType)
                .ToList();
            preferences.IncludesDynamicTokens = AllowDynamicElements;
            preferences.IncludesRandomTokens = AllowRandomElements;

            ClientFourzyGame game;
            GamePieceData random = GameContentManager.Instance.piecesDataHolder.random.data;
            Player opponent = new Player(2, "Player2", aiProfiles[currentProfileIndex]) { HerdId = random.ID };
            if (area == Area.NONE)
            {
                preferences.TargetComplexityLow = (int)low.value;
                preferences.TargetComplexityHigh = (int)high.value;

                game = new ClientFourzyGame(UserManager.Instance.meAsPlayer, opponent, Preferences: preferences);
            }
            else
            {
                game = new ClientFourzyGame(
                    area,
                    UserManager.Instance.meAsPlayer,
                    opponent,
                    1,
                    Preferences: preferences);
            }

            if (opponent.Profile == AIProfile.Player)
            {
                game._Type = GameType.PASSANDPLAY;
            }
            else
            {
                game._Type = GameType.AI;
            }
            game.UpdateFirstState();

            GameManager.Instance.StartGame(game, GameTypeLocal.LOCAL_GAME);

            CloseSelf();
        }

        protected void OnAreaWidgetTap(PracticeScreenAreaSelectWidget widget)
        {
            if (currentAreaWidget)
            {
                currentAreaWidget.Deselect();

                if (currentAreaWidget == widget)
                {
                    currentAreaWidget = null;
                }
                else
                {
                    currentAreaWidget = widget;
                }
            }
            else
            {
                currentAreaWidget = widget;
            }

            low.transform.parent.gameObject.SetActive(currentAreaWidget == null);
            high.transform.parent.gameObject.SetActive(currentAreaWidget == null);

            if (currentAreaWidget)
            {
                currentAreaWidget.Select();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //doprdown
            aiProfiles = new Dictionary<int, AIProfile>();
            AIProfile[] values = (AIProfile[])Enum.GetValues(typeof(AIProfile));
            for (int index = 0; index < values.Length; index++)
            {
                aiProfiles.Add(index, values[index]);
            }
            aiProfileDropdown.AddOptions(values.Select(_value => _value.ToString()).ToList());
            aiProfileDropdown.value = currentProfileIndex = 0;

            //areas
            bool first = true;
            foreach (AreasDataHolder.GameArea areaData in GameContentManager.Instance.enabledAreas)
            {
                if (first)
                {
                    first = false;
                    OnAreaWidgetTap(AddAreaWidget(areaData.areaID));
                }
                else
                {
                    AddAreaWidget(areaData.areaID);
                }
            }

            tokenViews = new List<GeneratorTesterTokenView>();
            //tokens
            foreach (TokensDataHolder.TokenData data in GameContentManager.Instance.enabledTokens)
            {
                tokenViews.Add(Instantiate(generatorTesterTokenView, tokensParent)
                    .SetData(data.tokenType));
            }
        }

        protected PracticeScreenAreaSelectWidget AddAreaWidget(Area area)
        {
            PracticeScreenAreaSelectWidget instance = Instantiate(areaWidgetPrefab, areasContainer.content)
                .SetData(area);
            instance.button.onTap += data => OnAreaWidgetTap(instance);

            return instance;
        }
    }
}