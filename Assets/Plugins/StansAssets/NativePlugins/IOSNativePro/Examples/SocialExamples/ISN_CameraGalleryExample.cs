using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;

using SA.iOS.UIKit;
using SA.iOS.AVFoundation;
using SA.Foundation.Utility;

public class ISN_CameraGalleryExample : MonoBehaviour {

    [SerializeField] Button m_loadFromGallery = null;
    [SerializeField] Button m_loadFromCamera = null;
    [SerializeField] Button m_saveToGallery = null;


    [SerializeField] Image m_image = null;
    [SerializeField] GameObject m_go  = null;


    private void Start()
    {
        m_loadFromGallery.onClick.AddListener(() => {



    /*

            ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.IMAGE};
            picker.MaxImageSize = 1024;
            picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
            picker.ImageCompressionRate = 0.8f;
            picker.Present((result) => {
                if (result.IsSucceeded) {
                    Debug.Log("IMAGE local path: " + result.ImageURL);
                   // mySprite = result.Image.ToSprite();
                } else {
                   // canceled = true;
                    Debug.Log("Media picker failed with reason: " + result.Error.Message);
                }
            });
            */


            ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.MOVIE };
           

            picker.Present((result) => {
                if (result.IsSucceeded) {

                    Debug.Log("MOVIE local path: " + result.MediaURL);

                    try{
                        byte[] movieBytes = File.ReadAllBytes(result.MediaURL);
                        Debug.Log("movie size bytes: " + movieBytes.Length);
                    } catch(System.Exception ex) {
                        Debug.Log(ex.Message);
                    }

                  
                } else {
                    // canceled = true;
                    Debug.Log("Media picker failed with reason: " + result.Error.Message);
                }
            });


            //File

             /*

            ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Album;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.IMAGE,  ISN_UIMediaType.MOVIE};
            picker.MaxImageSize = 512;
            picker.ImageCompressionFormat = ISN_UIImageCompressionFormat.JPEG;
            picker.ImageCompressionRate = 0.8f;

            picker.Present((result) => {
                if (result.IsSucceeded) {
                    switch(result.MediaType) {
                        case ISN_UIMediaType.IMAGE:
                            Debug.Log("IMAGE local path: " + result.ImageURL);
                            m_image.sprite = result.Image.ToSprite();
                            m_go.GetComponent<Renderer>().material.mainTexture = result.Image;
                            break;
                        case ISN_UIMediaType.MOVIE:
                            Debug.Log("MOVIE local path: " + result.MediaURL);
                            Texture2D image = ISN_AVAssetImageGenerator.CopyCGImageAtTime(result.MediaURL, 0);
                            m_image.sprite = image.ToSprite();
                            m_go.GetComponent<Renderer>().material.mainTexture = image;
                            break;
                    }
                } else {
                    Debug.Log("Madia picker failed with reason: " + result.Error.Message);
                }
            });
            */
        });



        m_loadFromCamera.onClick.AddListener(() => {
            ISN_UIImagePickerController picker = new ISN_UIImagePickerController();
            picker.SourceType = ISN_UIImagePickerControllerSourceType.Camera;
            picker.MediaTypes = new List<string>() { ISN_UIMediaType.IMAGE };

            picker.Present((result) => {
                if (result.IsSucceeded) {
                    Debug.Log("Image captured: " + result.Image);
                    m_image.sprite = result.Image.ToSprite();
                    m_go.GetComponent<Renderer>().material.mainTexture = result.Image;
                } else {
                    Debug.Log("Madia picker failed with reason: " + result.Error.Message);
                }
            });
        });


        m_saveToGallery.onClick.AddListener(() => {
            SA_ScreenUtil.TakeScreenshot((image) => {
                ISN_UIImagePickerController.SaveTextureToCameraRoll(image, (result) => {
                    if (result.IsSucceeded) {
                        Debug.Log("Image saved");
                    } else {
                        Debug.Log("Error: " + result.Error.Message);
                    }
                });
            });
        });

    }

}
