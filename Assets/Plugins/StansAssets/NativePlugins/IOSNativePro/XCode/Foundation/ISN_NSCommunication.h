#import "JSONModel.h"
#import "ISN_Foundation.h"



@protocol ISN_NSArrayModel;
@interface ISN_NSArrayModel : JSONModel
@property (nonatomic) NSArray<NSString*> *m_value;
@end

@protocol ISN_NSKeyValueObject;
@interface ISN_NSKeyValueObject : JSONModel
@property (nonatomic) NSString *m_key;
@property (nonatomic) NSString *m_value;

-(id) initWithData:(NSString *) key value:(NSString *) value;
@end


@interface  ISN_NSKeyValueResult : SA_Result
@property (nonatomic) ISN_NSKeyValueObject *m_keyValueObject;

-(id) initWithNSKeyValueObject:(ISN_NSKeyValueObject *) keyValueObject;
@end

@interface ISN_NSStoreDidChangeExternallyNotification : JSONModel
@property (nonatomic) int m_reason;
@property (nonatomic) NSArray<ISN_NSKeyValueObject> *m_updatedData;
@end


//So far those models are only used for a user notifications API

@interface ISN_NSDateComponents : JSONModel
@property (nonatomic) long Hour;
@property (nonatomic) long Minute;
@property (nonatomic) long Second;
@property (nonatomic) long Nanosecond;

@property (nonatomic) long Year;
@property (nonatomic) long Month;
@property (nonatomic) long Day;

-(id) initWithNSDateComponents:(NSDateComponents *) date;
-(NSDateComponents *) getNSDateComponents;
@end

@protocol ISN_NSRange;
@interface ISN_NSRange : JSONModel

@property(nonatomic) long m_location;
@property(nonatomic) long m_length;

-(id) initWithNSRange:(NSRange ) range;
-(NSRange ) getNSRange;

@end

@protocol ISN_NSURL;
@interface ISN_NSURL : JSONModel
@property(nonatomic) NSString* m_url;
@property(nonatomic) int m_type;

-(NSURL* ) toNSURL;
@end

@protocol ISN_NSLocale;
@interface ISN_NSLocale : JSONModel
@property (nonatomic) NSString* m_identifier;
@property (nonatomic) NSString* m_countryCode;
@property (nonatomic) NSString* m_languageCode;
@property (nonatomic) NSString* m_currencySymbol;
@property (nonatomic) NSString* m_currencyCode;


-(id) initWithNSLocale:(NSLocale *) locale;
@end
