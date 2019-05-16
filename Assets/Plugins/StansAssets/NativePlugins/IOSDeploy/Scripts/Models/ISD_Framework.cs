
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SA.Foundation.Utility;

namespace SA.iOS.XCode {

	[System.Serializable]
	public class ISD_Framework  {

		public bool IsOptional;
		public bool IsEmbeded;

        public ISD_iOSFramework Type;
	

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SA.iOS.XCode.ISD_Framework"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="optional">If set to <c>true</c> optional.</param>
        public ISD_Framework(ISD_iOSFramework type, bool optional = false){
			Type = type;
            IsOptional = optional;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SA.iOS.XCode.ISD_Framework"/> class.
        /// </summary>
        /// <param name="frameworkName">Framework name.</param>
        /// <param name="optional">If set to <c>true</c> optional.</param>
        public ISD_Framework(string frameworkName, bool optional = false){
			frameworkName = frameworkName.Replace (".framework", string.Empty);
            Type =   SA_EnumUtil.ParseEnum<ISD_iOSFramework> (frameworkName);

            IsOptional = optional;
		}
			

        /// <summary>
        /// Gets the full framework name.
        /// </summary>
		public string Name {
			get{
				return Type.ToString () + ".framework";
			}
		}


	}


}