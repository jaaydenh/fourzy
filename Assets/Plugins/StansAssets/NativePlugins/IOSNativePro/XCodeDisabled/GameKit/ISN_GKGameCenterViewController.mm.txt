#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "ISN_Foundation.h"
#import "ISN_GKCommunication.h"

//viwe close callback
static UnityAction s_callback;
static GKGameCenterViewController* leaderboardsView;


@interface INS_GKGameCenterControllerDelegate : NSObject<GKGameCenterControllerDelegate>
@end

@implementation INS_GKGameCenterControllerDelegate

-(void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController {
    [gameCenterViewController dismissViewControllerAnimated:YES completion:nil];
    SA_Result* result = [[SA_Result alloc] init];
    ISN_SendCallbackToUnity(s_callback, [result toJSONString]);
}
@end

@interface ISN_GKGameCenterViewController : NSObject

@property (nonatomic) INS_GKGameCenterControllerDelegate *m_viewDelegate;

- (void) present:(ISN_GKGameCenterViewControllerShowResult *) request;

@end

@implementation ISN_GKGameCenterViewController

static ISN_GKGameCenterViewController * s_sharedInstance;

+ (id)sharedInstance {
    if (s_sharedInstance == nil)  {
        s_sharedInstance = [[self alloc] init];
    }
    return s_sharedInstance;
}

- (void) present:(ISN_GKGameCenterViewControllerShowResult *) request {
    UIViewController *vc =  UnityGetGLViewController();
    leaderboardsView     = [[GKGameCenterViewController alloc] init];
    self.m_viewDelegate  = [[INS_GKGameCenterControllerDelegate alloc] init];
    leaderboardsView.gameCenterDelegate = self.m_viewDelegate;
    
      #if !TARGET_OS_TV
    switch (request.m_viewState) {
        case -1:
             leaderboardsView.viewState = GKGameCenterViewControllerStateDefault;
             break;
        case 0:
            leaderboardsView.leaderboardIdentifier  = request.m_leaderboardIdentifier;
            leaderboardsView.leaderboardTimeScope   = request.m_leaderboardTimeScope;
            leaderboardsView.viewState = GKGameCenterViewControllerStateLeaderboards;
            break;
        case 1:
            leaderboardsView.viewState = GKGameCenterViewControllerStateAchievements;
            break;
        case 2:
            leaderboardsView.viewState = GKGameCenterViewControllerStateChallenges;
            break;
            
        default:
            break;
    }
    #endif
    
    [vc presentViewController: leaderboardsView animated: YES completion:nil];
}

@end



extern "C" {
    void _ISN_GKGameCenterViewControllerShow(char* data, UnityAction callback) {
        s_callback = callback;
        [ISN_Logger LogNativeMethodInvoke:"_ISN_GKGameCenterViewControllerShow" data:data];

        NSError *jsonError;
        ISN_GKGameCenterViewControllerShowResult *request = [[ISN_GKGameCenterViewControllerShowResult alloc] initWithChar:data error:&jsonError];
        if (jsonError) {
            NSLog(@"_ISN_Show JSON parsing error: %@", jsonError.description);
        }
        
        [[ISN_GKGameCenterViewController sharedInstance] present:request];
    }
}
