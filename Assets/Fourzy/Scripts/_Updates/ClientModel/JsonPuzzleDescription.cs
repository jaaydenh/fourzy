//@vadym udod

using FourzyGameModel.Model;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.ClientModel
{
    [System.Serializable]
    public class JsonPuzzleDescription
    {
        public string playerName;
        public string herdID;
        public string filename;
        public ResourceItem resource;

        private int[] _bannedSpells;
        private SpellId[] __bannedSpells;

        public SpellId[] bannedSpells
        {
            get
            {
                if (__bannedSpells == null)
                    __bannedSpells = _bannedSpells.Cast<SpellId>().ToArray();

                return __bannedSpells;
            }
        }

        public JsonPuzzleDescription(ResourceItem resource)
        {
            JObject jObject = JObject.Parse(resource.Load<TextAsset>().text);

            playerName = jObject["playerName"].ToObject<string>();
            herdID = jObject["herdID"].ToObject<string>();
            filename = jObject["filename"].ToObject<string>();
            _bannedSpells = jObject["bannedSpells"].ToObject<int[]>();
        }
    }
}