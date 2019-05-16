#import "ISN_Foundation.h"


@interface ISN_UIAvailableMediaTypes : JSONModel
@property (nonatomic) NSArray <NSString *> *m_types;
-(id) initWithArray:(NSArray <NSString *> *) array;
@end


#if !TARGET_OS_TV
@interface ISN_UIPickerControllerRequest : JSONModel
@property (nonatomic) NSArray <NSString *> *m_mediaTypes;
@property (nonatomic) UIImagePickerControllerSourceType m_sourceType;
@property (nonatomic) bool m_allowsEditing;

@property (nonatomic) float m_imageCompressionRate;
@property (nonatomic) int m_maxImageSize;
@property (nonatomic) int m_encodingType;

@end
#endif

@interface ISN_UIPickerControllerResult : SA_Result
@property (nonatomic) NSString*  m_mediaURL;
@property (nonatomic) NSString*  m_imageURL;
@property (nonatomic) NSString*  m_mediaType;
@property (nonatomic) NSString*  m_encodedImage;

@end



@interface UIImage (fixOrientation)
- (UIImage *)fixOrientation;
@end



//--------------------------------------
//  Build Info
//--------------------------------------

@protocol ISN_BuildInfo;
@interface ISN_BuildInfo : JSONModel
@property (nonatomic) NSString* m_appVersion;
@property (nonatomic) NSString* m_buildNumber;
@end


//--------------------------------------
//  UIAlertController
//--------------------------------------

@protocol ISN_UIAlertAction;
@interface ISN_UIAlertAction : JSONModel
@property (nonatomic) int m_id;
@property (nonatomic) NSString* m_title;
@property (nonatomic) NSString* m_image;
@property (nonatomic) UIAlertActionStyle m_style;



@property (nonatomic) bool m_enabled;
@property (nonatomic) bool m_preffered;
@end


@protocol ISN_UIAlertController;
@interface ISN_UIAlertController : JSONModel
@property (nonatomic) int m_id;
@property (nonatomic) NSString* m_title;
@property (nonatomic) NSString* m_message;
@property (nonatomic) UIAlertControllerStyle m_preferredStyle;
@property (nonatomic) NSArray <ISN_UIAlertAction>* m_actions;
@end



@interface ISN_UIAlertActionId : JSONModel
@property (nonatomic) int m_alertId;
@property (nonatomic) int m_actionId;
@end


@interface ISN_UIRegisterRemoteNotificationsResult : SA_Result
@property (nonatomic) NSString* m_deviceTokenUTF8;
@end








