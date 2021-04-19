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
using UnityEngine.EventSystems;
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
        public TMP_Dropdown recipeDropdown;
        public ButtonExtended toggleComplexityButton;
        public ButtonExtended toggleMagicButton;
        public ButtonExtended toggleRandomElementsButton;
        public ButtonExtended toggleDynamicElementsButton;

        private List<GeneratorTesterTokenView> tokenViews;
        private Dictionary<int, AIProfile> aiProfiles;
        private int currentProfileIndex = -1;

        public PracticeScreenAreaSelectWidget currentAreaWidget { get; private set; }
        public bool UseComplexity { get; private set; } = true;
        public bool AllowMagic { get; private set; } = true;
        public bool AllowRandomElements { get; private set; } = false;
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

        public void OnRecipeSelected(int value)
        {
            recipe.text = recipeDropdown.options[value].text;
        }
        
        public void ToggleMagic()
        {
            AllowMagic = !AllowMagic;

            toggleMagicButton.GetBadge().badge.SetState(AllowMagic);
        }

        public void ToggleComplexity()
        {
            UseComplexity = !UseComplexity;

            toggleComplexityButton.GetBadge().badge.SetState(UseComplexity);
            toggleComplexityButton.GetBadge("box").badge.SetState(UseComplexity);
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

        public void OnRecipeDropdownClick(BaseEventData data)
        {
            recipeDropdown.ClearOptions();
            recipeDropdown.AddOptions(BoardFactory.GetRecipeListForComplexityRange(
                currentAreaWidget.area,
                (int)percentage.value));
        }

        public void GenerateAndTry()
        {
            Area area = currentAreaWidget ? currentAreaWidget.area : Area.TRAINING_GARDEN;

            BoardGenerationPreferences preferences = new BoardGenerationPreferences(area, (int)percentage.value);
            GameOptions options = new GameOptions() { PlayersUseSpells = AllowMagic };
            
            preferences.RequestedRecipe = recipe.text;
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
            GamePieceData random = GameContentManager.Instance.piecesDataHolder.random;
            Player opponent = new Player(2, "Player2", aiProfiles[currentProfileIndex]) { HerdId = random.Id };

            if (UseComplexity)
            {
                preferences.TargetComplexityLow = (int)low.value;
                preferences.TargetComplexityHigh = (int)high.value;
            }
            else
            {
                preferences.TargetComplexityLow = -1;
                preferences.TargetComplexityHigh = -1;
            }
            game = new ClientFourzyGame(
                area,
                UserManager.Instance.meAsPlayer,
                opponent,
                1,
                options,
                preferences);

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
            if (currentAreaWidget && currentAreaWidget != widget)
            {
                currentAreaWidget.Deselect();
            }

            currentAreaWidget = widget;

            BoardGenerator Gen = BoardGeneratorFactory.CreateGenerator(currentAreaWidget.area);
            low.value = Gen.MinComplexity;
            high.value = Gen.MaxComplexity;

            currentAreaWidget.Select();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            low.minValue = Constants.MIN_COMPLEXCITY_SCORE;
            low.maxValue = Constants.MAX_COMPLEXCITY_SCORE - 1;
            high.minValue = Constants.MIN_COMPLEXCITY_SCORE + 1;
            high.maxValue = Constants.MAX_COMPLEXCITY_SCORE;

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