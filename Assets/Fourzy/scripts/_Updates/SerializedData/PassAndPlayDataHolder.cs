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

        public void Initialize()
        {
            gameboards = new List<GameBoardDefinition>();

            foreach (EditorGameboardView _board in boards.list)
                gameboards.Add(_board.GetGameboardDefinition());
        }

        public GameBoardDefinition random => gameboards[Random.Range(0, gameboards.Count)];
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
        public string elementName;

        [HelpBox("#Context", below = false, messageType = MessageType.None)]
        [StackableField]
        public TextAsset boardFile;

        public string Context()
        {
            if (boardFile == null)
            {
                elementName = "";
                return "No file specified.";
            }

            GameBoardDefinition board = null;

            try
            {
                board = JsonConvert.DeserializeObject<GameBoardDefinition>(boardFile.text);
                elementName = board.BoardName;

                return "Name: " + board.BoardName + "\nID: " + board.ID;
            }
            catch (JsonReaderException)
            {
                elementName = "";
                return "Wrong file";
            }
        }

        public GameBoardDefinition GetGameboardDefinition()
        {
            return JsonConvert.DeserializeObject<GameBoardDefinition>(boardFile.text);
        }
    }
}