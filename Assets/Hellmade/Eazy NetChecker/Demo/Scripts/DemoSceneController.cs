using Hellmade.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoSceneController : MonoBehaviour
{
    public Image statusImg;
    public Text statusText;
    public Text reachabilityText;
    public Text descriptionText;

    private void Awake()
    {
        // Listen to event
        EazyNetChecker.OnConnectionStatusChanged += OnNetStatusChanged;

        // Start a check
        CheckNow();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // The below functionality can also be achieved by listening to the OnCheckStarted and OnCheckFinished events
        // This demonstrates the freedom that Eazy NetChecker gives you to implement your own logic whatever way you prefer
        if (EazyNetChecker.IsChecking)
        {
            descriptionText.text = "Checking connection..";
        }
        else
        {
            descriptionText.text = "Next check in " + (int)EazyNetChecker.NextCheckRemaingSeconds;
        }

        switch(EazyNetChecker.ReachabilityType)
        {
            case NetworkReachability.NotReachable:
                reachabilityText.text = "";
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                reachabilityText.text = "Connected through carrier data";
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                reachabilityText.text = "Connected through cable/wifi";
                break;

        }
    }

    /// <summary>
    /// Function that starts a continuous check
    /// </summary>
    public void CheckNow()
    {
        // Start a continuous check. Note the second parameter that is passed. True is passed for interruptActiveChecks.
        // This is done because the function CheckNow is also used for the "Check Now" button in the demo.
        // This way, when pressed, it will cancel the previous continuous check (it does not have to wait for its next check), and will immediately start another check.
        EazyNetChecker.StartConnectionCheck(false, true);

        statusText.text = "CHECKING..";
    }

    private void OnNetStatusChanged()
    {
        // Change status text depending on the status
        switch (EazyNetChecker.Status)
        {
            case NetStatus.Connected:
                statusText.text = "CONNECTED";
                statusImg.color = Color.green;
                break;
            case NetStatus.NoDNSConnection:
                statusText.text = "OFFLINE";
                statusImg.color = Color.red;
                break;
            case NetStatus.WalledGarden:
                statusText.text = "RESRTICTED";
                statusImg.color = Color.yellow;
                break;
            case NetStatus.PendingCheck:
                statusText.text = "PENDING";
                statusImg.color = Color.grey;
                break;
        }
    }
}
