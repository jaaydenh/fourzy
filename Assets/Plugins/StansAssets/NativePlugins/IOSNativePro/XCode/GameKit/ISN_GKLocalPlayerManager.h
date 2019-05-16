#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface ISN_GKLocalPlayerManager : NSObject
+ (id)sharedInstance;
- (void) authenticateLocalPlayer;

#if !TARGET_OS_TV
- (void) fetchSavedGames:(NSString *) requestId;
- (void) saveGameData:(NSString *) requestId withName:(NSString *)name withData:(NSData *)data;
- (void) deleteSavedGame:(NSString *) requestId withName:(NSString *)name withUniqueId:(NSString *) uniqueId;
- (void) loadSaveData:(NSString *)requestId withName:(NSString *)name withUniqueId:(NSString *) uniqueId;
- (void) cacheSavedGame:(GKSavedGame *)game withId:(NSString *)uniqueId;
- (void) resolveConflictingSavedGames:(NSString *) requestId withConflictedSavedGamesIds:(NSArray<NSString *> *)conflictedSavedGamesIds withData:(NSData*)data;
- (void) removeSavedGameFromCache:(NSString*) uniqueId;
- (GKSavedGame*) getCachedSavedGame:(NSString*) uniqueId;

#endif

@property (nonatomic) NSMutableDictionary* loadedSavedGames;
@property (nonatomic) ISN_GKLocalPlayerListener* playerListener;

@end
