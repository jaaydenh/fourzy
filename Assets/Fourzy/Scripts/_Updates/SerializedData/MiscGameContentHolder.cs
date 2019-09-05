//@vadym udod

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultMiscGameContentHolder", menuName = "Create Misc GameContent Holder")]
    public class MiscGameContentHolder : ScriptableObject
    {
        [BoxGroup("Icons"), InfoBox("$iconsInfo", "iconsInfoToggle")]
        public List<IconData> icons;

        public IconData GetIcon(string id) => icons.Find(icon => icon.id == id);

        private string iconsInfo
        {
            get
            {
                List<string> duplicateIDs = icons.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                return duplicateIDs.Count == 0 ? "" : "Duplicate IDs: " + string.Join(",", duplicateIDs);
            }
        }

        private bool iconsInfoToggle => iconsInfo.Length > 0;

        [System.Serializable]
        public class IconData
        {
            public string id;
            public Sprite sprite;
        }
    }
}