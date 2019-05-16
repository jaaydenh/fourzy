////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using SA.Foundation.Templates;

using SA.iOS.GameKit.Internal;
using System.Text;
using System.Collections.Generic;

namespace SA.iOS.GameKit
{
    /// <summary>
    /// An object representing the authenticated Game Center player on a device.
    /// 
    /// At any given time, only one player may be authenticated on the device; 
    /// this player must log out before another player can log in. 
    /// Your game must authenticate the local player before using any Game Center features.
    ///  Authenticating the player ensures that the player has created an account and is connected to Game Center. 
    /// </summary>
    [Serializable]
    public class ISN_GKLocalPlayer : ISN_GKPlayer
    {

        private static event Action<SA_Result> m_onAuthenticateLocalPlayerComplete = delegate {};
        private static event Action<SA_Result> m_onDeleteSavedGameComplete = delegate { };

        private static event Action<ISN_GKSavedGameFetchResult> m_onFetchSavedGamesComplete = delegate { };
        private static event Action<ISN_GKSavedGameSaveResult> m_onSavedGameComplete = delegate { };
        private static event Action<ISN_GKSavedGameLoadResult> m_onLoadGameDataComplete = delegate { };
        private static event Action<ISN_GKSavedGameFetchResult> m_onResolveSavedGamesComplete = delegate { };

        private static ISN_GKResolveSavedGamesRequest m_request;

        private static SA_Result m_successAuthenticateResultCache = null;
        private static bool m_isAuthenticateInProgress = false;

        private static bool m_isFetchSavedGamesInProgress = false;

        [SerializeField] private bool m_authenticated = false;
        [SerializeField] private bool m_underage = false;

        /// <summary>
        /// Authenticate GK Local Player
        /// 
        /// Your game should authenticate the player as early as possible after launching, 
        /// ideally as soon as you can present a user interface to the player. 
        /// For example, your game may be launched because the player accepted an invitation to join a match 
        /// or to take a turn in a turn-based match, 
        /// so you want your game to authenticate the player 
        /// and process the match invitation as quickly as possible. 
        /// After you set a handler, authentication begins automatically 
        /// and is repeated when your game moves to the background and then back to the foreground.
        /// </summary>
        public static void Authenticate(Action<SA_Result> callback) {
            
            if (m_successAuthenticateResultCache != null) {
                callback.Invoke(m_successAuthenticateResultCache);
                return;
            }

            m_onAuthenticateLocalPlayerComplete += callback;
            if (m_isAuthenticateInProgress) { return; }


            m_isAuthenticateInProgress = true;

            ISN_GKLib.API.AuthenticateLocalPlayer((SA_Result result) => {

                m_isAuthenticateInProgress = false;
                if (result.IsSucceeded) {
                    m_successAuthenticateResultCache = result;
                }
                m_onAuthenticateLocalPlayerComplete.Invoke(result);
                m_onAuthenticateLocalPlayerComplete = delegate { };

            });
        }

        /// <summary>
        /// Generates a signature that allows a third party server to authenticate the local player.
        /// 
        /// When this method is called, it creates a new background task to handle the request. 
        /// The method then returns control to your game. 
        /// Later, when the task is complete, Game Kit calls your completion callback. 
        /// The completion handler is always called on the main thread.
        /// </summary>
        /// <param name="callback">Background task completion callback.</param>
        public void GenerateIdentityVerificationSignatureWithCompletionHandler(Action<ISN_GKIdentityVerificationSignatureResult> callback) {
            ISN_GKLib.API.GenerateIdentityVerificationSignatureWithCompletionHandler(callback);
        }


        /// <summary>
        /// Retrieves the shared instance of the local player.
        /// You never directly create a local player object. Instead, you retrieve the Singleton object by calling this class method.
        /// </summary>
        public static ISN_GKLocalPlayer LocalPlayer {
            get { return ISN_GKLib.API.LocalPlayer; }
        }

        /// <summary>
        /// A Boolean value that indicates whether a local player is currently signed in to Game Center.
        /// </summary>
        public bool Authenticated {
            get { return m_authenticated; }
        }

        /// <summary>
        /// A Boolean value that declares whether the local player is underage.
        /// Some Game Center features are disabled if the local player is underage. 
        /// Your game can also test this property if it wants to disable some of its own features based on the player’s age.
        /// </summary>
        public bool Underage {
            get { return m_underage; }
        }

        /// <summary>
        /// Retrieves all available saved games.
        /// </summary>
        public static void FetchSavedGames(Action<ISN_GKSavedGameFetchResult> callback) {
            m_onFetchSavedGamesComplete += callback;
            if (m_isFetchSavedGamesInProgress) { return; }

            m_isFetchSavedGamesInProgress = true;

            ISN_GKLib.API.FetchSavedGames((ISN_GKSavedGameFetchResult result) => {
                m_isFetchSavedGamesInProgress = false;

                m_onFetchSavedGamesComplete.Invoke(result);
                m_onFetchSavedGamesComplete = delegate {};
            });
        }

        /// <summary>
        /// Saves game data under the specified name.
        /// </summary>
        public static void SavedGame(string name, byte[] data, Action<ISN_GKSavedGameSaveResult> callback) {
            m_onSavedGameComplete += callback;

            string stringData = Convert.ToBase64String(data);

            ISN_GKLib.API.SavedGame(name, stringData, (ISN_GKSavedGameSaveResult result) => {
                m_onSavedGameComplete.Invoke(result);
                m_onSavedGameComplete = delegate { };
            });
        }

        /// <summary>
        /// Deletes a specific saved game.
        /// </summary>
        public static void DeleteSavedGame(ISN_GKSavedGame game, Action<SA_Result> callback) {
            m_onDeleteSavedGameComplete += callback;

            ISN_GKLib.API.DeleteSavedGame(game, (SA_Result result) => {
                m_onDeleteSavedGameComplete.Invoke(result);
                m_onDeleteSavedGameComplete = delegate { };
            });
        }

        /// <summary>
        /// Loads specific saved game data.
        /// </summary>
        public static void LoadGameData(ISN_GKSavedGame game, Action<ISN_GKSavedGameLoadResult> callback) {
            m_onLoadGameDataComplete += callback;

            ISN_GKLib.API.LoadGameData(game, (ISN_GKSavedGameLoadResult result) => {
                m_onLoadGameDataComplete.Invoke(result);
                m_onLoadGameDataComplete = delegate { };
            });
        }

        /// <summary>
        /// Resolves conflicted saved games.
        /// </summary>
        public static void ResolveConflictingSavedGames(List<string> conflictedGames, byte[] data, Action<ISN_GKSavedGameFetchResult> callback) {
            m_onResolveSavedGamesComplete += callback;

            m_request = new ISN_GKResolveSavedGamesRequest(conflictedGames, stringData: Encoding.UTF8.GetString(data));

            ISN_GKLib.API.ResolveConflictingSavedGames(m_request, (ISN_GKSavedGameFetchResult result) => {
                m_onResolveSavedGamesComplete.Invoke(result);
                m_onResolveSavedGamesComplete = delegate { };
            });
        }
    }
}