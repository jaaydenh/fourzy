using UnityEngine;
using UnityEngine.UI;


using SA.iOS.MediaPlayer;
using SA.iOS.Foundation;

public class ISN_MediaPlayerExample : MonoBehaviour
{
    
#pragma warning disable 649

    [Header("Info Panel")]
    [SerializeField] Text m_title;
    [SerializeField] Text m_artist;
    [SerializeField] Text m_playbackState;


    [Header("Buttons")]
    [SerializeField] Button m_play;
    [SerializeField] Button m_stop;
    [SerializeField] Button m_pause;
    [SerializeField] Button m_next;
    [SerializeField] Button m_previos;
    
#pragma warning restore 649

    // Start is called before the first frame update


    private ISN_MPMusicPlayerController m_player;

    void Start()
    {

        m_player = ISN_MPMusicPlayerController.SystemMusicPlayer;
        UpdatePlayerStateUI();

        m_play.onClick.AddListener(() => {
            m_player.Play();
        });

        m_stop.onClick.AddListener(() => {
            m_player.Stop();
        });


        m_pause.onClick.AddListener(() => {
            m_player.Pause();
        });

        m_next.onClick.AddListener(() => {
            m_player.SkipToNextItem();
        });

        m_previos.onClick.AddListener(() => {
            m_player.SkipToPreviousItem();
        });


        //Subscribing ot the events
        m_player.BeginGeneratingPlaybackNotifications();

        var center = ISN_NSNotificationCenter.DefaultCenter;
        center.AddObserverForName(ISN_MPMusicPlayerController.NowPlayingItemDidChange, 
            (ISN_NSNotification notification) => {
                UpdatePlayerStateUI();
                Debug.Log("MusicPlayer Now Playing Item Did Change");
            });


        center.AddObserverForName(ISN_MPMusicPlayerController.PlaybackStateDidChange,
           (ISN_NSNotification notification) => {
               UpdatePlayerStateUI();
               Debug.Log("MusicPlayer Playback State Did Change");
           });
    }


    private void UpdatePlayerStateUI() {
        ISN_MPMediaItem item = m_player.NowPlayingItem;

        m_title.text = "Title: " + item.Title;
        m_artist.text = "Artist: " + item.Artist;

        m_playbackState.text = "PlaybackState: " + m_player.PlaybackState.ToString();
    }

}
