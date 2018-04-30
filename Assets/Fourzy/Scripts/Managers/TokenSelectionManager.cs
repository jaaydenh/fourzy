using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class TokenSelectionManager : MonoBehaviour
    {
        public GameObject tokenGrid;
        public List<Sprite> tokens = new List<Sprite>();
        public GameObject tokenPrefab;
        TokenData[] tokenData;

        private static TokenSelectionManager _instance;
        public static TokenSelectionManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TokenSelectionManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public string GetTokenName(int tokenId)
        {
            for (int i = 0; i < tokenData.Length; i++)
            {
                if (tokenData[i].ID == tokenId.ToString())
                {
                    return tokenData[i].Name;
                }
            }
            return "Error";
        }

        public void LoadTokens()
        {
            tokenData = TokenBoardLoader.instance.GetAllTokens();

            foreach (var token in tokenData)
            {
                GameObject go = Instantiate(tokenPrefab) as GameObject;
                //go.transform.localScale = new Vector3(1, 1, 1);
                TokenUI tokenUI = go.GetComponent<TokenUI>();
                tokenUI.id = token.ID;
                tokenUI.tokenName = token.Name;
                tokenUI.tokenNameText.text = token.Name;
                tokenUI.tokenImage.sprite = tokens[int.Parse(token.ID)];
                tokenUI.showBackgroundTile = token.showBackgroundTile;
                tokenUI.arenaName = token.Arena;
                tokenUI.description = token.Description;
                if (token.showBackgroundTile) {
                    tokenUI.tileBGImage.SetActive(true);
                }
                //go.GetComponentInChildren<Image>().sprite = tokens[int.Parse(piece.ID)];

                go.gameObject.transform.SetParent(tokenGrid.transform, false);

                //var toggle = go.GetComponent<Toggle>();
                //ToggleGroup tg = tokenGrid.GetComponent<ToggleGroup>();
                //toggle.group = tg;

                //if (string.Equals(piece.ID, gamePieceId))
                //{
                //    gamePieceUI.ActivateSelector();
                //}
            }
        }
    }
}