#if UNITY_ANDROID

using Google.JarResolver;
using UnityEditor;
using UnityEngine;
using VoxelBusters.NativePlugins.Internal;
using VoxelBusters.Utility;
using GooglePlayServices;

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
			EditorInvoke.Invoke(()=>{
				CreateDependencies();		
			}, 0.1f);
		}

		private static void CreateDependencies()
		{
			PlayServicesSupport svcSupport = PlayServicesSupport.CreateInstance(
				PluginName,
				EditorPrefs.GetString("AndroidSdkRoot"),
				DependencyFileDirectory);

			svcSupport.ClearDependencies();

			if (NPSettings.Application.SupportedFeatures.UsesGameServices)
			{
				svcSupport.DependOn("com.google.android.gms",
				                    "play-services-games",
				                    "LATEST");
				
				// need nearby too, even if it is not used.
				svcSupport.DependOn("com.google.android.gms",
				                    "play-services-nearby",
				                    "LATEST");
			}

			if (NPSettings.Application.SupportedFeatures.UsesNotificationService)
			{
				svcSupport.DependOn("com.google.android.gms",
				                    "play-services-gcm",
				                    "LATEST");
			}
			
			// Marshmallow permissions requires app-compat. Also used by some old API's for compatibility.
			svcSupport.DependOn("com.android.support",
			                    "support-v4",
			                    "23.+");

			// If not enabled by default, resolve manually.
			if (!PlayServicesResolver.Resolver.AutomaticResolutionEnabled())
			{
				PlayServicesResolver.Resolver.DoResolution(svcSupport, "Assets/Plugins/Android", PlayServicesResolver.HandleOverwriteConfirmation);
				AssetDatabase.Refresh();
			}
		}
	}
}
#endif