using System;
using UnityEngine;


namespace SA.iOS.MediaPlayer
{


    /// <summary>
    /// A collection of properties that represents a single item contained in the media library.
    /// https://developer.apple.com/documentation/mediaplayer/mpmediaitem?language=objc
    /// </summary>

    [Serializable]
    public class ISN_MPMediaItem
    {
        [SerializeField] string m_title = string.Empty;
        [SerializeField] string m_artist = string.Empty;
        [SerializeField] string m_albumTitle = string.Empty;
        [SerializeField] string m_composer = string.Empty;
        [SerializeField] string m_genre = string.Empty;
        [SerializeField] string m_lyrics = string.Empty;


        /// <summary>
        /// The title (or name) of the media item.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }

        }

        /// <summary>
        /// The performing artist(s) for a media item—which may vary 
        /// from the primary artist for the album that a media item belongs to.
        /// </summary>
        public string Artist {
            get {
                return m_artist;
            }
        }

        /// <summary>
        /// The title of an album, such as “Live On Mars”, 
        /// as opposed to the title of an individual song on the album, 
        /// such as “Crater Dance (radio edit)”.
        /// </summary>
        public string AlbumTitle {
            get {
                return m_albumTitle;
            }
        }


        /// <summary>
        /// The musical composer for the media item.
        /// </summary>
        public string Composer {
            get {
                return m_composer;
            }
        }


        /// <summary>
        /// The musical or film genre of the media item.
        /// </summary>
        public string Genre {
            get {
                return m_genre;
            }
        }


        /// <summary>
        /// The lyrics for the media item.
        /// </summary>
        public string Lyrics {
            get {
                return m_lyrics;
            }
        }
    }
}