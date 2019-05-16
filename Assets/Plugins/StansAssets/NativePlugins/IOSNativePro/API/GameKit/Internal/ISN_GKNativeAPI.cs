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
using SA.Foundation.Events;
using SA.Foundation.Templates;
using SA.Foundation.Utility;
using SA.iOS.Utilities;
using UnityEngine;


#if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.GameKit.Internal
{

    internal class ISN_GKNativeAPI : ISN_Singleton<ISN_GKNativeAPI>, ISN_iGKAPI
    {
        #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)

        [DllImport("__Internal")] static extern void _ISN_AuthenticateLocalPlayer();
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_GenerateIdentityVerificationSignatureWithCompletionHandler(IntPtr callback);
        [DllImport("__Internal")] static extern string _ISN_GetGKLocalPlayerInfo();
        [DllImport("__Internal")] static extern void _ISN_GKGameCenterViewControllerShow(string data, IntPtr callback);
      
        //GKPlayer
        [DllImport("__Internal")] static extern void _ISN_GKPlayer_LoadPhotoForSize(string playerId, int size, IntPtr callback);
       
      
        //Saved Games
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_FetchSavedGames(string requestId);
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_SaveGameData(string requestId, string name, string data);
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_DeleteSavedGame(string requestId, string name, string uniqueId);
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_LoadGameData(string requestId, string name, string uniqueId);
        [DllImport("__Internal")] static extern void _ISN_GKLocalPlayer_ResolveSavedGames(string requestId, string jsonContent);


        //Achievements
        [DllImport("__Internal")] static extern void _ISN_GKAchievement_LoadAchievements(string requestId);
        [DllImport("__Internal")] static extern void _ISN_GKAchievement_ResetAchievements(string requestId);
        [DllImport("__Internal")] static extern void _ISN_GKAchievement_ReportAchievements(string requestId, string contentJSON);



        //Leaderboards
        [DllImport("__Internal")] static extern void _ISN_GKLeaderboar_LoadLeaderboards(string requestId_string);
        [DllImport("__Internal")] static extern void _ISN_GKLeaderboar_LoadScores(string requestId_string, string leaderboardJSON);
        [DllImport("__Internal")] static extern void _ISN_GKLeaderboar_ReportScore(string requestId_string, string scoresJSON);

        #endif

        private Dictionary<string, System.Object> m_callbackList = new Dictionary<string, System.Object>();


        private Action<SA_Result> m_AuthenticateCallback;
        public void AuthenticateLocalPlayer(Action<SA_Result> callback) {
            m_AuthenticateCallback = callback;

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_AuthenticateLocalPlayer();
            #endif
        }

        void OnAuthenticateResponse(string data) {
            var result = JsonUtility.FromJson<SA_Result>(data);
            m_AuthenticateCallback.Invoke(result);
        }


        public void GenerateIdentityVerificationSignatureWithCompletionHandler(Action<ISN_GKIdentityVerificationSignatureResult> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_GenerateIdentityVerificationSignatureWithCompletionHandler(ISN_MonoPCallback.ActionToIntPtr(callback));
            #endif
        }



        public void GKPlayerLoadPhotoForSize(string playerId, int size, Action<ISN_GKImageLoadResult> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            _ISN_GKPlayer_LoadPhotoForSize(playerId, size, ISN_MonoPCallback.ActionToIntPtr(callback));
            #endif
        }
     

        public ISN_GKLocalPlayer LocalPlayer {
            get {
                #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                    return JsonUtility.FromJson<ISN_GKLocalPlayer>(_ISN_GetGKLocalPlayerInfo());
                #else
                    return new ISN_GKLocalPlayer();
                #endif 
            }
        }

        public SA_iEvent<ISN_GKSavedGameSaveResult> DidModifySavedGame {
            get {
                return m_DidModifySavedGame;
            }
        }

        public SA_iEvent<ISN_GKSavedGameFetchResult> HasConflictingSavedGames {
            get {
                return m_HasConflictingSavedGames;
            }
        }

        public void ShowGameKitView(ISN_GKGameCenterViewController view, Action<SA_Result> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            _ISN_GKGameCenterViewControllerShow(JsonUtility.ToJson(view), ISN_MonoPCallback.ActionToIntPtr(callback));
            #endif
        }

        public void FetchSavedGames(Action<ISN_GKSavedGameFetchResult> callback) {
            string requestId = SA_IdFactory.RandomString;

            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_FetchSavedGames(requestId);
            #endif
        }

 
        void OnFetchSavedGamesResponse(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameFetchResult>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<ISN_GKSavedGameFetchResult>).Invoke(result);
        }

        public void SavedGame(string name, string data, Action<ISN_GKSavedGameSaveResult> callback) {
            string requestId = SA_IdFactory.RandomString;

            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_SaveGameData(requestId, name, data);
            #endif
        }

        void OnSavedGameResponse(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameSaveResult>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<ISN_GKSavedGameSaveResult>).Invoke(result);
        }

        public void DeleteSavedGame(ISN_GKSavedGame game, Action<SA_Result> callback) {
            string requestId = SA_IdFactory.RandomString;

            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_DeleteSavedGame(requestId, game.Name, game.Id);
            #endif
        }

        void OnDeleteSavedGameResponse(string data) {
            var result = JsonUtility.FromJson<SA_Result>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<SA_Result>).Invoke(result);
        }

        public void LoadGameData(ISN_GKSavedGame game, Action<ISN_GKSavedGameLoadResult> callback) {
            string requestId = SA_IdFactory.RandomString;

            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_LoadGameData(requestId, game.Name, game.Id);
            #endif
        }

        void OnLoadSavedGameDataResponse(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameLoadResult>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;
                
            (m_callbackList[result.RequestId] as Action<ISN_GKSavedGameLoadResult>).Invoke(result);
        }

        public void ResolveConflictingSavedGames(ISN_GKResolveSavedGamesRequest request, Action<ISN_GKSavedGameFetchResult> callback) {
            string requestId = SA_IdFactory.RandomString;

            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
                _ISN_GKLocalPlayer_ResolveSavedGames(requestId, JsonUtility.ToJson(request));
            #endif
        }

        void OnResolveSavedGamesResponse(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameFetchResult>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<ISN_GKSavedGameFetchResult>).Invoke(result);
        }

        private SA_Event<ISN_GKSavedGameSaveResult> m_DidModifySavedGame = new SA_Event<ISN_GKSavedGameSaveResult>();
        void DidModifySavedGameEvent(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameSaveResult>(data);

            m_DidModifySavedGame.Invoke(result);
        }

        public SA_iEvent<ISN_GKSavedGameSaveResult> DidModifySavedGameEventResponse {
            get {
                return m_DidModifySavedGame;
            }
        }

        private SA_Event<ISN_GKSavedGameFetchResult> m_HasConflictingSavedGames = new SA_Event<ISN_GKSavedGameFetchResult>();
        void HasConflictingSavedGamesEvent(string data) {
            var result = JsonUtility.FromJson<ISN_GKSavedGameFetchResult>(data);

            m_HasConflictingSavedGames.Invoke(result);
        }

        public SA_iEvent<ISN_GKSavedGameFetchResult> HasConflictingSavedGamesEventResponse {
            get {
                return m_HasConflictingSavedGames;
            }
        }





        //--------------------------------------
        // Achievements
        //--------------------------------------

       


        public void LoadAchievements(Action<ISN_GKAchievementsResult> callback) {
            string requestId = SA_IdFactory.RandomString;
            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            _ISN_GKAchievement_LoadAchievements(requestId);
            #endif
        }

        void onAchievementsLoadedResponse(string data) {
            var result = JsonUtility.FromJson<ISN_GKAchievementsResult>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;
            
            (m_callbackList[result.RequestId] as Action<ISN_GKAchievementsResult>).Invoke(result);
        }


        public void RsesetAchievements(Action<SA_Result> callback) {
            string requestId = SA_IdFactory.RandomString;
            m_callbackList.Add(requestId, callback);

            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            _ISN_GKAchievement_ResetAchievements(requestId);
             #endif
        }

        void onAchievementsResetResponse(string data) {
            var result = JsonUtility.FromJson<SA_Result>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<SA_Result>).Invoke(result);
        }



        public void ReportAchievements(List<ISN_GKAchievement> achievements, Action<SA_Result> callback) {

            string requestId = SA_IdFactory.RandomString;
            m_callbackList.Add(requestId, callback);


            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            var request = new ISN_GKAchievementsResult(achievements);
            _ISN_GKAchievement_ReportAchievements(requestId, JsonUtility.ToJson(request));
            #endif

        }


        void onAchievementProgressChangedResponse(string data) {
            var result = JsonUtility.FromJson<SA_Result>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<SA_Result>).Invoke(result);
        }




        //--------------------------------------
        // Leaderboards
        //--------------------------------------


        public void LoadLeaderboards(Action<ISN_GKLeaderboardsResult> callback) {  
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            string requestId = GenerateRequestId(callback);
            _ISN_GKLeaderboar_LoadLeaderboards(requestId);
            #endif
        }

        void onLoadLeaderboardsLoadedResponse(string data) {
            SendResultCallback<ISN_GKLeaderboardsResult>(data);
        }

        public void LoadScores(ISN_GKLeaderboard leaderboard,  Action<ISN_GKScoreLoadResult> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            string requestId = GenerateRequestId(callback);
            _ISN_GKLeaderboar_LoadScores(requestId, JsonUtility.ToJson(leaderboard));
            #endif
        }

        void onLeaderboardloadScoresResponse(string data) {
            SendResultCallback<ISN_GKScoreLoadResult>(data);
        }


        public void ReportScore(ISN_GKScoreRequest scoresRequest, Action<SA_Result> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && GAME_KIT_API_ENABLED)
            string requestId = GenerateRequestId(callback);
            _ISN_GKLeaderboar_ReportScore(requestId, JsonUtility.ToJson(scoresRequest));
            #endif
        }


        void onLeaderboardsReportScore(string data) {
            SendResultCallback<SA_Result>(data);
        }

        //--------------------------------------
        // Private Methods
        //--------------------------------------


        private string GenerateRequestId(object callback) {
            string requestId = SA_IdFactory.RandomString;
            m_callbackList.Add(requestId, callback);

            return requestId;
        }

        private void SendResultCallback<T>(string data) where T : SA_Result {
            T result = JsonUtility.FromJson<T>(data);

            if (!m_callbackList.ContainsKey(result.RequestId))
                return;

            (m_callbackList[result.RequestId] as Action<T>).Invoke(result);
        }

    }
}
