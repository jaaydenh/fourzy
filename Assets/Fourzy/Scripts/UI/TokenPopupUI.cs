using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenPopupUI : MonoBehaviour {

    public Text tokenName;
    public Text tokenArena;
    public TextMeshProUGUI description;
    public Image tokenImage;
    public GameObject tileBGImage;

    public GameObject closeButton;

	void Start () {
        Button btn = closeButton.GetComponent<Button>();
        btn.onClick.AddListener(Close);
	}
	
    public void Open() {
        this.gameObject.SetActive(true);
    }

    public void Close()
	{
        this.gameObject.SetActive(false);
	}
}
