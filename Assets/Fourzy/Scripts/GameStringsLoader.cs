using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameStringsLoader : UnitySingleton<GameStringsLoader>
    {
        private GameStrings gameStrings = new GameStrings();

        protected override void Awake()
        {
            base.Awake();

            this.LoadGameStrings();
        }

        private void LoadGameStrings()
        {
            TextAsset dataAsJson = Resources.Load<TextAsset>("GameStrings");
            if (dataAsJson)
            {
                gameStrings = JsonUtility.FromJson<GameStrings>(dataAsJson.text);
            }
        }

        public List<string> GetMatchMakingWaitingStrings()
        {
            return gameStrings.MatchmakingWaitingStrings;
        }
    }
}