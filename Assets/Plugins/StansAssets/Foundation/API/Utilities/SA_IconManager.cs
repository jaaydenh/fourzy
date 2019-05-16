using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Foundation.Utility
{

	public static class SA_IconManager   {

        public enum ImageFilterMode : int
        {
            Nearest = 0,
            Biliner = 1,
            Average = 2
        }

        private static Dictionary<string, Texture2D> s_icons = new Dictionary<string, Texture2D>();
		private static Dictionary<float, Texture2D> s_colorIcons = new Dictionary<float, Texture2D>();


		/// <summary>
		/// Generates plane color 1*1 <see cref="Texture2D"/> object from a give html color string.
		/// </summary>
		/// <param name="htmlString"> Case insensitive html string to be converted into a color.</param> 
		public static Texture2D GetIconFromHtmlColorString(string htmlString) {
			return GetIconFromHtmlColorString(htmlString, Color.black);
		}


		/// <summary>
		/// Generates plane color 1*1 <see cref="Texture2D"/> object from a give html color string.
		/// </summary>
		/// <param name="htmlString"> Case insensitive html string to be converted into a color.</param> 
		/// <param name="fallback"> Fall back  <see cref="Color"/> in case of unsuccessful color convertation.</param> 
		public static Texture2D GetIconFromHtmlColorString (string htmlString, Color fallback ) {
			Color color = fallback;
			ColorUtility.TryParseHtmlString (htmlString, out color);
			return GetIcon (color);
		}



		/// <summary>
		/// Generates plane color <see cref="Texture2D"/> object of a given size
		/// </summary>
		/// <param name="color"> Texture color. </param> 
		/// <param name="width"> Texture width. </param> 
		/// <param name="width"> Texture height. </param> 
		public static Texture2D GetIcon(Color color, int width = 1, int height = 1) {
			float colorId = color.r * 100000f + color.g * 10000f + color.b * 1000f + color.a * 100f + width * 10f + height;

			if (s_colorIcons.ContainsKey(colorId) && s_colorIcons[colorId] != null) {
				return s_colorIcons[colorId];
			} else {


				Texture2D tex = new Texture2D(width, height);
				for (int w = 0; w < width; w++) {
					for (int h = 0; h < height; h++) {
						tex.SetPixel(w, h, color);
					}
				}

				tex.Apply();


				s_colorIcons[colorId] = tex;
				return GetIcon(color, width, height);
			}
		}



		/// <summary>
		/// Loads a <see cref="Texture2D"/> object from the spesified Resources folder relative path.
		/// Object also will be cached in memory.
		/// </summary>
		/// <param name="path"> Resources folder relative path. </param> 
		public static Texture2D GetIconAtPath(string path) {

			if(s_icons.ContainsKey(path)) {
				return s_icons[path];
			} else {
				Texture2D tex = Resources.Load(path) as Texture2D;
				if(tex == null) {
					tex = new Texture2D(1, 1);
				}

				s_icons.Add(path, tex);
				return GetIconAtPath(path);
			}
		}

		/// <summary>
		/// Rotates <see cref="Texture2D"/> pixels to a spesified angle
		/// </summary>
		/// <param name="tex"> Source texture to rotate. </param> 
		/// <param name="angle"> Rotate angle </param> 
		public static Texture2D Rotate(Texture2D tex, float angle) {
			Texture2D rotImage = new Texture2D(tex.width, tex.height);
			int x, y;
			float x1, y1, x2, y2;

			int w = tex.width;
			int h = tex.height;
			float x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
			float y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

			float dx_x = rot_x(angle, 1.0f, 0.0f);
			float dx_y = rot_y(angle, 1.0f, 0.0f);
			float dy_x = rot_x(angle, 0.0f, 1.0f);
			float dy_y = rot_y(angle, 0.0f, 1.0f);



			x1 = x0;
			y1 = y0;

			for (x = 0; x < tex.width; x++) {
				x2 = x1;
				y2 = y1;
				for (y = 0; y < tex.height; y++) {
					//rotImage.SetPixel (x1, y1, Color.clear);          

					x2 += dx_x;//rot_x(angle, x1, y1);
					y2 += dx_y;//rot_y(angle, x1, y1);
					rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex, x2, y2));
				}

				x1 += dy_x;
				y1 += dy_y;

			}

			rotImage.Apply();
			return rotImage;
		}


       
        public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale) {

            //*** Variables
            int i;

            //*** Get All the source pixels
            Color[] aSourceColor = pSource.GetPixels(0);
            Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

            //*** Calculate New Size
            float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);
            float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);

            //*** Make New
            Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

            //*** Make destination array
            int xLength = (int)xWidth * (int)xHeight;
            Color[] aColor = new Color[xLength];

            Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

            //*** Loop through destination pixels and process
            Vector2 vCenter = new Vector2();
            for (i = 0; i < xLength; i++) {

                //*** Figure out x&y
                float xX = (float)i % xWidth;
                float xY = Mathf.Floor((float)i / xWidth);

                //*** Calculate Center
                vCenter.x = (xX / xWidth) * vSourceSize.x;
                vCenter.y = (xY / xHeight) * vSourceSize.y;

                //*** Do Based on mode
                //*** Nearest neighbour (testing)
                if (pFilterMode == ImageFilterMode.Nearest) {

                    //*** Nearest neighbour (testing)
                    vCenter.x = Mathf.Round(vCenter.x);
                    vCenter.y = Mathf.Round(vCenter.y);

                    //*** Calculate source index
                    int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

                    //*** Copy Pixel
                    aColor[i] = aSourceColor[xSourceIndex];
                }

                //*** Bilinear
                else if (pFilterMode == ImageFilterMode.Biliner) {

                    //*** Get Ratios
                    float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
                    float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

                    //*** Get Pixel index's
                    int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
                    int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

                    //*** Calculate Color
                    aColor[i] = Color.Lerp(
                        Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
                        Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
                        xRatioY
                    );
                }

                //*** Average
                else if (pFilterMode == ImageFilterMode.Average) {

                    //*** Calculate grid around point
                    int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                    int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                    int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                    int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                    //*** Loop and accumulate
                  //  Vector4 oColorTotal = new Vector4();
                    Color oColorTemp = new Color();
                    float xGridCount = 0;
                    for (int iy = xYFrom; iy < xYTo; iy++) {
                        for (int ix = xXFrom; ix < xXTo; ix++) {

                            //*** Get Color
                            oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                            //*** Sum
                            xGridCount++;
                        }
                    }

                    //*** Average Color
                    aColor[i] = oColorTemp / (float)xGridCount;
                }
            }

            //*** Set Pixels
            oNewTex.SetPixels(aColor);
            oNewTex.Apply();

            //*** Return
            return oNewTex;
        }

        public static Texture2D Resize(Texture2D source, int newWidth, int newHeight) {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            var nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            return nTex;
        }


        /// <summary>
        /// Attempts to convert a html color string.
        /// </summary>
        /// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
        public static Color GetColorFromHtml(string htmlString) {
			return GetColorFromHtml(htmlString, Color.black);
		}


		/// <summary>
		/// Attempts to convert a html color string.
		/// </summary>
		/// <param name="htmlString">Case insensitive html string to be converted into a color.</param>
		/// <param name="fallback">The fallback color in case convertation error.</param>
		public static Color GetColorFromHtml(string htmlString, Color fallback) {
			Color color = fallback;
			ColorUtility.TryParseHtmlString(htmlString, out color);
			return color;
		}


		private static Color getPixel(Texture2D tex, float x, float y) {
			Color pix;
			int x1 = (int)Mathf.Floor(x);
			int y1 = (int)Mathf.Floor(y);

			if (x1 > tex.width || x1 < 0 ||
				y1 > tex.height || y1 < 0) {
				pix = Color.clear;
			} else {
				pix = tex.GetPixel(x1, y1);
			}

			return pix;
		}

		private static float rot_x(float angle, float x, float y) {
			float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
			float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
			return (x * cos + y * (-sin));
		}
		private static float rot_y(float angle, float x, float y) {
			float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
			float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
			return (x * sin + y * cos);
		}




	}
}
