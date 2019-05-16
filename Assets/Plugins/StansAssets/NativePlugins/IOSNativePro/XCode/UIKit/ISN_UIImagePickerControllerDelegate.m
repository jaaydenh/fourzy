#if !TARGET_OS_TV
#import "ISN_UIImagePickerControllerDelegate.h"
@implementation ISN_UIImagePickerControllerDelegate



-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    
    NSLog(@"imagePickerControllerDidCancel");
    
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    SA_Error * error = [[SA_Error alloc] initWithCode:1 message:@"PickerControllerDidCancel"];
    ISN_UIPickerControllerResult *result = [[ISN_UIPickerControllerResult alloc] initWithError:error];
    
    ISN_SendMessage(UNITY_UI_LISTENER, "didFinishPickingMedia", [result toJSONString]);
}



-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    ISN_UIPickerControllerResult *result = [[ISN_UIPickerControllerResult alloc] init];
    
    UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
    if(photo != NULL) {
        NSString *encodedImage = [self EncodeImage:photo];
        [result setM_encodedImage:encodedImage];
    }
    
    NSURL *mediaUrl = (NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
    if(mediaUrl != NULL) {
        NSString *path = [mediaUrl absoluteString];
        [result setM_mediaURL:path];
    }
    
    if (@available(iOS 11.0, *)) {
        NSURL *imageUrl = (NSURL*)[info objectForKey:UIImagePickerControllerImageURL];
        if(imageUrl != NULL) {
            NSString *path = [imageUrl absoluteString];
            [result setM_imageURL:path];
        }
    } else {
        // Fallback on earlier versions
    }
    
    
    
    NSString* mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    [result setM_mediaType:mediaType];
    
    ISN_SendMessage(UNITY_UI_LISTENER, "didFinishPickingMedia", [result toJSONString]);
}



- (NSString*) EncodeImage:(UIImage *)image {
    image = [image fixOrientation];
    
    //int encodingType = [self.controllerRequest m_encodingType];
    int maxImageSize = [self.controllerRequest m_maxImageSize];
    //  int imageCompressionRate = [self.controllerRequest m_imageCompressionRate];
    
    
    if(image.size.width >  maxImageSize || image.size.height > maxImageSize ) {
        [ISN_Logger Log:@"resizing image"];
        CGSize s = CGSizeMake(maxImageSize, maxImageSize);
        
        if(image.size.width > image.size.height) {
            CGFloat new_height = maxImageSize / (image.size.width / image.size.height);
            s.height = new_height;
            
        } else {
            CGFloat new_width = maxImageSize / (image.size.height / image.size.width);
            s.width = new_width;
            
        }
        image =  [self imageWithImage:image scaledToSize:s];
        
    }
    
    return  ISN_ConvertImageToBase64(image);
}



- (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

@end



#endif
