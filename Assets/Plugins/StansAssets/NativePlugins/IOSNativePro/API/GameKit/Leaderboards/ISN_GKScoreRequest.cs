using System;
using System.Collections.Generic;
using UnityEngine;



namespace SA.iOS.GameKit.Internal
{

    [Serializable]
    public class ISN_GKScoreRequest 
    {

        [SerializeField] List<ISN_GKScore> m_scores;


        public ISN_GKScoreRequest(List<ISN_GKScore> scores)  {
            m_scores = scores;
        }


        /// <summary>
        /// Scores to submit
        /// </summary>
        public List<ISN_GKScore> Scores {
            get {
                return m_scores;
            }
        }

      
    }
}