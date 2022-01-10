//@vadym udod

using System;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayerPositioningPromptScreen : PromptScreen
    {
        public static PlayerPositioning PlayerPositioning;

        private Action postSelection;

        public override void OnBack()
        {
            CloseSelf();
        }

        public void _Prompt(Action postSelection)
        {
            this.postSelection = postSelection;

            Prompt(LocalizationManager.Value("select_player_positioning"), null, LocalizationManager.Value("player_positioning_across"), LocalizationManager.Value("player_positioning_side_by_side"), OnAcrossSelected, OnSideBySideSelected);
            CloseOnAccept();
            CloseOnDecline();
        }

        public void OnAcrossSelected()
        {
            PlayerPositioning = PlayerPositioning.ACROSS;
            postSelection?.Invoke();
        }

        public void OnSideBySideSelected()
        {
            PlayerPositioning = PlayerPositioning.SIDE_BY_SIDE;
            postSelection?.Invoke();
        }
    }

    public enum PlayerPositioning
    {
        SIDE_BY_SIDE,
        ACROSS,
    }
}