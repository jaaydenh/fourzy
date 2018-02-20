using UnityEngine;
using UnityEngine.UI;

public class Badge : MonoBehaviour {

    public Text gameCountLabel;

	// Use this for initialization
	void Start () {
		
	}

    public void SetGameCount(int count) {
        Debug.Log("Badge SetGameCount: "+ count);
        gameCountLabel.text = count.ToString();
    }
}
