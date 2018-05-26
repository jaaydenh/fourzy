using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AreaSelectUI : MonoBehaviour {

	// public GameObject areaButton;
	public string AreaName {get; private set;}

	void Start () {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OpenArea);
	}
	
	void OpenArea () {
		
	}
}
