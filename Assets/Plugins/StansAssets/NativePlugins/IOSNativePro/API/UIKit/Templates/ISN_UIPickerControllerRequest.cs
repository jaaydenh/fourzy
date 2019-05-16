using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{
    [Serializable]
    public class ISN_UIPickerControllerRequest 
    {
        [SerializeField] public List<string> m_mediaTypes  = new List<string>();
        [SerializeField] public ISN_UIImagePickerControllerSourceType m_sourceType = ISN_UIImagePickerControllerSourceType.PhotoLibrary;
        [SerializeField] public bool m_allowsEditing = false;

        [SerializeField] public float m_imageCompressionRate = 1;
        [SerializeField] public int m_maxImageSize = 1024;
        [SerializeField] public ISN_UIImageCompressionFormat m_encodingType = ISN_UIImageCompressionFormat.JPEG;

       
    }
}