#import "ISN_Foundation.h"
#import <AVFoundation/AVFoundation.h>


extern "C" {
    
    char* _ISN_CopyCGImageAtTime(char* url, double seconds) {
        
        NSString* assetUrl = [NSString stringWithUTF8String: url];
        NSURL *mediaUrl = [NSURL URLWithString:assetUrl];
        
        AVURLAsset *asset = [[AVURLAsset alloc] initWithURL:mediaUrl options:nil];
      
        NSLog(@"asset: %@", asset);
        AVAssetImageGenerator *gen = [[AVAssetImageGenerator alloc] initWithAsset:asset];
        gen.appliesPreferredTrackTransform = YES;
        CMTime time = CMTimeMakeWithSeconds(seconds, 600);
        NSError *error = nil;
        CMTime actualTime;
        
        CGImageRef image = [gen copyCGImageAtTime:time actualTime:&actualTime error:&error];
         NSLog(@"image: %@", image);
        NSLog(@"error: %@", error);
        UIImage *thumb = [[UIImage alloc] initWithCGImage:image];
         NSLog(@"thumb: %@", thumb);
        CGImageRelease(image);
      
        
        if(thumb == NULL) {
            return ISN_ConvertToChar(@"");
        }
        
        
        NSData *imageData  = UIImageJPEGRepresentation(thumb, 0.8);
        NSString* dataimageDataEncoded = [imageData base64EncodedStringWithOptions:NSDataBase64DecodingIgnoreUnknownCharacters];
        
        return ISN_ConvertToChar(dataimageDataEncoded);

    }
    
   
}
