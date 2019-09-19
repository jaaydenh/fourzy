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
        [BoxGroup("Products"), InfoBox("$productsInfo", "productsInfoToggle")]
        public List<StoreItemExtraData> products;

        public IconData GetIcon(string id) => icons.Find(icon => icon.id == id);

        public StoreItemExtraData GetStoreItem(string id) => products.Find(icon => icon.id == id);

        private string iconsInfo
        {
            get
            {
                List<string> duplicateIDs = icons.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                return duplicateIDs.Count == 0 ? "" : "Duplicate IDs: " + string.Join(",", duplicateIDs);
            }
        }

        private string productsInfo
        {
            get
            {
                List<string> duplicateIDs = products.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                return duplicateIDs.Count == 0 ? "" : "Duplicate IDs: " + string.Join(",", duplicateIDs);
            }
        }

        private bool iconsInfoToggle => iconsInfo.Length > 0;

        private bool productsInfoToggle => productsInfo.Length > 0;

        [System.Serializable]
        public class IconData
        {
            public string id;
            public Sprite sprite;
        }

        [System.Serializable]
        public class StoreItemExtraData
        {
            public string id;
            public Sprite icon;
            public int quantity;

            public static implicit operator bool (StoreItemExtraData data) => data != null;
        }
    }
}