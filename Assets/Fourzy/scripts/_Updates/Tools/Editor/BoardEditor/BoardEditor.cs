//@vadym udod

#if UNITY_EDITOR
using Fourzy._Updates.Serialized;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string[] rotatingArrowFrequencyDisplayOptions = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9 " };
        public static int[] rotatingArrowFrequencyOptions = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static string[] rotatingArrowCountdownDisplayOptions = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static int[] rotatingArrowCountdownOptions = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        private static Dictionary<TokenType, Sprite> tokensSprites;
        private static GameBoardDefinition currentBoard;
        private static List<BoardSpaceWrapper> selectedBoardSpaceData;
        private static List<BoardSpaceTokenWrapper> commonTokens;
        private static int selectedTokenIndex;
        private static int selectedInitialMove;
        private static TokenType prevTokenType;
        private static string selectedPath;
        private static string selectedFileName;

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

        private static bool pointerInsideTab;

        private Vector2 tokensScrollViewPosition;
        private Vector2 initialMovesScrollViewPosition;
        public static int PuzzleSearchDepth = 3;

        [MenuItem("Window/Board Editor")]
        public static void ShowWindow()
        {
            instance = GetWindow<BoardEditor>("Board Editor");

            initialMoveButtonSize = new Vector2(gridCellSize.x * .5f, gridCellSize.y * .5f);
            initialMoveButtonPadding = new Vector2((gridCellSize.x - initialMoveButtonSize.x) * .5f, (gridCellSize.y - initialMoveButtonSize.y) * .5f);
            gridSize = new Vector2(8 * (gridCellSize.x + gridCellPadding.x) - gridCellPadding.x, 8 * (gridCellSize.y + gridCellPadding.y) - gridCellPadding.y);
            windowSize = new Vector2(gridOrigin.x + gridSize.x + gridBorder.x, gridOrigin.y + gridSize.y + gridBorder.y + 50);
            instance.minSize = instance.maxSize = windowSize;

            defaultButtonsStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft };
            selectedButtonStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, contentOffset = Vector2.right * 20f };
            selectedInitialMoveBoxStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };

            string[] guids = AssetDatabase.FindAssets("Player1BoardEditorGamePieceTexture");
            player1GamePieceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));

            guids = AssetDatabase.FindAssets("Player2BoardEditorGamePieceTexture");
            player2GamePieceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));

            selectedBoardSpaceData = new List<BoardSpaceWrapper>();
            commonTokens = new List<BoardSpaceTokenWrapper>();

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

            //draw explorere tab
            BeginWindows();

            Rect editorWindowRect = GUILayout.Window(ushort.MaxValue, new Rect(0f, 0f, tabSize.x, tabSize.y), EditorTab, "Editor Tab");

            if (@event.type == EventType.Repaint)
                pointerInsideTab = @event.mousePosition.x >= editorWindowRect.x && @event.mousePosition.y >= editorWindowRect.y && @event.mousePosition.x <= editorWindowRect.size.x && @event.mousePosition.y <= editorWindowRect.size.y;

            EndWindows();

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

                                BoardSpaceWrapper spaceData = selectedBoardSpaceData.Find(i => i.location.y == row && i.location.x == col);

                                if (spaceData != null)
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
                                                case TokenType.ROTATING_ARROW:
                                                    textureToSet = arrowGraphics[(int)TokenConstants.IdentifyDirection(tokenValue.Substring(1, 1))];
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

                        if (Event.current.control)
                        {
                            if (_selectedBoardSpaceData != null)
                            {
                                BoardSpaceWrapper spaceData = selectedBoardSpaceData.Find(i => i.data == _selectedBoardSpaceData);

                                if (spaceData != null)
                                    selectedBoardSpaceData.Remove(spaceData);
                                else
                                    selectedBoardSpaceData.Add(new BoardSpaceWrapper() { data = _selectedBoardSpaceData, location = BoardSpaceDataLocation(_selectedBoardSpaceData), });

                                if (_selectedBoardSpaceData != null)
                                {
                                    if (_selectedBoardSpaceData.T != null)
                                    {
                                        if (_selectedBoardSpaceData.T.Count > 0)
                                            selectedTokenIndex = 0;
                                        else
                                            selectedTokenIndex = -1;
                                    }
                                    else
                                        selectedTokenIndex = -1;
                                }

                                UpdateCommonTokens();
                            }
                        }
                        else
                        {
                            if (_selectedBoardSpaceData != null)
                            {
                                bool selected = false;
                                if (selectedBoardSpaceData.Count > 0)
                                    selected = selectedBoardSpaceData[0].data == _selectedBoardSpaceData;

                                selectedBoardSpaceData.Clear();

                                if (!selected)
                                    selectedBoardSpaceData.Add(new BoardSpaceWrapper() { data = _selectedBoardSpaceData, location = BoardSpaceDataLocation(_selectedBoardSpaceData), });

                                //if (spaceData != null)
                                //    selectedBoardSpaceData.Remove(spaceData);
                                //else
                                //    selectedBoardSpaceData.Add(new BoardSpaceWrapper() { data = _selectedBoardSpaceData, location = BoardSpaceDataLocation(_selectedBoardSpaceData), });

                                if (_selectedBoardSpaceData != null)
                                {
                                    if (_selectedBoardSpaceData.T != null)
                                    {
                                        if (_selectedBoardSpaceData.T.Count > 0)
                                            selectedTokenIndex = 0;
                                        else
                                            selectedTokenIndex = -1;
                                    }
                                    else
                                        selectedTokenIndex = -1;
                                }

                                UpdateCommonTokens();
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
                        }
                        break;

                    case EventType.KeyDown:
                        switch (@event.keyCode)
                        {
                            case KeyCode.Alpha1:
                                if (!pointerInsideTab)
                                {
                                    foreach (BoardSpaceWrapper boardSpace in selectedBoardSpaceData)
                                        boardSpace.data.P = "1";
                                }

                                break;

                            case KeyCode.Alpha2:
                                if (!pointerInsideTab)
                                {
                                    foreach (BoardSpaceWrapper boardSpace in selectedBoardSpaceData)
                                        boardSpace.data.P = "2";
                                }

                                break;

                            case KeyCode.Delete:
                            case KeyCode.D:
                                if (!pointerInsideTab && @event.control)
                                {
                                    foreach (BoardSpaceWrapper boardSpace in selectedBoardSpaceData)
                                    {
                                        boardSpace.data.P = null;
                                        boardSpace.data.T = null;

                                        selectedTokenIndex = -1;
                                    }
                                }

                                return;
                        }

                        break;
                }
            }
        }

        private static void LoadTokensData(TokensDataHolder newData)
        {
            if (newData == null)
                return;

            tokensSprites = newData.GetTokensSprites();
        }

        private static void LoadGameboard(GameBoardDefinition gameboard)
        {
            selectedBoardSpaceData = new List<BoardSpaceWrapper>();
            selectedTokenIndex = -1;
            selectedInitialMove = -1;

            if (string.IsNullOrEmpty(gameboard.ID))
                gameboard.ID = Guid.NewGuid().ToString();
        }

        private static void UpdateCommonTokens()
        {
            if (selectedBoardSpaceData.Count > 0 && selectedBoardSpaceData[0].data.T != null)
            {
                commonTokens.Clear();

                foreach (string token in selectedBoardSpaceData[0].data.T)
                {
                    if (selectedBoardSpaceData.All(i => i.data.T != null && i.data.T.Contains(token)))
                        commonTokens.Add(new BoardSpaceTokenWrapper() { token = token, indicies = selectedBoardSpaceData.Select(boardSpace => boardSpace.data.T.IndexOf(token)).ToList(), });
                }
            }
            else
                commonTokens.Clear();

            if (selectedTokenIndex > -1)
            {
                if (commonTokens.Count == 0)
                    selectedTokenIndex = -1;
                else if (selectedTokenIndex >= commonTokens.Count)
                    selectedTokenIndex = commonTokens.Count - 1;
            }
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
                        selectedFileName = Path.GetFileName(path);
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
                    ////save board
                    if (GUILayout.Button("Save board", GUILayout.Width(90f)))
                    {
                        string path = "";

                        if (string.IsNullOrEmpty(selectedPath))
                            path = EditorUtility.SaveFilePanelInProject("Save board", currentBoard.BoardName, "json", "");
                        else
                            path = EditorUtility.SaveFilePanelInProject("Save board", selectedFileName, "json", "", selectedPath);

                        if (path.Length != 0)
                            File.WriteAllText(path, JsonConvert.SerializeObject(currentBoard));
                    }

                    if (GUILayout.Button("Save puzzle", GUILayout.Width(90f)))
                    {
                        string path = "";

                        if (string.IsNullOrEmpty(selectedPath))
                            path = EditorUtility.SaveFilePanelInProject("Save puzzle", currentBoard.BoardName, "json", "Assets/Fourzy/Resources/PuzzleDrafts");
                        else
                            path = EditorUtility.SaveFilePanelInProject("Save puzzle", selectedFileName, "json", "", selectedPath);

                        if (path.Length != 0)
                        {
                            GameState GS = new GameState(currentBoard,1);
                            PuzzleTestResults R = PuzzleTestTools.EvaluateState(GS, PuzzleSearchDepth);
                            if (R.NumberOfVictoryPaths > 0)
                            {
                                FourzyPuzzleData PData = new FourzyPuzzleData(GS, R);
                                File.WriteAllText(path, JsonConvert.SerializeObject(PData));
                            }
                            else
                            {
                                bool b = EditorUtility.DisplayDialog("Problem With Puzzle. Puzzle Not Created.",
                                    "No Solutions Found.", "", "Ok");
                            }
                        }
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
                    if (GUILayout.Button("Reset ID"))
                        currentBoard.ID = Guid.NewGuid().ToString();
                    currentBoard.Enabled = EditorGUILayout.Toggle("Enabled", currentBoard.Enabled);
                    currentBoard.EnabledGallery = EditorGUILayout.Toggle("Enabled Gallery", currentBoard.EnabledGallery);
                    currentBoard.EnabledRealtime = EditorGUILayout.Toggle("Enabled Realtime", currentBoard.EnabledRealtime);
                    currentBoard.Area = (Area)EditorGUILayout.EnumPopup("Area", currentBoard.Area);
                    PuzzleSearchDepth = int.Parse(EditorGUILayout.TextField("PuzzleSearchDepth", PuzzleSearchDepth.ToString()));

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
                if (selectedBoardSpaceData.Count > 0)
                {
                    PlayerEnum playerValue;

                    if (!string.IsNullOrEmpty(selectedBoardSpaceData[0].data.P))
                    {
                        if (selectedBoardSpaceData.All(boardSpace => !string.IsNullOrEmpty(boardSpace.data.P)))
                        {
                            if (selectedBoardSpaceData.All(boardSpace => boardSpace.data.P.Substring(0, 1) == "1"))
                                playerValue = PlayerEnum.ONE;
                            else if (selectedBoardSpaceData.All(boardSpace => boardSpace.data.P.Substring(0, 1) == "2"))
                                playerValue = PlayerEnum.TWO;
                            else
                                playerValue = PlayerEnum.NONE;
                        }
                        else
                            playerValue = PlayerEnum.NONE;
                    }
                    else
                        playerValue = PlayerEnum.NONE;

                    PlayerEnum selectedValue = (PlayerEnum)EditorGUILayout.EnumPopup("Game piece", playerValue);

                    if (selectedValue == PlayerEnum.ONE || selectedValue == PlayerEnum.TWO || selectedValue == PlayerEnum.NONE)
                    {
                        foreach (BoardSpaceWrapper boardSpace in selectedBoardSpaceData)
                        {
                            if (selectedValue == PlayerEnum.ONE || selectedValue == PlayerEnum.TWO)
                                boardSpace.data.P = selectedValue == PlayerEnum.ONE ? "1" : "2";
                            else
                                boardSpace.data.P = null;
                        }
                    }

                    GUILayout.Label("Tokens");

                    GUILayout.BeginVertical("Box");
                    {
                        tokensScrollViewPosition = GUILayout.BeginScrollView(tokensScrollViewPosition, GUILayout.Height(40f));
                        {
                            //only display common tokens
                            if (commonTokens.Count > 0)
                                for (int i = 0; i < commonTokens.Count; i++)
                                    if (GUILayout.Button(i + ": Token, : " + commonTokens[i].token, (selectedTokenIndex == i) ? selectedButtonStyle : defaultButtonsStyle))
                                    {
                                        if (selectedTokenIndex == i)
                                            selectedTokenIndex = -1;
                                        else
                                        {
                                            selectedTokenIndex = i;
                                            prevTokenType = TokenType.NONE;
                                        }
                                    }
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();

                    if (selectedTokenIndex > -1)
                    {
                        TokenType tokenType = (TokenType)EditorGUILayout.EnumPopup("Token type", (System.Enum)commonTokens[selectedTokenIndex].token.StringToToken());
                        Direction directionEnum = Direction.NONE;
                        Rotation rotationEnum = Rotation.NONE;
                        MoveMethod moveMethodEnum = MoveMethod.NONE;
                        int countdown = 0;
                        int frequency = 0;
                        bool check = false;

                        //if token have extra data to set
                        switch (tokenType)
                        {
                            case TokenType.ARROW:
                                //if selected token isnt set to 'tokenType', but 'tokenType' was selected, set current token to 'tokenType' on all selected tiles
                                if (commonTokens[selectedTokenIndex].token.StringToToken() != TokenType.ARROW)
                                {
                                    commonTokens[selectedTokenIndex].token = tokenType.TokenTypeToString() + TokenConstants.DirectionString(Direction.UP);

                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] =
                                            tokenType.TokenTypeToString() + TokenConstants.DirectionString(Direction.UP);
                                }

                                //display direction popup
                                directionEnum = (Direction)EditorGUILayout.EnumPopup("Arrow Direction", TokenConstants.IdentifyDirection(commonTokens[selectedTokenIndex].token.Substring(1)));

                                //only accept left/down/up/right
                                if ((int)directionEnum < 4)
                                {
                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                    {
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] =
                                            tokenType.TokenTypeToString() 
                                            + TokenConstants.DirectionString(directionEnum);
                                    }
                                }
                                break;

                            case TokenType.ROTATING_ARROW:
                                //if selected token isnt set to 'tokenType', but 'tokenType' was selected, set current token to 'tokenType' on all selected tiles
                                if (commonTokens[selectedTokenIndex].token.StringToToken() != TokenType.ROTATING_ARROW)
                                {
                                    string notation =
                                        tokenType.TokenTypeToString()
                                        + TokenConstants.DirectionString(Direction.UP)
                                        + TokenConstants.NotateRotation(Rotation.CLOCKWISE)
                                        + 1
                                        + 0;

                                    commonTokens[selectedTokenIndex].token = notation;

                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] = notation;
                                }

                                //display direction popup
                                directionEnum = (Direction)EditorGUILayout.EnumPopup("Arrow Direction", TokenConstants.IdentifyDirection(commonTokens[selectedTokenIndex].token.Substring(1, 1)));

                                //display rotation enum
                                rotationEnum = (Rotation)EditorGUILayout.EnumPopup("Arrow Rotation", TokenConstants.GetRotation(commonTokens[selectedTokenIndex].token.Substring(2, 1)[0]));
                                
                                frequency = EditorGUILayout.IntPopup(
                                    "Frequency",
                                    int.Parse(commonTokens[selectedTokenIndex].token.Substring(3, 1)), 
                                    rotatingArrowFrequencyDisplayOptions, 
                                    rotatingArrowFrequencyOptions);

                                countdown = EditorGUILayout.IntPopup(
                                    "Countdown", 
                                    int.Parse(commonTokens[selectedTokenIndex].token.Substring(4, 1)), 
                                    rotatingArrowCountdownDisplayOptions, 
                                    rotatingArrowCountdownOptions);

                                //only accept left/down/up/right and correcnt rotation
                                if ((int)directionEnum < 4 && (int)rotationEnum < 2)
                                {
                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                    {
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] =
                                            tokenType.TokenTypeToString()
                                            + TokenConstants.DirectionString(directionEnum)
                                            + TokenConstants.NotateRotation(rotationEnum)
                                            + frequency
                                            + countdown;
                                    }
                                }
                                break;

                            case TokenType.MOVING_GHOST:
                                //if selected token isnt set to 'tokenType', but 'tokenType' was selected, set current token to 'tokenType' on all selected tiles
                                if (commonTokens[selectedTokenIndex].token.StringToToken() != TokenType.MOVING_GHOST)
                                {
                                    string notation =
                                        tokenType.TokenTypeToString()
                                        + TokenConstants.DirectionString(Direction.UP)
                                        + TokenConstants.MoveString(MoveMethod.CLOCKWISE)
                                        + 0
                                        + 1
                                        + 0;

                                    commonTokens[selectedTokenIndex].token = notation;

                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] = notation;
                                }
                                
                                //display direction popup
                                directionEnum = (Direction)EditorGUILayout.EnumPopup("Direction", TokenConstants.IdentifyDirection(commonTokens[selectedTokenIndex].token.Substring(1, 1)));

                                //display move method popup
                                moveMethodEnum = (MoveMethod)EditorGUILayout.EnumPopup("Move Method", TokenConstants.IdentifyMoveMethod(commonTokens[selectedTokenIndex].token.Substring(2, 1)));

                                countdown = EditorGUILayout.IntPopup(
                                    "Countdown",
                                    int.Parse(commonTokens[selectedTokenIndex].token.Substring(3, 1)),
                                    rotatingArrowCountdownDisplayOptions,
                                    rotatingArrowCountdownOptions);

                                frequency = EditorGUILayout.IntPopup(
                                    "Frequency",
                                    int.Parse(commonTokens[selectedTokenIndex].token.Substring(4, 1)),
                                    rotatingArrowFrequencyDisplayOptions,
                                    rotatingArrowFrequencyOptions);

                                check = EditorGUILayout.Toggle("Tired", (commonTokens[selectedTokenIndex].token.Substring(5, 1) == "1" ? true : false));


                                //only accept left/down/up/right and correcnt moveMethod
                                if ((int)directionEnum < 4 && (int)moveMethodEnum < 4)
                                {
                                    for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                    {
                                        selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] =
                                            tokenType.TokenTypeToString()
                                            + TokenConstants.DirectionString(directionEnum)
                                            + TokenConstants.MoveString(moveMethodEnum)
                                            + countdown
                                            + frequency
                                            + (check ? "1" : "0");
                                    }
                                }
                                break;

                            default:
                                for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                    selectedBoardSpaceData[boardSpaceIndex].data.T[commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]] = tokenType.TokenTypeToString();
                                break;
                        }

                        UpdateCommonTokens();
                        prevTokenType = tokenType;
                    }

                    GUILayout.BeginHorizontal("Box");
                    {
                        if (GUILayout.Button("+", GUILayout.Width(30f)))
                        {
                            for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                            {
                                if (selectedBoardSpaceData[boardSpaceIndex].data.T == null)
                                    selectedBoardSpaceData[boardSpaceIndex].data.T = new List<string>();

                                selectedBoardSpaceData[boardSpaceIndex].data.T.Add("B");
                                selectedTokenIndex = selectedBoardSpaceData[boardSpaceIndex].data.T.Count - 1;
                            }

                            UpdateCommonTokens();
                        }

                        if (selectedTokenIndex > -1)
                        {
                            if (GUILayout.Button("-", GUILayout.Width(30f)))
                            {
                                for (int boardSpaceIndex = 0; boardSpaceIndex < selectedBoardSpaceData.Count; boardSpaceIndex++)
                                {
                                    selectedBoardSpaceData[boardSpaceIndex].data.T.RemoveAt(commonTokens[selectedTokenIndex].indicies[boardSpaceIndex]);
                                }

                                commonTokens.RemoveAt(selectedTokenIndex);

                                if (commonTokens.Count == 0)
                                    selectedTokenIndex = -1;
                                else if (selectedTokenIndex == commonTokens.Count)
                                    selectedTokenIndex--;

                                UpdateCommonTokens();
                            }

                            if (selectedBoardSpaceData.Count == 1)
                            {
                                if (selectedTokenIndex > 0)
                                    if (GUILayout.Button("▲", GUILayout.Width(30f)))
                                    {
                                        selectedBoardSpaceData[0].data.T.MoveItem(selectedTokenIndex, selectedTokenIndex - 1);
                                        selectedTokenIndex--;

                                        UpdateCommonTokens();
                                    }

                                if (selectedTokenIndex < selectedBoardSpaceData[0].data.T.Count - 1)
                                    if (GUILayout.Button("▼", GUILayout.Width(30f)))
                                    {
                                        selectedBoardSpaceData[0].data.T.MoveItem(selectedTokenIndex, selectedTokenIndex + 1);
                                        selectedTokenIndex++;

                                        UpdateCommonTokens();
                                    }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        public class BoardSpaceWrapper
        {
            public BoardSpaceData data;
            public Vector2Int location;
        }

        public class BoardSpaceTokenWrapper
        {
            public string token;
            public List<int> indicies;

            public BoardSpaceTokenWrapper()
            {
                indicies = new List<int>();
            }
        }
    }
}
#endif