//@vadym udod

#if UNITY_EDITOR
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    public class BoardEditor : EditorWindow
    {
        public static BoardEditor instance;

        private static Vector2 tabSize = new Vector2(300f, 0f);
        private static Vector2Int gridBorder = new Vector2Int(80, 80);
        private static Vector2Int gridOrigin = new Vector2Int(400, 80);
        private static Vector2Int gridCellSize = new Vector2Int(42, 42);
        private static Vector2Int gridCellPadding = new Vector2Int(4, 4);

        private static Dictionary<TokenType, Sprite> tokensSprites;
        private static GameBoardDefinition currentBoard;
        private static BoardSpaceData selectedBoardSpaceData;
        private static Vector2Int selectedBoardSpaceLocation;
        private static int selectedToken;
        private static int selectedInitialMove;
        private static TokenType prevTokenType;
        private static string selectedPath;

        private static TokensDataHolder tokensData;
        private static TokensDataHolder tempTokensData;
        private static Vector2 gridSize;
        private static Vector2 windowSize;
        private static Vector2 initialMoveButtonSize;
        private static Vector2 initialMoveButtonPadding;

        public static Texture player1GamePieceTexture;
        public static Texture player2GamePieceTexture;
        public static Texture[] arrowGraphics;

        public static GUIStyle defaultButtonsStyle;
        public static GUIStyle selectedButtonStyle;
        public static GUIStyle selectedInitialMoveBoxStyle;

        private Vector2 tokensScrollViewPosition;
        private Vector2 initialMovesScrollViewPosition;

        [MenuItem("Window/Board Editor")]
        public static void ShowWindow()
        {
            instance = GetWindow<BoardEditor>("Board Editor");

            initialMoveButtonSize = new Vector2(gridCellSize.x * .5f, gridCellSize.y * .5f);
            initialMoveButtonPadding = new Vector2((gridCellSize.x - initialMoveButtonSize.x) * .5f, (gridCellSize.y - initialMoveButtonSize.y) * .5f);
            gridSize = new Vector2(8 * (gridCellSize.x + gridCellPadding.x) - gridCellPadding.x, 8 * (gridCellSize.y + gridCellPadding.y) - gridCellPadding.y);
            windowSize = new Vector2(gridOrigin.x + gridSize.x + gridBorder.x, gridOrigin.y + gridSize.y + gridBorder.y);
            instance.minSize = instance.maxSize = windowSize;

            defaultButtonsStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft };
            selectedButtonStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, contentOffset = Vector2.right * 20f };
            selectedInitialMoveBoxStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };

            string[] guids = AssetDatabase.FindAssets("Player1BoardEditorGamePieceTexture");
            player1GamePieceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));

            guids = AssetDatabase.FindAssets("Player2BoardEditorGamePieceTexture");
            player2GamePieceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));

            //load arrows
            arrowGraphics = new Texture[4];
            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                guids = AssetDatabase.FindAssets("board_editor_arrow" + directionIndex);
                arrowGraphics[directionIndex] = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            guids = AssetDatabase.FindAssets("DefaultTokensDataHolder");

            if (guids != null && guids.Length > 0)
            {
                tokensData = AssetDatabase.LoadAssetAtPath<TokensDataHolder>(AssetDatabase.GUIDToAssetPath(guids[0]));

                LoadTokensData(tokensData);
            }
        }

        public void OnGUI()
        {
            //get current event
            Event @event = Event.current;

            if (currentBoard != null)
            {
                switch (@event.type)
                {
                    case EventType.Repaint:
                        instance.Repaint();

                        //draw grid
                        for (int row = 0; row < 8; row++)
                        {
                            for (int col = 0; col < 8; col++)
                            {
                                Color prev = GUI.color;

                                if (selectedBoardSpaceData != null && row == selectedBoardSpaceLocation.y && col == selectedBoardSpaceLocation.x)
                                    GUI.color = Color.yellow;

                                GUI.Box(new Rect(
                                    gridOrigin.x + (col * (gridCellSize.x + gridCellPadding.x)),
                                    gridOrigin.y + (row * (gridCellSize.y + gridCellPadding.y)),
                                    gridCellSize.x,
                                    gridCellSize.y), "");

                                Rect position = new Rect(
                                            gridOrigin.x + (col * (gridCellSize.x + gridCellPadding.x)) + gridCellPadding.x * .5f,
                                            gridOrigin.y + (row * (gridCellSize.y + gridCellPadding.y)) + gridCellPadding.y * .5f,
                                            gridCellSize.x - gridCellPadding.x,
                                            gridCellSize.y - gridCellPadding.y);

                                if (currentBoard.BoardSpaceData[row * 8 + col].T != null)
                                    foreach (string tokenValue in currentBoard.BoardSpaceData[row * 8 + col].T)
                                    {
                                        Texture textureToSet = null;

                                        if (tokensSprites.ContainsKey(tokenValue.StringToToken()))
                                            switch (tokenValue.StringToToken())
                                            {
                                                case TokenType.ARROW:
                                                    textureToSet = arrowGraphics[(int)TokenConstants.IdentifyDirection(tokenValue.Substring(1))];
                                                    break;

                                                default:
                                                    textureToSet = tokensSprites[tokenValue.StringToToken()].texture;
                                                    break;
                                            }
                                        else
                                            textureToSet = tokensSprites[TokenType.NONE].texture;

                                        GUI.DrawTexture(position, textureToSet, ScaleMode.ScaleToFit);
                                    }

                                switch (currentBoard.BoardSpaceData[row * 8 + col].P)
                                {
                                    case "1":
                                        GUI.DrawTexture(position, player1GamePieceTexture, ScaleMode.ScaleToFit);
                                        break;

                                    case "2":
                                        GUI.DrawTexture(position, player2GamePieceTexture, ScaleMode.ScaleToFit);
                                        break;
                                }

                                GUI.color = prev;
                            }
                        }

                        //draw initial moves buttons
                        for (int initialMoveIndex = 0; initialMoveIndex < currentBoard.InitialMoves.Count; initialMoveIndex++)
                        {
                            Color prev = GUI.color;

                            if (selectedInitialMove == initialMoveIndex)
                                GUI.color = Color.green;
                            else
                            {
                                if (currentBoard.InitialMoves[initialMoveIndex].Piece.PlayerId == (int)PlayerEnum.ONE)
                                    GUI.color = Color.red;
                                else
                                    GUI.color = Color.blue;
                            }

                            GUI.Box(GetInitialMoveBoxRect(currentBoard.InitialMoves[initialMoveIndex]), initialMoveIndex + "");

                            GUI.color = prev;
                        }
                        break;

                    case EventType.MouseDown:
                        if (@event.mousePosition.x < tabSize.x)
                            break;

                        BoardSpaceData _selectedBoardSpaceData = BoardSpaceDataFromPoint(@event.mousePosition - gridOrigin);

                        if (_selectedBoardSpaceData != null)
                        {
                            if (_selectedBoardSpaceData == selectedBoardSpaceData)
                                selectedBoardSpaceData = null;
                            else
                                selectedBoardSpaceData = _selectedBoardSpaceData;

                            if (selectedBoardSpaceData != null)
                            {
                                selectedBoardSpaceLocation = BoardSpaceDataLocation(selectedBoardSpaceData);

                                if (selectedBoardSpaceData.T != null)
                                {
                                    if (selectedBoardSpaceData.T.Count > 0)
                                        selectedToken = 0;
                                    else
                                        selectedToken = -1;
                                }
                                else
                                    selectedToken = -1;
                            }
                        }
                        //check if we selected initial move box
                        else
                        {
                            int _selectedInitialMove = InitialMoveFromPoint(@event.mousePosition);

                            if (selectedInitialMove > -1)
                                selectedInitialMove = -1;
                            else
                                selectedInitialMove = _selectedInitialMove;
                        }
                        break;
                }
            }

            //draw shapes explorer window
            BeginWindows();

            Rect editorWindowRect = GUILayout.Window(ushort.MaxValue, new Rect(0f, 0f, tabSize.x, tabSize.y), EditorTab, "Editor Tab");

            EndWindows();
        }

        private static void LoadTokensData(TokensDataHolder newData)
        {
            if (newData == null)
                return;

            tokensSprites = newData.GetTokensSprites();
        }

        private static void LoadGameboard(GameBoardDefinition gameboard)
        {
            selectedBoardSpaceData = null;
            selectedToken = -1;
            selectedInitialMove = -1;
        }

        private BoardSpaceData BoardSpaceDataFromPoint(Vector2 position)
        {
            if (position.x < 0f || position.y < 0f || position.x > gridSize.x || position.y > gridSize.y)
                return null;

            Vector2Int location = new Vector2Int(
                Mathf.Clamp(Mathf.FloorToInt(position.x / (gridCellSize.x + gridCellPadding.x)), 0, 7),
                Mathf.Clamp(Mathf.FloorToInt(position.y / (gridCellSize.y + gridCellPadding.y)), 0, 7));

            if (currentBoard != null)
                return currentBoard.BoardSpaceData[Mathf.Clamp(location.y * 8 + location.x, 0, 63)];

            return null;
        }

        private Vector2Int BoardSpaceDataLocation(BoardSpaceData data)
        {
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    if (currentBoard.BoardSpaceData[row * 8 + col] == data)
                        return new Vector2Int(col, row);

            return new Vector2Int(-1, -1);
        }

        private Rect GetInitialMoveBoxRect(SimpleMove move)
        {
            Rect initialMoveBoxRect = new Rect(0f, 0f, initialMoveButtonSize.x, initialMoveButtonSize.y);

            switch (move.Direction)
            {
                case Direction.DOWN:
                    initialMoveBoxRect.x = gridOrigin.x + (move.Location * (gridCellSize.x + gridCellPadding.x)) + initialMoveButtonPadding.x;
                    initialMoveBoxRect.y = gridOrigin.y - GetNextInitilMoveOffset(move);
                    break;

                case Direction.UP:
                    initialMoveBoxRect.x = gridOrigin.x + (move.Location * (gridCellSize.x + gridCellPadding.x)) + initialMoveButtonPadding.x;
                    initialMoveBoxRect.y = gridOrigin.y + gridSize.y + GetNextInitilMoveOffset(move);
                    break;

                case Direction.LEFT:
                    initialMoveBoxRect.x = gridOrigin.x + gridSize.x + GetNextInitilMoveOffset(move);
                    initialMoveBoxRect.y = gridOrigin.y + (move.Location * (gridCellSize.y + gridCellPadding.y)) + initialMoveButtonPadding.y;
                    break;

                case Direction.RIGHT:
                    initialMoveBoxRect.x = gridOrigin.x - GetNextInitilMoveOffset(move);
                    initialMoveBoxRect.y = gridOrigin.y + (move.Location * (gridCellSize.y + gridCellPadding.y)) + initialMoveButtonPadding.y;
                    break;
            }

            return initialMoveBoxRect;
        }

        private int InitialMoveFromPoint(Vector2 position)
        {
            if (currentBoard == null)
                return -1;

            for (int initialMoveIndex = 0; initialMoveIndex < currentBoard.InitialMoves.Count; initialMoveIndex++)
            {
                Rect intialMoveBoxRect = GetInitialMoveBoxRect(currentBoard.InitialMoves[initialMoveIndex]);

                if (position.x >= intialMoveBoxRect.x && position.x <= intialMoveBoxRect.x + intialMoveBoxRect.width &&
                    position.y >= intialMoveBoxRect.y && position.y <= intialMoveBoxRect.y + intialMoveBoxRect.height)

                    return initialMoveIndex;
            }

            return -1;
        }

        private float GetNextInitilMoveOffset(SimpleMove move)
        {
            float stepValue = initialMoveButtonSize.y;
            float offset = stepValue;

            if (move.Direction == Direction.LEFT || move.Direction == Direction.RIGHT)
                stepValue = initialMoveButtonSize.x;

            if (move.Direction == Direction.UP || move.Direction == Direction.LEFT)
                offset = 0f;

            foreach (SimpleMove _move in currentBoard.InitialMoves)
            {
                if (move == _move)
                    return offset;

                if (_move.Direction == move.Direction && _move.Location == move.Location)
                    offset += stepValue;
            }

            return offset;
        }

        //inner window
        private void EditorTab(int id)
        {
            tempTokensData = (TokensDataHolder)EditorGUILayout.ObjectField("Tokens Data", tokensData, typeof(TokensDataHolder), false);

            if (tempTokensData != tokensData)
                LoadTokensData(tempTokensData);

            tokensData = tempTokensData;

            if (tokensData == null)
            {
                GUILayout.Label("No data file selected", EditorStyles.boldLabel);
                return;
            }

            GUILayout.BeginHorizontal("Box");
            {
                //board loader
                if (GUILayout.Button("Load Board", GUILayout.Width(90f)))
                {
                    string path = EditorUtility.OpenFilePanel("Load board", "", "json");

                    if (path.Length != 0)
                    {
                        GameBoardDefinition gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(File.ReadAllText(path));

                        selectedPath = path;
                        currentBoard = gameboard;

                        if (gameboard != null)
                            LoadGameboard(currentBoard);
                    }
                }

                //new/clear board
                if (GUILayout.Button("New board", GUILayout.Width(90f)))
                {
                    currentBoard = new GameBoardDefinition();
                    selectedPath = "";

                    currentBoard.LoadEmpty();

                    LoadGameboard(currentBoard);
                }

                if (currentBoard != null)
                {
                    //save board
                    if (GUILayout.Button("Save board", GUILayout.Width(90f)))
                    {
                        string path = "";

                        if (string.IsNullOrEmpty(selectedPath))
                            path = EditorUtility.SaveFilePanelInProject("Save board", currentBoard.BoardName, "json", "");
                        else
                            path = EditorUtility.SaveFilePanelInProject("Save board", currentBoard.BoardName, "json", "", selectedPath);

                        if (path.Length != 0)
                            File.WriteAllText(path, JsonConvert.SerializeObject(currentBoard));
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (currentBoard != null)
            {
                GUILayout.BeginVertical("Box");
                {
                    currentBoard.BoardName = EditorGUILayout.TextField("Board name", currentBoard.BoardName);
                    currentBoard.ID = EditorGUILayout.TextField("Board ID", currentBoard.ID);
                    currentBoard.Enabled = EditorGUILayout.Toggle("Enabled", currentBoard.Enabled);
                    currentBoard.EnabledGallery = EditorGUILayout.Toggle("Enabled Gallery", currentBoard.EnabledGallery);
                    currentBoard.EnabledRealtime = EditorGUILayout.Toggle("Enabled Realtime", currentBoard.EnabledRealtime);
                    currentBoard.Area = (Area)EditorGUILayout.EnumPopup("Area", currentBoard.Area);
                }
                GUILayout.EndVertical();

                //intial moves list
                GUILayout.Label("Initial moves");
                GUILayout.BeginVertical("Box");
                {
                    initialMovesScrollViewPosition = GUILayout.BeginScrollView(initialMovesScrollViewPosition, GUILayout.Height(60f));
                    {
                        if (currentBoard.InitialMoves != null)
                            for (int moveIndex = 0; moveIndex < currentBoard.InitialMoves.Count; moveIndex++)
                                if (GUILayout.Button(moveIndex + ": Move, D: " + currentBoard.InitialMoves[moveIndex].Direction + ", L: " + currentBoard.InitialMoves[moveIndex].Location + ", P: " + currentBoard.InitialMoves[moveIndex].Piece.PlayerId,
                                    (selectedInitialMove == moveIndex) ? selectedButtonStyle : defaultButtonsStyle))
                                {
                                    if (selectedInitialMove == moveIndex)
                                        selectedInitialMove = -1;
                                    else
                                        selectedInitialMove = moveIndex;
                                }
                    }
                    GUILayout.EndScrollView();

                    GUILayout.BeginHorizontal("Box");
                    {
                        //buttons
                        if (GUILayout.Button("+", GUILayout.Width(30f)))
                        {
                            if (currentBoard.InitialMoves == null)
                                currentBoard.InitialMoves = new List<SimpleMove>();

                            int switchedID = -1;
                            if (currentBoard.InitialMoves.Count > 0)
                            {
                                if (currentBoard.InitialMoves[currentBoard.InitialMoves.Count - 1].Piece.PlayerId == (int)PlayerEnum.ONE)
                                    switchedID = 2;
                                else
                                    switchedID = 1;
                            }

                            currentBoard.InitialMoves.Add(new SimpleMove((switchedID > 0) ? new Piece(switchedID) : new Piece((int)PlayerEnum.ONE), Direction.DOWN, 0));
                            selectedInitialMove = currentBoard.InitialMoves.Count - 1;
                        }

                        if (selectedInitialMove > -1)
                        {
                            if (GUILayout.Button("-", GUILayout.Width(30f)))
                            {
                                currentBoard.InitialMoves.RemoveAt(selectedInitialMove);

                                if (currentBoard.InitialMoves.Count == 0)
                                    selectedInitialMove = -1;
                                else if (selectedInitialMove == currentBoard.InitialMoves.Count)
                                    selectedInitialMove--;
                            }

                            if (selectedInitialMove > 0)
                                if (GUILayout.Button("▲", GUILayout.Width(30f)))
                                {
                                    currentBoard.InitialMoves.MoveItem(selectedInitialMove, selectedInitialMove - 1);
                                    selectedInitialMove--;
                                }

                            if (selectedInitialMove < currentBoard.InitialMoves.Count - 1)
                                if (GUILayout.Button("▼", GUILayout.Width(30f)))
                                {
                                    currentBoard.InitialMoves.MoveItem(selectedInitialMove, selectedInitialMove + 1);
                                    selectedInitialMove++;
                                }
                        }
                    }
                    GUILayout.EndHorizontal();

                    //extra buttons
                    if (selectedInitialMove > -1)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            currentBoard.InitialMoves[selectedInitialMove].Piece.PlayerId =
                                (int)((PlayerEnum)EditorGUILayout.EnumPopup("Game piece", (PlayerEnum)currentBoard.InitialMoves[selectedInitialMove].Piece.PlayerId));

                            currentBoard.InitialMoves[selectedInitialMove].Direction = (Direction)EditorGUILayout.EnumPopup("Direction", currentBoard.InitialMoves[selectedInitialMove].Direction);
                            currentBoard.InitialMoves[selectedInitialMove].Location = EditorGUILayout.IntPopup(
                                "Location",
                                currentBoard.InitialMoves[selectedInitialMove].Location,
                                new string[] { "0", "1", "2", "3", "4", "5", "6", "7", },
                                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, });
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();

                //tokens list
                if (selectedBoardSpaceData != null)
                {
                    GUILayout.Label("Tokens");

                    PlayerEnum playerValue;

                    if (!string.IsNullOrEmpty(selectedBoardSpaceData.P) && (selectedBoardSpaceData.P.Substring(0, 1) == "1" || selectedBoardSpaceData.P.Substring(0, 1) == "2"))
                        playerValue = selectedBoardSpaceData.P.Substring(0, 1) == "1" ? PlayerEnum.ONE : PlayerEnum.TWO;
                    else
                        playerValue = PlayerEnum.NONE;
                    
                    PlayerEnum selectedValue = (PlayerEnum)EditorGUILayout.EnumPopup("Game piece", playerValue);

                    if (selectedValue == PlayerEnum.ONE || selectedValue == PlayerEnum.TWO)
                        selectedBoardSpaceData.P = selectedValue == PlayerEnum.ONE ? "1" : "2";
                    else
                        selectedBoardSpaceData.P = null;

                    GUILayout.BeginVertical("Box");
                    {
                        tokensScrollViewPosition = GUILayout.BeginScrollView(tokensScrollViewPosition, GUILayout.Height(40f));
                        {
                            if (selectedBoardSpaceData.T != null)
                                for (int i = 0; i < selectedBoardSpaceData.T.Count; i++)
                                    if (GUILayout.Button(i + ": Token, : " + selectedBoardSpaceData.T[i], (selectedToken == i) ? selectedButtonStyle : defaultButtonsStyle))
                                    {
                                        if (selectedToken == i)
                                            selectedToken = -1;
                                        else
                                        {
                                            selectedToken = i;
                                            prevTokenType = TokenType.NONE;
                                        }
                                    }
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();

                    if (selectedToken > -1)
                    {
                        TokenType tokenType = (TokenType)EditorGUILayout.EnumPopup("Token type", (System.Enum)selectedBoardSpaceData.T[selectedToken].StringToToken());

                        //if token have extra data to set
                        switch (tokenType)
                        {
                            case TokenType.ARROW:
                                if (selectedBoardSpaceData.T[selectedToken].StringToToken() != TokenType.ARROW)
                                    selectedBoardSpaceData.T[selectedToken] = tokenType.TokenTypeToString() + TokenConstants.DirectionString(Direction.UP);

                                Direction directionEnum =
                                    (Direction)EditorGUILayout.EnumPopup("Arrow Direction", (System.Enum)TokenConstants.IdentifyDirection(selectedBoardSpaceData.T[selectedToken].Substring(1)));

                                if ((int)directionEnum < 4)
                                    selectedBoardSpaceData.T[selectedToken] = selectedBoardSpaceData.T[selectedToken][0] + directionEnum.ToString();
                                break;

                            default:
                                selectedBoardSpaceData.T[selectedToken] = tokenType.TokenTypeToString();
                                break;
                        }

                        prevTokenType = tokenType;
                    }

                    GUILayout.BeginHorizontal("Box");
                    {
                        if (GUILayout.Button("+", GUILayout.Width(30f)))
                        {
                            if (selectedBoardSpaceData.T == null)
                                selectedBoardSpaceData.T = new List<string>();

                            selectedBoardSpaceData.T.Add("B");
                            selectedToken = selectedBoardSpaceData.T.Count - 1;
                        }

                        if (selectedToken > -1)
                        {
                            if (GUILayout.Button("-", GUILayout.Width(30f)))
                            {
                                selectedBoardSpaceData.T.RemoveAt(selectedToken);

                                if (selectedBoardSpaceData.T.Count == 0)
                                    selectedToken = -1;
                                else if (selectedToken == selectedBoardSpaceData.T.Count)
                                    selectedToken--;
                            }

                            if (selectedToken > 0)
                                if (GUILayout.Button("▲", GUILayout.Width(30f)))
                                {
                                    selectedBoardSpaceData.T.MoveItem(selectedToken, selectedToken - 1);
                                    selectedToken--;
                                }

                            if (selectedToken < selectedBoardSpaceData.T.Count - 1)
                                if (GUILayout.Button("▼", GUILayout.Width(30f)))
                                {
                                    selectedBoardSpaceData.T.MoveItem(selectedToken, selectedToken + 1);
                                    selectedToken++;
                                }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
#endif