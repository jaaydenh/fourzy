#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_Foundation.h"
#import "ISN_GKCommunication.h"
#import "ISN_GKPlayer.h"



@implementation ISN_GKPlayer

static NSMutableDictionary * s_playersCache = [[NSMutableDictionary alloc] init];

-(id) init { return self = [super init]; }
-(id) initWithGKPlayer:(GKPlayer *) player {
    self = [super init];
    if(self) {
        self.m_playerID      = player.playerID    == NULL ? @"" : player.playerID;
        self.m_alias         = player.alias       == NULL ? @"" : player.alias;
        self.m_displayName   = player.displayName == NULL ? @"" : player.displayName;
    }
    return self;
}


+ (void) cachePlayer:(GKPlayer *)player {
    NSLog(@"cachePlayer uniqueId:  %@", player.playerID);
    [s_playersCache setObject:player forKey:player.playerID];
}


+ (GKPlayer*) getCachedPlayer:(NSString*) playerID {
    return [s_playersCache objectForKey:playerID];
}



@end



#ifdef __cplusplus
extern "C" {
#endif
    
    void _ISN_GKPlayer_LoadPhotoForSize(char* playerId, int size, UnityAction callback) {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKLeaderboar_LoadLeaderboards" data:playerId];
        
        NSString* playerID =  ISN_ConvertToString(playerId);
        GKPlayer* player = [ISN_GKPlayer getCachedPlayer:playerID];
        
        GKPhotoSize photoSize;
        
        switch (size) {
            case 0:
                photoSize = GKPhotoSizeSmall;
                break;
                
            default:
                photoSize = GKPhotoSizeNormal;
                break;
        }
        
        [player loadPhotoForSize:photoSize withCompletionHandler:^(UIImage * _Nullable photo, NSError * _Nullable error) {
            
            ISN_GKLocalPlayerImageLoadResult* result;
            if(error == nil) {
                result.m_base64Image = ISN_ConvertImageToBase64(photo);
            } else {
                result = [[ISN_GKLocalPlayerImageLoadResult alloc] initWithNSError:error];
            }
            
            ISN_SendCallbackToUnity(callback, [result toJSONString]);
        }];
    }
    
    
    
#if __cplusplus
}   // Extern C
#endif
