//@vadym udod

using FourzyGameModel.Model;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSGamePromptBossPowerWidget : WidgetBase
    {
        public Image bossPowerIcon;
        public TMP_Text bossPowerText;

        public VSGamePromptBossPowerWidget SetData(Player playerData)
        {
            switch (playerData.Profile)
            {
                case AIProfile.BossAI:
                    //temp solution
                    switch (playerData.BossType)
                    {
                        case BossType.EntryWay:
                        case BossType.DirectionMaster:
                        case BossType.MadBomber:
                        case BossType.Necrodancer:
                        case BossType.Treant:
                            gameObject.SetActive(true);

                            bossPowerIcon.sprite = GameContentManager.Instance.miscGameDataHolder.GetIcon($"boss_power_{playerData.BossType.ToString()}").sprite;
                            bossPowerText.text = playerData.BossType.ToString();

                            break;

                        default:
                            bossPowerIcon.sprite = GameContentManager.Instance.miscGameDataHolder.GetIcon($"boss_power_{BossType.EntryWay.ToString()}").sprite;
                            bossPowerText.text = BossType.EntryWay.ToString();

                            break;
                    }

                    break;

                default:
                    gameObject.SetActive(false);

                    break;
            }

            return this;
        }
    }
}