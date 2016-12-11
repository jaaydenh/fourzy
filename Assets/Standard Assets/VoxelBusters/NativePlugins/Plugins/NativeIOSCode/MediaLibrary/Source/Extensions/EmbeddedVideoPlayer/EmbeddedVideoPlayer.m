//
//  EmbeddedVideoPlayer.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 21/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "EmbeddedVideoPlayer.h"

const NSString *kEVPlayerScheme					= @"embeddedplayer";

const NSString *kEVPlayerEventOnReady			= @"OnReady";
const NSString *kEVPlayerEventOnStateChange		= @"OnStateChange";
const NSString *kEVPlayerEventOnError			= @"OnError";

const NSString *kEVPlayerStateUnstartedCode		= @"UNSTARTED";
const NSString *kEVPlayerStateEndedCode			= @"ENDED";
const NSString *kEVPlayerStatePlayingCode		= @"PLAYING";
const NSString *kEVPlayerStatePausedCode		= @"PAUSED";
const NSString *kEVPlayerStateBufferingCode		= @"BUFFERING";
const NSString *kEVPlayerStateCuedCode			= @"CUED";
const NSString *kEVPlayerStateUserExitedCode	= @"USER_EXITED";
const NSString *kEVPlayerStateUnknownCode		= @"UNKNOWN";

const NSString *kEVPlayerCommandStopVideo		= @"stopVideo();";
const NSString *kEVPlayerCommandPlayVideo		= @"playVideo();";
const NSString *kEVPlayerCommandPauseVideo		= @"pauseVideo();";
const NSString *kEVPlayerCommandGetState		= @"getPlayerState();";
const NSString *kEVPlayerCommandResizeView		= @"resizePlayer();";

@interface EmbeddedVideoPlayer ()

// Properties
@property(nonatomic)			BOOL		playbackHasEnded;
@property(nonatomic, retain)	NSString*	playerInternalStateCode;

@end

@implementation EmbeddedVideoPlayer

@synthesize embeddedHTMLString;
@synthesize delegate;
@synthesize playbackState = _playbackState;

@synthesize playbackHasEnded;
@synthesize playerInternalStateCode;

- (id)init
{
	return [self initWithFrame:CGRectZero];
}

- (id)initWithFrame:(CGRect)frame
{
	return [self initWithFrame:frame tag:@"video-player"];
}

- (id)initWithFrame:(CGRect)frame tag:(NSString *)tag
{
	self = [super initWithFrame:CGRectZero tag:tag];
	
    if (self)
	{
		// Change webview properties
		UIWebView *webview	= [self webview];
		[[webview scrollView] setScrollEnabled:NO];
		[webview setAllowsInlineMediaPlayback:YES];
		[webview setMediaPlaybackRequiresUserAction:NO];
		[webview setAutoresizesSubviews:YES];
		[webview setAutoresizingMask:(UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth)];
		
		// Set loading spinner properties
		[[self loadingSpinner] setActivityIndicatorViewStyle:UIActivityIndicatorViewStyleWhite];
		[self setShowSpinnerOnLoad:YES];
		
		// Set other properties
		[self setCanBounce:NO];
		[self setAutoShowOnLoadFinish:NO];
		[self setControlType:WebviewControlTypeCloseButton];
		[self setBackgroundColor:[UIColor blackColor]];
		
		// Add schema
		[self addNewURLScheme:(NSString *)kEVPlayerScheme];
		
		// Reset player to default state
		[self reset];
	}
	
    return self;
}

- (void)dealloc
{
	// Release
	self.embeddedHTMLString			= nil;
	self.playerInternalStateCode	= nil;
	
	[super dealloc];
}

#pragma mark - Player Methods

- (void)reset
{
	// Default state value
	self.playbackHasEnded			= false;
	self.playerInternalStateCode	= (NSString *)kEVPlayerStateUnstartedCode;
}

- (void)play
{
	// Dint set html string
	if (![self embeddedHTMLString])
	{
		[self onFinishedPlaying:MPMovieFinishReasonPlaybackEnded];
		return;
	}
	
	// Reset
	[self reset];
	
	// Start load request
	[self loadHTMLString:[self embeddedHTMLString] baseURL:[[NSBundle mainBundle] resourceURL]];
}

- (void)pause
{
	[self stringByEvaluatingJavaScriptFromString:(NSString *)kEVPlayerCommandPauseVideo];
}

- (void)stop
{
	[self stopLoading];
	[self stringByEvaluatingJavaScriptFromString:(NSString *)kEVPlayerCommandStopVideo];
	
	// Forcibly sending this state code, as youtube can stay on any non playing state when stop is called
	[self setPlayerInternalStateCode:(NSString *)kEVPlayerStateUserExitedCode];
}

- (void)setPlayerInternalStateCode:(NSString *)newStateCode
{
	[playerInternalStateCode release], playerInternalStateCode = NULL;

	// Set new value
	playerInternalStateCode		= [newStateCode retain];
	
	// Update playback state
	if ([newStateCode isEqualToString:(NSString *)kEVPlayerStatePlayingCode])
	{
		_playbackState	= MPMoviePlaybackStatePlaying;
	}
	else if ([newStateCode isEqualToString:(NSString *)kEVPlayerStatePausedCode])
	{
		_playbackState	= MPMoviePlaybackStatePaused;
	}
	else if ([newStateCode isEqualToString:(NSString *)kEVPlayerStateEndedCode])
	{
		_playbackState	= MPMoviePlaybackStateStopped;
		
		[self onFinishedPlaying:MPMovieFinishReasonPlaybackEnded];
	}
	else if ([newStateCode isEqualToString:(NSString *)kEVPlayerStateUserExitedCode])
	{
		_playbackState	= MPMoviePlaybackStateStopped;
		
		[self onFinishedPlaying:MPMovieFinishReasonUserExited];
	}
	else
	{
		_playbackState	= MPMoviePlaybackStateStopped;
	}
}

#pragma mark - Button Action Methods

- (void)onPressingCloseButton:(id)sender
{
	NSLog(@"[VideoEmbeddedView] pressed close");
	
	// Stop player
	[self stop];
}

#pragma mark - WebView Methods

- (void)foundMatchingURLScheme:(NSURL *)requestURL
{
	NSMutableDictionary *parsedDict	= [self parseURLScheme:requestURL];
	NSMutableDictionary *argsDict	= [parsedDict objectForKey:@"arguments"];
	NSString *host					= [parsedDict objectForKey:@"host"];
	NSString *value					= [argsDict objectForKey:@"value"];
	
	if (IsNullOrEmpty(value))
		value		= kNSStringDefault;
	
	if ([host isEqualToString:(NSString *)kEVPlayerEventOnReady])
	{
		// Play video
		[self stringByEvaluatingJavaScriptFromString:(NSString *)kEVPlayerCommandPlayVideo];
	}
	else if ([host isEqualToString:(NSString *)kEVPlayerEventOnStateChange])
	{
		[self setPlayerInternalStateCode:value];
	}
	else if ([host isEqualToString:(NSString *)kEVPlayerEventOnError])
	{
		[self onError];
	}
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
	[super webView:webView didFailLoadWithError:error];
	
	// Invoke handler
	[self onError];
}

- (void)onError
{
	// Send player state to unknown and send callback
	[self setPlayerInternalStateCode:(NSString *)kEVPlayerStateUnknownCode];
	[self onFinishedPlaying:MPMovieFinishReasonPlaybackError];
}

#pragma mark - Send Events Methods

- (void)didRotateToOrientation:(UIDeviceOrientation)toOrientation fromOrientation:(UIDeviceOrientation)fromOrientation
{
	[super didRotateToOrientation:toOrientation fromOrientation:fromOrientation];
	[self stringByEvaluatingJavaScriptFromString:(NSString *)kEVPlayerCommandResizeView];
}

- (void)onFinishedPlaying:(MPMovieFinishReason)reason
{
	// Check if callback finished was already sent
	if ([self playbackHasEnded])
		return;
	
	// Mark as playback ended
	[self setPlaybackHasEnded:YES];
	
	// Send callback to observers
	if ([self delegate] != nil && [[self delegate] conformsToProtocol:@protocol(EmbeddedVideoPlayerDelegate)])
	{
		[[self delegate] embeddedVideoPlayer:self didFinishPlaying:reason];
	}
}

@end