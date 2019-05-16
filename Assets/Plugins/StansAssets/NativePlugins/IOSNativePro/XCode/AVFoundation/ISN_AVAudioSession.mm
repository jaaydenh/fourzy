#import <AVFoundation/AVFoundation.h>
#import "ISN_Foundation.h"

#if !TARGET_OS_TV


@interface ISN_AVAudioSession : NSObject
-(SA_Result*) setSessionActive:(bool) isActive;
-(SA_Result*) setSessionCategory:(int)category;
@end

@implementation ISN_AVAudioSession

static ISN_AVAudioSession * s_sharedInstance;
+ (id)sharedInstance {
    
    if (s_sharedInstance == nil)  {
        s_sharedInstance = [[self alloc] init];
    }
    return s_sharedInstance;
}

-(id) init {
    [ISN_Logger Log: @"ISN_AVAudioSession::init"];
    if(self = [super init]){
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(audioRouteChanged:) name:AVAudioSessionRouteChangeNotification object:nil];
    }
    return self;
}

-(void)audioRouteChanged:(NSNotification *)notification{
    
    
    NSDictionary *dict = notification.userInfo;
    NSInteger reason = [[dict valueForKey:AVAudioSessionRouteChangeReasonKey] integerValue];
    
    ISN_SendMessage(UNITY_AV_LISTENER, "OnAudioSessionRouteChangeNotification", [NSString stringWithFormat:@"%ld",(long) reason]);
}


-(SA_Result*) setSessionCategory:(int)category {
    NSString *sessionCategory = @"";
    
    switch (category) {
        case 0:
            sessionCategory = AVAudioSessionCategoryAmbient;
            break;
        case 1:
            sessionCategory = AVAudioSessionCategorySoloAmbient;
            break;
        case 2:
            sessionCategory = AVAudioSessionCategoryPlayback;
            break;
        case 3:
            sessionCategory = AVAudioSessionCategoryRecord;
            break;
        case 4:
            sessionCategory = AVAudioSessionCategoryPlayAndRecord;
            break;
        case 5:
            sessionCategory = AVAudioSessionCategoryMultiRoute;
            break;
        default:
            sessionCategory = AVAudioSessionCategorySoloAmbient;
            break;
    }
    
    NSError * error = nil;
    [[AVAudioSession sharedInstance] setCategory: sessionCategory error: &error];
    
    SA_Result* result = [[SA_Result alloc] initWithNSError:error];
    return  result;
}

-(SA_Result*) setSessionActive:(bool) isActive {
    NSError * error = nil;
    [[AVAudioSession sharedInstance] setActive:isActive error:&error];
    
    SA_Result* result = [[SA_Result alloc] initWithNSError:error];
    return  result;
}

@end

extern "C" {
    
    int _ISN_AV_AudioSessionRecordPermission() {
        NSLog(@"recordPermission: %lu", (unsigned long)[[AVAudioSession sharedInstance] recordPermission]);
        return [[AVAudioSession sharedInstance] recordPermission];
    }
    
    void _ISN_AV_AudioSessionRequestRecordPermission(UnityAction callback) {
        [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) {
            if(granted) {
                 ISN_SendCallbackToUnity(callback, ISN_ConvertBoolToString(granted));
            }
        }];
    }
    
    char* _ISN_AV_AudioSessionSetCategory(int category) {
        NSString *tmp = [NSString stringWithFormat:@"%d", category];
        [ISN_Logger LogNativeMethodInvoke:"_ISN_AV_AudioSessionSetCategory" data:[tmp UTF8String]];
        
        
        SA_Result* result = [[ISN_AVAudioSession sharedInstance] setSessionCategory:category];
        return  ISN_ConvertToChar([result toJSONString]);
    }
    
    char* _ISN_AV_AudioSessionSetActive(bool isActive) {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_AV_AudioSessionSetActive" data: isActive ? "enabled"  : "disabled"];
        
        SA_Result* result = [[ISN_AVAudioSession sharedInstance] setSessionActive:isActive];
        return  ISN_ConvertToChar([result toJSONString]);
    }
    
    
    char * _ISN_AV_AudioSessionCategory() {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_AV_AudioSessionCategory" data: ""];
        return ISN_ConvertToChar([AVAudioSession sharedInstance].category);
    }
    
    
}


#endif
