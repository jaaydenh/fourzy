#import "JSONModel.h"
#import "ISN_Logger.h"


#define SYSTEM_VERSION_GRATERTHAN_OR_EQUALTO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)

//--------------------------------------
// Constants
//--------------------------------------

extern const char* UNITY_SK_LISTENER;
extern const char* UNITY_RP_LISTENER;
extern const char* UNITY_CN_LISTENER;
extern const char* UNITY_AV_LISTENER;
extern const char* UNITY_PH_LISTENER;
extern const char* UNITY_UI_LISTENER;
extern const char* UNITY_CK_LISTENER;
extern const char* UNITY_GK_LISTENER;
extern const char* UNITY_UN_LISTENER;

extern const char* UNITY_APP_DELEGATE;


//--------------------------------------
// Templates
//--------------------------------------

@interface SA_Error : JSONModel
@property (nonatomic) int m_code;
@property (nonatomic, strong) NSString *m_message;

-(id) initWithCode:(int)code message:(NSString* ) message;
-(id) initWithNSError:(NSError *) error;
@end


@interface SA_Result : JSONModel
@property (nonatomic, strong) SA_Error *m_error;
@property (nonatomic, strong) NSString *m_requestId;

-(id) initWithError:(SA_Error*)error;
-(id) initWithNSError:(NSError *) error;

-(void) setRequestId:(NSString *) requestId;
@end

//--------------------------------------
// Extentions
//--------------------------------------


@interface NSData (Base64)
+ (NSData *)InitFromBase64String:(NSString *)aString;
- (NSString *)AsBase64String;
@end

@interface NSDictionary (JSON)
- (NSString *)AsJSONString;
@end


//--------------------------------------
// Mono Callback
//--------------------------------------

typedef const void* UnityAction;
void SendCallbackDataToUnity(UnityAction callback, NSString* data);



//--------------------------------------
// extern "C"
//--------------------------------------


#ifdef __cplusplus
extern "C" {
#endif
    void  ISN_SendMessage(const char* obj, const char* method, NSString* msg);
    char* ISN_ConvertToChar(NSString* nsString);
    NSString* ISN_ConvertToString(char* data);
    NSString* ISN_ConvertBoolToString(BOOL value);
   
    
    void ISN_SendCallbackToUnity(UnityAction callback, NSString* data);
    NSString* ISN_ConvertToBase64(NSData* data);
    NSString* ISN_ConvertImageToBase64(UIImage* image);
    
#if __cplusplus
}   // Extern C
#endif

