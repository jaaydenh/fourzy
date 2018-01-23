using UnityEngine;
using UnityEngine.UI;

namespace Fourzy {
    public class MiniGameBoard : MonoBehaviour {

        public delegate void SetTokenBoard(TokenBoard tokenboard);
        public static event SetTokenBoard OnSetTokenBoard;
        public GameObject gameboard;
        public TokenBoard tokenBoard;
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;
        public GameObject blockerToken;
        public GameObject ghostToken;
        public GameObject iceSheetToken;
        public GameObject pitToken;
        public GameObject ninetyRightArrowToken;
        public GameObject ninetyLeftArrowToken;
        public GameObject bumperToken;
        public GameObject coinToken;

        public GameObject glow;
        public GameObject questionMark;
        private GameObject tokens;

        public void SetToggleGroup() {
            Transform parent = this.GetComponentInParent<Transform>();

            var toggle = this.GetComponent<Toggle>();
            toggle.group = parent.gameObject.GetComponent<ToggleGroup>();
        }

        public void CreateTokens() {

            tokens = new GameObject("Tokens");
            tokens.transform.parent = gameObject.transform;

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    Token token = (Token)tokenBoard.tokenData[row, col];
                    GameObject go;
                    switch (token)
                    {
                        case Token.UP_ARROW:
                            go = Instantiate(upArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.DOWN_ARROW:
                            go = Instantiate(downArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.LEFT_ARROW:
                            go = Instantiate(leftArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0, 0, -90);
                            break;
                        case Token.RIGHT_ARROW:
                            go = Instantiate(rightArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0, 0, 90);
                            break;
                        case Token.STICKY:
                            go = Instantiate(stickyToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.BLOCKER:
                            go = Instantiate(blockerToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.GHOST:
                            go = Instantiate(ghostToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.ICE_SHEET:
                            go = Instantiate(iceSheetToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.PIT:
                            go = Instantiate(pitToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.NINETY_RIGHT_ARROW:
                            go = Instantiate(ninetyRightArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.NINETY_LEFT_ARROW:
                            go = Instantiate(ninetyLeftArrowToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.BUMPER:
                            go = Instantiate(bumperToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.COIN:
                            go = Instantiate(coinToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        default:
                            break;
                    }
                }
            }

            tokens.transform.localPosition = new Vector3(-1.63f,1.63f);
            tokens.transform.localScale = new Vector3(0.46f,0.46f,1);
        }

        public void SetAsRandom() {
            questionMark.SetActive(true);
        }

        public void BoardSelect() {
            Toggle toggle = this.GetComponentInChildren<Toggle>();
            // Debug.Log("shader: " + gameboard.GetComponent<SpriteRenderer>().material.shader);
            if (toggle.isOn) {
                //Debug.Log("GAME BOARD SELECTED ON");
                glow.SetActive(true);
                if (OnSetTokenBoard != null)
                    OnSetTokenBoard(tokenBoard);
            } else {
                //Debug.Log("GAME BOARD SELECTED OFF");
                glow.SetActive(false);
            }
        }
    }
}