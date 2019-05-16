#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "ISN_Foundation.h"



@interface ISN_UIDevice : JSONModel
@property (nonatomic) NSString *m_name;
@property (nonatomic) NSString *m_systemName;
@property (nonatomic) NSString *m_model;
@property (nonatomic) NSString *m_localizedModel;
@property (nonatomic) NSString *m_systemVersion;
@property (nonatomic) UIUserInterfaceIdiom m_userInterfaceIdiom;

@property (nonatomic) NSString * m_identifierForVendor;

//Addtional fileds
@property (nonatomic) int m_majorIOSVersion;
@end

@implementation ISN_UIDevice
-(id) init {
    self = [super init];
    if(self) {
        UIDevice* device = [UIDevice currentDevice] ;
        self.m_name         = device.name    == NULL ? @"" : device.name;
        self.m_systemName         = device.systemName    == NULL ? @"" : device.systemName;
        self.m_model         = device.model    == NULL ? @"" : device.model;
        self.m_localizedModel         = device.localizedModel    == NULL ? @"" : device.localizedModel;
        self.m_systemVersion         = device.systemVersion    == NULL ? @"" : device.systemVersion;
        self.m_userInterfaceIdiom = UI_USER_INTERFACE_IDIOM();
       
        NSUUID *vendorIdentifier = [[UIDevice currentDevice] identifierForVendor];
        uuid_t uuid;
        [vendorIdentifier getUUIDBytes:uuid];
        
        NSData *vendorData = [NSData dataWithBytes:uuid length:16];
        NSString *encodedString = [vendorData base64EncodedStringWithOptions:NSDataBase64DecodingIgnoreUnknownCharacters];
        self.m_identifierForVendor = encodedString;
        
        
        if(device.systemVersion!=NULL) {
            NSArray* vComp = [device.systemVersion componentsSeparatedByString:@"."];
            self.m_majorIOSVersion  = [[vComp objectAtIndex:0] intValue];
        } else {
            self.m_majorIOSVersion = 0;
        }
       
    }
    
    return self;
}
@end





extern "C" {
    
    char* _ISN_UI_GetCurrentDevice() {
        ISN_UIDevice * device = [[ISN_UIDevice alloc] init];
        const char* string = [[device toJSONString] UTF8String];
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }

  
}

    



