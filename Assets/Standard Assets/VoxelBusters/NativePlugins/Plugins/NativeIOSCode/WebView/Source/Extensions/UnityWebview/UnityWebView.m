//
//  UnityWebView.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 21/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "UnityWebView.h"

#define kShowing  						"WebViewDidShow"
#define kDismissed						"WebViewDidHide"
#define kDestroyed						"WebViewDidDestroy"
#define kDidStartLoad 					"WebViewDidStartLoad"
#define kDidFinishLoad 					"WebViewDidFinishLoad"
#define kDidFailLoadWithError			"WebViewDidFailLoadWithError"
#define kFinishedEvaluatingJavaScript	"WebViewDidFinishEvaluatingJS"
#define kReceivedMessage 				"WebViewDidReceiveMessage"

#define kURLKey							@"url"
#define kTagKey							@"tag"
#define kErrorKey						@"error"

@implementation UnityWebView

- (id)initWithFrame:(CGRect)frame tag:(NSString *)tag
{
	self = [super initWithFrame:frame tag:tag];
	
    if (self)
	{
		// Initialisation code
		[[self webview] setOpaque:NO];
    }
	
    return self;
}

- (void)dealloc
{	
	// Notify unity
	NotifyEventListener(kDestroyed, [[self webviewTag] UTF8String]);
	
	[super dealloc];
}

#pragma mark - Override View Methods

- (void)show
{
	BOOL	currentlyShowing	= [self isShowing];
	
	if (!currentlyShowing)
	{
		// Add the view on the top of Unity view
		[super show];
		[UnityGetGLViewController().view addSubview:self];
		
		// Notify Unity
		NotifyEventListener(kShowing, [[self webviewTag] UTF8String]);
	}
}

- (void)dismiss
{
	BOOL	currentlyShowing	= [self isShowing];
	
	if (currentlyShowing)
	{
		// Removes view from super view
		[super dismiss];
		
		// Notify Unity
		NotifyEventListener(kDismissed, [[self webviewTag] UTF8String]);
	}
}

#pragma mark - Override Load Methods

- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)script
{
    NSString *result			= [super stringByEvaluatingJavaScriptFromString:script];
	
	// Notify unity
	NSMutableDictionary *data   = [NSMutableDictionary dictionary];
	data[@"tag"]                = [self webviewTag];
	
	if (result != NULL)
		data[@"result"]			= result;
	
	NotifyEventListener(kFinishedEvaluatingJavaScript, ToJsonCString(data));
	
	return result;
}

#pragma mark - Overide URL Scheme

- (void)foundMatchingURLScheme:(NSURL *)requestURL
{
	NSMutableDictionary *messageData	= [self parseURLScheme:requestURL];
	
	// Notify unity
	NSMutableDictionary *data  		= [NSMutableDictionary dictionary];
	data[@"tag"]              		= [self webviewTag];
	data[@"message-data"]       	= messageData;
	
	// Notify unity
    NotifyEventListener(kReceivedMessage, ToJsonCString(data));
}

#pragma mark - Override Webview Callback

- (void)webViewDidStartLoad:(UIWebView *)webView
{
	[super webViewDidStartLoad:webView];
	
	// Notify unity
	NotifyEventListener(kDidStartLoad, [[self webviewTag] UTF8String]);
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
	[super webViewDidFinishLoad:webView];
	
	// Notify unity
	NSMutableDictionary* data	= [NSMutableDictionary dictionary];
	data[kTagKey]               = [self webviewTag];
	
	if ([webView request])
		data[kURLKey]           = [[[webView request] URL] absoluteString];
	
	NotifyEventListener(kDidFinishLoad, ToJsonCString(data));
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
	[super webView:webView didFailLoadWithError:error];
    
    // Notify unity
    NSMutableDictionary* data	= [NSMutableDictionary dictionary];
	data[kTagKey]               = [self webviewTag];
	
	if ([webView request])
		data[kURLKey]           = [[[webView request] URL] absoluteString];
	
    data[kErrorKey]             = error.description;
    
    NotifyEventListener(kDidFailLoadWithError, ToJsonCString(data));
}

@end