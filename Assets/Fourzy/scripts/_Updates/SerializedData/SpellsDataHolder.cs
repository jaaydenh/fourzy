//@vadym udod

using FourzyGameModel.Model;
using StackableDecorator;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSpellsDataHolder", menuName = "Create Spells Data Holder")]
    public class SpellsDataHolder : ScriptableObject
    {
        [List]
        public SpellsCollection spells;

        public Spell GetSpell(SpellId type) => spells.list.Find(spell => spell.spellType == type);

        [System.Serializable]
        public class SpellsCollection
        {
            public List<Spell> list;
        }

        [System.Serializable]
        public class Spell
        {
            [HideInInspector]
            public string _name;

            [ShowIf("#Check")]
            [StackableField]
            public Sprite icon;
            public SpellId spellType;
            public int basePrice = 50;

            public int price
            {
                get
                {
                    //get price depending on spell
                    return basePrice;
                }
            }

            public bool Check()
            {
                _name = spellType.ToString();

                return true;
            }
        }
    }
}
