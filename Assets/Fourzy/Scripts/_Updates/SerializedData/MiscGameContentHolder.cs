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
        [ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "id")]
        public List<IconsCollection> collections;

        [
            BoxGroup("Products"), 
            InfoBox("$productsInfo", "productsInfoToggle"),
            ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "id")]
        public List<StoreItemExtraData> products;

        public IconData GetIcon(string collectionID, string id) => 
            collections.Find(_collection => _collection.id == collectionID).icons.Find(icon => icon.id == id);

        public StoreItemExtraData GetStoreItem(string id) => products.Find(icon => icon.id == id);

        private string productsInfo
        {
            get
            {
                List<string> duplicateIDs = products.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                return duplicateIDs.Count == 0 ? "" : "Duplicate IDs: " + string.Join(",", duplicateIDs);
            }
        }

        /// <summary>
        /// editor
        /// </summary>
        private bool productsInfoToggle => productsInfo.Length > 0;

        [System.Serializable]
        public class IconsCollection
        {
            public string id;
            [ListDrawerSettings(NumberOfItemsPerPage = 5, ListElementLabelName = "id")]
            public List<IconData> icons;
        }

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