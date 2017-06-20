using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy {
	public class BoardSelectionManager : MonoBehaviour {

		public GameObject createGameGameboardGrid;
		public GameObject miniBoardPrefab1;

        private static BoardSelectionManager _instance;
        public static BoardSelectionManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<BoardSelectionManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

		public void LoadMiniBoards() {
			TokenBoard[] boards = TokenBoardLoader.instance.GetAllTokenBoards();
			
			 if (createGameGameboardGrid.transform.childCount > 0) {
				for (int i = createGameGameboardGrid.transform.childCount-1; i >= 0; i--)
				{
					Transform board = createGameGameboardGrid.transform.GetChild(i);
					DestroyImmediate(board.gameObject);
					// Transform piece = createGameGameboardGrid.transform.GetChild(i);
					//Lean.LeanPool.Despawn(piece.gameObject);
				}
			}

			// Create Random Miniboard
			GameObject random = Instantiate(miniBoardPrefab1) as GameObject;
			random.transform.localScale = new Vector3(1,1,1);
			MiniGameBoard miniGameBoardr = random.GetComponent<MiniGameBoard>();
			miniGameBoardr.SetAsRandom();
			random.gameObject.transform.SetParent(createGameGameboardGrid.transform);
			
			var toggler = random.GetComponentInChildren<Toggle>();
			ToggleGroup tgr = createGameGameboardGrid.GetComponent<ToggleGroup>();
			toggler.group = tgr;
			toggler.isOn = true;

			foreach (var board in boards)
			{
				GameObject go = Instantiate(miniBoardPrefab1) as GameObject;
				go.transform.localScale = new Vector3(1,1,1);
				MiniGameBoard miniGameBoard = go.GetComponent<MiniGameBoard>();
				miniGameBoard.tokenBoard = board;
				miniGameBoard.CreateTokens();
				
				go.gameObject.transform.SetParent(createGameGameboardGrid.transform);
				
				var toggle = go.GetComponentInChildren<Toggle>();
				ToggleGroup tg = createGameGameboardGrid.GetComponent<ToggleGroup>();
				toggle.group = tg;
			}

			createGameGameboardGrid.transform.Translate(1900f,0,0);
		}
	}
}
