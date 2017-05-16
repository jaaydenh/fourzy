#if USES_WEBVIEW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed partial class WebViewNativeUnsupported : IWebViewPlatform 
	{
		#region Lifecycle Methods
		
		public void Create (string _tag, Rect _frame)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void Destroy (string _tag)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void Show (string _tag)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void Hide (string _tag)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		#endregion
		
		#region Load Methods
		
		public void LoadRequest (string _tag, string _URL)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif		
		}
		
		public void LoadHTMLString (string _tag, string _HTMLString, string _baseURL)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif		
		}
		
		public void LoadData (string _tag, byte[] _byteArray, string _MIMEType, string _textEncodingName, string _baseURL)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif		
		}
		
		public void EvaluateJavaScriptFromString (string _tag, string _javaScript)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif		
		}
		
		public void Reload (string _tag)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif		
		}
		
		public void StopLoading (string _tag)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		#endregion
		
		#region Property Modifier Methods
		
		public void SetCanHide (string _tag, bool _canHide)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetCanBounce (string _tag, bool _canBounce)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetControlType (string _tag, eWebviewControlType _type)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetShowSpinnerOnLoad (string _tag, bool _showSpinner)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetAutoShowOnLoadFinish (string _tag, bool _autoShow)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetScalesPageToFit (string _tag, bool _scaleToFit)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetFrame (string _tag, Rect _frame)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		public void SetBackgroundColor (string _tag, Color _color)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		#endregion
		
		#region Communication Methods

		public void AddNewURLSchemeName (string _tag, string _newURLSchemeName)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		#endregion
		
		#region Cache Methods
		
		public void ClearCache ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupportedInEditor);
#endif
		}
		
		#endregion
	}
}
#endif