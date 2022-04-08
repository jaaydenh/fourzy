using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using SkillzSDK.Settings;
using SkillzSDK.Extensions;

using JSONDict = System.Collections.Generic.Dictionary<string, object>;
using System.Linq;


namespace SkillzSDK
{
    /// <summary>
    /// A season as defined by the Skillz SDK for use with Progression features.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// A unique identifier for this season on the Skillz platform.
        /// </summary>
        public readonly string ID;

        /// <summary>
        /// The name for this season.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The description for this season.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Whether or not this season was currently active when the data was retrieved from Skillz.
        /// </summary>
        public readonly bool IsActive;

        /// <summary>
        /// The starting time of this season.
        /// </summary>
        public readonly DateTime StartTime;

        /// <summary>
        /// The ending time of this season.
        /// </summary>
        public readonly DateTime EndTime;

        /// <summary>
        /// The developer-defined parameters for this season. All values are stored as strings
        /// but may be parsed into appropriate data types based on their configuration.
        /// </summary>
        public readonly Dictionary<string, string> SeasonParams;

        /// <summary>
        /// The index of this season relative to the current season. 0 indicates this is the current season.
        /// A negative number indicates it is a previous season with -1 being the most recent season. A positive number
        /// indicates an upcoming season with 1 being the next season that will start.
        /// </summary>
        public readonly int SeasonIndex;

        public Season(string id, string name, string description, bool isActive, string startTime, string endTime, Dictionary<string, string> seasonParams, int index)
        {
            ID = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            SeasonParams = seasonParams;
            SeasonIndex = index;

            DateTime startTimeParsed;
            if (DateTime.TryParse(startTime, out startTimeParsed))
            {
                StartTime = startTimeParsed.ToUniversalTime();
            }
            else
            {
                Debug.Log("Season: DateTime failed to parse start time: " + startTime);
            }

            DateTime endTimeParsed;
            if (DateTime.TryParse(endTime, out endTimeParsed))
            {
                EndTime = endTimeParsed.ToUniversalTime();
            }
            else
            {
                Debug.Log("Season: DateTime failed to parse end time: " + endTime);
            }
        }

        public Season(JSONDict jsonData)
        {
            // Parse simple data values
            ID = jsonData.SafeGetStringValue("id");
            Name = jsonData.SafeGetStringValue("season_name");
            Description = jsonData.SafeGetStringValue("description");
            IsActive = (bool)jsonData.SafeGetBoolValue("is_active");
            SeasonIndex = (int)jsonData.SafeGetIntValue("season_index");

            string startTime = jsonData.SafeGetStringValue("start_time");
            string endTime = jsonData.SafeGetStringValue("end_time");

            DateTime startTimeParsed;
            if (DateTime.TryParse(startTime, out startTimeParsed))
            {
                StartTime = startTimeParsed.ToUniversalTime();
            }
            else
            {
                Debug.Log("Season: DateTime failed to parse start time: " + startTime);
            }

            DateTime endTimeParsed;
            if (DateTime.TryParse(endTime, out endTimeParsed))
            {
                EndTime = endTimeParsed.ToUniversalTime();
            }
            else
            {
                Debug.Log("Season: DateTime failed to parse end time: " + endTime);
            }

            // Parse season params
            List<object> paramsList = (List<object>)jsonData.SafeGetValue("season_params");
            Dictionary<string, string> seasonParams = new Dictionary<string, string>();
            foreach (object param in paramsList)
            {
                Dictionary<string, object> paramObj = (Dictionary<string, object>)param;
                string paramName = paramObj.SafeGetStringValue("name");
                string paramValue = paramObj.SafeGetStringValue("value");
                seasonParams.Add(paramName, paramValue);
            }
            SeasonParams = seasonParams;
        }

        public string ToString()
        {
            string paramsString = "{";
            foreach (string key in SeasonParams.Keys)
            {
                paramsString += String.Format("{0}: {1},", key, SeasonParams[key]);
            }
            paramsString += "}";
            return String.Format("Season id: {0} startTime: {1}, endTime: {2}, name: {3}, description: {4}, isActive: {5}, index: {6},  params: {7}",
                 ID, StartTime.ToString(), EndTime.ToString(), Name, Description, IsActive.ToString(), SeasonIndex, paramsString);
        }

        /// <summary>
        /// Returns a list of Seasons from the given JSON string array.
        /// Expects a JSON string in the form {seasons: [...seasonObjects]}
        /// </summary>
        public static List<Season> ParseSeasonsFromJSON(JSONDict jsonDict)
        {
            List<Season> parsedSeasons = new List<Season>();
            List<object> seasons = (List<object>)jsonDict.SafeGetValue("seasons");

            foreach (object season in seasons)
            {
                Dictionary<string, object> seasonObj = (Dictionary<string, object>)season;
                // Parse simple data values
                string id = seasonObj.SafeGetStringValue("id");
                string name = seasonObj.SafeGetStringValue("season_name");
                string description = seasonObj.SafeGetStringValue("description");
                bool isActive = (bool)seasonObj.SafeGetBoolValue("is_active");
                string startTime = seasonObj.SafeGetStringValue("start_time");
                string endTime = seasonObj.SafeGetStringValue("end_time");
                int index = (int)seasonObj.SafeGetIntValue("season_index");

                // Parse season params
                List<object> paramsList = (List<object>)seasonObj.SafeGetValue("season_params");
                Dictionary<string, string> seasonParams = new Dictionary<string, string>();
                foreach (object param in paramsList)
                {
                    Dictionary<string, object> paramObj = (Dictionary<string, object>)param;
                    string paramName = paramObj.SafeGetStringValue("name");
                    string paramValue = paramObj.SafeGetStringValue("value");
                    seasonParams.Add(paramName, paramValue);
                }

                parsedSeasons.Add(new Season(id, name, description, isActive, startTime, endTime, seasonParams, index));
                Debug.Log("Season parsed: " + season.ToString());
            }
            return parsedSeasons;
        }

    }
}