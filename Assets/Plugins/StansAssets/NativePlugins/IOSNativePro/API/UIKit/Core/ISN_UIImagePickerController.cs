using System;
using System.Collections.Generic;
using UnityEngine;


using SA.Foundation.Templates;
using SA.Foundation.Utility;
using SA.iOS.UIKit.Internal;
using SA.iOS.AVFoundation;

namespace SA.iOS.UIKit
{

    /// <summary>
    /// A view controller that manages the system interfaces for taking pictures, recording movies, 
    /// and choosing items from the user's media library.
    /// </summary>
    public class ISN_UIImagePickerController 
    {
        private ISN_UIPickerControllerRequest m_request = new ISN_UIPickerControllerRequest();

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Presents a view controller modally.
        /// 
        /// If you configured the view controller to use <see cref="ISN_UIImagePickerControllerSourceType.Camera"/>,
        /// the <see cref="ISN_AVMediaType.Video"/> permission will be checked automatically, 
        /// before presenting view controller. You can always do this yourself using the 
        /// <see cref="ISN_AVCaptureDevice.RequestAccess"/>
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void Present(Action<ISN_UIPickerControllerResult> callback) {

            //Need to make sure we have a permission for that
            if(m_request.m_sourceType == ISN_UIImagePickerControllerSourceType.Camera){
                ISN_AVCaptureDevice.RequestAccess(ISN_AVMediaType.Video, (status) => {
                    if(status == ISN_AVAuthorizationStatus.Authorized) {
                        StartPresenting(callback);
                    } else {
                        SA_Error error = new SA_Error(1, "AVMediaType.Video Permission is mising");
                        ISN_UIPickerControllerResult result = new ISN_UIPickerControllerResult(error);
                        callback.Invoke(result);
                    }
                });
            } else {
                StartPresenting(callback);
            }
        }


        //--------------------------------------
        // Get / Set 
        //--------------------------------------


        /// <summary>
        /// An array indicating the media types to be accessed by the media picker controller.
        /// 
        /// Depending on the media types you assign to this property, 
        /// the picker displays a dedicated interface for still images or movies, 
        /// or a selection control that lets the user choose the picker interface. 
        /// Before setting this property, 
        /// check which media types are available by calling the <see cref="GetAvailableMediaTypes"/> class method.
        /// 
        /// If you set this property to an empty array, 
        /// or to an array in which none of the media types is available for the current source, 
        /// the system throws an exception.
        /// 
        /// You may use madia type names from <see cref="ISN_UIMediaType"/>
        /// </summary>
        public List<string> MediaTypes {
            get {
                return m_request.m_mediaTypes;
            }

            set {
                m_request.m_mediaTypes = value;
            }
        }

        /// <summary>
        /// The type of picker interface to be displayed by the controller.
        /// 
        /// Prior to running the picker interface, set this value to the desired source type. 
        /// The source type you set must be available and an exception is thrown if it is not. 
        /// If you change this property while the picker is visible, 
        /// the picker interface changes to match the new value in this property.
        /// The various source types are listed in the <see cref="ISN_UIImagePickerControllerSourceType"/> enumeration.
        /// The default value is <see cref="ISN_UIImagePickerControllerSourceType.PhotoLibrary"/>.
        /// </summary>
        public ISN_UIImagePickerControllerSourceType SourceType {
            get {
                return m_request.m_sourceType;
            }

            set {
                m_request.m_sourceType = value;
            }
        }

        /// <summary>
        /// A Boolean value indicating whether the user is allowed to edit a selected still image or movie.
        /// This property is set to false by default.
        /// </summary>
        public bool AllowsEditing {
            get {
                return m_request.m_allowsEditing;
            }

            set {
                m_request.m_allowsEditing = value;
            }
        }

        /// <summary>
        /// Is <see cref="ImageCompressionFormat"/> is set tp <see cref="ISN_UIImageCompressionFormat.JPEG"/>
        /// commpression format value will be applayed.
        /// 
        /// The default value is 0.8f
        /// </summary>
        public float ImageCompressionRate {
            get {
                return m_request.m_imageCompressionRate;
            }

            set {
                m_request.m_imageCompressionRate = value;
            }
        }

        /// <summary>
        /// Max allowed image size. If bigger image is picked by user, image will be resized before sending to Unity.
        /// Most of the images in user photo library are big, so it's better to use this property to save some RAM.
        /// 
        /// The defaul value is 512
        /// </summary>
        /// <value>The size of the max image.</value>
        public int MaxImageSize {
            get {
                return m_request.m_maxImageSize;
            }

            set {
                m_request.m_maxImageSize = value;
            }
        }

        /// <summary>
        /// Image compression format. 
        /// Default value is JPEG
        /// </summary>
        public ISN_UIImageCompressionFormat ImageCompressionFormat {
            get {
                return m_request.m_encodingType;
            }

            set {
                m_request.m_encodingType = value;
            }
        }


        //--------------------------------------
        // Static Methods
        //--------------------------------------

        /// <summary>
        /// Adds the specified image to the user’s Camera Roll album.
        /// 
        /// When used on an iOS device without a camera, 
        /// this method adds the image to the Saved Photos album rather than to the Camera Roll album.
        /// </summary>
        /// <param name="texture">Texture you want to save to the album.</param>
        /// <param name="callback">Callback.</param>
        public static void SaveTextureToCameraRoll(Texture2D texture, Action<SA_Result> callback) {
            ISN_UILib.API.SaveTextureToCameraRoll(texture.ToBase64String(), callback);
        }


        /// <summary>
        /// Saves the screen screenshot to the saved photos album.
        /// </summary>
        /// <param name="callback">Callback.</param>
        public static void SaveScreenshotToCameraRoll(Action<SA_Result> callback) {
            SA_ScreenUtil.TakeScreenshot((texture) => {
                SaveTextureToCameraRoll(texture, callback);
            });
        }


        /// <summary>
        /// Returns an array of the available media types for the specified source type.
        /// 
        /// Some iOS devices support video recording. 
        /// Use this method, along with the <see cref="IsSourceTypeAvailable"/> method, 
        /// to determine if video recording is available on a device.
        /// </summary>
        /// <returns>The available media types.</returns>
        /// <param name="sourceType">The source to use to pick an image.</param>
        public static List<string> GetAvailableMediaTypes(ISN_UIImagePickerControllerSourceType sourceType) {
            return ISN_UILib.API.GetAvailableMediaTypes(sourceType);
        }

        /// <summary>
        /// Returns a Boolean value indicating whether the device supports picking media using the specified source type.
        /// 
        /// Because a media source may not be present or may be unavailable, 
        /// devices may not always support all source types. 
        /// For example, if you attempt to pick an image from the user’s library and the library is empty, 
        /// this method returns <c>false</c>. Similarly, if the camera is already in use, this method returns <c>false</c>.
        //
        /// Before attempting to use an <see cref="ISN_UIImagePickerController"/> object to pick an image, 
        /// you must call this method to ensure that the desired source type is available.
        /// </summary>
        public static bool IsSourceTypeAvailable(ISN_UIImagePickerControllerSourceType sourceType) {
            return ISN_UILib.API.IsSourceTypeAvailable(sourceType);
        }



        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void StartPresenting(Action<ISN_UIPickerControllerResult> callback) {
            if (m_request.m_mediaTypes.Count == 0) {
                m_request.m_mediaTypes = GetAvailableMediaTypes(m_request.m_sourceType);
            }
            ISN_UILib.API.PresentPickerController(m_request, callback);
        }

    }
}