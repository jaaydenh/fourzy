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

namespace SA.iOS.GameKit.Internal
{
    internal interface ISN_iGKAPI
    {
        void AuthenticateLocalPlayer(Action<SA_Result> callback);
        void GenerateIdentityVerificationSignatureWithCompletionHandler(Action<ISN_GKIdentityVerificationSignatureResult> callback);
        ISN_GKLocalPlayer LocalPlayer { get; }
        void ShowGameKitView(ISN_GKGameCenterViewController view, Action<SA_Result> callback);


        //--------------------------------------
        // GKPlayer
        //--------------------------------------

        void GKPlayerLoadPhotoForSize(string playerId, int size, Action<ISN_GKImageLoadResult> callback);


        //--------------------------------------
        // Saved Games
        //--------------------------------------

        void FetchSavedGames(Action<ISN_GKSavedGameFetchResult> callback);
        void SavedGame(string name, string data, Action<ISN_GKSavedGameSaveResult> callback);
        void DeleteSavedGame(ISN_GKSavedGame game, Action<SA_Result> callback);
        void LoadGameData(ISN_GKSavedGame game, Action<ISN_GKSavedGameLoadResult> callback);
        void ResolveConflictingSavedGames(ISN_GKResolveSavedGamesRequest request, Action<ISN_GKSavedGameFetchResult> callback);

        SA_iEvent<ISN_GKSavedGameSaveResult> DidModifySavedGameEventResponse { get; }
        SA_iEvent<ISN_GKSavedGameFetchResult> HasConflictingSavedGamesEventResponse { get; }


        //--------------------------------------
        // Achievements
        //--------------------------------------


        void RsesetAchievements(Action<SA_Result> callback);
        void LoadAchievements(Action<ISN_GKAchievementsResult> callback);
        void ReportAchievements(List<ISN_GKAchievement> achievements, Action<SA_Result> callback);



        //--------------------------------------
        // Leaderboards
        //--------------------------------------

        void LoadLeaderboards(Action<ISN_GKLeaderboardsResult> callback);
        void LoadScores(ISN_GKLeaderboard leaderboard, Action<ISN_GKScoreLoadResult> callback);
        void ReportScore(ISN_GKScoreRequest scoresRequest, Action<SA_Result> callback);


    }
}
