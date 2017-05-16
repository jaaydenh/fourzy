#if UNITY_ANDROID
using System;
using UnityEditor;
using UnityEngine;
using VoxelBusters.NativePlugins.Internal;
using VoxelBusters.Utility;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Play-Services Dependencies for Cross Platform Native Plugins.
	/// </summary>
	[InitializeOnLoad]
	public class NPAndroidLibraryDependencies
	{
		/// <summary>
		/// The name of your plugin.  This is used to create a settings file
		/// which contains the dependencies specific to your plugin.
		/// </summary>
		private static readonly string PluginName = "CrossPlatformNativePlugins";
		private static readonly string DependencyFileDirectory = "ProjectSettings";

		
		/// <summary>
		/// Initializes static members of the <see cref="NPAndroidLibraryDependencies"/> class.
		/// </summary>
		static NPAndroidLibraryDependencies()
		{
			EditorUtils.Invoke(()=>{
				CreateDependencies();		
			}, 0.1f);
		}

		private static void CreateDependencies()
		{
			// Setup the resolver using reflection as the module may not be
		    // available at compile time.
		    Type playServicesSupport = Google.VersionHandler.FindClass(
            "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
	        if (playServicesSupport == null) {
	            return;
	        }
	        object svcSupport = Google.VersionHandler.InvokeStaticMethod(
	            playServicesSupport, "CreateInstance",
	            new object[] {
	                PluginName,
	                EditorPrefs.GetString("AndroidSdkRoot"),
	                DependencyFileDirectory}
				);

			Google.VersionHandler.InvokeInstanceMethod(
			svcSupport, "ClearDependencies", null);

			if (NPSettings.Application.SupportedFeatures.UsesGameServices)
			{
				Google.VersionHandler.InvokeInstanceMethod(
	            svcSupport, "DependOn",
	            new object[] { 	"com.google.android.gms", 
								"play-services-games",
	                           	"9.8+" },
	            namedArgs: new Dictionary<string, object>() 
							{
		                		{
									"packageIds", 
									new string[] 
									{
		                       			"extra-google-m2repository",
		                        		"extra-android-m2repository"
									} 
								}
				            }
				);

				Google.VersionHandler.InvokeInstanceMethod(
	            svcSupport, "DependOn",
	            new object[] { 	"com.google.android.gms", 
								"play-services-nearby",
								"9.8+" },
	            namedArgs: null
				);
			}

			if (NPSettings.Application.SupportedFeatures.UsesNotificationService)
			{
				Google.VersionHandler.InvokeInstanceMethod(
	            svcSupport, "DependOn",
	            new object[] { 	"com.google.android.gms", 
								"play-services-gcm",
								"9.8+" },
	            namedArgs: null
				);
			}
			
			// Marshmallow permissions requires app-compat. Also used by some old API's for compatibility.
			Google.VersionHandler.InvokeInstanceMethod(
	            svcSupport, "DependOn",
	            new object[] { 	"com.android.support", 
								"support-v4",
	                           	"23.+" },
	            namedArgs: null
				);

			/*// If not enabled by default, resolve manually.
			if (!PlayServicesResolver.Resolver.AutomaticResolutionEnabled())
			{
				PlayServicesResolver.Resolver.DoResolution(svcSupport, "Assets/Plugins/Android", PlayServicesResolver.HandleOverwriteConfirmation);
				AssetDatabase.Refresh();
			}*/
		}
	}
}
#endif