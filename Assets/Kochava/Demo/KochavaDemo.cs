using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochavaDemo : MonoBehaviour {

	public void SendEvent() {
		//Example (Stnadard Event with Standard Parameters)
		Kochava.Event myEvent = new Kochava.Event (Kochava.EventType.Purchase);
		myEvent.name = "Gold Token";
		myEvent.price = 0.99;
		Kochava.Tracker.SendEvent (myEvent);
	}
}
