using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if USES_WEBVIEW
namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNative : MonoBehaviour 
	{
		#region Properties

		private Dictionary<string, WebView>	m_webviewCollection	= new Dictionary<string, WebView>();

		#endregion

		#region Lifecycle API's
		
		public virtual void Create (WebView _webview, Rect _frame)
		{
			// Add to collection
			AddWebViewToCollection(_webview.UniqueID, _webview);
		}
		
		public virtual void Destroy (WebView _webview)
		{}
		
		public virtual void Show (WebView _webview)
		{}
		
		public virtual void Hide (WebView _webview)
		{}
		
		#endregion
		
		#region Load API's
		
		public virtual void LoadRequest (string _URL, WebView _webview)
		{}
		
		public virtual void LoadHTMLString (string _HTMLString, string _baseURL, WebView _webview)
		{}
		
		public virtual void LoadData (byte[] _byteArray, string _MIMEType, string _textEncodingName, string _baseURL, WebView _webview)
		{}
		
		public virtual void EvaluateJavaScriptFromString (string _javaScript, WebView _webview)
		{}

		public virtual void Reload (WebView _webview)
		{}

		public virtual void StopLoading (WebView _webview)
		{}
		
		#endregion
		
		#region Property Access API's
		
		public virtual void SetCanHide (bool _canHide, WebView _webview)
		{}
		
		public virtual void SetCanBounce (bool _canBounce, WebView _webview)
		{}
		
		public virtual void SetControlType (eWebviewControlType _type, WebView _webview) 
		{}
		
		public virtual void SetShowSpinnerOnLoad (bool _showSpinner, WebView _webview)
		{}

		public virtual void SetAutoShowOnLoadFinish (bool _autoShow, WebView _webview)
		{}
		
		public virtual void SetScalesPageToFit (bool _scaleToFit, WebView _webview)
		{}
		
		public virtual void SetFrame (Rect _frame, WebView _webview)
		{}
		
		public virtual void SetBackgroundColor (Color _color, WebView _webview)
		{}
		
		#endregion
		
		#region URL Scheme
		
		public virtual void AddNewURLSchemeName (string _newURLScheme, WebView _webview)
		{}
		
		#endregion
		
		#region Cache Clearance API's
		
		public virtual void ClearCache ()
		{}
		
		#endregion
		
		#region Instance Collection Methods
		
		protected NativePlugins.WebView GetWebViewWithTag (string _tag)
		{
			if (m_webviewCollection.ContainsKey(_tag))
				return m_webviewCollection[_tag];
			
			return null;
		}
		
		protected void AddWebViewToCollection (string _tag, NativePlugins.WebView _webview)
		{
			m_webviewCollection[_tag]	= _webview;
		}
		
		protected void RemoveWebViewFromCollection (string _tag)
		{
			if (m_webviewCollection.ContainsKey(_tag))
				m_webviewCollection.Remove(_tag);
		}
		
		#endregion
	}
}
#endif