using System;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.iOS.GameKit
{

    /// <summary>
    /// Object reflects an image load result.
    /// </summary>
    [Serializable]
    public class ISN_GKImageLoadResult : SA_Result
    {

        private Texture2D m_image = null;
        [SerializeField] string m_imageBase64 = null;

        public ISN_GKImageLoadResult(SA_Error error) : base(error) { }


        public Texture2D Image {
            get {

                if (m_image == null) {
                    if (string.IsNullOrEmpty(m_imageBase64)) {
                        return null;
                    }

                    m_image = new Texture2D(1, 1);
                    m_image.LoadImageFromBase64(m_imageBase64);
                }

                return m_image;
            }
        }
    }
}