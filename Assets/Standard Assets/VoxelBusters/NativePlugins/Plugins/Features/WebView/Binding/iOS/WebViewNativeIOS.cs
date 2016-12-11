using UnityEngine;
using System.Collections;

#if USES_WEBVIEW && UNITY_IOS
using System.Runtime.InteropServices;
using System.IO;

namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNativeIOS : WebViewNative
	{
		private struct NormalisedRect
		{
			#region Properties
			
			public 	float 	x;
			public 	float 	y;
			public 	float 	width;
			public 	float 	height;
			
			#endregion
			
			#region Constructor
			
			public NormalisedRect (Rect _rect) : this ()
			{
				this.x		= (_rect.x / (float)Screen.width);
				this.y		= (_rect.y / (float)Screen.height);
				this.width	= (_rect.width / (float)Screen.width);
				this.height	= (_rect.height / (float)Screen.height);
			}
			
			#endregion
		}

		#region Native Methods

		[DllImport("__Internal")]
		private static extern void webviewCreate (string _tag, NormalisedRect _noramlisedRect);

		[DllImport("__Internal")]
		private static extern void webviewDestroy (string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewShow (string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewHide (string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewLoadRequest (string _URL, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewLoadHTMLString (string _HTMLString, string _baseURL, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewLoadData (byte[] _dataArray, int _dataArrayLength, string _MIMEType, string _textEncodingName, string _baseURL, string _tag);

		[DllImport("__Internal")]
		private static extern void webviewEvaluateJavaScriptFromString (string _javaScript, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewStopLoading (string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewReload (string _tag);

		[DllImport("__Internal")]
		private static extern void webviewSetCanHide (bool _canHide, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewSetCanBounce (bool _canBounce, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewSetControlType (int _type, string _tag);

		[DllImport("__Internal")]
		private static extern void webviewSetShowSpinnerOnLoad (bool _showSpinner,	string _tag);

		[DllImport("__Internal")]
		private static extern void webviewSetAutoShowOnLoadFinish (bool _autoShow, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewSetScalesPageToFit (bool _scaleToFit, string _tag);

		[DllImport("__Internal")]
		private static extern void webviewSetNormalisedFrame (NormalisedRect _noramlisedRect, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewSetBackgroundColor (float _r, float _g, float _b, float _alpha, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewAddNewURLScheme (string _newURLScheme, string _tag);
		
		[DllImport("__Internal")]
		private static extern void webviewClearCache ();

		#endregion

		#region Lifecycle API's

		public override void Create (WebView _webview, Rect _frame)
		{
			base.Create(_webview, _frame);

			// Create native instance and set frame
			webviewCreate(_webview.UniqueID, new NormalisedRect(_frame));
		}
		
		public override void Destroy (VoxelBusters.NativePlugins.WebView _webview)
		{
			base.Destroy(_webview);

			// Destroy native webview
			webviewDestroy(_webview.UniqueID);
		}
		
		public override void Show (WebView _webview)
		{
			base.Show(_webview);

			// Native call
			webviewShow(_webview.UniqueID);
		}
		
		public override void Hide (WebView _webview)
		{
			base.Hide(_webview);

			// Native call
			webviewHide(_webview.UniqueID);
		}

		#endregion

		#region Load API's

		public override void LoadRequest (string _URL, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.LoadRequest(_URL, _webview);

			// Native call
			webviewLoadRequest(_URL, _webview.UniqueID);
		}

		public override void LoadHTMLString (string _HTMLString, string _baseURL, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.LoadHTMLString(_HTMLString, _baseURL, _webview);

			// Native call
			webviewLoadHTMLString(_HTMLString, _baseURL, _webview.UniqueID);
		}
		
		public override void LoadData (byte[] _byteArray, string _MIMEType, string _textEncodingName, string _baseURL, WebView _webview)
		{
			base.LoadData(_byteArray, _MIMEType, _textEncodingName, _baseURL, _webview);

			// Native call
			if (_byteArray != null)
			{
				webviewLoadData(_byteArray, _byteArray.Length, _MIMEType, _textEncodingName, _baseURL, _webview.UniqueID);
			}
		}

		public override void EvaluateJavaScriptFromString (string _javaScript, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.EvaluateJavaScriptFromString(_javaScript, _webview);

			// Native call
			webviewEvaluateJavaScriptFromString(_javaScript, _webview.UniqueID);
		}
		
		public override void Reload (WebView _webview)
		{
			base.Reload(_webview);

			// Native call
			webviewReload(_webview.UniqueID);
		}
		
		public override void StopLoading (WebView _webview)
		{
			base.StopLoading(_webview);

			// Native call
			webviewStopLoading(_webview.UniqueID);
		}

		#endregion

		#region Property Access API's

		public override void SetCanHide (bool _canHide, WebView _webview)
		{
			base.SetCanHide(_canHide, _webview);

			// Native call
			webviewSetCanHide(_canHide, _webview.UniqueID);
		}
		
		public override void SetCanBounce (bool _canBounce, WebView _webview)
		{
			base.SetCanBounce(_canBounce, _webview);

			// Native call
			webviewSetCanBounce(_canBounce, _webview.UniqueID);
		}

		public override void SetControlType (eWebviewControlType _type, WebView _webview) 
		{
			base.SetControlType(_type, _webview);

			// Native call
			webviewSetControlType((int)_type, _webview.UniqueID);
		}
	
		public override void SetShowSpinnerOnLoad (bool _showSpinner, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.SetShowSpinnerOnLoad(_showSpinner, _webview);

			// Native call
			webviewSetShowSpinnerOnLoad(_showSpinner, _webview.UniqueID);
		}
		
		public override void SetAutoShowOnLoadFinish (bool _autoShow, WebView _webview)
		{
			base.SetAutoShowOnLoadFinish(_autoShow, _webview);

			// Native call
			webviewSetAutoShowOnLoadFinish(_autoShow, _webview.UniqueID);
		}
		
		public override void SetScalesPageToFit (bool _scaleToFit, WebView _webview)
		{
			base.SetScalesPageToFit(_scaleToFit, _webview);

			// Native call
			webviewSetScalesPageToFit(_scaleToFit, _webview.UniqueID);
		}

		public override void SetFrame (Rect _frame, WebView _webview)
		{
			base.SetFrame(_frame, _webview);

			// Native call
			webviewSetNormalisedFrame(new NormalisedRect(_frame), _webview.UniqueID);
		}

		public override void SetBackgroundColor (Color _color, WebView _webview)
		{
			base.SetBackgroundColor(_color, _webview);

			// Native call
			webviewSetBackgroundColor(_color.r, _color.g, _color.b, _color.a, _webview.UniqueID);
		}

		#endregion

		#region URL Scheme
		
		public override void AddNewURLSchemeName (string _newURLSchemeName, WebView _webview)
		{
			base.AddNewURLSchemeName(_newURLSchemeName, _webview);

			// Native call
			webviewAddNewURLScheme(_newURLSchemeName, _webview.UniqueID);
		}

		#endregion
		
		#region Cache Clearance API's

		public override void ClearCache ()
		{
			base.ClearCache();

			// Native call
			webviewClearCache();
		}

		#endregion
	}
}
#endif