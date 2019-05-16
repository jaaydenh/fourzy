#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>


@interface ISN_GKAchievementManager : NSObject

+ (id)sharedInstance;

- (void) loadAchievements:(NSString *) requestId;
- (void) resetAchievements:(NSString *) requestId;
- (void) reportAchievements:(NSString *) requestId withAchievementsArray:(NSArray<ISN_GKAchievement> *) achievementsArray;


@end
