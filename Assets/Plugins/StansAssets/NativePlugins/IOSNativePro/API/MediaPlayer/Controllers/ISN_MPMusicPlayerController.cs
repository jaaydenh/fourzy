using UnityEngine;
using SA.iOS.Foundation;

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.MediaPlayer
{

    /// <summary>
    /// An object used to play audio media items from the device's Music app library.
    /// https://developer.apple.com/documentation/mediaplayer/mpmusicplayercontroller
    /// </summary>
    public class ISN_MPMusicPlayerController
    {
        private string m_nativePlayerHashId = string.Empty;



#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
        [DllImport("__Internal")] static extern string _ISN_Get_SystemMusicPlayer();
        [DllImport("__Internal")] static extern string _ISN_Get_ApplicationMusicPlayer();
        [DllImport("__Internal")] static extern string _ISN_Get_ApplicationQueuePlayer();

        [DllImport("__Internal")] static extern int _ISN_MPMusicPlaye_PlaybackState(string playerId);
        [DllImport("__Internal")] static extern string _ISN_MPMusicPlaye_NowPlayingItem(string playerId);
       

        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_Play(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_Stop(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_Pause(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_SkipToNextItem(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_SkipToPreviousItem(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_SetQueueWithStoreIDs(string playerId, string data);

        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_BeginGeneratingPlaybackNotifications(string playerId);
        [DllImport("__Internal")] static extern void _ISN_MPMusicPlayer_EndGeneratingPlaybackNotifications(string playerId);
#endif


        //--------------------------------------
        // Const
        //--------------------------------------


        /// <summary>
        /// Posted when the playback state has been changed programmatically or by user action.
        /// </summary>
        public const string PlaybackStateDidChange = "MPMusicPlayerControllerPlaybackStateDidChangeNotification";


        /// <summary>
        /// Posted when the currently playing media item has changed.
        /// </summary>
        public const string NowPlayingItemDidChange = "MPMusicPlayerControllerNowPlayingItemDidChangeNotification";


        /// <summary>
        ///Posted when the audio playback volume for the music player has changed.
        /// </summary>
        public const string VolumeDidChange = "MPMusicPlayerControllerVolumeDidChangeNotification";


        //--------------------------------------
        // Initialization
        //--------------------------------------


        private ISN_MPMusicPlayerController(string nativePlayerHashId) {
            m_nativePlayerHashId = nativePlayerHashId;
        }


        //--------------------------------------
        // Public Methods
        //--------------------------------------


        /// <summary>
        /// Play current player track
        /// </summary>
        public void Play() {
            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            _ISN_MPMusicPlayer_Play(m_nativePlayerHashId);
#endif
        }


        /// <summary>
        /// Stop current player track
        /// </summary>
        public void Stop() {
            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            _ISN_MPMusicPlayer_Stop(m_nativePlayerHashId);
#endif
        }


        /// <summary>
        /// Pause current player track
        /// </summary>
        public void Pause() {
            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

            //Only works with native part.
#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            _ISN_MPMusicPlayer_Pause(m_nativePlayerHashId);
#endif
        }

        /// <summary>
        /// Starts playback of the next media item in the playback queue; 
        /// or, the music player is not playing, designates the next media item as the next to be played.
        /// </summary>
        public void SkipToNextItem() {
            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            _ISN_MPMusicPlayer_SkipToNextItem(m_nativePlayerHashId);
#endif
        }


        /// <summary>
        /// Starts playback of the previous media item in the playback queue; 
        /// or, the music player is not playing, designates the previous media item as the next to be played.
        /// </summary>
        public void SkipToPreviousItem() {

            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            _ISN_MPMusicPlayer_SkipToPreviousItem(m_nativePlayerHashId);
#endif
        }

        /// <summary>
        /// Sets a music player's playback queue using with media items identified by the store identifiers.
        /// </summary>
        /// <param name="storeIDs">An array of store identifiers associated with the media items to be added to the queue.</param>
        public void SetQueueWithStoreIDs(params string[] storeIDs) {

            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)
            ISN_NSArrayModel model = new ISN_NSArrayModel();
            foreach(var id in storeIDs) {
                model.Add(id);
            }
            string data = JsonUtility.ToJson(model);
            _ISN_MPMusicPlayer_SetQueueWithStoreIDs(m_nativePlayerHashId, data);
#endif
        }



        /// <summary>
        /// Starts the generation of playback notifications.
        /// </summary>
        public void BeginGeneratingPlaybackNotifications() {

            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
            _ISN_MPMusicPlayer_BeginGeneratingPlaybackNotifications(m_nativePlayerHashId);
#endif
        }

        /// <summary>
        /// Ends the generation of playback notifications.
        /// </summary>
        public void EndGeneratingPlaybackNotifications() {

            if (Application.isEditor) {
                //Just Do nothing 
                return;
            }

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
            _ISN_MPMusicPlayer_EndGeneratingPlaybackNotifications(m_nativePlayerHashId);
#endif
        }



        //--------------------------------------
        // Get / Set
        //--------------------------------------

        /// <summary>
        /// You determine a music player’s state by checking the playbackState property. 
        /// Depending on the property’s value, 
        /// you can update your application’s user interface or take other appropriate action.
        /// </summary>
        public ISN_MPMusicPlaybackState PlaybackState {
            get {
#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
                return (ISN_MPMusicPlaybackState) _ISN_MPMusicPlaye_PlaybackState(m_nativePlayerHashId);
#else
                return ISN_MPMusicPlaybackState.Stopped;
#endif
            }
        }


        /// <summary>
        /// The currently-playing media item, or the media item, 
        /// within a queue, that you have designated to begin playback with.
        /// </summary>
        public ISN_MPMediaItem NowPlayingItem {
            get {
#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED)  && !UNITY_EDITOR
                string itemJSON = _ISN_MPMusicPlaye_NowPlayingItem(m_nativePlayerHashId);
                return JsonUtility.FromJson<ISN_MPMediaItem>(itemJSON);
#else
                return new ISN_MPMediaItem();
#endif
            }
        }

        /// <summary>
        /// Objective-C linked object hash Id.
        /// </summary>
        public string PlayerHashId {
            get {
                return m_nativePlayerHashId;
            }
        }


        //--------------------------------------
        // Static
        //--------------------------------------

        /// <summary>
        /// Returns the system music player, which controls the Music app’s state.
        /// 
        /// The system music player employs the built-in Music app on your behalf. 
        /// On instantiation, it takes on the current Music app state, such as the identification of the now-playing item. 
        /// If a user switches away from your app while music is playing, that music continues to play. 
        /// The Music app then has your music player’s most recently-set repeat mode, shuffle mode, playback state, and now-playing item.
        /// </summary>
        public static ISN_MPMusicPlayerController SystemMusicPlayer {
            get {
                string nativePlayerHashId = string.Empty;

#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
                nativePlayerHashId = _ISN_Get_SystemMusicPlayer();
#endif
                return new ISN_MPMusicPlayerController(nativePlayerHashId);

            }
        }

        /// <summary>
        /// The application music player.
        /// 
        /// The application music player plays music locally within your app. 
        /// The music player does not affect the Music app’s state. 
        /// When your app moves to the background, the music player stops playing the current media.
        /// </summary>
        public static ISN_MPMusicPlayerController ApplicationMusicPlayer {
            get {
                string nativePlayerHashId = string.Empty;
#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
                nativePlayerHashId = _ISN_Get_ApplicationMusicPlayer();
#endif
                return new ISN_MPMusicPlayerController(nativePlayerHashId);

            }
        }



        /// <summary>
        /// The application queue music player.
        /// 
        /// The application queue music player plays music locally within your app. 
        /// The application queue music player provides more functionality and greater control over the music played 
        /// than the application music player.
        /// 
        /// The music player does not affect the Music app’s state.
        /// When your app moves to the background, the music player stops playing the current media.
        /// </summary>
        public static ISN_MPMusicPlayerController ApplicationQueuePlayer {
            get {
                string nativePlayerHashId = string.Empty;
#if (UNITY_IPHONE && MEDIA_PLAYER_API_ENABLED) && !UNITY_EDITOR
                nativePlayerHashId = _ISN_Get_ApplicationQueuePlayer();
#endif
                return new ISN_MPMusicPlayerController(nativePlayerHashId);

            }
        }


    }
}