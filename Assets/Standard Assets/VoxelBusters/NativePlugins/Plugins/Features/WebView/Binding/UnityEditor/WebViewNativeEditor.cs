using UnityEngine;
using System.Collections;

#if USES_WEBVIEW && UNITY_EDITOR
using System.Collections.Generic;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNativeEditor : WebViewNative 
	{
		#region Lifecycle API's
		
		public override void Create (WebView _webview, Rect _frame)
		{
			base.Create(_webview, _frame);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void Destroy (VoxelBusters.NativePlugins.WebView _webview)
		{
			base.Destroy(_webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);

			WebViewDidDestroy(_webview);
		}
		
		public override void Show (WebView _webview)
		{
			base.Show(_webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);

			WebViewDidShow(_webview);
		}
		
		public override void Hide (WebView _webview)
		{
			base.Hide(_webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);

			WebViewDidHide(_webview);
		}
		
		#endregion
		
		#region Load API's
		
		public override void LoadRequest (string _URL, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.LoadRequest(_URL, _webview);
			
			// Feature isnt supported
			WebViewDidFailLoadWithError(_webview, null, Constants.kNotSupportedInEditor);
		}
		
		public override void LoadHTMLString (string _HTMLString, string _baseURL, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.LoadHTMLString(_HTMLString, _baseURL, _webview);
			
			// Feature isnt supported
			WebViewDidFailLoadWithError(_webview, null, Constants.kNotSupportedInEditor);
		}
		
		public override void LoadData (byte[] _byteArray, string _MIMEType, string _textEncodingName, string _baseURL, WebView _webview)
		{
			base.LoadData(_byteArray, _MIMEType, _textEncodingName, _baseURL, _webview);
			
			// Feature isnt supported
			WebViewDidFailLoadWithError(_webview, null, Constants.kNotSupportedInEditor);
		}
		
		public override void EvaluateJavaScriptFromString (string _javaScript, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.EvaluateJavaScriptFromString(_javaScript, _webview);
			
			// Feature isnt supported
			WebViewDidFinishEvaluatingJS(_webview, Constants.kNotSupportedInEditor);
		}
		
		public override void Reload (WebView _webview)
		{
			base.Reload(_webview);
			
			// Feature isnt supported
			WebViewDidFailLoadWithError(_webview, null, Constants.kNotSupportedInEditor);
		}
		
		public override void StopLoading (WebView _webview)
		{
			base.StopLoading(_webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		#endregion
		
		#region Property Access API's
		
		public override void SetCanHide (bool _canHide, WebView _webview)
		{
			base.SetCanHide(_canHide, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetCanBounce (bool _canBounce, WebView _webview)
		{
			base.SetCanBounce(_canBounce, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetControlType (eWebviewControlType _type, WebView _webview) 
		{
			base.SetControlType(_type, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetShowSpinnerOnLoad (bool _showSpinner, VoxelBusters.NativePlugins.WebView _webview)
		{
			base.SetShowSpinnerOnLoad(_showSpinner, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetAutoShowOnLoadFinish (bool _autoShow, WebView _webview)
		{
			base.SetAutoShowOnLoadFinish(_autoShow, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetScalesPageToFit (bool _scaleToFit, WebView _webview)
		{
			base.SetScalesPageToFit(_scaleToFit, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetFrame (Rect _frame, WebView _webview)
		{
			base.SetFrame(_frame, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		public override void SetBackgroundColor (Color _color, WebView _webview)
		{
			base.SetBackgroundColor(_color, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		#endregion
		
		#region URL Scheme
		
		public override void AddNewURLSchemeName (string _newURLSchemeName, WebView _webview)
		{
			base.AddNewURLSchemeName(_newURLSchemeName, _webview);
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		#endregion
		
		#region Cache Clearance API's
		
		public override void ClearCache ()
		{
			base.ClearCache();
			
			// Feature isnt supported
			Console.LogError(Constants.kDebugTag, Constants.kNotSupportedInEditor);
		}
		
		#endregion
	}
}
#endif