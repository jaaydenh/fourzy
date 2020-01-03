#include "KochavaTracker.h"

char* AutonomousStringCopy (const char* string)
{
	if (string == NULL)
	return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

@interface NativeWrapper: NSObject <KochavaTrackerDelegate>

@end

@implementation NativeWrapper

// Decodes the attribution dictionary from the callback delegate or getter and returns a string.
+ (NSString *)decodeAttributionDictionary:(NSDictionary *)attributionDictionary {
    // Ensure it is json.
    if (attributionDictionary == nil || ![NSJSONSerialization isValidJSONObject:attributionDictionary])
    {
        return @"";
    }

    // Attempt to decode it into NSData.
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:attributionDictionary options:0 error:&error];
    if(!jsonData) {
        return @"";
    }

    // Convert the NSData into an NSString.
    NSString *attributionString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    if(attributionString == nil) {
        return @"";
    }

    // If it was valid return the string.
    return attributionString;
}

- (void)tracker:(nonnull KochavaTracker *)tracker didRetrieveAttributionDictionary:(nonnull NSDictionary *)attributionDictionary
{
    NSString * attributionString = [NativeWrapper decodeAttributionDictionary:attributionDictionary];
    // send this message back to the host app, which must always have a game object and listener method with these names
    const char* a = "KochavaTracker";
    const char* b = "KochavaAttributionListener";
    UnitySendMessage(a, b, AutonomousStringCopy([attributionString UTF8String]));
}

// Clears the running instance of the KochavaTracker so it is no longer rnning.
+ (void)invalidateKochava
{
    [KochavaTracker.shared performSelector:@selector(invalidate)];
}

// Removes all saved state for the KochavaTracer. Should call invalidate first.
+ (void)removeKochavaUserDefaults
{
    // Remove the NSUserDefaults keys.
    NSArray *keyArray = NSUserDefaults.standardUserDefaults.dictionaryRepresentation.allKeys;
    for (id key in keyArray)
    {
        NSRange kochavaPrefixRange = [key rangeOfString:@"com.kochava"];
        if ( kochavaPrefixRange.location != NSNotFound )
        {
            [NSUserDefaults.standardUserDefaults removeObjectForKey:key];
        }
    }

    // Remove the deviceId backup file.
    NSURL *documentDirectoryURL = [NSFileManager.defaultManager URLsForDirectory:NSDocumentDirectory inDomains:NSUserDomainMask][0];
    NSURL *kochavaDirectoryURL = [documentDirectoryURL URLByAppendingPathComponent:@"com.kochava.KochavaTracker" isDirectory:true];
    NSURL *backupURL = [kochavaDirectoryURL URLByAppendingPathComponent:@"deviceIdString"];
    
    NSError *error = nil;
    [NSFileManager.defaultManager removeItemAtURL:backupURL error:&error];
}

@end

// create the object for the class
NativeWrapper *nativeWrapper;

// convert a c# stringified dictionary to an NSDictionary
NSMutableDictionary * ConvertToNSDictionary(const char *stringifiedDictionary) {
    if(stringifiedDictionary == NULL)
    {
        return nil;
    }
	
	NSString *str = [NSString stringWithUTF8String:stringifiedDictionary];
    NSData* data = [str dataUsingEncoding:NSUTF8StringEncoding];
    
    NSError *jsonSerializationError;
    id responseObject = [NSJSONSerialization JSONObjectWithData: data options: NSJSONReadingAllowFragments|NSJSONReadingMutableContainers error: &jsonSerializationError];
    
    return responseObject;
}

extern "C" {

    static dispatch_once_t wrapper_product_dispatch_once_t = 0;
	
    // migrate the previously persisted data from unity v1
    void NativeMigrate()
    {
        // MIGRATE LEGACY UNITY SDK'S NSUSERDEFAULTS
        // oldDeviceIdStringKey, oldWatchlistPropertiesKey, oldKochavaQueueStorageKey, and oldAttributionDictionaryStringKey
        NSString * const oldDeviceIdStringKey = @"kochava_device_id";
        
        NSString * const oldWatchlistPropertiesKey = @"watchlistProperties";
        
        NSString * const oldKochavaQueueStorageKey = @"kochava_queue_storage";
        
        NSString * const oldAttributionDictionaryStringKey = @"attribution";
        
        // oldDeviceIdString
        NSString *oldDeviceIdString = [NSUserDefaults.standardUserDefaults objectForKey:oldDeviceIdStringKey];
        
        // Discussion:  We only proceed if we find an oldDeviceIdString.  If we don't, we assume that this is either a new install or else already migrated.
        if (oldDeviceIdString != nil)
        {
            // watchlistPropertiesObject
            NSObject *watchlistPropertiesObject = [NSUserDefaults.standardUserDefaults objectForKey:oldWatchlistPropertiesKey];
            
            // oldKochavaQueueStorageObject
            NSObject *oldKochavaQueueStorageObject = [NSUserDefaults.standardUserDefaults objectForKey:oldKochavaQueueStorageKey];
            
            // oldKochavaQueueStorageString
            NSString *oldKochavaQueueStorageString = nil;
            
            if ([oldKochavaQueueStorageObject isKindOfClass:NSString.class])
            {
                oldKochavaQueueStorageString = (NSString *)oldKochavaQueueStorageObject;
            }
            
            // watchlistPropertiesExistsBool
            BOOL watchlistPropertiesExistsBool = (watchlistPropertiesObject != nil);
            
            // oldKochavaQueueStorageContainsInitialBool
            BOOL oldKochavaQueueStorageContainsInitialBool = NO;
            
            if ((oldKochavaQueueStorageString != nil) && (oldKochavaQueueStorageString.length > 0))
            {
                NSRange range = [oldKochavaQueueStorageString rangeOfString:@"initial" options:NSCaseInsensitiveSearch];
                
                if (range.location != NSNotFound)
                {
                    oldKochavaQueueStorageContainsInitialBool = YES;
                }
            }
            
            // oldAttributionObject
            NSObject *oldAttributionObject = [NSUserDefaults.standardUserDefaults objectForKey:oldAttributionDictionaryStringKey];
            
            // oldAttributionString
            NSString *oldAttributionString = nil;
            
            if ([oldAttributionObject isKindOfClass:NSString.class])
            {
                oldAttributionString = (NSString *)oldAttributionObject;
            }
            
            // oldAttributionDictionary
            NSDictionary *oldAttributionDictionary = nil;
            
            if ([oldAttributionObject isKindOfClass:NSDictionary.class])
            {
                oldAttributionDictionary = (NSDictionary *)oldAttributionObject;
            }
            
            // oldAttributionStringData
            NSData *oldAttributionStringData = nil;
            
            if ((oldAttributionDictionary == nil) && (oldAttributionString != nil))
            {
                oldAttributionStringData = [oldAttributionString dataUsingEncoding:NSUTF8StringEncoding];
            }
            
            // attributionJSONObject and oldAttributionStringDataError
            id oldAttributionJSONObject = nil;
            
            NSError *oldAttributionStringDataError = nil;
            
            if (oldAttributionStringData != nil)
            {
                oldAttributionJSONObject = [NSJSONSerialization JSONObjectWithData:oldAttributionStringData options:NSJSONReadingMutableContainers error:&oldAttributionStringDataError];
            }
            
            // oldAttributionJSONDictionary
            NSDictionary *oldAttributionJSONDictionary = nil;
            
            if ([oldAttributionJSONObject isKindOfClass:NSDictionary.class])
            {
                oldAttributionJSONDictionary = (NSDictionary *)oldAttributionJSONObject;
            }
            
            // newAttributionDictionary
            NSDictionary *newAttributionDictionary = nil;
            
            if (oldAttributionDictionary != nil)
            {
                newAttributionDictionary = oldAttributionDictionary;
            }
            else if (oldAttributionJSONDictionary != nil)
            {
                newAttributionDictionary = oldAttributionJSONDictionary;
            }
            else if (oldAttributionString != nil)
            {
                newAttributionDictionary = @{ @"attribution": oldAttributionString };
            }
            
            // installNetTransactionFirstCompletedBool
            BOOL installNetTransactionFirstCompletedBool = (watchlistPropertiesExistsBool && !oldKochavaQueueStorageContainsInitialBool);
            
            // deviceIdStringAdapterDictionary
            NSDictionary *deviceIdStringAdapterDictionary = nil;
            
            if (oldDeviceIdString != nil)
            {
                NSMutableDictionary *deviceIdStringAdapterValueDictionary = NSMutableDictionary.dictionary;
                deviceIdStringAdapterValueDictionary[@"rawObject"] = oldDeviceIdString;
                deviceIdStringAdapterValueDictionary[@"valueSourceNameString"] = @"Tracker.deviceIdStringAdapter";
                deviceIdStringAdapterValueDictionary[@"serverObject"] = oldDeviceIdString;
                deviceIdStringAdapterValueDictionary[@"startDate"] = NSDate.date; // Normally a iso8601DateString now, but NSDate is also supported.
                
                deviceIdStringAdapterDictionary = @{ @"value" :  deviceIdStringAdapterValueDictionary };
            }
            
            // installSentBoolAdapterValueDictionary
            NSMutableDictionary *installSentBoolAdapterValueDictionary = NSMutableDictionary.dictionary;
            installSentBoolAdapterValueDictionary[@"rawObject"] = @(installNetTransactionFirstCompletedBool);
            installSentBoolAdapterValueDictionary[@"valueSourceNameString"] = @"Tracker.installSentBoolAdapter";
            installSentBoolAdapterValueDictionary[@"serverObject"] = @(installNetTransactionFirstCompletedBool);
            installSentBoolAdapterValueDictionary[@"startDate"] = NSDate.date; // Normally a iso860DateString now, but NSDate is also supported.
            
            // installSentBoolAdapterDictionary
            NSDictionary *installSentBoolAdapterDictionary = @{ @"value" : installSentBoolAdapterValueDictionary  };
            
            // attributionDictionaryAdapterDictionary
            NSDictionary *attributionDictionaryAdapterDictionary = nil;
            
            if (newAttributionDictionary != nil)
            {
                NSMutableDictionary *attributionDictionaryAdapterValueDictionary = NSMutableDictionary.dictionary;
                attributionDictionaryAdapterValueDictionary[@"rawObject"] = newAttributionDictionary;
                attributionDictionaryAdapterValueDictionary[@"valueSourceNameString"] = @"Tracker.attributionDictionaryAdapter";
                attributionDictionaryAdapterValueDictionary[@"serverObject"] = newAttributionDictionary;
                attributionDictionaryAdapterValueDictionary[@"startDate"] = NSDate.date; // Normally a iso8601DateString now, but NSDate is also supported.
                
                attributionDictionaryAdapterDictionary = @{ @"value" : attributionDictionaryAdapterValueDictionary  };
            }
            
            // NSUserDefaults.standardUserDefaults
            // ... set the new keys
            if (attributionDictionaryAdapterDictionary != nil)
            {
                [NSUserDefaults.standardUserDefaults setObject:attributionDictionaryAdapterDictionary forKey:@"com.kochava.KochavaTracker.Tracker.attributionDictionaryAdapter"];
            }
            
            [NSUserDefaults.standardUserDefaults setObject:installSentBoolAdapterDictionary forKey:@"com.kochava.KochavaTracker.Tracker.installSentBoolAdapter"];
            
            if (deviceIdStringAdapterDictionary != nil)
            {
                [NSUserDefaults.standardUserDefaults setObject:deviceIdStringAdapterDictionary forKey:@"com.kochava.KochavaTracker.Tracker.deviceIdStringAdapter"];
            }
            
            // ... remove the old keys
            [NSUserDefaults.standardUserDefaults removeObjectForKey:oldAttributionDictionaryStringKey];
            
            [NSUserDefaults.standardUserDefaults removeObjectForKey:oldKochavaQueueStorageKey];
            
            [NSUserDefaults.standardUserDefaults removeObjectForKey:oldWatchlistPropertiesKey];
            
            [NSUserDefaults.standardUserDefaults removeObjectForKey:oldDeviceIdStringKey];
        }
    }
	
	// initializer
	void iOSNativeStart(const char *inputParameters)
	{
		// migrate settings from the previous v1 unity sdk if applicable
		NativeMigrate();
		
		nativeWrapper = [[NativeWrapper alloc] init];

		NSMutableDictionary *dictionary = ConvertToNSDictionary(inputParameters);
        if(dictionary == nil)
        {
            NSLog(@"KochavaWrapper.configure Invalid Input");
            return;
        }

        // Check for the existence of the hidden unconfigure key.
        if ([dictionary objectForKey:@"INTERNAL_UNCONFIGURE"]) {
            NSLog(@"KochavaWrapper.configure UnConfigure.");
            [NativeWrapper invalidateKochava];
            return;
        }
        
        // Check for the existence of the hidden reset key.
        if ([dictionary objectForKey:@"INTERNAL_RESET"]) {
            NSLog(@"KochavaWrapper.configure Reset.");
            [NativeWrapper removeKochavaUserDefaults];
            return;
        }

        // Wrapper Extension info
        NSString *wrapperNameString = dictionary[@"wrapperNameString"];
        NSString *wrapperVersionString = dictionary[@"wrapperVersionString"];
        NSString *wrapperBuildDateString = dictionary[@"wrapperBuildDateString"];

        dictionary[@"wrapperNameString"] = nil;
        dictionary[@"wrapperVersionString"] = nil;
        dictionary[@"wrapperBuildDateString"] = nil;

        dispatch_once(&wrapper_product_dispatch_once_t, ^{
            
            // wrapperProduct
            // ⓘ Configure and register.
            // a. The apiVerionString should always match the current KVACoreProduct.shared.apiVersionString
            //    for all products with an organizationNameString of "Kochava".
            //    Expect that this value may (or may not) be updated when the native SDK's major version changes.
            // b. The bundleIdentifierString should be set if the wrapper is a framework.
            //    For static libraries it should be nil.
            // c. The nameString should be prefixed with "lib" if the wrapper is a static library.
            //    It should not be if it is not.
            // d. The versionString should match the current version of the wrapper product.
            KVAProduct *wrapperProduct = [KVAProduct productWithAPIVersionString:@"3"/*[a]*/ buildDateString:wrapperBuildDateString bundleIdentifierString:nil/*[b]*/ compilerFlagNameStringArray:nil compilerFlagPredicateSubstitutionVariablesDictionary:nil nameString:wrapperNameString/*[c]*/ organizationNameString:@"Kochava" reverseDomainNameString:@"com.kochava.KochavaUnity" versionString:wrapperVersionString/*[d]*/];
        
            [wrapperProduct register];
        
            // KVAProduct(s)
            // ⓘ Configure.  Also register any which are optional.
            KVACoreProduct.shared.wrapperProduct = wrapperProduct;
            KVATrackerProduct.shared.wrapperProduct = wrapperProduct;
        });

        // KoochavaTracker 
		[KochavaTracker.shared configureWithParametersDictionary:dictionary delegate:nativeWrapper];

        //Check if intelligent consent management is on and apply as necessary.
        BOOL intelligentManagementBool = [[dictionary objectForKey:@"consentIntelligentManagement"] boolValue];
        if(intelligentManagementBool) {
            KochavaTracker.shared.consent.didUpdateBlock = ^(KVAConsent * _Nonnull consent)
            {
                const char* a = "KochavaTracker";
                const char* b = "KochavaConsentStatusChangeListener";
                UnitySendMessage(a, b, AutonomousStringCopy([@"{}" UTF8String]));
            };
        }
	}

}

extern "C" {

	void iOSNativeSendEvent(const char *eventName, const char *eventInfo)
	{
		NSString *evName = nil;
        if(eventName != NULL)
        {
            evName = [NSString stringWithUTF8String:eventName];
        }
		NSString *evInfo = nil;
        if(eventInfo != NULL) 
        {
            evInfo = [NSString stringWithUTF8String:eventInfo];
        }

		[KochavaTracker.shared sendEventWithNameString:evName infoString:evInfo];
	}

	void iOSNativeSendKochavaEvent(const char *eventName, const char *kochavaEventStringifiedDictionary, const char *appStoreReceiptBase64EncodedString)
	{
		// create a native KochavaEvent.
		KochavaEvent* event = [KochavaEvent eventWithEventTypeEnum:KochavaEventTypeEnumCustom];
		if(event == nil)
        {
            return;
        }

        // Set the event name.
        if(eventName != NULL)
        {
            event.customEventNameString = [NSString stringWithUTF8String:eventName];
        }

        // Set the parameters dictionary.
        NSMutableDictionary *stdParamsDictionary = NSMutableDictionary.dictionary;
		stdParamsDictionary = ConvertToNSDictionary(kochavaEventStringifiedDictionary);
        if (stdParamsDictionary != nil) 
        {
            event.infoDictionary = stdParamsDictionary;
        }

        // If a receipt exists then set it.
        if(appStoreReceiptBase64EncodedString != NULL)
        {
            event.appStoreReceiptBase64EncodedString = [NSString stringWithUTF8String:appStoreReceiptBase64EncodedString];
        }

        // now send it
        [KochavaTracker.shared sendEvent: event];
	}

	void iOSNativeSendDeepLink(const char *openURL, const char *sourceApplicationString)
	{
        // Decode the url (can be nil)
        NSString *strOpenUrl = nil;
        if(openURL != NULL)
        {
            strOpenUrl = [NSString stringWithUTF8String:openURL];
        }

        // Decode the source application (can be nil)
		NSString *strSourceApplicationString = nil;
        if(sourceApplicationString != NULL)
        {
            strSourceApplicationString = [NSString stringWithUTF8String:sourceApplicationString];
        }

        // Create and send the deeplink event.
        KochavaEvent *event = [KochavaEvent eventWithEventTypeEnum:KochavaEventTypeEnumDeepLink];
        event.uriString = strOpenUrl;
        event.sourceString = strSourceApplicationString;
        [KochavaTracker.shared sendEvent:event];
	}

	void iOSNativeSendIdentityLink(const char *identityLinkDictionary)
	{    
		NSMutableDictionary *sendIdLinkDictionary = ConvertToNSDictionary(identityLinkDictionary);
		[KochavaTracker.shared sendIdentityLinkWithDictionary:sendIdLinkDictionary];
	}

	char* iOSNativeGetDeviceId()
	{
		NSString *kochavaTrackerDeviceIdString = KochavaTracker.shared.deviceIdString;
        if(kochavaTrackerDeviceIdString == nil)
        {
            kochavaTrackerDeviceIdString = @"";
        }
		return AutonomousStringCopy([kochavaTrackerDeviceIdString UTF8String]);
	}

	char* iOSNativeGetAttributionString()
	{
		NSDictionary *attributionDictionary = KochavaTracker.shared.attributionDictionary;
        NSString * attributionString = [NativeWrapper decodeAttributionDictionary:attributionDictionary];
        return AutonomousStringCopy([attributionString UTF8String]);
	}

	void iOSNativeSetAppLimitAdTrackingBool(bool value) {		
		[KochavaTracker.shared setAppLimitAdTrackingBool:value];		
	}

    void iOSNativeSetSleepBool(bool value) {		
		[KochavaTracker.shared setSleepBool:value];		
	}

    bool iOSNativeGetSleepBool() {		
		return [KochavaTracker.shared sleepBool];		
	}

	char* iOSNativeGetVersion()
	{
		NSString *kochavaTrackerVersionString = KochavaTracker.shared.sdkVersionString;
        if(kochavaTrackerVersionString == nil)
        {
            kochavaTrackerVersionString = @"";
        }
		return AutonomousStringCopy([kochavaTrackerVersionString UTF8String]);		
	}

    void iOSNativeAddPushToken(char* bytes, unsigned long length)
	{
        NSData *deviceToken = nil;
        if(bytes != NULL)
        {
            deviceToken = [NSData dataWithBytesNoCopy:bytes length:length freeWhenDone:NO];
        }
		[KochavaTracker.shared addRemoteNotificationsDeviceToken:deviceToken];
	}

    void iOSNativeRemovePushToken(char* bytes, unsigned long length)
	{
		NSData *deviceToken = nil;
        if(bytes != NULL)
        {
            deviceToken = [NSData dataWithBytesNoCopy:bytes length:length freeWhenDone:NO];
        }
		[KochavaTracker.shared removeRemoteNotificationsDeviceToken:deviceToken];
	}

    char* iOSNativeGetConsentDescription()
    { 
        NSString *descriptionString = KochavaTracker.shared.consent.descriptionString;
        if(descriptionString == nil)
        {
            descriptionString = @"";
        }
        return AutonomousStringCopy([descriptionString UTF8String]);
    }

    long iOSNativeGetConsentResponseTime()
    {
        NSDate *didRespondDate = KochavaTracker.shared.consent.responseDate;
        if(didRespondDate == nil)
        {
            return 0;
        }
        return [[NSNumber numberWithDouble:[didRespondDate timeIntervalSince1970]] longValue];
    }

    bool iOSNativeGetConsentRequired()
    {
        return KochavaTracker.shared.consent.requiredBool;
    }

    void iOSNativeSetConsentRequired(bool value) {		
		KochavaTracker.shared.consent.requiredBool = value;
	}

    bool iOSNativeGetConsentGranted()
    { 
        return KochavaTracker.shared.consent.isGrantedBool;
    }

    char* iOSNativeGetConsentPartnersJson()
    {
        NSString *consentPartnersString = nil; 
        NSObject *consentPartnersAsForContextObject = [(NSObject<KVAAsForContextObjectProtocol> *)KochavaTracker.shared.consent.partnerArray kva_asForContextObjectWithContext:KVAContext.sdkWrapper];
        if (consentPartnersAsForContextObject != nil)
        {
            NSError *error = nil;
            NSData *consentPartnersJSONData = [NSJSONSerialization dataWithJSONObject:consentPartnersAsForContextObject options:0 error:&error];
            
            if (consentPartnersJSONData != nil)
            {
                consentPartnersString = [[NSString alloc] initWithData:consentPartnersJSONData encoding:NSUTF8StringEncoding];
            }
        }

        if(consentPartnersString == nil)
        {
            consentPartnersString = @"[]";
        }
        return AutonomousStringCopy([consentPartnersString UTF8String]);
    }

    void iOSNativeSetConsentGranted(bool isGranted)
    {
        NSNumber *consentGranted = [NSNumber numberWithBool:isGranted];
        if(consentGranted != nil)
        {
            [KochavaTracker.shared.consent didPromptWithDidGrantBoolNumber:consentGranted];
        }
    }

    bool iOSNativeGetConsentShouldPrompt()
    {
        return KochavaTracker.shared.consent.shouldPromptBool;
    }

    void iOSNativeSetConsentPrompted()
    {
        [KochavaTracker.shared.consent willPrompt];
    }

    bool iOSNativeGetConsentRequirementsKnown()
    {
        return KochavaTracker.shared.consent.requiredBoolNumber != nil;
    }

}
