#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

#import "JSONModel.h"
#import "ISN_Foundation.h"




@interface ISN_GKPlayer : JSONModel

@property (nonatomic) NSString *m_playerID;
@property (nonatomic) NSString *m_alias;
@property (nonatomic) NSString *m_displayName;

-(id) initWithGKPlayer:(GKPlayer *) player;

+ (void) cachePlayer:(GKPlayer *)player;
+ (GKPlayer*) getCachedPlayer:(NSString*) playerID;

@end
