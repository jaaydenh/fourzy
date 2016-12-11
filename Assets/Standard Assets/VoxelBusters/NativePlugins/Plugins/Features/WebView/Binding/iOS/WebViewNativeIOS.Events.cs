using UnityEngine;
using System.Collections;

#if USES_WEBVIEW && UNITY_IOS
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNativeIOS : WebViewNative
	{
		#region Constants

		private		const 	string		kTagKey		= "tag";
		private		const 	string		kURLKey		= "url";
		private		const 	string		kErrorKey	= "error";
		private		const 	string		kResultKey	= "result";
		private		const 	string		kMessageKey	= "message-data";

		#endregion

		#region Parse Methods

		protected override void ParseLoadFinishedData (IDictionary _dataDict, out string _tag, out string _URL)
		{
			_tag		= _dataDict[kTagKey] as string;
			_URL		= _dataDict.GetIfAvailable<string>(kURLKey);
		}

		protected override void ParseLoadFailedData (IDictionary _dataDict, out string _tag, out string _URL, out string _error)
		{
			_tag		= _dataDict[kTagKey] as string;
			_URL		= _dataDict.GetIfAvailable<string>(kURLKey);
			_error		= _dataDict.GetIfAvailable<string>(kErrorKey);
		}
		
		protected override void ParseEvalJSData (IDictionary _dataDict, out string _tag, out string _result)
		{
			_tag		= _dataDict[kTagKey] as string;
			_result		= _dataDict.GetIfAvailable<string>(kResultKey);
		}
		
		protected override void ParseMessageData (IDictionary _dataDict, out string _tag, out WebViewMessage _message)
		{
			_tag		= _dataDict[kTagKey] as string;
			_message	= new iOSWebViewMessage(_dataDict[kMessageKey] as IDictionary);
		}
	
		#endregion
	}
}
#endif