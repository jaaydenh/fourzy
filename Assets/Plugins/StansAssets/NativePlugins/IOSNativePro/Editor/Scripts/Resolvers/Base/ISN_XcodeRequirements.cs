using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS
{

    public class ISN_XcodeRequirements
    {
        private List<ISD_Framework> m_frameworks = new List<ISD_Framework>();
        private List<ISD_Library>   m_libraries = new List<ISD_Library>();
        private List<ISD_BuildProperty> m_properties = new List<ISD_BuildProperty>();
        private List<ISD_PlistKey> m_plistKeys = new List<ISD_PlistKey>();

        //This is used for drawing UI only
        public List<string> Capabilities = new List<string>();



        public void AddFramework(ISD_Framework framework) {
            m_frameworks.Add(framework);
        }


        public void AddInfoPlistKey(ISD_PlistKey variables) {
            m_plistKeys.Add(variables);
        }

        public void AddLibrary(ISD_Library library) {
            m_libraries.Add(library);
        }

        public void AddBuildProperty(ISD_BuildProperty property) {
            m_properties.Add(property);
        }


        public List<ISD_Framework> Frameworks {
            get {
                return m_frameworks;
            }
        }

        public List<ISD_Library> Libraries {
            get {
                return m_libraries;
            }
        }

        public List<ISD_BuildProperty> Properties {
            get {
                return m_properties;
            }
        }

        public List<ISD_PlistKey> PlistKeys {
            get {
                return m_plistKeys;
            }
        }

    }
}