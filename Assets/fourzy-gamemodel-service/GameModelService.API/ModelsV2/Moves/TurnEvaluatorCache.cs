//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace FourzyGameModel.Model
//{
//    public static class AITurnEvaluatorCache
//    {

//        public static Dictionary<string, Dictionary<string, GameState>> GSCache;
//        private static bool isInit = false;
//        private static void Initialize()
//        {
//            if (isInit) return;

//            GSCache = new Dictionary<string, Dictionary<string, GameState>>();
//            isInit = true;
//        }

//        public static void Reset()
//        {
//            GSCache = new Dictionary<string, Dictionary<string, GameState>>();
//            isInit = true;
//        }


//        public static void AddState(string StateString, string MoveString, GameState State)
//        {
//            Initialize();
//            if (!GSCache.ContainsKey(StateString)) 
//                GSCache.Add(StateString, new Dictionary<string, GameState>() { });

//            if (GSCache[StateString].ContainsKey(MoveString)) return;

//            GSCache[StateString].Add(MoveString, new GameState(State));
//        }

//        public static GameState GetState(string StateString, string MoveString)
//        {
//            Initialize();
//            if (!GSCache.ContainsKey(StateString) ) return null;

//            if (!GSCache[StateString].ContainsKey(MoveString)) return null;

//            return new GameState(GSCache[StateString][MoveString]);
//        }
//    }
//}