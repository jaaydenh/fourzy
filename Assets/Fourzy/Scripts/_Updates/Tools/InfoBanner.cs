//modded @vadym udod

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoBanner : MonoBehaviour
{
    public GameObject bannerPanel;
    public Image panel;
    public Text infoText;

    private RectTransform rect;

    void Start()
    {
        rect = panel.GetComponent<RectTransform>();
    }

    public IEnumerator ShowText(string text)
    {
        // Debug.Log("bannerPanel.transform.position.y: " + bannerPanel.transform.position.y);
        // Debug.Log("bannerPanel.transform.localPosition.y: " + bannerPanel.transform.localPosition.y);
        infoText.text = text;
        if (bannerPanel != null)
        {
            bannerPanel.transform.DOLocalMoveY(bannerPanel.transform.localPosition.y - 60, 0.6f);
        }

        yield return new WaitForSeconds(5.3f);

        if (bannerPanel != null)
        {
            bannerPanel.transform.DOLocalMoveY(bannerPanel.transform.localPosition.y + 60, 0.6f);
        }

        if (this)
        {
            Destroy(this.gameObject, 6f);
        }
    }
}
