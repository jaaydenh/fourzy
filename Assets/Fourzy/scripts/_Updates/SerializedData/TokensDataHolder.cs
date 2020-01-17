//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultTokensDataHolder", menuName = "Create Tokens Data Holder")]
    public class TokensDataHolder : ScriptableObject
    {
        public Sprite missingTokenGraphics;
        [List]
        public TokenDataList tokens;

        public TokenView GetToken(TokenType tokenType, Area theme)
        {
            TokenData data = GetTokenData(tokenType);

            if (data == null)
                return tokens.list[0].themesTokens.list[0].tokenPrefab;

            ThemeTokenPrefabPair prefabData = data.themesTokens.list.Find(_prefabData => Tools.Utils.IsSet(_prefabData.theme, theme));

            if (prefabData != null)
                return prefabData.tokenPrefab;
            else
                return data.themesTokens.list[0].tokenPrefab;
        }

        public TokenData GetTokenData(TokenType tokenType) => tokens.list.Find(_data => _data.tokenType == tokenType);

        public TokenData GetTokenData(SpellId spellID) => tokens.list.Find(_data => _data.isSpell && _data.spellID == spellID);

        public Dictionary<TokenType, Sprite> GetTokensSprites()
        {
            Dictionary<TokenType, Sprite> tokensSprites = new Dictionary<TokenType, Sprite>();
            tokensSprites.Add(TokenType.NONE, missingTokenGraphics);

            foreach (TokenData tokenData in tokens.list) tokensSprites.Add(tokenData.tokenType, tokenData.GetTokenSprite());

            return tokensSprites;
        }

        public Sprite GetTokenSprite(TokenType tokenType) => GetTokenSprite(GetTokenData(tokenType));

        public Sprite GetTokenSprite(TokenData tokenData)
        {
            Sprite sprite = tokenData.GetTokenSprite();

            if (sprite) return sprite;

            return missingTokenGraphics;
        }

        public void ResetTokenInstructions()
        {
            tokens.list.ForEach(token => PlayerPrefsWrapper.SetInstructionPopupDisplayed((int)token.tokenType, false));
        }

        [Serializable]
        public class TokenDataList
        {
            [StackableField]
            public List<TokenData> list;
        }

        [Serializable]
        public class TokenData
        {
            [HideInInspector]
            public string elementName;

            public TokenType tokenType;
            public Sprite tokenIcon;
            public bool enabled = true;
            /// <summary>
            /// Just for some UI features
            /// </summary>
            public bool isSpell = false;
            [ShowIf("#ShowSpellData")]
            [StackableField]
            public SpellId spellID;
            [ShowIf("#ShowSpellData")]
            [StackableField]
            public int basePrice;

            [ShowIf("#ShowOther")]
            [StackableField]
            public string description;
            [ShowIf("#ShowOther")]
            [StackableField]
            public bool showBackgroundTile;
            [ShowIf("#ShowColor")]
            [StackableField]
            public Color backgroundTileColor;
            [ShowIf("#ShowOther")]
            [StackableField]
            public string gameboardInstructionID;

            [List]
            [ShowIf("#ShowOther")]
            [StackableField]
            public ThemeTokensList themesTokens;

            public string name
            {
                get
                {
                    if (themesTokens.list.Count > 0 && themesTokens.list[0].tokenPrefab)
                        return themesTokens.list[0].tokenPrefab.name;

                    return "token_name_default";
                }
            }

            public int price
            {
                get
                {
                    //get price depending on spell
                    return basePrice;
                }
            }

            public List<ThemesDataHolder.GameTheme> GetTokenAreas(ThemesDataHolder themesData)
            {
                List<ThemesDataHolder.GameTheme> result = new List<ThemesDataHolder.GameTheme>();

                foreach (Enum value in Enum.GetValues(typeof(Area)))
                    foreach (ThemeTokenPrefabPair prefabData in themesTokens.list)
                        if (prefabData.theme.HasFlag(value) && themesData.IsThemeEnabled((Area)value))
                            result.Add(themesData.GetTheme((Area)value));

                return result;
            }

            public List<string> GetAreaNames(ThemesDataHolder themesDataHolder) => GetTokenAreas(themesDataHolder)?.Select(area => LocalizationManager.Value(area.id)).ToList() ?? null;

            public Sprite GetTokenSprite()
            {
                if (tokenIcon == null)
                {
                    SpriteRenderer tokenSpriteRenderer = themesTokens.list[0].tokenPrefab.body.GetComponent<SpriteRenderer>();

                    if (tokenSpriteRenderer)
                        return tokenSpriteRenderer.sprite;
                }
                else
                    return tokenIcon;

                return null;
            }

            private bool ShowOther()
            {
                elementName = tokenType.ToString() + (!enabled ? ", disabled" : "");

                return enabled;
            }

            private bool ShowColor()
            {
                return ShowOther() && showBackgroundTile;
            }

            public bool ShowSpellData()
            {
                return ShowOther() && isSpell;
            }
        }

        [Serializable]
        public class ThemeTokensList
        {
            [StackableField]
            public List<ThemeTokenPrefabPair> list;
        }

        [Serializable]
        public class ThemeTokenPrefabPair
        {
            [EnumMaskPopup]
            public Area theme;
            public TokenView tokenPrefab;
        }
    }
}
