﻿//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Sirenix.OdinInspector;
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
        [ListDrawerSettings(NumberOfItemsPerPage = 16, ListElementLabelName = "tokenType")]
        public List<TokenData> tokens;

        public TokenView GetToken(TokenType tokenType, Area theme)
        {
            TokenData data = GetTokenData(tokenType);

            if (data == null)
            {
                return tokens[0].themesTokens[0].tokenPrefab;
            }

            ThemeTokenPrefabPair prefabData = 
                data.themesTokens.Find(_prefabData => Tools.Utils.IsSet(_prefabData.theme, theme));

            if (prefabData != null)
            {
                return prefabData.tokenPrefab;
            }
            else
            {
                return data.themesTokens[0].tokenPrefab;
            }
        }

        public TokenData GetTokenData(TokenType tokenType) => 
            tokens.Find(_data => _data.tokenType == tokenType);

        public TokenData GetTokenData(SpellId spellID) => 
            tokens.Find(_data => _data.isSpell && _data.spellID == spellID);

        public Dictionary<TokenType, Sprite> GetTokensSprites()
        {
            Dictionary<TokenType, Sprite> tokensSprites = new Dictionary<TokenType, Sprite>();
            tokensSprites.Add(TokenType.NONE, missingTokenGraphics);

            foreach (TokenData tokenData in tokens)
            {
                tokensSprites.Add(tokenData.tokenType, tokenData.GetTokenSprite());
            }

            return tokensSprites;
        }

        public Sprite GetTokenSprite(TokenType tokenType) => 
            GetTokenSprite(GetTokenData(tokenType));

        public Sprite GetTokenSprite(TokenData tokenData)
        {
            Sprite sprite = tokenData.GetTokenSprite();

            if (sprite)
            {
                return sprite;
            }

            return missingTokenGraphics;
        }

        public void ResetTokenInstructions()
        {
            tokens.ForEach(token =>
                PlayerPrefsWrapper.SetInstructionPopupDisplayed((int)token.tokenType, false));
        }

        [Serializable]
        public class TokenData
        {
            public TokenType tokenType;
            public Sprite tokenIcon;
            public bool enabled = true;
            /// <summary>
            /// Just for some UI features
            /// </summary>
            public bool isSpell = false;
            [ShowIf("@enabled && isSpell")]
            public SpellId spellID;

            [ShowIf("enabled")]
            public string description;
            [ShowIf("enabled")]
            public bool showBackground;
            [ShowIf("@enabled && showBackground")]
            public Color backgroundTileColor;
            [ShowIf("enabled")]
            public string gameboardInstructionID;
            [ShowIf("enabled")]
            public List<ThemeTokenPrefabPair> themesTokens;

            public string name
            {
                get
                {
                    if (themesTokens.Count > 0 && themesTokens[0].tokenPrefab)
                    {
                        return themesTokens[0].tokenPrefab.name;
                    }

                    return "token_name_default";
                }
            }

            public int price
            {
                get
                {
                    //get price depending on spell
                    if (isSpell)
                    {
                        return spellID.GetSpellPrice();
                    }
                    else
                    {
                        return 10;
                    }
                }
            }

            public List<AreasDataHolder.GameArea> GetTokenAreas(AreasDataHolder themesData)
            {
                List<AreasDataHolder.GameArea> result = new List<AreasDataHolder.GameArea>();

                foreach (Enum value in Enum.GetValues(typeof(Area)))
                {
                    foreach (ThemeTokenPrefabPair prefabData in themesTokens)
                    {
                        if (prefabData.theme.HasFlag(value) && themesData.IsAreaEnabled((Area)value))
                        {
                            result.Add(themesData[(Area)value]);
                        }
                    }
                }

                return result;
            }

            public List<string> GetAreaNames(AreasDataHolder themesDataHolder) =>
                GetTokenAreas(themesDataHolder)?.Select(area => LocalizationManager.Value(area.name)).ToList() ?? null;

            public Sprite GetTokenSprite()
            {
                if (tokenIcon == null)
                {
                    SpriteRenderer tokenSpriteRenderer =
                        themesTokens[0].tokenPrefab.body.GetComponent<SpriteRenderer>();

                    if (tokenSpriteRenderer)
                    {
                        return tokenSpriteRenderer.sprite;
                    }
                }
                else
                {
                    return tokenIcon;
                }

                return null;
            }
        }

        [Serializable]
        public class ThemeTokenPrefabPair
        {
            [StackableDecorator.EnumMaskPopup]
            public Area theme;
            public TokenView tokenPrefab;
        }
    }
}
