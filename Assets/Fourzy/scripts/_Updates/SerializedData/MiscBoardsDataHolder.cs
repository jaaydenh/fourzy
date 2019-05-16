//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json;
using StackableDecorator;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultMiscBoardsDataHolder", menuName = "Create Misc Boards Data Holder")]
    public class MiscBoardsDataHolder : ScriptableObject
    {
        [Buttons(titles = "Load from folder", actions = "LoadFromFolder")]
        [StackableField]
        public string path = "Fourzy/Resources/MiscBoards";
        [List]
        public EditorGameboardsList boards;

        public List<GameBoardDefinition> gameboards { get; private set; }

        public void Initialize()
        {
            gameboards = new List<GameBoardDefinition>();

            foreach (EditorGameboardView _board in boards.list)
                gameboards.Add(_board.GetGameboardDefinition());
        }

        private void LoadFromFolder()
        {
#if UNITY_EDITOR
            TextAsset[] boardFiles = Tools.Utils.GetAtPath<TextAsset>(path);

            foreach (TextAsset boardFile in boardFiles)
            {
                GameBoardDefinition gameboardDefinition = JsonConvert.DeserializeObject<GameBoardDefinition>(boardFile.text);

                if (gameboardDefinition != null && boards.list.Find(board => board.GetGameboardDefinition().ID == gameboardDefinition.ID) == null)
                    boards.list.Add(new EditorGameboardView() { boardFile = boardFile });
            }
#endif
        }
    }
}