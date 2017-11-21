using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

    public Text infoText;

    public void Open(string infoText, Color textColor, bool setColor = false)
    {
        this.infoText.text = infoText;
        if (setColor) {
            this.infoText.color = textColor;
        }
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
