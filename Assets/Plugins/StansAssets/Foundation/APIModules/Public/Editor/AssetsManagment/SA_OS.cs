using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Foundation.Editor
{
    public static class SA_OS 
    {
        private static SA_FilesBrowser s_browser;
        public static SA_FilesBrowser FilesBrowser {
            get {
                if(s_browser == null) {
                    s_browser = new SA_FilesBrowser();
                }

                return s_browser;
            }
        }
    }
}