////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;

using SA.Foundation.Async;

namespace SA.Foundation.Utility {

    /// <summary>
    /// Utility class for a screen manipulations
    /// </summary>
	public static class SA_ScreenUtil  {

        /// <summary>
        /// Takes a screenshot with no size restirctions
        /// </summary>
        /// <param name="callback"> Result callback.</param> 
		public static void TakeScreenshot( Action<Texture2D> callback) {
            SA_Coroutine.Start(TakeScreenshotCoroutine(0, callback));
		}


        /// <summary>
        /// Takes a screenshot
        /// </summary>
        /// <param name="maxSize">Max size of picture result</param>
        /// <param name="callback">Result callback.</param> 
        public static void TakeScreenshot(int maxSize, Action<Texture2D> callback) {
            SA_Coroutine.Start(TakeScreenshotCoroutine(maxSize, callback));
        }


        /// <summary>
        /// Takes the screenshot from a spesific camera
        /// </summary>
        /// <param name="camera">Camera to take screenshot from</param>
        /// <param name="callback">Result callback.</param>
        public static void TakeScreenshot(Camera camera, Action<Texture2D> callback) {
            var capturer = new GameObject("SA_Screenshot").AddComponent<CameraScreenshot>();
            capturer.m_camera = camera;
            capturer.m_callback = callback;
            capturer.resWidth = Screen.width;
            capturer.resHeight = Screen.height;

        }


        private static IEnumerator TakeScreenshotCoroutine(int maxSize, Action<Texture2D> callback) {

            yield return new WaitForEndOfFrame();
            // Create a texture the size of the screen, RGB24 format
            int width =  Screen.width;
            int height = Screen.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            if (maxSize > 0) {
                int biggestSide = width;
                if (height > width) {
                    biggestSide = height;
                }

                //TODO fix it, looks ugly
                if (biggestSide > maxSize) {
                    float scale = (float)maxSize / (float)biggestSide;
                    Texture2D resized = SA_IconManager.ResizeTexture(tex, SA_IconManager.ImageFilterMode.Nearest, scale);
                    tex = resized;
                }
            }
            callback.Invoke(tex);
        }




        private class CameraScreenshot : MonoBehaviour
        {
            internal Camera m_camera;
            internal Action<Texture2D> m_callback;

            internal int resWidth;
            internal int resHeight;


            void LateUpdate() {

                RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
                m_camera.targetTexture = rt;
                Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
                m_camera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                screenShot.Apply();
                m_camera.targetTexture = null;
                RenderTexture.active = null; // JC: added to avoid errors
                Destroy(rt);

                m_callback.Invoke(screenShot);
                Destroy(gameObject);

            }

        }

    }

}

