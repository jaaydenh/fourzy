////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using SA.iOS.GameKit;
using UnityEngine;

[Serializable]
public class ISN_GKResolveSavedGamesRequest
{

    [SerializeField] List<string> m_conflictedGames = new List<string>();
    [SerializeField] string m_data = string.Empty;

    public ISN_GKResolveSavedGamesRequest(List<string> list, string stringData) {
        m_conflictedGames = list;
        m_data = stringData;
    }

    public string DataStringValue {
        get { return m_data; }
    }

    public byte[] DataByteValue {
        get { return Encoding.UTF8.GetBytes(m_data); }
    }

    public List<string> ConflictedGames {
        get { return m_conflictedGames; }
    }
}
