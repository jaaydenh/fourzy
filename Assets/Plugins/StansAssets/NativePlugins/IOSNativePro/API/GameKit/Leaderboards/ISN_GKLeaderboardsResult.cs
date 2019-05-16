using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;



namespace SA.iOS.GameKit
{

    [Serializable]
    public class ISN_GKLeaderboardsResult : SA_Result 
    {

        [SerializeField] List<ISN_GKLeaderboard> m_leaderboards;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISN_GKLeaderboardsResult"/> class.
        /// </summary>
        /// <param name="leaderboards">Leaderboards.</param>
        public ISN_GKLeaderboardsResult(List<ISN_GKLeaderboard> leaderboards) {
            m_leaderboards = leaderboards;
        }

        /// <summary>
        /// An array of <see cref="ISN_GKLeaderboard"/> objects that provides the leaderboards for your game. 
        /// If an error occurred, this value may be non-null. 
        /// In this case, the array holds whatever data Game Kit was able to download before the error occurred.
        /// </summary>
        public List<ISN_GKLeaderboard> Leaderboards {

            get {
                return m_leaderboards;
            }
        }
    }
}