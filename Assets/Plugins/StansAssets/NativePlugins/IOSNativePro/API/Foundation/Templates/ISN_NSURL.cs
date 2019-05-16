using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace SA.iOS.Foundation
{
    /// <summary>
    /// You can use URL objects to construct URLs and access their parts. 
    /// For URLs that represent local files, you can also manipulate properties of those files directly, 
    /// such as changing the file’s last modification date. 
    /// Finally, you can pass URL objects to other APIs to retrieve the contents of those URLs.
    /// </summary>
    [Serializable]
    public class ISN_NSURL
    {

        private enum URLType
        {
            Default = 0,
            File = 1
        }

        [SerializeField] string m_url = string.Empty;
        [SerializeField] URLType m_type;


        /// <summary>
        /// The initial url string it was created with
        /// All initial url string adjustmens will take place inside the native plugin part
        /// </summary>
        /// <value>The URL.</value>
        public string Url {
            get {
                return m_url;
            }
        }


        /// <summary>
        /// The url type.
        /// Url will be transaformed on native plugin side depending of url type
        /// </summary>
        /// <value>The type.</value>
        private URLType Type {
            get {
                return m_type;
            }
        }


        /// <summary>
        /// Creates and returns an NSURL object initialized with a provided URL string.
        /// </summary>
        /// <param name="url">
        /// The URL string with which to initialize the NSURL object. 
        /// Must be a URL that conforms to RFC 2396. 
        /// This method parses URLString according to RFCs 1738 and 1808..
        /// </param>
        public static ISN_NSURL URLWithString(string url) {
            var uri = new ISN_NSURL();
            uri.m_url = url;
            uri.m_type = URLType.Default;

            return uri;
        }


        /// <summary>
        /// Initializes and returns a newly created <see cref="ISN_NSURL"/> object as a file URL with a specified path.
        /// </summary>
        /// <param name="path">The path that the <see cref="ISN_NSURL"/> object will represent. 
        /// path should be a valid system path, and must not be an empty path.
        /// </param>
        public static ISN_NSURL FileURLWithPath(string path) {
            var uri = new ISN_NSURL();
            uri.m_url = path;
            uri.m_type = URLType.File;

            return uri;
        }

        /// <summary>
        /// Initializes and returns a newly created <see cref="ISN_NSURL"/> object as a file URL with a specified path 
        /// relative to the unity StreamingAssets folder.
        /// </summary>
        /// <param name="path">The path that the <see cref="ISN_NSURL"/> object will represent. 
        /// path should be a valid StreamingAssets folder relative path, and must not be an empty path.
        /// </param>
        public static ISN_NSURL StreamingAssetsURLWithPath(string path) {
            var uri = new ISN_NSURL();
            uri.m_url = Path.Combine(Application.streamingAssetsPath, path);
            uri.m_type = URLType.File;

            return uri;
        }

    }
}