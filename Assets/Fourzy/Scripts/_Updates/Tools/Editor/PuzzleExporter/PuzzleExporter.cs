//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using PuzzleData = Fourzy.GameManager.PuzzleData;
using GameBoardDefinitionData = Fourzy.GameManager.GameBoardDefinitionData;

namespace Fourzy._Updates.Tools
{
    public class PuzzleExporter : EditorWindow
    {
        public static PuzzleExporter instance;

        private static PuzzleData data = new PuzzleData();

        private Vector2 puzzlesScrollViewPosition;
        private static GUIStyle defaultButtonsStyle;
        private static GUIStyle selectedButtonStyle;
        private static int puzzleToDisplay = -1;
        private static string selectedPath;

        private static TextAsset previousAsset = null;

        //private static 

        [MenuItem("Window/Puzzle Exporter")]
        public static void ShowWindow()
        {
            instance = GetWindow<PuzzleExporter>("Puzzle Exporter");
            instance.minSize = new Vector2(300f, 500f);

            defaultButtonsStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft };
            selectedButtonStyle = new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, contentOffset = Vector2.right * 20f };
        }

        public void OnGUI()
        {
            //draw UI
            GUILayout.BeginHorizontal("Box");
            {
                //load/save
                if (GUILayout.Button("Load Puzzle Data", GUILayout.Width(110f)))
                {
                    string path = EditorUtility.OpenFilePanel("Load puzzle data", "", "json");

                    if (path.Length != 0)
                    {
                        PuzzleData _data = JsonConvert.DeserializeObject<PuzzleData>(File.ReadAllText(path));

                        selectedPath = path;

                        if (_data != null) LoadData(_data);
                    }
                }

                //save board
                if (GUILayout.Button("Save Puzzle Data", GUILayout.Width(110f)))
                {
                    string path = "";

                    if (string.IsNullOrEmpty(selectedPath))
                        path = EditorUtility.SaveFilePanelInProject("Save board", data.packName, "json", "");
                    else
                        path = EditorUtility.SaveFilePanelInProject("Save board", data.packName, "json", "", selectedPath);

                    if (path.Length != 0)
                        File.WriteAllText(path, JsonConvert.SerializeObject(data));
                }
            }
            GUILayout.EndHorizontal();

            data.packID = EditorGUILayout.IntField("Pack ID", data.packID);
            data.packName = EditorGUILayout.TextField("Pack Name", data.packName);

            GUILayout.BeginVertical("Box");
            {
                puzzlesScrollViewPosition = GUILayout.BeginScrollView(puzzlesScrollViewPosition, GUILayout.Height(200));
                {
                    for (int puzzleIndex = 0; puzzleIndex < data.puzzles.Count; puzzleIndex++)
                    {
                        if (GUILayout.Button(puzzleIndex + ": Name: " + data.puzzles[puzzleIndex].puzzleName, (puzzleToDisplay == puzzleIndex) ? selectedButtonStyle : defaultButtonsStyle))
                        {
                            if (puzzleToDisplay == puzzleIndex)
                                puzzleToDisplay = -1;
                            else
                                puzzleToDisplay = puzzleIndex;
                        }
                    }
                }
                GUILayout.EndScrollView();

                GUILayout.BeginHorizontal("Box");
                {
                    //buttons
                    if (GUILayout.Button("+", GUILayout.Width(30f)))
                    {
                        data.puzzles.Add(new GameBoardDefinitionData());
                    }

                    if (puzzleToDisplay > -1)
                    {
                        if (GUILayout.Button("-", GUILayout.Width(30f)))
                        {
                            data.puzzles.RemoveAt(puzzleToDisplay);

                            if (data.puzzles.Count == 0)
                                puzzleToDisplay = -1;
                            else if (puzzleToDisplay == data.puzzles.Count)
                                puzzleToDisplay--;
                        }

                        if (puzzleToDisplay > 0)
                            if (GUILayout.Button("▲", GUILayout.Width(30f)))
                            {
                                data.puzzles.MoveItem(puzzleToDisplay, puzzleToDisplay - 1);
                                puzzleToDisplay--;
                            }

                        if (puzzleToDisplay < data.puzzles.Count - 1)
                            if (GUILayout.Button("▼", GUILayout.Width(30f)))
                            {
                                data.puzzles.MoveItem(puzzleToDisplay, puzzleToDisplay + 1);
                                puzzleToDisplay++;
                            }
                    }
                }
                GUILayout.EndHorizontal();

                if (puzzleToDisplay > -1)
                {
                    GUILayout.BeginVertical("Box");
                    {
                        data.puzzles[puzzleToDisplay].puzzleName = EditorGUILayout.TextField("Puzzle name", data.puzzles[puzzleToDisplay].puzzleName);
                        TextAsset _asset = (TextAsset)EditorGUILayout.ObjectField("Board Definition Data", data.puzzles[puzzleToDisplay].assetFile, typeof(TextAsset), false);

                        if (_asset != null)
                        {
                            if (previousAsset != _asset)
                            {
                                GameBoardDefinition _board = JsonConvert.DeserializeObject<GameBoardDefinition>(_asset.text);

                                previousAsset = _asset;

                                if (_board != null)
                                {
                                    data.puzzles[puzzleToDisplay].assetFile = _asset;
                                    data.puzzles[puzzleToDisplay].gameboard = _board;

                                    //get GUID
                                    data.puzzles[puzzleToDisplay].assetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_asset));
                                }
                            }
                        }
                        else
                        {
                            data.puzzles[puzzleToDisplay].assetFile = null;
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }

        private void LoadData(PuzzleData _data)
        {
            //load data
            data.packID = _data.packID;
            data.packName = _data.packName;
            //...

            data.puzzles = new List<GameBoardDefinitionData>();

            //read boards data
            for (int boardIndex = 0; boardIndex < _data.puzzles.Count; boardIndex++)
            {
                GameBoardDefinitionData board = new GameBoardDefinitionData();

                //read data
                board.puzzleName = _data.puzzles[boardIndex].puzzleName;
                //...

                board.assetGUID = _data.puzzles[boardIndex].assetGUID;
                board.assetFile = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(_data.puzzles[boardIndex].assetGUID));

                data.puzzles.Add(board);
            }

            puzzleToDisplay = -1;
        }
    }
}
