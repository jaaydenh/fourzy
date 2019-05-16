#import "JSONModel.h"
#import "ISN_Foundation.h"
#import "ISN_NSCommunication.h"

#import <AVFoundation/AVFoundation.h>


@protocol ISN_AVPlayer;
@interface ISN_AVPlayer : JSONModel

@property(nonatomic) ISN_NSURL* m_url;
@property(nonatomic) float m_volume;


-(AVPlayer* ) toAVPlayer;

@end
