using System;

namespace SA.Foundation.Config {
    [Serializable]
    public class PluginVersion {
        public int MajorVersion;
        public int MinorVersion;

        public PluginVersion() {
            
        }

        public Action SaveDelegate;

        public override string ToString() {
            var minorVersionString = MinorVersion > 0 ? "b"+MinorVersion:string.Empty;
            return string.Format("{0}{1}", MajorVersion, minorVersionString);;
        }

        public void UpgradeMinorVersion() {
            MinorVersion++;
        }
        

        public void UpgradeMajorVersion() {
            if (MinorVersion > 0) {
                MajorVersion++;
                MinorVersion = 0;
            }
        }
    }
}