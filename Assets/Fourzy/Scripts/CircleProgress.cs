using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CircleProgress : MonoBehaviour 
{
    [SerializeField]
    private Color progressColor = Color.white;

    [SerializeField, Range(0.0f, 1.0f)]
    private float currentValue = 0;

    private Material progressMaterial;
    private Coroutine progressCoroutine;

    private int progressUniform = Shader.PropertyToID("_Progress");
    private int colorUniform = Shader.PropertyToID("_Color");
    private int rectMainTexUniform = Shader.PropertyToID("_RectMainTex");

    private void Awake()
    {
        this.Init();
        this.SetupNewValue(currentValue);
    }

    void OnValidate()
    {
        if (progressMaterial != null)
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
        Sprite sprite = null;

        Image image = this.GetComponent<Image>();
        if (image)
        {
            progressMaterial = image.material;
            sprite = image.sprite;
        }

        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            progressMaterial = spriteRenderer.material; 
            sprite = spriteRenderer.sprite;
        }

        if (sprite != null)
        {
            Vector4 rectMainTex = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
                             sprite.textureRect.min.y / sprite.texture.height,
                             sprite.textureRect.max.x / sprite.texture.width,
                             sprite.textureRect.max.y / sprite.texture.height);
            progressMaterial.SetVector(rectMainTexUniform, rectMainTex);
        }

        progressMaterial.SetColor(colorUniform, progressColor);
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
        progressMaterial.SetFloat(progressUniform, currentValue * Mathf.PI * 2);
        progressMaterial.SetColor(colorUniform, progressColor);
    }

    public void SetupNewColor(Color color)
    {
        progressColor = color;
        progressMaterial.SetColor(colorUniform, progressColor);
    }
}
