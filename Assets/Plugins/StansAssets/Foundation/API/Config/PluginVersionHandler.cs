using System;
using System.IO;
using UnityEngine;

namespace SA.Foundation.Config {
    public class PluginVersionHandler {
        private string m_filename;
        private PluginVersion m_pluginVersion;

        public PluginVersionHandler(string basePath) {
            m_filename = basePath + "/pluginVersion.json";
        }

        public string GetVersion() {
            return PluginVersion.ToString();
        }

        public void UpgrageMinorVersion() {
            PluginVersion.UpgradeMinorVersion();
            Save();
        }

        public void UpgrageMajorVersionIfNeed() {
            PluginVersion.UpgradeMajorVersion();
            Save();
        }

        private PluginVersion PluginVersion {
            get {
                if (m_pluginVersion == null) {
                    if (File.Exists(m_filename)) {
                        string json = string.Empty;
                        try {
                            json = File.ReadAllText(m_filename);
                            m_pluginVersion = JsonUtility.FromJson<PluginVersion>(json);
                        }
                        catch (Exception e) {
                            Debug.LogError(e.Message);
                            Debug.LogError("Failed to red from: " + m_filename + " JSON: " + json);
                            m_pluginVersion = new PluginVersion();
                            throw;
                        }
                    } else {
                        m_pluginVersion = new PluginVersion();
                    }
                }

                return m_pluginVersion;
            }
        }

        private void Save() {
            File.WriteAllText(m_filename, JsonUtility.ToJson(PluginVersion));
        }

        public bool HasChanges() {
            return PluginVersion.MinorVersion > 0;
        }
    }
}