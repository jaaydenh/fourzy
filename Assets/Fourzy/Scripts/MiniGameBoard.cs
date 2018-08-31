using UnityEngine;
using UnityEngine.UI;

namespace Fourzy {
    public class MiniGameBoard : MonoBehaviour {

        public delegate void SetTokenBoard(string tokenBoardId);
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
        public GameObject fruitToken;
        public GameObject fruitTreeToken;
        public GameObject webToken;
        public GameObject spiderToken;
        public GameObject sandToken;
        public GameObject waterToken;

        public GameObject glow;
        public GameObject questionMark;
        private GameObject tokens;
        private GameObject gamePieces;
        public Sprite Player1Piece;
        public Sprite Player2Piece;
        public GameObject gamePiecePrefab;

        public void SetToggleGroup() {
            Transform parent = this.GetComponentInParent<Transform>();

            var toggle = this.GetComponent<Toggle>();
            toggle.group = parent.gameObject.GetComponent<ToggleGroup>();
        }

        public void CreateGamePieces() {
            gamePieces = new GameObject("GamePieces");
            gamePieces.transform.parent = gameObject.transform;

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = tokenBoard.initialGameBoard[row * Constants.numRows + col];

                    if (piece == (int)Piece.BLUE)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, PlayerEnum.ONE);
                        // SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        // Color c = pieceSprite.color;
                        // c.a = 0.0f;
                        // pieceSprite.color = c;

                        // gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, PlayerEnum.TWO);
                        // SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        // Color c = pieceSprite.color;
                        // c.a = 0.0f;
                        // pieceSprite.color = c;

                        // gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                }
            }

            gamePieces.transform.localPosition = new Vector3(-1.63f,1.63f);
            gamePieces.transform.localScale = new Vector3(0.46f,0.46f,1);
        }

        GameObject SpawnPiece(float posX, float posY, PlayerEnum player)
        {
            GameObject gamePiece = Instantiate(gamePiecePrefab, new Vector3(posX, posY, -5),
                Quaternion.identity, gamePieces.transform) as GameObject;
            
            // gamePiece.transform.position = new Vector3(xPos, yPos, 10);

            if (player == PlayerEnum.ONE) {
                gamePiece.GetComponent<SpriteRenderer>().sprite = Player1Piece;
            } else {
                gamePiece.GetComponent<SpriteRenderer>().sprite = Player2Piece;
            }
            gamePiece.GetComponent<SpriteRenderer>().enabled = true;

            return gamePiece;
        }

        public void CreateTokens() {

            tokens = new GameObject("Tokens");
            tokens.layer = 5;
            tokens.transform.parent = gameObject.transform;

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    Token token = (Token)tokenBoard.tokenBoard[row, col];
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
                        case Token.FRUIT:
                            go = Instantiate(fruitToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.FRUIT_TREE:
                            go = Instantiate(fruitTreeToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.WEB:
                            go = Instantiate(webToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.SPIDER:
                            go = Instantiate(spiderToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.SAND:
                            go = Instantiate(sandToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        case Token.WATER:
                            go = Instantiate(waterToken, new Vector3(col, row * -1, -5), Quaternion.identity, tokens.transform);
                            break;
                        default:
                            break;
                    }
                }
            }

            tokens.transform.localPosition = new Vector3(-1.68f,1.68f,4);
            tokens.transform.localScale = new Vector3(0.48f,0.48f,1);
        }

        public void SetAsRandom() {
            questionMark.SetActive(true);
        }

        public void BoardSelect() {
            // Debug.Log("BoardSelect");
            Toggle toggle = this.GetComponentInChildren<Toggle>();

            if (toggle.isOn) {
                Debug.Log("GAME BOARD SELECTED ON: tokenboard.id: " + tokenBoard.id);
                glow.SetActive(true);
                if (OnSetTokenBoard != null && tokenBoard != null)
                    OnSetTokenBoard(tokenBoard.id);
            } else {
                //Debug.Log("GAME BOARD SELECTED OFF");
                glow.SetActive(false);
            }
        }
    }
}