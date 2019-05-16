using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.iOS;
using SA.iOS.UIKit;
using SA.iOS.AVFoundation;
using SA.iOS.AVKit;
using SA.iOS.Foundation;


public class ISN_UIImagePickerControllerExample : MonoBehaviour {

    [SerializeField] RawImage m_Image = null;
    [SerializeField] Image m_Sprite = null;

    [Header("Image")]
    [SerializeField] Button m_ImageCapture = null;
    [SerializeField] Button m_ImageLibrary = null;
    [SerializeField] Button m_ImageAlbum = null;

    [Header("Video")]
    [SerializeField] Button m_VideoCapture = null;
    [SerializeField] Button m_VideoLibrary = null;
    [SerializeField] Button m_VideoAlbum = null;
    [SerializeField] Button m_VideoPlay = null;


    private ISN_UIPickerControllerResult m_LastPickerResult = null;
  
    private void Awake() {

        AddFitter(m_Image.gameObject);
        AddFitter(m_Sprite.gameObject);
       
        m_ImageCapture.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.Camera, ISN_UIMediaType.IMAGE);
        });

        m_ImageLibrary.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.PhotoLibrary, ISN_UIMediaType.IMAGE);
        });
        
        m_ImageAlbum.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.Album, ISN_UIMediaType.IMAGE);
        });
        
        
        
        m_VideoCapture.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.Camera, ISN_UIMediaType.MOVIE);
        });

        m_VideoLibrary.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.PhotoLibrary, ISN_UIMediaType.MOVIE);
        });
        
        m_VideoAlbum.onClick.AddListener(() =>
        {
            StartPicker(ISN_UIImagePickerControllerSourceType.Album, ISN_UIMediaType.MOVIE);
        });
        
        m_VideoPlay.onClick.AddListener(() =>
        {
            if (!ISN_Settings.Instance.AVKit)
            {
                DisplayMessage("AVKit should be enabled in plugin setting in order to play a video");
                return;
            }
            
            var url = ISN_NSURL.URLWithString(m_LastPickerResult.MediaURL);
            var player = new ISN_AVPlayer(url);

            var viewController = new ISN_AVPlayerViewController {Player = player};

            viewController.Show();
        });


        UpdateUI();
    }



    
    private void AddFitter(GameObject go) {
        var fitter = go.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = 1;
    }


    private void UpdateUI()
    {
        m_VideoPlay.interactable = !(m_LastPickerResult == null 
                                     || m_LastPickerResult.IsFailed
                                     || !m_LastPickerResult.MediaType.Equals(ISN_UIMediaType.MOVIE));
    }
    
    private void StartPicker(ISN_UIImagePickerControllerSourceType sourceType,  string mediaType)
    {
        ISN_UIImagePickerController picker = new ISN_UIImagePickerController
        {
            SourceType = sourceType,
            MediaTypes = new List<string>() {mediaType},
            MaxImageSize = 512,
            ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG,
            ImageCompressionRate = 0.8f
        };

        picker.Present(DisplayResult);
    }

    private void DisplayResult(ISN_UIPickerControllerResult result)
    {
        m_LastPickerResult = result;
        
        if (result.IsSucceeded) {

            if (result.MediaType.Equals(ISN_UIMediaType.IMAGE))
            {
                DisplayMessage("Image Loaded!");
                ApplyImageToGui(result.Image);
            }
            
            if (result.MediaType.Equals(ISN_UIMediaType.MOVIE))
            {
                DisplayMessage("Video Loaded!", () =>
                {
                    if (!ISN_Settings.Instance.AVKit)
                    {
                        DisplayMessage("AVKit should be enabled in plugin setting in order to retrieve video thumbnail");
                    }
                    else
                    {
                        Texture2D image = ISN_AVAssetImageGenerator.CopyCGImageAtTime(result.MediaURL, 0);
                        ApplyImageToGui(image);
                    }
                });
            }
        } 
        else
        {
            DisplayMessage("Failed: " + result.Error.FullMessage);
        }
        
        UpdateUI();
    }




    private void ApplyImageToGui(Texture2D image) {

        float aspectRatio = (float)image.width / (float)image.height;

        m_Image.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;
        m_Sprite.GetComponent<AspectRatioFitter>().aspectRatio = aspectRatio;

        //m_image is a UnityEngine.UI.RawImage
        m_Image.texture = image;

        //m_sprite is a UnityEngine.UI.Image
        m_Sprite.sprite = image.ToSprite();
    }

    private void DisplayMessage(string message, Action onClose = null)
    {
        ISN_UIAlertController alert = new ISN_UIAlertController("UIImagePickerController", 
            message, 
            ISN_UIAlertControllerStyle.Alert);
        ISN_UIAlertAction defaultAction = new ISN_UIAlertAction("Ok", ISN_UIAlertActionStyle.Default, () => {
            if (onClose != null)
            {
                onClose.Invoke();
            }
        });

        alert.AddAction(defaultAction);
        alert.Present();
    }

}
