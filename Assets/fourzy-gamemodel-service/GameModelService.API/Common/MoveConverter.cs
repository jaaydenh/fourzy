using System;
using System.Collections;
using System.Collections.Generic;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FourzyGameModel.Common
{
    public class MoveConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMove);
        }
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            //List<IMove> move = default(List<IMove>);
            var moves = new List<IMove>();

            var jsonArray = JArray.Load(reader);
            foreach (var item in jsonArray)
            {
                JObject jsonObject = item as JObject;
                int moveType = jsonObject["moveType"].Value<int>();
                var move = default(IMove);

                switch (moveType)
                {
                    case 0:
                        move = new SimpleMove();
                        serializer.Populate(jsonObject.CreateReader(), move);

                        break;

                    //spell
                    case 1:
                        int spellID = jsonObject["SpellId"].Value<int>();

                        switch (spellID)
                        {
                            case (int)SpellId.HEX:
                                move = new HexSpell();

                                break;

                            case (int)SpellId.HOLD_FOURZY:
                                move = new HoldFourzySpell();

                                break;

                            case (int)SpellId.PRISON:
                                move = new PrisonSpell();

                                break;

                            case (int)SpellId.DARKNESS:
                                move = new DarknessSpell();

                                break;

                            case (int)SpellId.FRUIT:
                                move = new FruitSpell();

                                break;

                            case (int)SpellId.SLURP:
                                move = new SlurpSpell();

                                break;

                            case (int)SpellId.SQUIRT_WATER:
                                move = new SquirtWaterSpell();

                                break;

                            case (int)SpellId.THROW_BOMB:
                                move = new BombSpell();

                                break;

                            case (int)SpellId.PLACE_LURE:
                                move = new LureSpell();

                                break;

                            case (int)SpellId.ICE_WALL:
                                move = new IceWallSpell();

                                break;

                            case (int)SpellId.FIRE_WALL:
                                move = new FireWallSpell();

                                break;

                            case (int)SpellId.DIG:
                                move = new DigSpell();

                                break;

                            case (int)SpellId.GROWL:
                                move = new GrowlSpell();

                                break;

                            case (int)SpellId.SUMMON_SPECTER:
                                move = new SpecterSpell();

                                break;

                            case (int)SpellId.RAINBOW:
                                move = new RainbowSpell();

                                break;

                            case (int)SpellId.PUNCH:
                                move = new PunchSpell();

                                break;

                            case (int)SpellId.FREEZE:
                                move = new FreezeSpell();

                                break;

                            case (int)SpellId.MELT:
                                move = new MeltSpell();

                                break;

                            case (int)SpellId.LIFE:
                                move = new LifeSpell();

                                break;

                            case (int)SpellId.DEATH:
                                move = new DeathSpell();

                                break;
                        }
                        serializer.Populate(jsonObject.CreateReader(), move);

                        break;
                }

                moves.Add(move);
            }

            return moves;
        }
    }
}
