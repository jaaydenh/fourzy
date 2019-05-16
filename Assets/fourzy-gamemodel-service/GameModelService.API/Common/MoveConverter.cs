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
                                serializer.Populate(jsonObject.CreateReader(), move);
                                break;
                        }

                        break;
                }

                moves.Add(move);
            }

            return moves;
        }
    }
}
