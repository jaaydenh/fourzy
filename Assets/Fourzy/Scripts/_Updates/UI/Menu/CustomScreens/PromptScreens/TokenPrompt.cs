//@vadym udod

using mixpanel;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TokenPrompt : PromptScreen
    {
        public TMP_Text areaLabel;
        public Image tokenImage;
        public TokenInstructionGameboardManager instructionManager;

        public TokenData data { get; private set; }

        public virtual void Prompt(TokenData data)
        {
            this.data = data;

            var props = new Value();
            props["Token Type"] = data.Name;
            Mixpanel.Track("Open Token Detail", props);

            areaLabel.text = data.Arena;
            tokenImage.sprite = GameContentManager.Instance.GetTokenSprite(data.ID);

            //play instruction
            instructionManager.Init(data.GameBoardInstructionID);

            Prompt(data.Name, data.Description, accept: null);
        }

        public override void Close()
        {
            base.Close();

            instructionManager.Close();
        }
    }
}
