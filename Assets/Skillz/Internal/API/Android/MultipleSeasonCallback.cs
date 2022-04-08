using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SkillzSDK.Internal.API.Android
{
    class MultipleSeasonCallback : AndroidJavaProxy
    {
        Action<List<Season>> successCallback;
        Action<string> failureCallback;
        public MultipleSeasonCallback(Action<List<Season>> success, Action<string> failure)
            : base("com.skillz.progression.MultipleSeasonCallback")
        {
            successCallback = success;
            failureCallback = failure;
        }

        void success(AndroidJavaObject dataObj)
        {
            if (dataObj == null)
            {
                if (successCallback != null)
                {
                    successCallback(null);
                }
                return;
            }
            List<Season> data = new List<Season>();
            int seasonsCount = dataObj.Call<int>("size");
            for (int j = 0; j < seasonsCount; j++)
            {
                AndroidJavaObject season = dataObj.Call<AndroidJavaObject>("get", j);
                string jsonString = season.Get<string>("jsonString");
                Dictionary<string, object> seasonDict = MiniJSON.Json.Deserialize(jsonString) as Dictionary<string, object>;
                Season dataEntry = new Season(seasonDict);
                data.Add(dataEntry);
            }

            if (successCallback != null)
            {
                successCallback(data);
            }
        }

        void failure(string errorMessage)
        {
            if (failureCallback != null)
            {
                failureCallback(errorMessage);
            }
        }
    }
}
