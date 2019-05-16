#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_GKCommunication.h"
#import "ISN_GKAchievementManager.h"


#ifdef __cplusplus
extern "C" {
#endif
    
    void _ISN_GKLeaderboar_LoadLeaderboards(char* requestId_string) {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLeaderboar_LoadLeaderboards" data:requestId_string];
        
        NSString* requestId =  ISN_ConvertToString(requestId_string);
        
        [GKLeaderboard loadLeaderboardsWithCompletionHandler:^(NSArray<GKLeaderboard *> * _Nullable leaderboards, NSError * _Nullable error) {
            
            ISN_GKLeaderboardsResult *result;
            if (error == nil) {
                NSMutableArray<ISN_GKLeaderboard> *leaderboardsArray = [[NSMutableArray<ISN_GKLeaderboard> alloc] init];
                
                for (GKLeaderboard* leaderboard in leaderboards) {
                    ISN_GKLeaderboard *ldb = [[ISN_GKLeaderboard alloc] initWithGKLeaderboard:leaderboard];
                    [leaderboardsArray addObject:ldb];
                }
                result = [[ISN_GKLeaderboardsResult alloc] initWithGKLeaderboards:leaderboardsArray];
            } else {
                result = [[ISN_GKLeaderboardsResult alloc] initWithNSError:error];
            }
            [result setRequestId:requestId];
            
            ISN_SendMessage(UNITY_GK_LISTENER, "onLoadLeaderboardsLoadedResponse", [result toJSONString]);
        }];
    }
    

    
     void _ISN_GKLeaderboar_LoadScores(char* requestId_string, char* leaderboardJSON) {
         
         [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLeaderboar_LoadScores" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s contentJSON: %s", requestId_string, leaderboardJSON])];
         
        NSString* requestId =  ISN_ConvertToString(requestId_string);
         
         NSError *jsonError;
         ISN_GKLeaderboard *requestData = [[ISN_GKLeaderboard alloc] initWithChar:leaderboardJSON error:&jsonError];
         if (jsonError) {
             [ISN_Logger LogError:@"ISN_GKAchievementSubmitRequest JSON parsing error: %@", jsonError.description];
         }

         GKLeaderboard *leaderboardRequest = [requestData toGKLeaderboard];

      
         [leaderboardRequest loadScoresWithCompletionHandler:^(NSArray<GKScore *> * _Nullable scores, NSError * _Nullable error) {

             ISN_GKScoreLoadResult * result;
             if(error != NULL) {
                 result = [[ISN_GKScoreLoadResult alloc] initWithNSError:error];
             } else {
                 result = [[ISN_GKScoreLoadResult alloc] init];
                 NSMutableArray<ISN_GKScore> *scoresArray = [[NSMutableArray<ISN_GKScore> alloc] init];
                 for (GKScore* score in scores) {
                     ISN_GKScore *isn_score = [[ISN_GKScore alloc] initWithGKScore:score];
                     [scoresArray addObject:isn_score];
                 }
                 
                 result.m_scores = scoresArray;
                 result.m_leaderboard = [[ISN_GKLeaderboard alloc] initWithGKLeaderboard:leaderboardRequest];
             }
             
             [result setRequestId:requestId];
             
             ISN_SendMessage(UNITY_GK_LISTENER, "onLeaderboardloadScoresResponse", [result toJSONString]);
         }];
         
     }
    
    void _ISN_GKLeaderboar_ReportScore(char* requestId_string, char* scoresJSON) {
        
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLeaderboar_ReportScore" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s contentJSON: %s", requestId_string, scoresJSON])];
        
        NSString* requestId =  ISN_ConvertToString(requestId_string);
        
        NSError *jsonError;
        ISN_GKScoreRequest *requestData = [[ISN_GKScoreRequest alloc] initWithChar:scoresJSON error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"_ISN_GKLeaderboar_ReportScore JSON parsing error: %@", jsonError.description];
        }
        
        NSMutableArray<GKScore*> *scores = [[NSMutableArray<GKScore*> alloc] init];
        for(ISN_GKScore* score in requestData.m_scores) {
            GKScore* gk_socre  = [score toGKScore];
            [scores addObject:gk_socre];
        }
        
        
        [GKScore reportScores:scores withCompletionHandler:^(NSError *error) {
            SA_Result* result = [[SA_Result alloc] initWithNSError:error];
            [result setRequestId:requestId];
            ISN_SendMessage(UNITY_GK_LISTENER, "onLeaderboardsReportScore", [result toJSONString]);
        }];
    }
    
    
    
#if __cplusplus
}   // Extern C
#endif
