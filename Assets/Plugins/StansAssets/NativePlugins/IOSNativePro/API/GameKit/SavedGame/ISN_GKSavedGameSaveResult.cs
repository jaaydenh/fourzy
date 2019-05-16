////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.GameKit
{
    [Serializable]
    public class ISN_GKSavedGameSaveResult : SA_Result
    {
        [SerializeField] ISN_GKSavedGame m_savedGame;

        public ISN_GKSavedGameSaveResult(ISN_GKSavedGame savedGame) {
            m_savedGame = savedGame;
        }

        public ISN_GKSavedGameSaveResult(SA_Error error) : base(error) {

        }

        /// <summary>
        /// Returns the saved games.
        /// </summary>
        public ISN_GKSavedGame SavedGame {
            get {
                return m_savedGame;
            }
        }
    }
}