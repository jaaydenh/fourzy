#if !TARGET_OS_TV

////////////////////////////////////////////////////////////////////////////////
//
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets)
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#import <Foundation/Foundation.h>
#import <Accounts/Accounts.h>
#import <Social/Social.h>
#import <MessageUI/MessageUI.h>

#import "ISN_Foundation.h"


@protocol ISN_UIActivityViewControllerResult;
@interface ISN_UIActivityViewControllerResult : SA_Result
@property (nonatomic) NSString*  m_activityType;
//@property (nonatomic) NSArray <NSString*>* m_returnedItems;
@property (nonatomic) bool m_completed;
@end


@implementation ISN_UIActivityViewControllerResult
-(id) init {return [super init]; }
@end



@interface ISN_UIActivityViewController : JSONModel
@property (nonatomic) NSString*  m_text;
@property (nonatomic) NSArray <NSString*>* m_images;
@property (nonatomic) NSArray <NSString*>* m_excludedActivityTypes;
@end


@implementation ISN_UIActivityViewController
-(id) init {return [super init]; }
@end



extern "C" {
    

    void _ISN_SOCIAL_PresentActivityViewController(char* data, UnityAction callback) {
        
        
        [ISN_Logger LogNativeMethodInvoke:"_ISN_UI_SaveToCameraRoll" data:data];
        
        NSError *jsonError;
        ISN_UIActivityViewController *request = [[ISN_UIActivityViewController alloc] initWithChar:data error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"_ISN_LoadStore JSON parsing error: %@", jsonError.description];
        }
        
        
        UIActivityViewController *controller;
        
        NSMutableArray* activityItems  = [[NSMutableArray alloc] init];
        if(request.m_text.length != 0) {
            [activityItems addObject:request.m_text];
        }
        
        for(NSString* media in request.m_images) {
            NSData *imageData = [[NSData alloc] initWithBase64EncodedString:media options:NSDataBase64DecodingIgnoreUnknownCharacters];
            UIImage *image = [[UIImage alloc] initWithData:imageData];
            [activityItems addObject:image];
        }
        
        controller = [[UIActivityViewController alloc] initWithActivityItems:activityItems applicationActivities:nil];
        
        
        NSMutableArray* excludedActivityTypes  = [[NSMutableArray alloc] init];
        for(NSString* activityType in request.m_excludedActivityTypes) {
            NSLog(@"Excluded activityType: %@", activityType);
            [excludedActivityTypes addObject:activityType];
        }
        
        if(excludedActivityTypes.count > 0) {
            controller.excludedActivityTypes =excludedActivityTypes;
        }
        
        //controller set
        
        UIViewController *vc =  UnityGetGLViewController();
        NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
        if ([[vComp objectAtIndex:0] intValue] >= 8) {
            UIPopoverPresentationController *presentationController = [controller popoverPresentationController];
            presentationController.sourceView = vc.view;
        }
        
        controller.completionWithItemsHandler = ^(NSString *activityType, BOOL completed, NSArray *returnedItems, NSError *activityError) {
            
            ISN_UIActivityViewControllerResult* result;
            if(activityError == NULL) {
                result = [[ISN_UIActivityViewControllerResult alloc] init];
                [result setM_activityType:activityType];
               // [result setM_returnedItems:returnedItems];
                [result setM_completed:completed];
            } else {
                result = [[ISN_UIActivityViewControllerResult alloc] initWithNSError:activityError];
            }
            
            ISN_SendCallbackToUnity(callback, [result toJSONString]);
         
        };
        
        [vc presentViewController:controller animated:YES completion:nil];
        
        
    }

}


#endif
