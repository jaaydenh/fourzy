

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKLocalPlayer_Authenticate_Test : SA_BaseTest
    {

        public override void Test() {

            ISN_GKLocalPlayer.Authenticate((SA_Result result) => {
               if(result.IsSucceeded) {
                    ISN_GKLocalPlayer player = ISN_GKLocalPlayer.LocalPlayer;
                    ISN_Logger.Log("player.PlayerID: " + player.PlayerID);
                    ISN_Logger.Log("player.Alias: " + player.Alias);
                    ISN_Logger.Log("player.DisplayName: " + player.DisplayName);
                    ISN_Logger.Log("player.Authenticated: " + player.Authenticated);
                    ISN_Logger.Log("player.Underage: " + player.Underage);
                }

                SetAPIResult(result);
            });

        }
    }
}