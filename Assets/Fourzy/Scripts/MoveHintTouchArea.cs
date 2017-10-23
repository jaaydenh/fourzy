using UnityEngine;

public class MoveHintTouchArea : MonoBehaviour {

    //public GameObject moveHintAreas;
    public GameObject topHintArea;
    public GameObject bottomHintArea;
    public GameObject leftHintArea;
    public GameObject rightHintArea;

    public void FadeInAndOutSprite () {
        gameObject.SetActive(true);

        SpriteRenderer srTopHintArea = topHintArea.GetComponent<SpriteRenderer>();
        SpriteRenderer srBottomHintArea = bottomHintArea.GetComponent<SpriteRenderer>();
        SpriteRenderer srLeftHintArea = leftHintArea.GetComponent<SpriteRenderer>();
        SpriteRenderer srRightHintArea = rightHintArea.GetComponent<SpriteRenderer>();
        srTopHintArea.FadeInAndOutSprite(this, 4.0f);
        srBottomHintArea.FadeInAndOutSprite(this, 4.0f);
        srLeftHintArea.FadeInAndOutSprite(this, 4.0f);
        srRightHintArea.FadeInAndOutSprite(this, 4.0f);
	}
	
}
