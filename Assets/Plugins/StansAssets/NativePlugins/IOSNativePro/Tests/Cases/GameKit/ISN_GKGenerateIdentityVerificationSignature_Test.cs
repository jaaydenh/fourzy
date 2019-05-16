

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKGenerateIdentityVerificationSignature_Test : SA_BaseTest
    {

        public override void Test() {

            ISN_GKLocalPlayer player = ISN_GKLocalPlayer.LocalPlayer;
            player.GenerateIdentityVerificationSignatureWithCompletionHandler((signatureResult) => {
                if (signatureResult.IsSucceeded) {
                    ISN_Logger.Log("signatureResult.PublicKeyUrl: " + signatureResult.PublicKeyUrl);
                    ISN_Logger.Log("signatureResult.Timestamp: " + signatureResult.Timestamp);
                    ISN_Logger.Log("signatureResult.Salt.Length: " + signatureResult.Salt.Length);
                    ISN_Logger.Log("signatureResult.Signature.Length: " + signatureResult.Signature.Length);
                }

                SetAPIResult(signatureResult);
            });

        }
    }
}