#if !TARGET_OS_TV
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "ISN_Foundation.h"
#import "ISN_UIImagePickerControllerDelegate.h"
#import "ISN_UICommunication.h"



@interface ISN_UIImagePickerController : NSObject


@property (nonatomic)  ISN_UIImagePickerControllerDelegate *m_pickerDelegate;
@end




@implementation ISN_UIImagePickerController


//--------------------------------------
//  Initialization
//--------------------------------------


static ISN_UIImagePickerController * s_sharedInstance;
+ (id)sharedInstance {
    if (s_sharedInstance == nil)  {
        s_sharedInstance = [[self alloc] init];
    }
    return s_sharedInstance;
}

-(id) init {
    self = [super init];
    if(self) {
        self.m_pickerDelegate = [[ISN_UIImagePickerControllerDelegate alloc] init];
    }
    return self;
}



//--------------------------------------
// Save
//--------------------------------------


- (void) saveToCameraRoll:(NSString *)media {
    NSData *imageData = [[NSData alloc] initWithBase64EncodedString:media options:NSDataBase64DecodingIgnoreUnknownCharacters];
    UIImage *image = [[UIImage alloc] initWithData:imageData];
    
    UIImageWriteToSavedPhotosAlbum(image,
                                   self, // send the message to 'self' when calling the callback
                                   @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), // the selector to tell the method to call on completion
                                   NULL); // you generally won't need a contextInfo here
}

- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
    SA_Result * result = [[SA_Result alloc] initWithNSError:error];
    ISN_SendMessage(UNITY_UI_LISTENER, "OnImageSave", [result toJSONString]);
}

//--------------------------------------
// Pick
//--------------------------------------

-(void) presentPickerController:(ISN_UIPickerControllerRequest*) request {
    
    [self.m_pickerDelegate setControllerRequest:request];
    
    UIViewController *vc =  UnityGetGLViewController();
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    [picker setModalPresentationStyle: UIModalPresentationOverCurrentContext];
    
    picker.mediaTypes = request.m_mediaTypes;
    picker.sourceType = request.m_sourceType;
    picker.allowsEditing  = request.m_allowsEditing;
    
    picker.delegate = self.m_pickerDelegate;
    [vc presentViewController:picker animated:YES completion:nil];
}

@end




extern "C" {
    
    
    
    char* _ISN_UI_GetAvailableMediaTypesForSourceType(int type) {
        UIImagePickerControllerSourceType sourceType = static_cast<UIImagePickerControllerSourceType>(type);
        NSArray * array =  [UIImagePickerController availableMediaTypesForSourceType:sourceType];
        
        ISN_UIAvailableMediaTypes * result = [[ISN_UIAvailableMediaTypes alloc] initWithArray:array];
        const char* string = [[result toJSONString] UTF8String];
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    bool _ISN_UI_IsSourceTypeAvailable(int type) {
        UIImagePickerControllerSourceType sourceType = static_cast<UIImagePickerControllerSourceType>(type);
        return [UIImagePickerController isSourceTypeAvailable:sourceType];
    }
    
    void _ISN_UI_SaveToCameraRoll(char* encodedMedia) {
        NSString *media = [NSString stringWithUTF8String: encodedMedia];
        [[ISN_UIImagePickerController sharedInstance] saveToCameraRoll:media];
    }
    
    
    void _ISN_UI_PresentPickerController(char* data) {
        [ISN_Logger LogNativeMethodInvoke:"_ISN_UI_SaveToCameraRoll" data:data];
        
        NSError *jsonError;
        ISN_UIPickerControllerRequest *request = [[ISN_UIPickerControllerRequest alloc] initWithChar:data error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"_ISN_LoadStore JSON parsing error: %@", jsonError.description];
        }
        
        [[ISN_UIImagePickerController sharedInstance] presentPickerController:request];
        
    }
    
}


#endif
