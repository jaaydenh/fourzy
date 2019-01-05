using UnityEngine;
using DG.Tweening;

public class tokenCoin : MonoBehaviour
{
    private SpriteRenderer sprite;
    private AudioSource as1;

    void Start()
    {
        as1 = gameObject.GetComponent<AudioSource>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "GamePiece")
        {
            as1.Play();
            sprite.DOFade(0, 0.8f);
            transform.DOMoveY(transform.position.y + 1.1f, 0.6f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
