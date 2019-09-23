using UnityEngine;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.TestTools;
#endif

/// <summary>
/// Utility methods and custom overrides for MoPub unit tests
/// </summary>
public class MoPubTest
{
    public static class LogAssert
    {
        public static void Expect(LogType logType, string message)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.TestTools.LogAssert.Expect(logType, message);
#endif
            Debug.LogFormat("The previous {0} log was expected.", logType);
        }
    }
}
