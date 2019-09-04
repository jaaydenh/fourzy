//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json;
using StackableDecorator;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultPassAndPlayDataHolder", menuName = "Create Pass&Play Data Holder")]
    public class PassAndPlayDataHolder : ScriptableObject
    {
        [List]
        public EditorGameboardsList boards;

        public List<GameBoardDefinition> gameboards { get; private set; }

        public GameBoardDefinition random => gameboards[Random.Range(0, gameboards.Count)];

        public void Initialize()
        {
            gameboards = new List<GameBoardDefinition>();

            foreach (EditorGameboardView _board in boards.list) gameboards.Add(_board.GetGameboardDefinition());
        }
    }

    [System.Serializable]
    public class EditorGameboardsList
    {
        public List<EditorGameboardView> list;
    }

    [System.Serializable]
    public class EditorGameboardView
    {
        //to replace "Element {index} with board name
        [HideInInspector]
        public string _name;

        [ShowIf("#FileCheck")]
        [StackableField]
        public TextAsset boardFile;

        public bool FileCheck()
        {
            if (boardFile == null)
            {
                _name = "No file specified.";
                return true;
            }

            GameBoardDefinition board = null;

            try
            {
                board = JsonConvert.DeserializeObject<GameBoardDefinition>(boardFile.text);
                _name = $"Name: {board.BoardName}, ID: {board.ID}";
            }
            catch (JsonReaderException)
            {
                _name = "Wrong file";
            }

            return true;
        }

        public GameBoardDefinition GetGameboardDefinition() => JsonConvert.DeserializeObject<GameBoardDefinition>(boardFile.text);
    }
}