using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

#if USES_WEBVIEW
namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNative : MonoBehaviour 
	{	
		#region Constants

		// Events
		private const string kOnDidShowEvent						= "OnDidShow";
		private const string kOnDidHideEvent						= "OnDidHide";
		private const string kOnDidDestroyEvent						= "OnDidDestroy";
		private const string kOnDidStartLoadEvent					= "OnDidStartLoad";
		private const string kOnDidFinishLoadEvent					= "OnDidFinishLoad";
		private const string kOnDidFailLoadWithErrorEvent			= "OnDidFailLoadWithError";
		private const string kOnDidFinishEvaluatingJavaScriptEvent	= "OnDidFinishEvaluatingJavaScript";
		private const string kOnDidReceiveMessageEvent				= "OnDidReceiveMessage";

		#endregion

		#region Native Callback Methods

		protected void WebViewDidShow (string _tag)
		{
			WebView		_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidShow(_webview);
		}

		protected void WebViewDidShow (WebView _webview)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidShowEvent);
		}
		
		protected void WebViewDidHide (string _tag)
		{
			WebView		_webview	= GetWebViewWithTag(_tag);
			
			// Invoke handler
			WebViewDidHide(_webview);
		}
		
		protected void WebViewDidHide (WebView _webview)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidHideEvent);
		}

		protected void WebViewDidDestroy (string _tag)
		{
			WebView		_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidDestroy(_webview);
		}

		protected void WebViewDidDestroy (WebView _webview)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidDestroyEvent);

			RemoveWebViewFromCollection(_webview.UniqueID);
			Destroy(_webview.gameObject);
		}
		
		protected void WebViewDidStartLoad (string _tag)
		{
			WebView		_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidStartLoad(_webview);
		}

		protected void WebViewDidStartLoad (WebView _webview)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidStartLoadEvent);
		}
		
		protected void WebViewDidFinishLoad (string _dataStr)
		{
			// Parse received data
			IDictionary _dataDict	= JSONUtility.FromJSON(_dataStr) as IDictionary;
			string 		_tag;
			string 		_URL;
			
			ParseLoadFinishedData(_dataDict, out _tag, out _URL);
			
			// Find owner object
			WebView		_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidFinishLoad(_webview, _URL);
		}

		protected void WebViewDidFinishLoad (WebView _webview, string _URL)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidFinishLoadEvent, 
				                      new object[] {
					_URL
				},
				new System.Type[] {
					typeof(string)
				});
		}
		
		protected void WebViewDidFailLoadWithError (string _dataStr)
		{	
			// Parse received data
			IDictionary _dataDict	= JSONUtility.FromJSON(_dataStr) as IDictionary;
			string 		_tag;
			string 		_URL;
			string 		_error;

			ParseLoadFailedData(_dataDict, out _tag, out _URL, out _error);

			// Find owner object
			WebView		_webview	= GetWebViewWithTag(_tag);

			WebViewDidFailLoadWithError(_webview, _URL, _error);
		}

		protected void WebViewDidFailLoadWithError (WebView _webview, string _URL, string _error)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidFailLoadWithErrorEvent, 
				                      new object[] {
					_URL,
					_error
				},
				new System.Type[] {
					typeof(string),
					typeof(string)
				});
		}
		
		protected void WebViewDidFinishEvaluatingJS (string _dataStr)
		{
			// Parse received data
			IDictionary _dataDict	= JSONUtility.FromJSON(_dataStr) as IDictionary;
			string 		_tag;
			string 		_result;

			ParseEvalJSData(_dataDict, out _tag, out _result);

			// Find owner object
			WebView		_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidFinishEvaluatingJS(_webview, _result);
		}

		protected void WebViewDidFinishEvaluatingJS (WebView _webview, string _result)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidFinishEvaluatingJavaScriptEvent, 
				                      new object[] {
					_result
				},
				new System.Type[] {
					typeof(string)
				});
		}

		protected void WebViewDidReceiveMessage (string _dataStr)
		{
			// Parse received data
			IDictionary 	_dataDict	= JSONUtility.FromJSON(_dataStr) as IDictionary;
			string 			_tag;
			WebViewMessage	_message;

			ParseMessageData(_dataDict, out _tag, out _message);

			// Find owner object
			WebView			_webview	= GetWebViewWithTag(_tag);

			// Invoke handler
			WebViewDidReceiveMessage(_webview, _message);
		}

		protected void WebViewDidReceiveMessage (WebView _webview, WebViewMessage _message)
		{
			// Send event
			if (_webview != null)
				_webview.InvokeMethod(kOnDidReceiveMessageEvent, _message);
		}

		#endregion

		#region Parse Methods

		protected virtual void ParseLoadFinishedData (IDictionary _dataDict, out string _tag, out string _URL)
		{
			_tag		= null;
			_URL		= null;
		}

		protected virtual void ParseLoadFailedData (IDictionary _dataDict, out string _tag, out string _URL, out string _error)
		{
			_tag		= null;
			_URL		= null;
			_error		= null;
		}

		protected virtual void ParseEvalJSData (IDictionary _dataDict, out string _tag, out string _result)
		{
			_tag		= null;
			_result		= null;
		}
	
		protected virtual void ParseMessageData (IDictionary _dataDict, out string _tag, out WebViewMessage _message)
		{
			_tag		= null;
			_message	= null;
		}

		#endregion
	}
}
#endif