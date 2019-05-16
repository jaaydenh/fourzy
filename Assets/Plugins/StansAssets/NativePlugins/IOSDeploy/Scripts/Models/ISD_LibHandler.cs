﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace  SA.iOS.XCode {
	
	public class ISD_LibHandler : MonoBehaviour {

		public static List<ISD_Library> AvailableLibraries{
			get{
				List<ISD_Library> resultList = new List<ISD_Library> ();
				List<string> strings = new List<string>( Enum.GetNames (typeof(ISD_iOSLibrary)));
				foreach (ISD_Library addedLibrary in ISD_Settings.Instance.Libraries) {
					if (strings.Contains(addedLibrary.Name)) {
						strings.Remove (addedLibrary.Name);
					}
				}

				foreach (ISD_iOSLibrary v in Enum.GetValues(typeof(ISD_iOSLibrary))) {
					if(strings.Contains(v.ToString())){
						resultList.Add(new ISD_Library((ISD_iOSLibrary)v) );
					}
				}
				return resultList;
			}
		}

		public static string[] BaseLibrariesArray(){
			List<string> array = new List<string> (AvailableLibraries.Capacity);
			foreach (ISD_Library library in AvailableLibraries) {
				array.Add (library.Name);
			}
			return array.ToArray ();
		}

		public static string stringValueOf(ISD_iOSLibrary value)
		{
			#if !UNITY_WSA
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			else
			{
				return value.ToString();
			}
			#else
			return string.Empty;
			#endif
		}

		public static object enumValueOf(string value)
		{
			Type enumType = typeof(ISD_iOSLibrary);
			string[] names = Enum.GetNames(enumType);
			foreach (string name in names)
			{
				if (stringValueOf((ISD_iOSLibrary)Enum.Parse(enumType, name)).Equals(value))
				{
					return Enum.Parse(enumType, name);
				}
			}

			throw new ArgumentException("The string is not a description or value of the specified enum...\n " + value + " is not right enum");
		}
	


	}
}
