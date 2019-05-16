#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_Foundation.h"
#import "ISN_GKCommunication.h"
#import "ISN_GKLocalPlayerListener.h"
#import "ISN_GKLocalPlayerManager.h"
#import "ISN_Logger.h"
#import "ISN_GKPlayer.h"

@implementation ISN_GKLocalPlayerManager
static ISN_GKLocalPlayerManager * s_sharedInstance;

+ (id)sharedInstance {
    if (s_sharedInstance == nil)  {
        s_sharedInstance = [[self alloc] init];
    }
    return s_sharedInstance;
}

-(id) init {
    self = [super init];
    if(self) {
        self.loadedSavedGames = [[NSMutableDictionary alloc] init];
    }
    return self;
}

#if !TARGET_OS_TV

- (void) cacheSavedGame:(GKSavedGame *)game withId:(NSString *)uniqueId {
  
    NSLog(@"CacheSavedGame uniqueId:  %@", uniqueId);
    
    [[self loadedSavedGames] setObject:game forKey:uniqueId];

}
- (GKSavedGame*) getCachedSavedGame:(NSString*) uniqueId {
    return [[self loadedSavedGames] objectForKey:uniqueId];
}

- (void) removeSavedGameFromCache:(NSString*) uniqueId {
    NSLog(@"getCachedSavedGame uniqueId: %@", uniqueId);
    [[self loadedSavedGames] removeObjectForKey:uniqueId];
}


-(void) authenticateLocalPlayer {
    
    [[GKLocalPlayer localPlayer] setAuthenticateHandler:(^(UIViewController* viewcontroller, NSError *error) {
        
        if(viewcontroller) {//Show login if player is not logged in
            UIViewController *vc =  UnityGetGLViewController();
            [vc presentViewController:viewcontroller animated:YES completion:nil];
            return;
        }
        
        SA_Result *result;
        if(error != nil) {
            result = [[SA_Result alloc] initWithNSError:error];
            ISN_SendMessage(UNITY_GK_LISTENER, "OnAuthenticateResponse", [result toJSONString]);
            return;
        }
        
        if ([GKLocalPlayer localPlayer].isAuthenticated) {//Player is already authenticated & logged in, load game center
            self.playerListener = [[ISN_GKLocalPlayerListener alloc] init];
            
            GKLocalPlayer* localPlayer = [GKLocalPlayer localPlayer];
            [localPlayer registerListener:self.playerListener];
            
            [ISN_GKPlayer cachePlayer:localPlayer];
            
            result = [[SA_Result alloc] init];
        } else {
            SA_Error *sa_error = [[SA_Error alloc] initWithCode:1 message:@"Game center is not enabled on the user device!"];
            result = [[SA_Result alloc] initWithError:sa_error];
        }
        
        ISN_SendMessage(UNITY_GK_LISTENER, "OnAuthenticateResponse", [result toJSONString]);
    })];
}

-(void) fetchSavedGames:(NSString *) requestId {
    [[GKLocalPlayer localPlayer] fetchSavedGamesWithCompletionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
        ISN_GKSavedGameFetchResult *result;
        if(error == NULL) {
            NSMutableArray<ISN_GKSavedGame> *savedGamesArray = [[NSMutableArray<ISN_GKSavedGame> alloc] init];
            
            NSString *uniqueId;
            for (GKSavedGame *save in savedGames) {
                uniqueId = [[NSProcessInfo processInfo] globallyUniqueString];
                [self cacheSavedGame:save withId:uniqueId];
                
                ISN_GKSavedGame *game = [[ISN_GKSavedGame alloc] initWithGKSavedGameWithId:save withId:(uniqueId)];
                [savedGamesArray addObject:game];
            }
            
            result = [[ISN_GKSavedGameFetchResult alloc] initWithSKSavedGamesArray:savedGamesArray];
        } else {
            result = [[ISN_GKSavedGameFetchResult alloc] initWithNSError:error];
        }
        [result setRequestId:requestId];
        
        ISN_SendMessage(UNITY_GK_LISTENER, "OnFetchSavedGamesResponse", [result toJSONString]);
    }];
}

-(void) saveGameData:(NSString *) requestId withName:(NSString *)name withData:(NSData *)data {
    [[GKLocalPlayer localPlayer] saveGameData:data withName:name completionHandler:^(GKSavedGame * _Nullable savedGame, NSError * _Nullable error) {
        ISN_GKSavedGameSaveResult *result;
        if(error == NULL) {
            
            NSString *uniqueId = [[NSProcessInfo processInfo] globallyUniqueString];
            [self cacheSavedGame:savedGame withId:uniqueId];
            ISN_GKSavedGame *game = [[ISN_GKSavedGame alloc] initWithGKSavedGameWithId:savedGame withId:(uniqueId)];
            result = [[ISN_GKSavedGameSaveResult alloc] initWithGKSavedGameData:game];
        } else {
            result = [[ISN_GKSavedGameSaveResult alloc] initWithNSError:error];
        }
        [result setRequestId:requestId];
        
        ISN_SendMessage(UNITY_GK_LISTENER, "OnSavedGameResponse", [result toJSONString]);
    }];
}

- (void) deleteSavedGame:(NSString *) requestId withName:(NSString *)name withUniqueId:(NSString *) uniqueId {
    [[GKLocalPlayer localPlayer] deleteSavedGamesWithName:name completionHandler:^(NSError * _Nullable error) {
        SA_Result *result;
        if(error == NULL) {
            [self removeSavedGameFromCache:uniqueId];
            result = [[SA_Result alloc] init];
        } else {
            result = [[SA_Result alloc] initWithNSError:error];
        }
        [result setRequestId:requestId];
        
        ISN_SendMessage(UNITY_GK_LISTENER, "OnDeleteSavedGameResponse", [result toJSONString]);
    }];
}

- (void) loadSaveData:(NSString *)requestId withName:(NSString *)name withUniqueId:(NSString *) uniqueId {
    NSLog(@"LoadSaveData uniqueId:  %@", uniqueId);
    GKSavedGame *save = [self getCachedSavedGame:uniqueId];
    if(save != NULL) {
        [save loadDataWithCompletionHandler:^(NSData * _Nullable data, NSError * _Nullable error) {
            
            ISN_GKSavedGameLoadResult *result;
            if(error == NULL) {
                result = [[ISN_GKSavedGameLoadResult alloc] initWithGKLoadData:[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]];
            } else {
                result = [[ISN_GKSavedGameLoadResult alloc] initWithNSError:error];
            }
            [result setRequestId:requestId];
            
            ISN_SendMessage(UNITY_GK_LISTENER, "OnLoadSavedGameDataResponse", [result toJSONString]);
        }];
    } else {
        SA_Error *sa_error = [[SA_Error alloc] initWithCode:999 message:@"Saved game not found. Check the game's name of fetch saved games before using this method, please!"];
        ISN_GKSavedGameLoadResult *result = [[ISN_GKSavedGameLoadResult alloc] initWithError:sa_error];
        [result setRequestId:requestId];
        
        ISN_SendMessage(UNITY_GK_LISTENER, "OnLoadSavedGameDataResponse", [result toJSONString]);
    }
}

-(void) resolveConflictingSavedGames:(NSString *) requestId withConflictedSavedGamesIds:(NSArray<NSString *> *)conflictedSavedGamesIds withData:(NSData*)data {
     NSMutableArray <GKSavedGame *> * conflicts = [[NSMutableArray alloc] init];
     for (NSString *uniqueId in conflictedSavedGamesIds) {
         GKSavedGame *save = [self getCachedSavedGame:uniqueId];
         if(save != nil) {
             [conflicts addObject:save];
         }
     }
    
     [[GKLocalPlayer localPlayer] resolveConflictingSavedGames:conflicts withData:data completionHandler:^(NSArray<GKSavedGame *> * _Nullable savedGames, NSError * _Nullable error) {
         ISN_GKSavedGameFetchResult *result;
         if(error == NULL) {
             NSMutableArray<ISN_GKSavedGame> *savedGamesArray = [[NSMutableArray<ISN_GKSavedGame> alloc] init];
             
             NSString *uniqueId;
             for (GKSavedGame *save in savedGames) {
                 uniqueId = [[NSProcessInfo processInfo] globallyUniqueString];
                 [[ISN_GKLocalPlayerManager sharedInstance] cacheSavedGame:save withId:uniqueId];
                 
                 ISN_GKSavedGame *game = [[ISN_GKSavedGame alloc] initWithGKSavedGameWithId:save withId:uniqueId];
                 [savedGamesArray addObject:game];
             }
             
             result = [[ISN_GKSavedGameFetchResult alloc] initWithSKSavedGamesArray:savedGamesArray];
         } else {
             result = [[ISN_GKSavedGameFetchResult alloc] initWithNSError:error];
         }
         [result setRequestId:requestId];
         
         ISN_SendMessage(UNITY_GK_LISTENER, "OnResolveSavedGamesResponse", [result toJSONString]);
     }];
 }

#endif

@end


#ifdef __cplusplus
extern "C" {
#endif
    
    void _ISN_AuthenticateLocalPlayer ()  {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_AuthenticateLocalPlayer" data:""];
        [[ISN_GKLocalPlayerManager sharedInstance] authenticateLocalPlayer];
    }
    
    char* _ISN_GetGKLocalPlayerInfo ()  {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GetGKLocalPlayerInfo" data:""];
        ISN_GKLocalPlayer *player = [[ISN_GKLocalPlayer alloc] initWithGKLocalPlayer:[GKLocalPlayer localPlayer]];
        
        return ISN_ConvertToChar([player toJSONString]);
    }
    
    void _ISN_GKLocalPlayer_FetchSavedGames(char* requestId) {
        #if !TARGET_OS_TV
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLocalPlayer_FetchSavedGames" data:requestId];
        [[ISN_GKLocalPlayerManager sharedInstance] fetchSavedGames:ISN_ConvertToString(requestId)];
#endif
    }
    
    void _ISN_GKLocalPlayer_SaveGameData(char* requestId, char* name, char* data) {
        #if !TARGET_OS_TV
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLocalPlayer_SaveGameData" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s name: %s data: %s: ", requestId, name, data])];
        
        NSString* m_requestId = ISN_ConvertToString(requestId);
        
        NSString* m_name = ISN_ConvertToString(name);
        
        NSString* m_dataString = ISN_ConvertToString(data);
        
        NSData *m_data = [m_dataString dataUsingEncoding:NSUTF8StringEncoding];
        
        [[ISN_GKLocalPlayerManager sharedInstance] saveGameData:m_requestId withName:m_name withData:m_data];
#endif
    }
    
    void _ISN_GKLocalPlayer_DeleteSavedGame(char* requestId, char* name, char* uniqueId) {
        #if !TARGET_OS_TV
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLocalPlayer_DeleteSavedGame" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s name: %s uniqueId: %s: ", requestId, name, uniqueId])];
        
        [[ISN_GKLocalPlayerManager sharedInstance] deleteSavedGame:ISN_ConvertToString(requestId) withName:ISN_ConvertToString(name) withUniqueId:ISN_ConvertToString(uniqueId)];
#endif
    }
    
    void _ISN_GKLocalPlayer_LoadGameData(char* requestId, char* name, char* uniqueId) {
        #if !TARGET_OS_TV
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLocalPlayer_LoadGameData" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s name: %s uniqueId: %s", requestId, name, uniqueId])];
        
        [[ISN_GKLocalPlayerManager sharedInstance] loadSaveData:ISN_ConvertToString(requestId) withName:ISN_ConvertToString(name) withUniqueId:ISN_ConvertToString(uniqueId)];
#endif
    }
    
    void _ISN_GKLocalPlayer_ResolveSavedGames(char* requestId, char* jsonContent) {
        #if !TARGET_OS_TV
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLocalPlayer_ResolveSavedGames" data:ISN_ConvertToChar([NSString stringWithFormat:@"requestId: %s contentJSON: %s", requestId, jsonContent])];
        
        NSError *jsonError;
        ISN_GKResolveSavedGamesRequest *requestData = [[ISN_GKResolveSavedGamesRequest alloc] initWithChar:jsonContent error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"ISN_GKResolveSavedGamesRequest JSON parsing error: %@", jsonError.description];
        }
        
        NSData *m_data = [requestData.m_data dataUsingEncoding:NSUTF8StringEncoding];
        [[ISN_GKLocalPlayerManager sharedInstance] resolveConflictingSavedGames:ISN_ConvertToString(requestId) withConflictedSavedGamesIds:requestData.m_conflictedGames withData:m_data];
#endif
    }
    
    
    void _ISN_GKLocalPlayer_GenerateIdentityVerificationSignatureWithCompletionHandler(UnityAction callback) {
        
        #if !TARGET_OS_TV
        GKLocalPlayer *localPlayer =  [GKLocalPlayer localPlayer];
        
        [localPlayer generateIdentityVerificationSignatureWithCompletionHandler:^(NSURL *publicKeyUrl, NSData *signature, NSData *salt, uint64_t timestamp, NSError *error) {
            
            ISN_IdentityVerificationSignatureResult* result;
            if(error != NULL) {
                result = [[ISN_IdentityVerificationSignatureResult alloc] initWithNSError:error];
            } else {
                result = [[ISN_IdentityVerificationSignatureResult alloc] init];
                

                NSLog(@"generateIdentityVerificationSignatureWithCompletionHandler");
                NSLog(@"timestamp: %llu", timestamp);
            
                [result setM_salt:[salt base64EncodedStringWithOptions:NSDataBase64DecodingIgnoreUnknownCharacters]];
                [result setM_signature:[signature base64EncodedStringWithOptions:NSDataBase64DecodingIgnoreUnknownCharacters]];
                [result setM_publicKeyUrl:publicKeyUrl.absoluteString];
                [result setM_timestamp:timestamp];
            }
            
            ISN_SendCallbackToUnity(callback, [result toJSONString]);
            
        }];
#endif
    }
    
#if __cplusplus
}   // Extern C
#endif


