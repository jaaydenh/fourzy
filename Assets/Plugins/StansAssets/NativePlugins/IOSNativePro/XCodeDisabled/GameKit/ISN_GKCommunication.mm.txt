#import <Foundation/Foundation.h>
#import "ISN_GKCommunication.h"


@implementation ISN_GKLocalPlayer
    -(id) init { return self = [super init]; }
    -(id) initWithGKLocalPlayer:(GKLocalPlayer *) player {
        self = [super initWithGKPlayer:player];
        if(self) {
            self.m_authenticated = player.authenticated;
            self.m_underage      = player.underage;
        }
        return self;
    }
@end

@implementation ISN_GKLocalPlayerImageLoadResult
 -(id) init { return self = [super init]; }
@end

@implementation ISN_GKGameCenterViewControllerShowResult
    -(id) init {  return self = [super init]; }
@end

#if !TARGET_OS_TV

@implementation ISN_GKSavedGame
    -(id) init { return self = [super init]; }
    -(id) initWithGKSavedGameWithId:(GKSavedGame *) savedGame withId:(NSString *)gameId {
        self = [super init];
        if(self) {
            self.m_deviceName       = savedGame.deviceName  == NULL ? @"" : savedGame.deviceName;
            self.m_name             = savedGame.name        == NULL ? @"" : savedGame.name;
            
            if(savedGame.modificationDate != NULL) {
                self.m_modificationDate = [savedGame.modificationDate timeIntervalSince1970];
            } else {
                self.m_modificationDate = 0;
            }
            self.m_id = gameId;
        }
        return self;
    }
@end

@implementation ISN_GKSavedGameFetchResult
-(id) init { return self = [super init]; }
-(id) initWithSKSavedGamesArray:(NSArray<ISN_GKSavedGame> *) array{
    self = [super init];
    if(self) {
        self.m_savedGames = array;
    }
    return  self;
}
@end

@implementation ISN_GKSavedGameSaveResult
    -(id) init { return self = [super init]; }
    -(id) initWithGKSavedGameData:(ISN_GKSavedGame *) savedGame {
        self = [super init];
        if(self) {
            self.m_savedGame = savedGame;
        }
        return self;
    }
@end

@implementation ISN_GKSavedGameDeleteResult
-(id) init { return self = [super init]; }
@end

@implementation ISN_GKSavedGameLoadResult
    -(id) init { return self = [super init]; }
    -(id) initWithGKLoadData:(NSString *) data {
        self = [super init];
        if(self) {
            self.m_data = data == NULL ? @"" : data;
        }
        return self;
    }
@end

@implementation ISN_GKResolveSavedGamesRequest
    -(id) init { return self = [super init]; }
@end

#endif

@implementation ISN_GKAchievement

-(id) init { return self = [super init]; }
-(id) initWithGKAchievementData:(GKAchievement *) achievement {
    self = [super init];
    if(self) {
        self.m_identifier = achievement.identifier;
        self.m_percentComplete = achievement.percentComplete;
        //self.m_completed = achievement.completed;
        
        if(achievement.lastReportedDate != NULL) {
            NSDate * mydate = [[NSDate alloc] init];
            NSTimeZone *zone = [NSTimeZone systemTimeZone];
            NSInteger interval = [zone secondsFromGMTForDate:achievement.lastReportedDate];
            mydate = [mydate dateByAddingTimeInterval:interval];
            self.m_lastReportedDate = [mydate timeIntervalSince1970];
        } else {
            self.m_lastReportedDate = 0;
        }
    }
    return self;
}
@end

@implementation ISN_GKAchievementsResult
-(id) init { return self = [super init]; }

-(id) initWithGKAchievementsData:(NSArray<ISN_GKAchievement> *) array {
    self = [super init];
    if(self) {
        self.m_achievements = array;
    }
    return  self;
}
@end



@implementation ISN_GKScore : JSONModel


- (id) initWithGKScore:(GKScore *) score {
    
    
    self = [super init];
    if(self) {
        self.m_rank = score.rank;
        self.m_context = score.context;
        self.m_value = score.value;
        
        
        if(score.date != NULL) {
            NSDate * mydate = [[NSDate alloc] init];
            NSTimeZone *zone = [NSTimeZone systemTimeZone];
            NSInteger interval = [zone secondsFromGMTForDate:score.date];
            mydate = [mydate dateByAddingTimeInterval:interval];
            self.m_date = [mydate timeIntervalSince1970];
        } else {
            self.m_date = 0;
        }
        
        self.m_leaderboardIdentifier = score.leaderboardIdentifier;
        self.m_formattedValue = score.formattedValue  == NULL ? @"" : score.formattedValue;
        
        if(score.player != NULL) {
            self.m_player = [[ISN_GKPlayer alloc] initWithGKPlayer:score.player];
        }
    }
    return self;
}

- (GKScore *) toGKScore {
    GKScore *score = [[GKScore alloc] initWithLeaderboardIdentifier: self.m_leaderboardIdentifier];
    score.value = self.m_value;
    score.context = self.m_context;
    
    return score;
}

@end


@implementation ISN_GKLeaderboard

-(id) init { return self = [super init]; }
-(id) initWithGKLeaderboard:(GKLeaderboard *) leaderboard {
    self = [super init];
    if(self) {
        self.m_identifier = leaderboard.identifier;
        self.m_groupIdentifier = leaderboard.groupIdentifier  == NULL ? @"" : leaderboard.groupIdentifier;
        self.m_title = leaderboard.title  == NULL ? @"" : leaderboard.title;
        
        self.m_playerScope = leaderboard.playerScope;
        self.m_timeScope = leaderboard.timeScope;
        
        self.m_range = [[ISN_NSRange alloc] initWithNSRange:leaderboard.range];
     
        self.m_maxRange = leaderboard.maxRange;
        
       NSMutableArray<ISN_GKScore> *scoresArray = [[NSMutableArray<ISN_GKScore> alloc] init];
        
        if(leaderboard.scores != NULL) {
            for (GKScore* score in leaderboard.scores) {
                ISN_GKScore *isn_score = [[ISN_GKScore alloc] initWithGKScore:score];
                [scoresArray addObject:isn_score];
            }
        }
        
        self.m_scores = scoresArray;
        
        if(leaderboard.localPlayerScore != NULL) {
            self.m_localPlayerScore = [[ISN_GKScore alloc] initWithGKScore:leaderboard.localPlayerScore];
        }
      
    }
    return self;
}

- (GKLeaderboard*) toGKLeaderboard {
    GKLeaderboard *leaderboard = [[GKLeaderboard alloc] init];
    
    if(self.m_identifier.length > 0) {
        leaderboard.identifier = self.m_identifier;
    }
    
    leaderboard.playerScope = self.m_playerScope;
    leaderboard.timeScope = self.m_timeScope;
    leaderboard.range = [self.m_range getNSRange];
    
  
    

    return leaderboard;
}

@end

@implementation ISN_GKLeaderboardsResult
-(id) init { return self = [super init]; }

-(id) initWithGKLeaderboards:(NSArray<ISN_GKLeaderboard> *) array {
    self = [super init];
    if(self) {
        self.m_leaderboards = array;
    }
    return  self;
}
@end


@implementation ISN_GKScoreLoadResult
-(id) init { return self = [super init]; }
@end


@implementation ISN_GKScoreRequest : JSONModel
-(id) init { return self = [super init]; }
@end


@implementation ISN_IdentityVerificationSignatureResult

-(id) init { return self = [super init]; }
@end




