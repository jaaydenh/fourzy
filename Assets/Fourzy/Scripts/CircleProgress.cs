using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CircleProgress : MonoBehaviour 
{
    [SerializeField]
    private Material progressMaterial;

    [SerializeField]
    private Color progressColor = Color.white;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float currentValue = 0;

    [HideInInspector]
    [SerializeField]
    private Material circleProgressMaterialCopy;

    private Coroutine progressCoroutine;

    private int progressUniform = Shader.PropertyToID("_Progress");
    private int colorUniform = Shader.PropertyToID("_Color");
    private int rectMainTexUniform = Shader.PropertyToID("_RectMainTex");

    private void Awake()
    {
        this.Init();
    }

    void OnValidate()
    {
        if (circleProgressMaterialCopy != null)
        {
            this.SetupNewValue(currentValue);
        }
        else
        {
            this.Init();
        }
    }

    private void Init()
    {
        circleProgressMaterialCopy = new Material(progressMaterial);

        Sprite sprite = null;

        Image image = this.GetComponent<Image>();
        if (image)
        {
            image.material = circleProgressMaterialCopy;
            sprite = image.sprite;
        }

        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sharedMaterial = circleProgressMaterialCopy;
            sprite = spriteRenderer.sprite;
        }

        if (sprite != null)
        {
            Vector4 rectMainTex = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
                             sprite.textureRect.min.y / sprite.texture.height,
                             sprite.textureRect.max.x / sprite.texture.width,
                             sprite.textureRect.max.y / sprite.texture.height);
            circleProgressMaterialCopy.SetVector(rectMainTexUniform, rectMainTex);
        }
    }

    public void SetupNewValue(float value, bool animated = true, float animationDuration = 1.5f)
    {
        if (progressCoroutine != null)
        {
            this.StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        if (animated)
        {
            progressCoroutine = StartCoroutine(SetupAnimatedProgress(value, animationDuration));
        }
        else
        {
            this.SetupNewValue(value);
        }
    }

    IEnumerator SetupAnimatedProgress(float value, float animationDuration)
    {
        float curValue = currentValue;
        float newValue = value;

        for (float time = 0; time < animationDuration; time += Time.deltaTime)
        {
            currentValue = Mathf.SmoothStep(curValue, newValue, time / animationDuration);
            this.SetupNewValue(currentValue);

            yield return null;
        }
    }

    private void SetupNewValue(float value)
    {
        currentValue = value;
        circleProgressMaterialCopy.SetFloat(progressUniform, currentValue * Mathf.PI * 2);
        circleProgressMaterialCopy.SetColor(colorUniform, progressColor);
    }
}
