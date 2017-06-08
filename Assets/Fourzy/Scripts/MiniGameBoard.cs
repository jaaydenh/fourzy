using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {


    public class MiniGameBoard : MonoBehaviour {

        // Use this for initialization
        void Start () {
            
        }
	
        // public void CreateTokens() {
        //     tokenViews = new List<GameObject>();

        //     for(int row = 0; row < Constan numRows; row++)
        //     {
        //         for(int col = 0; col < numColumns; col++)
        //         {
        //             Token token = tokenBoard.tokens[row, col].tokenType;
        //             GameObject go;
        //             switch (token)
        //             {
        //                 case Token.UP_ARROW:
        //                     go = Instantiate(upArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.DOWN_ARROW:
        //                     go = Instantiate(downArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.LEFT_ARROW:
        //                     go = Instantiate(leftArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.RIGHT_ARROW:
        //                     go = Instantiate(rightArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.STICKY:
        //                     go = Instantiate(stickyToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.BLOCKER:
        //                     go = Instantiate(blockerToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 case Token.GHOST:
        //                     go = Instantiate(ghostToken, new Vector3(col, row * -1, 5), Quaternion.identity, gamePieces.transform);
        //                     Utility.SetSpriteAlpha(go, 0.0f);
        //                     tokenViews.Add(go);
        //                     break;
        //                 default:
        //                     break;
        //             }
        //         }
        //     }
        // }
    }
}