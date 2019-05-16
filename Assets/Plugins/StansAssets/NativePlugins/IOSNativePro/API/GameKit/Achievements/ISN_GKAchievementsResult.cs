using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;


namespace SA.iOS.GameKit
{
    [Serializable]
    public class ISN_GKAchievementsResult  : SA_Result
    {

        [SerializeField] List<ISN_GKAchievement> m_achievements;


        /// <summary>
        /// Initializes a new instance of the <see cref="ISN_GKAchievementsResult"/> class.
        /// </summary>
        /// <param name="achievements">Achievements.</param>
        public ISN_GKAchievementsResult(List<ISN_GKAchievement> achievements) {
            m_achievements = achievements;
        }

        /// <summary>
        /// An array of <see cref="ISN_GKAchievement"/> objects 
        /// that represents all progress reported to Game Center for the local player. 
        /// If an error occurred, this parameter may be non-null, 
        /// in which case the array holds whatever achievement information Game Kit was able to fetch.
        /// </summary>
        public List<ISN_GKAchievement> Achievements {
            
            get {
                return m_achievements;
            }
        }
    }
}