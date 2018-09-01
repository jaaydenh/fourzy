using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class GameInfo : MonoBehaviour {

    public Text infoText;
    public GameObject rewardButton;
    private bool animateChest;
    private bool animating;
    public float strength = 3;
    public int vibrato = 12;
    public float randomness = 70;
    public float duration = 1.20f;

    public void Open(string infoText, Color textColor, bool setColor = false, bool showRewardButton = false)
    {
        this.infoText.text = infoText;
        if (setColor) {
            this.infoText.color = textColor;
        }
        if (showRewardButton) {
            rewardButton.SetActive(true);
            animateChest = true;
            animating = false;
        } else {
            rewardButton.SetActive(false);
        }

        gameObject.SetActive(true);

        this.ShowGameInfo();
    }

    private void ShowGameInfo()
    {
        Image backgroundImage = this.GetComponent<Image>();
        float finishAlpha = backgroundImage.color.a;
        Color color = backgroundImage.color;
        color.a = 0.0f;
        backgroundImage.color = color;
        Tween showPanel = backgroundImage.DOFade(finishAlpha, 0.25f).SetEase(Ease.InOutBack);

        Transform textTransform = this.infoText.transform;
        float posX = textTransform.position.x;
        Vector3 pos = textTransform.position;
        pos.x = 1000;
        textTransform.position = pos;
        Tween showText = textTransform.DOMoveX(posX, 0.5f).SetEase(Ease.OutBack);

        Sequence gameInfoAppear = DOTween.Sequence();
        gameInfoAppear.Append(showPanel);
        gameInfoAppear.Append(showText);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (animateChest && !animating) {
            animating = true;
            StartCoroutine(AnimateChest());
        }   
    }

    private IEnumerator AnimateChest() {
        Image chest = rewardButton.GetComponent<Image>();
        yield return new WaitForSeconds(1);
        chest.transform.DOShakeRotation(duration, strength, vibrato, randomness, false);
        chest.transform.DOShakePosition(duration, strength, vibrato, randomness, false, false).OnComplete(() => animating = false);

    }

    //private IEnumerator ResetAnimation() {
        
    //}
}
