#import "JSONModel.h"
#import <GameKit/GameKit.h>
#import "ISN_Foundation.h"
#import "ISN_NSCommunication.h"
#import "ISN_GKPlayer.h"


@interface ISN_GKLocalPlayer : ISN_GKPlayer

    @property (nonatomic) bool m_authenticated;
    @property (nonatomic) bool m_underage;

    -(id) initWithGKLocalPlayer:(GKLocalPlayer *) player;

@end


@interface ISN_GKLocalPlayerImageLoadResult : SA_Result
@property (nonatomic) NSString *m_base64Image;

@end


@interface ISN_GKGameCenterViewControllerShowResult : JSONModel

//Configuring the Game Center Controller’s Content
@property (nonatomic) NSString *m_leaderboardIdentifier;
@property (nonatomic) GKLeaderboardTimeScope m_leaderboardTimeScope;
@property (nonatomic) GKGameCenterViewControllerState m_viewState;

@end

#if !TARGET_OS_TV

@protocol ISN_GKSavedGame;
@interface ISN_GKSavedGame : JSONModel

//Retrieving Information About a Saved Game File
@property (nonatomic) NSString *m_deviceName;
@property (nonatomic) long m_modificationDate;
@property (nonatomic) NSString *m_name;
@property (nonatomic) NSString *m_id;

-(id) initWithGKSavedGameWithId:(GKSavedGame *) savedGame withId:(NSString *)gameId;

@end


@interface ISN_GKSavedGameFetchResult : SA_Result
    @property (nonatomic) NSArray <ISN_GKSavedGame> *m_savedGames;

    -(id) initWithSKSavedGamesArray:(NSArray<ISN_GKSavedGame> *) array;
@end

@interface ISN_GKSavedGameSaveResult : SA_Result
    @property (nonatomic) ISN_GKSavedGame *m_savedGame;

    -(id) initWithGKSavedGameData:(ISN_GKSavedGame *) savedGame;
@end

@interface ISN_GKSavedGameDeleteResult : SA_Result
@end

@interface ISN_GKResolveSavedGamesRequest : JSONModel
    @property (nonatomic) NSArray <NSString*> *m_conflictedGames;
    @property (nonatomic) NSString *m_data;
@end

@interface ISN_GKSavedGameLoadResult : SA_Result
    @property (nonatomic) NSString *m_data;

    -(id) initWithGKLoadData:(NSString *) data;
@end

#endif



@protocol ISN_GKAchievement;
@interface ISN_GKAchievement : JSONModel
    //Configuring an Achievement
    @property(copy, nonatomic) NSString *m_identifier;
    @property(assign, nonatomic) double m_percentComplete;

    //Reading the State of an Achievement
    //@property(nonatomic) bool m_completed;
    @property(nonatomic) long m_lastReportedDate;

    //Displaying a Notification Banner For an Achievement
    //@property(nonatomic) bool m_showsCompletionBanner;

    //@property(nonatomic) GKPlayer *m_player;

    - (id) initWithGKAchievementData:(GKAchievement *) achievement;
@end


@interface ISN_GKAchievementsResult : SA_Result
    @property (nonatomic) NSArray <ISN_GKAchievement> *m_achievements;

    -(id) initWithGKAchievementsData:(NSArray<ISN_GKAchievement> *) array;
@end




@protocol ISN_GKScore;
@interface ISN_GKScore : JSONModel

@property(nonatomic) long m_rank;
@property(nonatomic) long m_value;
@property(nonatomic) long m_context;
@property (nonatomic) long m_date;

@property(copy, nonatomic) NSString *m_formattedValue;
@property(copy, nonatomic) NSString *m_leaderboardIdentifier;

@property(copy, nonatomic) ISN_GKPlayer *m_player;


- (id) initWithGKScore:(GKScore *) score;
- (GKScore *) toGKScore;
@end




@protocol ISN_GKLeaderboard;
@interface ISN_GKLeaderboard : JSONModel

//out convertation only
@property(copy, nonatomic) NSString *m_identifier;
@property(copy, nonatomic) NSString *m_title;
@property(copy, nonatomic) NSString *m_groupIdentifier;


//in convertation
@property (nonatomic) GKLeaderboardPlayerScope m_playerScope;
@property (nonatomic) GKLeaderboardTimeScope m_timeScope;
@property (nonatomic) ISN_NSRange * m_range;
@property (nonatomic) long m_maxRange;
@property (nonatomic) NSArray <ISN_GKScore> *m_scores;
@property (nonatomic) ISN_GKScore * m_localPlayerScore;

- (GKLeaderboard*) toGKLeaderboard;
- (id) initWithGKLeaderboard:(GKLeaderboard *) leaderboard;

@end

@interface ISN_GKLeaderboardsResult : SA_Result
@property (nonatomic) NSArray <ISN_GKLeaderboard> *m_leaderboards;

-(id) initWithGKLeaderboards:(NSArray<ISN_GKLeaderboard> *) array;
@end

@interface ISN_GKScoreLoadResult : SA_Result
@property (nonatomic) NSArray <ISN_GKScore> *m_scores;
@property (nonatomic) ISN_GKLeaderboard *m_leaderboard;

//-(id) initWithGKLeaderboards:(NSArray<ISN_GKScore> *) array localPlayerScore (ISN_GKScore*) localPlayerScore;
@end

@interface ISN_GKScoreRequest : JSONModel
@property (nonatomic) NSArray <ISN_GKScore> *m_scores;
@end



@interface ISN_IdentityVerificationSignatureResult : SA_Result
@property (nonatomic) NSString * m_publicKeyUrl;
@property (nonatomic) NSString * m_signature;
@property (nonatomic) NSString * m_salt;
@property (nonatomic) uint64_t m_timestamp;

@end


