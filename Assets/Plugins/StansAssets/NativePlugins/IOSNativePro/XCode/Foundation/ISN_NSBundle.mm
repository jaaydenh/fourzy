#import <Foundation/Foundation.h>
#import "ISN_Foundation.h"

@interface ISN_NSBuildInfo : JSONModel
@property (nonatomic) NSString* m_appVersion;
@property (nonatomic) NSString* m_buildNumber;
@end

@implementation ISN_NSBuildInfo
-(id) init { return self = [super init]; }
@end

extern "C" {
    
    //we don't have this on Unity C# part
    bool _ISN_NS_isAppStoreReceiptSandbox() {
        #if TARGET_IPHONE_SIMULATOR
            return NO;
        #else
            NSURL *appStoreReceiptURL = NSBundle.mainBundle.appStoreReceiptURL;
            NSString *appStoreReceiptLastComponent = appStoreReceiptURL.lastPathComponent;
        
            BOOL isSandboxReceipt = [appStoreReceiptLastComponent isEqualToString:@"sandboxReceipt"];
            return isSandboxReceipt;
        #endif
    }
    
    bool _ISN_NS_IsRunningInAppStoreEnvironment() {
        #if TARGET_IPHONE_SIMULATOR
            return NO;
        #else
            bool hasEmbeddedMobileProvision = [[NSBundle mainBundle] pathForResource:@"embedded" ofType:@"mobileprovision"];
            if (_ISN_NS_isAppStoreReceiptSandbox() || hasEmbeddedMobileProvision) {
                return NO;
            }
            return YES;
        #endif
    }
    
    char* _ISN_NS_GetBuildInfo() {
        ISN_NSBuildInfo *buildInfo = [[ISN_NSBuildInfo alloc] init];
        NSDictionary *infoDict = [[NSBundle mainBundle] infoDictionary];
        [buildInfo setM_appVersion:[infoDict objectForKey:@"CFBundleShortVersionString"]]; // example: 1.0.0
        [buildInfo setM_buildNumber:[infoDict objectForKey:@"CFBundleVersion"]]; // example: 42
        
        return ISN_ConvertToChar([buildInfo toJSONString]);
    }
}


