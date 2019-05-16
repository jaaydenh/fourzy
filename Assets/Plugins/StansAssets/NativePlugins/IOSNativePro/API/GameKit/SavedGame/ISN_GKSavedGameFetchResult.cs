////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.GameKit
{
    [Serializable]
    public class ISN_GKSavedGameFetchResult : SA_Result
    {
        [SerializeField] List<ISN_GKSavedGame> m_savedGames = new List<ISN_GKSavedGame>();

        public ISN_GKSavedGameFetchResult(List<ISN_GKSavedGame> savedGames) {
            m_savedGames = savedGames;
        }

        public ISN_GKSavedGameFetchResult(SA_Error error) : base(error) {}

        /// <summary>
        /// Returns the saved games.
        /// </summary>
        public List<ISN_GKSavedGame> SavedGames {
            get {
                return m_savedGames;
            }
        }
    }
}