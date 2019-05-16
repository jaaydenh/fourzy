//@vadym udod

using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections.Generic;
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

            ThemeTokenPrefabPair prefabData = data.themesTokens.list.Find((_prefabData) => Tools.Utils.IsSet(_prefabData.theme, theme));

            if (prefabData != null)
                return prefabData.tokenPrefab;
            else
                return data.themesTokens.list[0].tokenPrefab;
        }

        public TokenData GetTokenData(TokenType tokenType)
        {
            return tokens.list.Find((_data) => _data.tokenType == tokenType);
        }

        public Dictionary<TokenType, Sprite> GetTokensSprites()
        {
            Dictionary<TokenType, Sprite> tokensSprites = new Dictionary<TokenType, Sprite>();
            tokensSprites.Add(TokenType.NONE, missingTokenGraphics);

            foreach (TokenData tokenData in tokens.list)
            {
                SpriteRenderer tokenSpriteRenderer = tokenData.themesTokens.list[0].tokenPrefab.body.GetComponent<SpriteRenderer>();

                //get sprite attached to body
                if (tokenSpriteRenderer)
                    tokensSprites.Add(tokenData.tokenType, tokenSpriteRenderer.sprite);
            }

            return tokensSprites;
        }

        public Sprite GetTokenSprite(TokenType tokenType)
        {
            return GetTokenSprite(GetTokenData(tokenType));
        }

        public Sprite GetTokenSprite(TokenData tokenData)
        {
            Sprite sprite = tokenData.GetTokenSprite();

            if (sprite)
                return sprite;

            return missingTokenGraphics;
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
            public bool enabled = true;

            [ShowIf("#ShowOther")]
            [StackableField]
            public string description;
            [ShowIf("#ShowOther")]
            [StackableField]
            public bool showBackgroundTile;
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

            public string[] GetTokenThemes(ThemesDataHolder themesData)
            {
                List<string> result = new List<string>();

                foreach (Enum value in Enum.GetValues(typeof(Area)))
                    foreach (ThemeTokenPrefabPair prefabData in themesTokens.list)
                        if (prefabData.theme.HasFlag(value) && themesData.IsThemeEnabled((Area)value))
                            result.Add(themesData.GetThemeName((Area)value));

                return result.ToArray();
            }

            public Sprite GetTokenSprite()
            {
                SpriteRenderer tokenSpriteRenderer = themesTokens.list[0].tokenPrefab.body.GetComponent<SpriteRenderer>();

                if (tokenSpriteRenderer)
                    return tokenSpriteRenderer.sprite;

                return null;
            }

            private bool ShowOther()
            {
                elementName = tokenType.ToString() + (!enabled ? ", disabled" : "");

                return enabled;
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
