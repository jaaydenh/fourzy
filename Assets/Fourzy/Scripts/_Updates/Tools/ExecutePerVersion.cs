//@vadym udod

using Fourzy._Updates.Serialized;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    /// <summary>
    /// Only execute once per specified version
    /// </summary>
    public class ExecutePerVersion
    {
        public const string kVersionKey = "execute_for_version";

        public static string version = "1.3.3";

        public static void TryExecute()
        {
            if (PlayerPrefs.GetString(kVersionKey, "") != version && Application.version == version)
            {
                Tasks();

                PlayerPrefs.SetString(kVersionKey, version);
            }
        }

        private static void Tasks()
        {
            Debug.Log("Gamepieces reset " + PlayerPrefs.GetString(kVersionKey, ""));

            //execute
        }
    }
}