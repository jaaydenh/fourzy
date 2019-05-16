//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class StoreScreen : MenuTab
    {
        public void AddCoins(int quantity) => UserManager.Instance.coins += quantity;

        public void AddGems(int quantity) => UserManager.Instance.gems += quantity;

        public void AddPortalPoints(int quantity) => UserManager.Instance.portalPoints += quantity;

        public void AddRarePortalPoints(int quantity) => UserManager.Instance.rarePortalPoints += quantity;

        public void AddTicket() => UserManager.Instance.tickets++;
    }
}