//@vadym udod

using ByteSheep.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class ListenTo : MonoBehaviour
    {
        public ListenTarget[] targets;

        private Dictionary<ListenValues, List<ListenTarget>> sorted;

        protected void Awake()
        {
            sorted = new Dictionary<ListenValues, List<ListenTarget>>();

            foreach (ListenTarget target in targets)
            {
                if (!sorted.ContainsKey(target.type))
                    sorted.Add(target.type, new List<ListenTarget>());

                sorted[target.type].Add(target);

                //add to listeners
                switch (target.type)
                {
                    case ListenValues.CHELLENGE_ID:
                        GameManager.OnUpdateGame += UpdateChallengeID;
                        break;

                    case ListenValues.PLAYER_TIMER:
                        GamePlayManager.OnTimerUpdate += UpdatePlayerTimer;
                        break;
                }
            }
        }

        protected void Start()
        {
            InvokeTargets(false);
        }

        protected void OnDestroy()
        {
            foreach (ListenTarget target in targets)
                switch (target.type)
                {
                    case ListenValues.CHELLENGE_ID:
                        GameManager.OnUpdateGame -= UpdateChallengeID;
                        break;

                    case ListenValues.PLAYER_TIMER:
                        GamePlayManager.OnTimerUpdate -= UpdatePlayerTimer;
                        break;
                }
        }

        public void InvokeTargets(bool force)
        {
            foreach (ListenTarget target in targets)
            {
                if (force || target.updateOnAwake)
                {
                    switch (target.type)
                    {
                        case ListenValues.CHELLENGE_ID:
                            UpdateChallengeID(GameManager.Instance.activeGame);
                            break;

                        case ListenValues.PLAYER_TIMER:
                            UpdatePlayerTimer(new TimeSpan(0, 0, 0, 0, Constants.playerMoveTimer_InitialTime).Ticks);
                            break;
                    }
                }
            }
        }

        public void UpdateChallengeID(Game game)
        {
            foreach (ListenTarget target in sorted[ListenValues.CHELLENGE_ID])
                target.events.Invoke(string.Format(target.targetText, game.challengeId));
        }

        public void UpdatePlayerTimer(long ticks)
        {
            DateTime time = new DateTime(ticks);

            foreach (ListenTarget target in sorted[ListenValues.PLAYER_TIMER])
                target.events.Invoke(string.Format(target.targetText, time.Minute, time.Second));
        }
    }

    [Serializable]
    public class ListenTarget
    {
        public ListenValues type;

        public string targetText;
        [Header("Update value on awake?")]
        public bool updateOnAwake;
        [Header("Will format target text with value")]
        public QuickStringEvent events;
    }

    public enum ListenValues
    {
        NONE = 0,

        CHELLENGE_ID = 1,
        PLAYER_TIMER = 2,

        LENGTH,
    }
}